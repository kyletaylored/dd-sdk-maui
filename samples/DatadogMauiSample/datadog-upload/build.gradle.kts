buildscript {
    repositories {
        mavenCentral()
        gradlePluginPortal()
    }
    dependencies {
        // Provide Kotlin Gradle plugin so the Datadog plugin can find KotlinCompile
        classpath("org.jetbrains.kotlin:kotlin-gradle-plugin:1.9.0")
    }
}

plugins {
  id("com.datadoghq.dd-sdk-android-gradle-plugin") version "1.22.0"
}

datadog {
  // REQUIRED
  site = System.getenv("DD_ANDROID_SITE") ?: "US1"
  mappingFilePath = System.getenv("DD_MAPPING_FILE") ?: "mapping.txt"

  // STRONGLY RECOMMENDED (so symbols match RUM events)
  serviceName = System.getenv("DD_ANDROID_SERVICE")
  env = System.getenv("DD_ENV")
  versionName = System.getenv("DD_VERSION")

  // We're only using this helper project to upload symbols
  checkProjectDependencies = "none"
}
