using System;
using System.IO;
using DirectoryStructureGenerator.PresetManager.Models;

namespace DirectoryStructureGenerator.PresetManager.Services;

public enum PresetMigrationMode
{
    CopyCurrentData,
    UseExistingDestination,
    CreateEmptyDestination
}

public sealed class PresetStorageService
{
    private readonly PresetCsvService _csvService;
    private readonly SettingsService _settingsService;

    public PresetStorageService(PresetCsvService csvService, SettingsService settingsService)
    {
        _csvService = csvService;
        _settingsService = settingsService;
    }

    public void ChangeLocation(
        string currentPath,
        string destinationPath,
        System.Collections.Generic.IEnumerable<Preset> currentPresets,
        PresetMigrationMode mode,
        AppSettings settings)
    {
        currentPath = Path.GetFullPath(currentPath);
        destinationPath = Path.GetFullPath(destinationPath);

        if (string.Equals(currentPath, destinationPath, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var destinationDirectory = Path.GetDirectoryName(destinationPath)
            ?? throw new InvalidOperationException("新しい保存先フォルダーを取得できません。");
        Directory.CreateDirectory(destinationDirectory);

        switch (mode)
        {
            case PresetMigrationMode.CopyCurrentData:
                BackupDestinationIfNeeded(destinationPath);
                _csvService.Save(destinationPath, currentPresets);
                break;

            case PresetMigrationMode.UseExistingDestination:
                if (!File.Exists(destinationPath))
                {
                    throw new FileNotFoundException("指定した保存先にCSVファイルがありません。", destinationPath);
                }
                _ = _csvService.Load(destinationPath);
                break;

            case PresetMigrationMode.CreateEmptyDestination:
                BackupDestinationIfNeeded(destinationPath);
                _csvService.Save(destinationPath, Array.Empty<Preset>());
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(mode));
        }

        // データ側の処理が成功した後にだけ settings.json を切り替える。
        settings.PresetFilePath = AppPaths.ToPortableSettingPath(destinationPath);
        _settingsService.Save(settings);
    }

    private static void BackupDestinationIfNeeded(string destinationPath)
    {
        if (!File.Exists(destinationPath)) return;

        var backupPath = destinationPath + ".backup_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
        File.Copy(destinationPath, backupPath, overwrite: false);
    }
}
