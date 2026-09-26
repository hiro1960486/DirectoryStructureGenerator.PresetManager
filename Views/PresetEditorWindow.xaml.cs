using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DirectoryStructureGenerator.PresetManager.Models;
using DirectoryStructureGenerator.PresetManager.Services;
using Microsoft.Win32;
namespace DirectoryStructureGenerator.PresetManager.Views;
public partial class PresetEditorWindow : Window {
 readonly Preset _preset; readonly IEnumerable<Preset> _all; readonly Preset? _original; readonly PresetCompareService _compare=new();
 public PresetEditorWindow(Preset preset,IEnumerable<Preset> all,Preset? original=null){InitializeComponent();_preset=preset;_all=all;_original=original;DataContext=preset;Loaded+=(_,_)=>{IncludedText.Text=Join(preset.Filters.IncludedExtensions);ExcludedDirsText.Text=Join(preset.Filters.ExcludedDirs);ExcludedExtText.Text=Join(preset.Filters.ExcludedExtensions);PatternsText.Text=Join(preset.Filters.Patterns);};}
 static string Join(IEnumerable<string> values)=>string.Join(Environment.NewLine,values);
 static List<string> Split(string value)=>value.Split(new[]{',',';','\r','\n'},StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
 void Sync(){_preset.Filters.IncludedExtensions=Split(IncludedText.Text);_preset.Filters.ExcludedDirs=Split(ExcludedDirsText.Text);_preset.Filters.ExcludedExtensions=Split(ExcludedExtText.Text);_preset.Filters.Patterns=Split(PatternsText.Text);}
 void Save_Click(object sender,RoutedEventArgs e){Sync();if(string.IsNullOrWhiteSpace(_preset.Name)){System.Windows.MessageBox.Show("名称を入力してください。");return;}var exact=_compare.Find(_preset,_all.Where(x=>!ReferenceEquals(x,_original))).FirstOrDefault(x=>x.Exact);if(exact!=null&&System.Windows.MessageBox.Show($"「{exact.Preset.Name}」と設定内容が完全一致します。別名で保存しますか？","完全一致",MessageBoxButton.YesNo,MessageBoxImage.Warning)!=MessageBoxResult.Yes)return;if(_preset.Default)foreach(var p in _all.Where(x=>!ReferenceEquals(x,_original)))p.Default=false;DialogResult=true;}
 void BrowseSource_Click(object sender,RoutedEventArgs e){var d=new OpenFolderDialog{Title="入力元フォルダーを選択"};if(d.ShowDialog(this)==true)_preset.Source=d.FolderName;}
 void BrowseOutput_Click(object sender,RoutedEventArgs e){var d=new OpenFolderDialog{Title="出力先フォルダーを選択"};if(d.ShowDialog(this)==true)_preset.Output=d.FolderName;}
}
