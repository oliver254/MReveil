using System.Linq;
using System.Runtime.InteropServices;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.IO.FileSystemTasks;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Application version")]
    readonly string Version = "1.0.0";

    [Solution]
    readonly Solution Solution;

    Project MauiProject => Solution.GetProject("MReveil");

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath MacOSArtifacts => ArtifactsDirectory / "macos";
    AbsolutePath WindowsArtifacts => ArtifactsDirectory / "windows";
    AbsolutePath AndroidArtifacts => ArtifactsDirectory / "android";
    AbsolutePath iOSArtifacts => ArtifactsDirectory / "ios";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").DeleteDirectories();
            ArtifactsDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s.SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
            .SetProjectFile(Solution)
            .SetConfiguration(Configuration)
            .SetFramework(Solution.Projects.First().GetTargetFrameworks()?.First())
            .EnableNoRestore());
        });

    Target Publish => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetPublish(s => s
            .SetProject(Solution)
            .SetConfiguration(Configuration)
            .SetFramework(Solution.Projects.First().GetTargetFrameworks()?.First())
            .SetOutput(ArtifactsDirectory)
            .EnableNoRestore());
        });

    // ===============================
    // macOS Build Targets
    // ===============================

    Target PublishMacOS => _ => _
        .DependsOn(Clean)
        .OnlyWhenStatic(() => RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        .Executes(() =>
        {
            Serilog.Log.Information("🍎 Building for macOS Catalyst...");

            DotNetPublish(s => s
                .SetProject(MauiProject)
                .SetConfiguration(Configuration)
                .SetFramework("net10.0-maccatalyst")
                .SetProperty("RuntimeIdentifier", "maccatalyst-arm64")
                .SetProperty("CreatePackage", "true")
                .SetProperty("UseHardenedRuntime", "true")
                .SetOutput(MacOSArtifacts));

            Serilog.Log.Information("✅ macOS build complete at {Path}", MacOSArtifacts);
        });

    Target CreateDMG => _ => _
        .DependsOn(PublishMacOS)
        .OnlyWhenStatic(() => RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        .Executes(() =>
        {
            var appPath = MacOSArtifacts / "MReveil.app";
            var dmgPath = ArtifactsDirectory / $"MReveil-{Version}.dmg";
            var tempDir = ArtifactsDirectory / "dmg-temp";

            if (!DirectoryExists(appPath))
            {
                throw new Exception($"App not found at {appPath}");
            }

            Serilog.Log.Information("💿 Creating DMG...");

            // Créer le répertoire temporaire
            EnsureCleanDirectory(tempDir);
            CopyDirectoryRecursively(appPath, tempDir / "MReveil.app");

            // Créer un lien symbolique vers Applications
            ProcessTasks.StartProcess("ln", $"-s /Applications {tempDir}/Applications").AssertZeroExitCode();

            // Supprimer l'ancien DMG s'il existe
            DeleteFile(dmgPath);

            // Créer le DMG
            ProcessTasks.StartProcess("hdiutil", 
                $"create -volname MReveil -srcfolder {tempDir} -ov -format UDZO {dmgPath}")
                .AssertZeroExitCode();

            // Nettoyer
            DeleteDirectory(tempDir);

            Serilog.Log.Information("✅ DMG created at {Path}", dmgPath);
            Serilog.Log.Information("📦 Size: {Size}", GetFileSize(dmgPath));
        });

    // ===============================
    // Windows Build Targets
    // ===============================

    Target PublishWindows => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            Serilog.Log.Information("🪟 Building for Windows...");

            DotNetPublish(s => s
                .SetProject(MauiProject)
                .SetConfiguration(Configuration)
                .SetFramework("net10.0-windows10.0.19041.0")
                .SetProperty("RuntimeIdentifier", "win-x64")
                .SetProperty("SelfContained", "true")
                .SetProperty("PublishSingleFile", "false")
                .SetOutput(WindowsArtifacts));

            Serilog.Log.Information("✅ Windows build complete at {Path}", WindowsArtifacts);
        });

    Target CreateMSIX => _ => _
        .DependsOn(PublishWindows)
        .OnlyWhenStatic(() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        .Executes(() =>
        {
            Serilog.Log.Information("📦 Creating MSIX package...");

            DotNetPublish(s => s
                .SetProject(MauiProject)
                .SetConfiguration(Configuration)
                .SetFramework("net10.0-windows10.0.19041.0")
                .SetProperty("RuntimeIdentifier", "win-x64")
                .SetProperty("GenerateAppxPackageOnBuild", "true")
                .SetProperty("AppxPackageSigningEnabled", "false")
                .SetOutput(WindowsArtifacts / "msix"));

            Serilog.Log.Information("✅ MSIX package created");
        });

    // ===============================
    // Android Build Targets
    // ===============================

    Target PublishAndroid => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            Serilog.Log.Information("🤖 Building for Android...");

            DotNetPublish(s => s
                .SetProject(MauiProject)
                .SetConfiguration(Configuration)
                .SetFramework("net10.0-android")
                .SetProperty("AndroidPackageFormat", "apk")
                .SetProperty("RuntimeIdentifier", "android-arm64")
                .SetOutput(AndroidArtifacts));

            Serilog.Log.Information("✅ Android APK created at {Path}", AndroidArtifacts);
        });

    Target PublishAndroidAAB => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            Serilog.Log.Information("🤖 Building Android App Bundle (AAB)...");

            DotNetPublish(s => s
                .SetProject(MauiProject)
                .SetConfiguration(Configuration)
                .SetFramework("net10.0-android")
                .SetProperty("AndroidPackageFormat", "aab")
                .SetOutput(AndroidArtifacts / "aab"));

            Serilog.Log.Information("✅ Android AAB created for Google Play");
        });

    // ===============================
    // iOS Build Targets
    // ===============================

    Target PublishIOS => _ => _
        .DependsOn(Clean)
        .OnlyWhenStatic(() => RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        .Executes(() =>
        {
            Serilog.Log.Information("📱 Building for iOS...");

            DotNetPublish(s => s
                .SetProject(MauiProject)
                .SetConfiguration(Configuration)
                .SetFramework("net10.0-ios")
                .SetProperty("RuntimeIdentifier", "ios-arm64")
                .SetProperty("CreatePackage", "true")
                .SetOutput(iOSArtifacts));

            Serilog.Log.Information("✅ iOS build complete at {Path}", iOSArtifacts);
        });

    // ===============================
    // Build All Platforms
    // ===============================

    Target BuildAll => _ => _
        .DependsOn(PublishMacOS, PublishWindows, PublishAndroid)
        .Executes(() =>
        {
            Serilog.Log.Information("🎉 All platforms built successfully!");
            Serilog.Log.Information("📂 Artifacts directory: {Path}", ArtifactsDirectory);
        });

    // ===============================
    // Helper Methods
    // ===============================

    string GetFileSize(AbsolutePath path)
    {
        var fileInfo = new System.IO.FileInfo(path);
        var bytes = fileInfo.Length;
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}
