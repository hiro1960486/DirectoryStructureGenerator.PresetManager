using System;
using System.IO;

namespace DirectoryStructureGenerator.PresetManager.Services;

public static class AppPaths
{
    public const string DefaultPresetRelativePath = "Config\\presets.csv";

    public static string BaseDirectory => AppContext.BaseDirectory;
    public static string ConfigDirectory => Path.Combine(BaseDirectory, "Config");
    public static string SettingsFile => Path.Combine(ConfigDirectory, "settings.json");
    public static string DefaultPresetsFile => Path.Combine(BaseDirectory, DefaultPresetRelativePath);
    public static string OutputDirectory => Path.Combine(BaseDirectory, "Output");
    public static string PreviewDirectory => Path.Combine(BaseDirectory, "Preview");
    public static string LogsDirectory => Path.Combine(BaseDirectory, "Logs");
    public static string TempDirectory => Path.Combine(BaseDirectory, "Temp");

    public static void EnsureApplicationDirectories()
    {
        foreach (var path in new[] { ConfigDirectory, OutputDirectory, PreviewDirectory, LogsDirectory, TempDirectory })
        {
            Directory.CreateDirectory(path);
        }
    }

    public static string ResolvePresetPath(string? configuredPath)
    {
        var value = string.IsNullOrWhiteSpace(configuredPath)
            ? DefaultPresetRelativePath
            : configuredPath.Trim();

        var fullPath = Path.IsPathRooted(value)
            ? Path.GetFullPath(value)
            : Path.GetFullPath(Path.Combine(BaseDirectory, value));

        return fullPath;
    }

    public static string ToPortableSettingPath(string fullPath)
    {
        fullPath = Path.GetFullPath(fullPath);
        var relative = Path.GetRelativePath(BaseDirectory, fullPath);

        return relative.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal)
            || relative.Equals("..", StringComparison.Ordinal)
            ? fullPath
            : relative;
    }
}
