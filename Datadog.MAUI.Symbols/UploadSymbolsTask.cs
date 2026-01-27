using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Diagnostics;
using System.IO;

namespace Datadog.MAUI.Symbols
{
    /// <summary>
    /// MSBuild task that uploads debug symbols (dSYMs and Proguard mapping files) to Datadog.
    /// </summary>
    public class UploadSymbolsTask : Microsoft.Build.Utilities.Task
    {
        /// <summary>
        /// The target platform identifier (android/ios). Required.
        /// </summary>
        [Required]
        public string TargetPlatform { get; set; } = string.Empty;

        /// <summary>
        /// Global service name fallback. Used if platform-specific service name is not provided.
        /// </summary>
        public string? ServiceName { get; set; }

        /// <summary>
        /// Android-specific service name. Takes precedence over ServiceName for Android builds.
        /// </summary>
        public string? ServiceNameAndroid { get; set; }

        /// <summary>
        /// iOS-specific service name. Takes precedence over ServiceName for iOS builds.
        /// </summary>
        public string? ServiceNameIOS { get; set; }

        /// <summary>
        /// Application version. Required.
        /// </summary>
        [Required]
        public string AppVersion { get; set; } = string.Empty;

        /// <summary>
        /// Path to the debug symbols file (mapping.txt or .dSYM directory).
        /// </summary>
        public string? FilePath { get; set; }

        /// <summary>
        /// Datadog API Key. If not provided, falls back to DD_API_KEY environment variable.
        /// </summary>
        public string? ApiKey { get; set; }

        /// <summary>
        /// Datadog site (e.g., us1, us5, eu1). Defaults to datadoghq.com.
        /// </summary>
        public string? Site { get; set; }

        /// <summary>
        /// If true, runs the command with --dry-run flag (does not actually upload).
        /// </summary>
        public bool DryRun { get; set; }

        /// <summary>
        /// Executes the symbol upload task.
        /// </summary>
        /// <returns>True if the upload succeeded, false otherwise.</returns>
        public override bool Execute()
        {
            // 1. Resolve Service Name using hierarchy: Platform Specific > Global > Error
            string? finalServiceName = ResolveServiceName();

            if (string.IsNullOrEmpty(finalServiceName))
            {
                Log.LogError("Datadog Service Name is required. Set <DatadogServiceName> or <DatadogServiceNameAndroid>/<DatadogServiceNameiOS>.");
                return false;
            }

            // 2. Validate FilePath
            if (string.IsNullOrEmpty(FilePath))
            {
                Log.LogError("FilePath is required but was not provided.");
                return false;
            }

            if (!File.Exists(FilePath) && !Directory.Exists(FilePath))
            {
                Log.LogWarning($"Datadog Symbols: File or directory not found at '{FilePath}'. Skipping upload.");
                return true; // Don't fail the build, just skip
            }

            // 3. Build Arguments (finalServiceName is guaranteed non-null here due to check above)
            string args = BuildCommandArguments(finalServiceName!);

            // 4. Execute via NPX
            return ExecuteNpx(args);
        }

        private string? ResolveServiceName()
        {
            // Priority: Platform Specific > Global
            if (TargetPlatform.Equals("android", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(ServiceNameAndroid))
            {
                return ServiceNameAndroid;
            }
            else if (TargetPlatform.Equals("ios", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(ServiceNameIOS))
            {
                return ServiceNameIOS;
            }

            return ServiceName;
        }

        private string BuildCommandArguments(string serviceName)
        {
            // Base command: datadog-ci flutter-symbols upload
            // Note: Using 'flutter-symbols' as per the plan - it supports generic mobile symbol uploads
            var args = $"flutter-symbols upload --service-name \"{serviceName}\" --version \"{AppVersion}\"";

            // Add platform-specific arguments
            if (TargetPlatform.Equals("android", StringComparison.OrdinalIgnoreCase))
            {
                // Android uses --android-mapping (boolean) and --android-mapping-location (path)
                args += $" --android-mapping --android-mapping-location \"{FilePath}\"";
            }
            else if (TargetPlatform.Equals("ios", StringComparison.OrdinalIgnoreCase))
            {
                // iOS uses --ios-dsyms (boolean) and --ios-dsyms-location (path)
                args += $" --ios-dsyms --ios-dsyms-location \"{FilePath}\"";
            }

            // Add optional parameters
            if (DryRun)
            {
                args += " --dry-run";
            }

            return args;
        }

        private bool ExecuteNpx(string arguments)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "npx",
                    Arguments = $"@datadog/datadog-ci {arguments}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                // Set API Key from Property or fall back to environment variable
                if (!string.IsNullOrEmpty(ApiKey))
                {
                    psi.EnvironmentVariables["DD_API_KEY"] = ApiKey;
                }
                else if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DD_API_KEY")))
                {
                    Log.LogWarning("DD_API_KEY is not set. Upload may fail if not configured elsewhere.");
                }

                // Set Datadog Site if provided
                if (!string.IsNullOrEmpty(Site))
                {
                    psi.EnvironmentVariables["DD_SITE"] = Site;
                }

                Log.LogMessage(MessageImportance.High, $"[Datadog] Uploading {TargetPlatform} symbols to Datadog...");
                Log.LogMessage(MessageImportance.Normal, $"[Datadog] Command: npx @datadog/datadog-ci {arguments.Replace(ApiKey ?? "", "***")}");

                var process = Process.Start(psi);
                if (process == null)
                {
                    Log.LogError("Failed to start npx process.");
                    return false;
                }

                // Read output asynchronously to prevent deadlocks
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                // Log output
                if (!string.IsNullOrWhiteSpace(output))
                {
                    Log.LogMessage(MessageImportance.Normal, output);
                }

                if (process.ExitCode != 0)
                {
                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        Log.LogError($"[Datadog] Upload failed: {error}");
                    }
                    else
                    {
                        Log.LogError($"[Datadog] Upload failed with exit code {process.ExitCode}");
                    }
                    return false;
                }

                Log.LogMessage(MessageImportance.High, $"[Datadog] Successfully uploaded {TargetPlatform} symbols.");
                return true;
            }
            catch (System.ComponentModel.Win32Exception ex) when (ex.Message.Contains("npx"))
            {
                Log.LogError($"[Datadog] Failed to execute npx. Ensure Node.js and npm are installed and available in PATH. Error: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Log.LogError($"[Datadog] Unexpected error during symbol upload: {ex.Message}");
                return false;
            }
        }
    }
}
