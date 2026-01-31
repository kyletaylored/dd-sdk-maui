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
        /// Optional build flavor/variant (e.g., "release", "debug", "staging").
        /// Defaults to "release" if not specified.
        /// </summary>
        public string? Flavor { get; set; }

        /// <summary>
        /// Executes the symbol upload task.
        /// </summary>
        /// <returns>True if the upload succeeded, false otherwise.</returns>
        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, "[Datadog] Symbol upload task started");
            Log.LogMessage(MessageImportance.High, $"[Datadog]   TargetPlatform: {TargetPlatform}");
            Log.LogMessage(MessageImportance.High, $"[Datadog]   AppVersion: {AppVersion}");
            Log.LogMessage(MessageImportance.High, $"[Datadog]   FilePath: {FilePath ?? "(not set)"}");
            Log.LogMessage(MessageImportance.High, $"[Datadog]   ServiceName: {ServiceName ?? "(not set)"}");
            Log.LogMessage(MessageImportance.High, $"[Datadog]   ServiceNameAndroid: {ServiceNameAndroid ?? "(not set)"}");
            Log.LogMessage(MessageImportance.High, $"[Datadog]   ServiceNameIOS: {ServiceNameIOS ?? "(not set)"}");
            Log.LogMessage(MessageImportance.High, $"[Datadog]   DryRun: {DryRun}");
            Log.LogMessage(MessageImportance.High, $"[Datadog]   ApiKey: {(string.IsNullOrEmpty(ApiKey) ? "(not set)" : "***SET***")}");

            // 1. Check if npx is available before doing anything else
            Log.LogMessage(MessageImportance.High, "[Datadog] Checking if npx is available...");
            if (!CheckNpxAvailable())
            {
                Log.LogError("[Datadog] Node.js and npm are required for symbol upload. Please install Node.js from https://nodejs.org/");
                Log.LogError("[Datadog] After installing, verify with: npx --version");
                Log.LogError("[Datadog] To disable symbol upload, set <DatadogUploadEnabled>false</DatadogUploadEnabled> in your .csproj");
                return false;
            }
            Log.LogMessage(MessageImportance.High, "[Datadog] npx is available");

            // 2. Resolve Service Name using hierarchy: Platform Specific > Global > Error
            string? finalServiceName = ResolveServiceName();
            Log.LogMessage(MessageImportance.High, $"[Datadog] Resolved service name: {finalServiceName ?? "(none)"}");

            if (string.IsNullOrEmpty(finalServiceName))
            {
                Log.LogError("[Datadog] Service Name is required. Set <DatadogServiceName> or <DatadogServiceNameAndroid>/<DatadogServiceNameiOS>.");
                return false;
            }

            // 3. Validate FilePath
            if (string.IsNullOrEmpty(FilePath))
            {
                Log.LogError("[Datadog] FilePath is required but was not provided.");
                Log.LogError("[Datadog] This usually means the MSBuild targets couldn't find the symbol files.");
                Log.LogError("[Datadog] For Android: Ensure ProGuard/R8 is enabled in Release mode.");
                Log.LogError("[Datadog] For iOS: Ensure debug symbols are enabled in project settings.");
                return false;
            }

            Log.LogMessage(MessageImportance.High, $"[Datadog] Checking if file/directory exists: {FilePath}");
            bool pathExists = File.Exists(FilePath) || Directory.Exists(FilePath);
            Log.LogMessage(MessageImportance.High, $"[Datadog] Path exists: {pathExists}");

            if (!pathExists)
            {
                Log.LogWarning($"[Datadog] Symbols: File or directory not found at '{FilePath}'. Skipping upload.");
                return true; // Don't fail the build, just skip
            }

            // 4. Build Arguments (finalServiceName is guaranteed non-null here due to check above)
            string args = BuildCommandArguments(finalServiceName!);
            Log.LogMessage(MessageImportance.High, $"[Datadog] Built command arguments successfully");

            // 5. Execute via NPX
            return ExecuteNpx(args, finalServiceName!);
        }

        private bool CheckNpxAvailable()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "npx",
                    Arguments = "--version",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                var process = Process.Start(psi);
                if (process == null)
                {
                    return false;
                }

                process.WaitForExit(5000); // 5 second timeout
                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
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

            if (!string.IsNullOrEmpty(Flavor))
            {
                args += $" --flavor \"{Flavor}\"";
            }

            return args;
        }

        private bool ExecuteNpx(string arguments, string serviceName)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "npx",
                    Arguments = $"--yes @datadog/datadog-ci {arguments}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                // Set API Key from Property or fall back to environment variable
                if (!string.IsNullOrEmpty(ApiKey))
                {
                    psi.EnvironmentVariables["DATADOG_API_KEY"] = ApiKey;
                }
                else if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DATADOG_API_KEY")) &&
                         string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DD_API_KEY")))
                {
                    // In dry-run mode, set a dummy key to avoid metrics initialization errors
                    if (DryRun)
                    {
                        psi.EnvironmentVariables["DATADOG_API_KEY"] = "dummy-key-for-dry-run";
                        Log.LogMessage(MessageImportance.High, "[Datadog] Dry-run mode: Using dummy API key.");
                    }
                    else
                    {
                        Log.LogWarning("[Datadog] DATADOG_API_KEY is not set. Upload may fail if not configured elsewhere.");
                    }
                }

                // Set Datadog Site if provided
                if (!string.IsNullOrEmpty(Site))
                {
                    psi.EnvironmentVariables["DD_SITE"] = Site;
                }

                Log.LogMessage(MessageImportance.High, $"[Datadog] ========================================");
                Log.LogMessage(MessageImportance.High, $"[Datadog] Uploading {TargetPlatform} symbols to Datadog");
                Log.LogMessage(MessageImportance.High, $"[Datadog] ========================================");
                Log.LogMessage(MessageImportance.High, $"[Datadog]   Service: {serviceName}");
                Log.LogMessage(MessageImportance.High, $"[Datadog]   Version: {AppVersion}");
                Log.LogMessage(MessageImportance.High, $"[Datadog]   Flavor: {Flavor ?? "(not set)"}");
                Log.LogMessage(MessageImportance.High, $"[Datadog]   Platform: {TargetPlatform}");
                Log.LogMessage(MessageImportance.High, $"[Datadog]   File: {FilePath}");
                Log.LogMessage(MessageImportance.High, $"[Datadog]   Unique Key: ({serviceName}, {AppVersion}, {Flavor ?? "none"})");

                // Sanitize command for logging (hide API key if present in arguments)
                string sanitizedArgs = !string.IsNullOrEmpty(ApiKey) ? arguments.Replace(ApiKey, "***") : arguments;
                Log.LogMessage(MessageImportance.High, $"[Datadog]   Command: npx @datadog/datadog-ci {sanitizedArgs}");
                Log.LogMessage(MessageImportance.High, $"[Datadog] ========================================");
                Log.LogMessage(MessageImportance.High, "[Datadog] Starting npx process...");
                Log.LogMessage(MessageImportance.High, "[Datadog] NOTE: First run may take several minutes to download @datadog/datadog-ci package");

                // Don't redirect streams - let output go directly to console
                psi.RedirectStandardOutput = false;
                psi.RedirectStandardError = false;
                psi.UseShellExecute = false;

                var process = Process.Start(psi);
                if (process == null)
                {
                    Log.LogError("[Datadog] Failed to start npx process.");
                    return false;
                }

                Log.LogMessage(MessageImportance.High, "[Datadog] Process started, waiting for completion...");

                // Wait with a reasonable timeout (10 minutes for large files/slow networks)
                bool exited = process.WaitForExit(600000); // 10 minutes

                if (!exited)
                {
                    Log.LogError("[Datadog] Symbol upload timed out after 10 minutes. Killing process.");
                    try
                    {
                        process.Kill();
                    }
                    catch { }
                    return false;
                }

                if (process.ExitCode != 0)
                {
                    Log.LogError($"[Datadog] Upload failed with exit code {process.ExitCode}");
                    Log.LogError($"[Datadog] See output above for details from datadog-ci");
                    return false;
                }

                Log.LogMessage(MessageImportance.High, $"[Datadog] ========================================");
                Log.LogMessage(MessageImportance.High, $"[Datadog] Successfully uploaded {TargetPlatform} symbols!");
                Log.LogMessage(MessageImportance.High, $"[Datadog]   Service: {serviceName}");
                Log.LogMessage(MessageImportance.High, $"[Datadog]   Version: {AppVersion}");
                Log.LogMessage(MessageImportance.High, $"[Datadog]   Flavor: {Flavor ?? "(not set)"}");
                Log.LogMessage(MessageImportance.High, $"[Datadog]   Unique Key: ({serviceName}, {AppVersion}, {Flavor ?? "none"})");
                Log.LogMessage(MessageImportance.High, $"[Datadog] ========================================");
                return true;
            }
            catch (System.ComponentModel.Win32Exception ex) when (ex.Message.Contains("npx"))
            {
                Log.LogError($"[Datadog] Failed to execute npx. This shouldn't happen as we checked earlier. Error: {ex.Message}");
                Log.LogError("[Datadog] Please ensure Node.js is in your PATH and try again.");
                return false;
            }
            catch (Exception ex)
            {
                Log.LogError($"[Datadog] Unexpected error during symbol upload: {ex.Message}");
                Log.LogError($"[Datadog] Stack trace: {ex.StackTrace}");
                return false;
            }
        }
    }
}
