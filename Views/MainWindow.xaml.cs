using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using DirectoryStructureGenerator.PresetManager.Models;
using DirectoryStructureGenerator.PresetManager.Services;
using DirectoryStructureGenerator.PresetManager.ViewModels;
using Microsoft.Win32;

namespace DirectoryStructureGenerator.PresetManager.Views;

public partial class MainWindow : Window
{
    private readonly MainViewModel _vm = new();
    private readonly PresetCsvService _csv = new();
    private readonly SettingsService _settingsService = new();
    private readonly PresetStorageService _storageService;
    private AppSettings _settings = new();
    private string _presetFilePath = AppPaths.DefaultPresetsFile;

    public MainWindow()
    {
        InitializeComponent();
        _storageService = new PresetStorageService(_csv, _settingsService);
        DataContext = _vm;
        Loaded += (_, _) => InitializeStorageAndLoad();
        Closing += (_, _) => { try { SaveData(); } catch { } };
    }

    private void InitializeStorageAndLoad()
    {
        AppPaths.EnsureApplicationDirectories();
        _settings = _settingsService.Load();
        _presetFilePath = AppPaths.ResolvePresetPath(_settings.PresetFilePath);
        _vm.PresetFilePath = _presetFilePath;

        if (!File.Exists(_presetFilePath))
        {
            _csv.Save(_presetFilePath, Array.Empty<Preset>());
        }

        LoadData();
    }

    private void LoadData()
    {
        try
        {
            _vm.Replace(_csv.Load(_presetFilePath));
            _vm.PresetFilePath = _presetFilePath;
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(ex.Message, "読込エラー", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SaveData() => _csv.Save(_presetFilePath, _vm.Presets);

    private void New_Click(object sender, RoutedEventArgs e)
    {
        var preset = new Preset { Name = "新しいプリセット", Order = _vm.Presets.Count == 0 ? 10 : _vm.Presets.Max(x => x.Order) + 10 };
        var window = new PresetEditorWindow(preset, _vm.Presets) { Owner = this };
        if (window.ShowDialog() == true)
        {
            _vm.Presets.Add(preset);
            _vm.Selected = preset;
            _vm.RefreshComputed();
        }
    }

    private void Edit_Click(object sender, RoutedEventArgs e)
    {
        if (_vm.Selected == null) return;
        var original = _vm.Selected;
        var clone = original.DeepClone();
        var window = new PresetEditorWindow(clone, _vm.Presets, original) { Owner = this };
        if (window.ShowDialog() == true)
        {
            var index = _vm.Presets.IndexOf(original);
            _vm.Presets[index] = clone;
            _vm.SortPresetsPreservingSelection(clone);
        }
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        if (_vm.Selected == null) return;
        if (System.Windows.MessageBox.Show($"「{_vm.Selected.Name}」を削除しますか？", "確認", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
        _vm.Presets.Remove(_vm.Selected);
        _vm.Selected = _vm.Presets.FirstOrDefault();
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            SaveData();
            System.Windows.MessageBox.Show($"保存しました。\n{_presetFilePath}", "保存完了");
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(ex.Message, "保存エラー", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Reload_Click(object sender, RoutedEventArgs e) => LoadData();

    private void ChangeStorage_Click(object sender, RoutedEventArgs e)
    {
        SaveData();

        var dialog = new SaveFileDialog
        {
            Title = "プリセットCSVの新しい保存先を選択",
            Filter = "CSVファイル (*.csv)|*.csv|すべてのファイル (*.*)|*.*",
            DefaultExt = ".csv",
            AddExtension = true,
            FileName = Path.GetFileName(_presetFilePath),
            InitialDirectory = Directory.Exists(Path.GetDirectoryName(_presetFilePath)) ? Path.GetDirectoryName(_presetFilePath) : null,
            OverwritePrompt = false
        };

        if (dialog.ShowDialog(this) != true) return;
        var destination = Path.GetFullPath(dialog.FileName);
        if (string.Equals(destination, _presetFilePath, StringComparison.OrdinalIgnoreCase)) return;

        PresetMigrationMode mode;
        if (File.Exists(destination))
        {
            var answer = System.Windows.MessageBox.Show(
                "選択先には既にCSVがあります。\n\nはい：既存CSVをそのまま使用\nいいえ：現在のプリセットで上書き（既存CSVは日時付きでバックアップ）\nキャンセル：変更しない",
                "保存先変更",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);
            if (answer == MessageBoxResult.Cancel) return;
            mode = answer == MessageBoxResult.Yes ? PresetMigrationMode.UseExistingDestination : PresetMigrationMode.CopyCurrentData;
        }
        else
        {
            var answer = System.Windows.MessageBox.Show(
                "新しい保存先へ現在のプリセットを移行しますか？\n\nはい：現在のプリセットをコピー\nいいえ：空のCSVを作成\nキャンセル：変更しない",
                "保存先変更",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);
            if (answer == MessageBoxResult.Cancel) return;
            mode = answer == MessageBoxResult.Yes ? PresetMigrationMode.CopyCurrentData : PresetMigrationMode.CreateEmptyDestination;
        }

        try
        {
            _storageService.ChangeLocation(_presetFilePath, destination, _vm.Presets, mode, _settings);
            _presetFilePath = destination;
            LoadData();
            System.Windows.MessageBox.Show($"保存先を変更しました。\n{_presetFilePath}", "変更完了");
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show("保存先は変更されませんでした。\n\n" + ex.Message, "変更エラー", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void OpenStorage_Click(object sender, RoutedEventArgs e)
    {
        var directory = Path.GetDirectoryName(_presetFilePath);
        if (string.IsNullOrWhiteSpace(directory)) return;
        Directory.CreateDirectory(directory);
        Process.Start(new ProcessStartInfo("explorer.exe", directory) { UseShellExecute = true });
    }

    private void ResetStorage_Click(object sender, RoutedEventArgs e)
    {
        var destination = AppPaths.DefaultPresetsFile;
        if (string.Equals(destination, _presetFilePath, StringComparison.OrdinalIgnoreCase))
        {
            System.Windows.MessageBox.Show("既に既定の保存先です。");
            return;
        }

        var answer = System.Windows.MessageBox.Show(
            "既定の Config\\presets.csv に現在のプリセットをコピーして戻しますか？\n\nはい：現在のプリセットをコピー\nいいえ：既定側に既存CSVがあれば使用\nキャンセル：変更しない",
            "既定の保存先に戻す",
            MessageBoxButton.YesNoCancel,
            MessageBoxImage.Question);
        if (answer == MessageBoxResult.Cancel) return;

        var mode = answer == MessageBoxResult.Yes
            ? PresetMigrationMode.CopyCurrentData
            : File.Exists(destination) ? PresetMigrationMode.UseExistingDestination : PresetMigrationMode.CreateEmptyDestination;

        try
        {
            SaveData();
            _storageService.ChangeLocation(_presetFilePath, destination, _vm.Presets, mode, _settings);
            _presetFilePath = destination;
            LoadData();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show("保存先は変更されませんでした。\n\n" + ex.Message, "変更エラー", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
