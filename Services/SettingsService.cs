using System;
using System.IO;
using System.Text;
using System.Text.Json;
using DirectoryStructureGenerator.PresetManager.Models;

namespace DirectoryStructureGenerator.PresetManager.Services;

public sealed class SettingsService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public AppSettings Load()
    {
        AppPaths.EnsureApplicationDirectories();

        if (!File.Exists(AppPaths.SettingsFile))
        {
            var defaults = new AppSettings();
            Save(defaults);
            return defaults;
        }

        try
        {
            var json = File.ReadAllText(AppPaths.SettingsFile, Encoding.UTF8);
            var settings = JsonSerializer.Deserialize<AppSettings>(json, JsonOptions) ?? new AppSettings();
            settings.PresetFilePath = string.IsNullOrWhiteSpace(settings.PresetFilePath)
                ? AppPaths.DefaultPresetRelativePath
                : settings.PresetFilePath.Trim();
            return settings;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            var damagedPath = AppPaths.SettingsFile + ".invalid_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
            try { File.Copy(AppPaths.SettingsFile, damagedPath, overwrite: false); } catch { }
            var defaults = new AppSettings();
            Save(defaults);
            return defaults;
        }
    }

    public void Save(AppSettings settings)
    {
        AppPaths.EnsureApplicationDirectories();
        var json = JsonSerializer.Serialize(settings, JsonOptions);
        AtomicWriteText(AppPaths.SettingsFile, json + Environment.NewLine);
    }

    private static void AtomicWriteText(string path, string content)
    {
        var directory = Path.GetDirectoryName(path) ?? throw new InvalidOperationException("設定ファイルの保存先を取得できません。");
        Directory.CreateDirectory(directory);

        var tempPath = path + ".tmp";
        var backupPath = path + ".bak_" + DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
        File.WriteAllText(tempPath, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

        if (File.Exists(path))
        {
            File.Replace(tempPath, path, backupPath, ignoreMetadataErrors: true);
        }
        else
        {
            File.Move(tempPath, path);
        }
    }
}
