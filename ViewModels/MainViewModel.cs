using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using DirectoryStructureGenerator.PresetManager.Models;
using DirectoryStructureGenerator.PresetManager.Services;
namespace DirectoryStructureGenerator.PresetManager.ViewModels;
public sealed class MainViewModel:INotifyPropertyChanged {
 readonly PresetCompareService _compare=new(); string _search=""; string _presetFilePath=""; Preset? _selected;
 public ObservableCollection<Preset> Presets {get;}=new(); public ICollectionView View {get;}
 public MainViewModel(){View=CollectionViewSource.GetDefaultView(Presets);View.Filter=o=>o is Preset p&&(string.IsNullOrWhiteSpace(Search)||p.Name.Contains(Search,System.StringComparison.OrdinalIgnoreCase)||p.Memo.Contains(Search,System.StringComparison.OrdinalIgnoreCase)||p.Filters.IncludedExtensions.Any(x=>x.Contains(Search,System.StringComparison.OrdinalIgnoreCase)));}
 public string Search{get=>_search;set{_search=value;On();View.Refresh();}}
 public string PresetFilePath{get=>_presetFilePath;set{_presetFilePath=value;On();}}
 public Preset? Selected{get=>_selected;set{_selected=value;On();On(nameof(Matches));On(nameof(ExactMessage));}}
 public IReadOnlyList<MatchResult> Matches=>Selected==null?new List<MatchResult>():_compare.Find(Selected,Presets).Where(x=>x.Score>=40).Take(5).ToList();
 public string ExactMessage=>Matches.FirstOrDefault(x=>x.Exact) is MatchResult m?$"「{m.Preset.Name}」と完全一致":"完全一致なし";
 public void Replace(IEnumerable<Preset> items){Presets.Clear();foreach(var p in items.OrderBy(x=>x.Order))Presets.Add(p);Selected=Presets.FirstOrDefault(x=>x.Default)??Presets.FirstOrDefault();}
 public void RefreshComputed(){On(nameof(Matches));On(nameof(ExactMessage));View.Refresh();}
 public event PropertyChangedEventHandler? PropertyChanged;void On([CallerMemberName]string? n=null)=>PropertyChanged?.Invoke(this,new(n));
}
