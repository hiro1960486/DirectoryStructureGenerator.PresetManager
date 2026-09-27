using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
namespace DirectoryStructureGenerator.PresetManager.Models;
public sealed class Preset : INotifyPropertyChanged {
 bool _default,_quick,_favorite; int _order; string _name="",_memo="",_source="",_output="";
 public bool Default { get=>_default; set=>Set(ref _default,value); }
 public bool Quick { get=>_quick; set=>Set(ref _quick,value); }
 public int Order { get=>_order; set=>Set(ref _order,value); }
 public bool Favorite { get=>_favorite; set=>Set(ref _favorite,value); }
 public string Name { get=>_name; set=>Set(ref _name,value); }
 public string Memo { get=>_memo; set=>Set(ref _memo,value); }
 public string Source { get=>_source; set=>Set(ref _source,value); }
 public string Output { get=>_output; set=>Set(ref _output,value); }
 public FilterSettings Filters { get; set; } = new();
 public FormatSettings Formats { get; set; } = new();
 public OrganizerSettings Organizer { get; set; } = new();
 [JsonIgnore] public string Badges => (Default?"◎":"")+(Favorite?"★":"")+(Quick?"⚡":"");
 [JsonIgnore] public string SourceDisplay => string.IsNullOrWhiteSpace(Source)?"実行時に選択":Source;
    [JsonIgnore] public string OutputDisplay => string.IsNullOrWhiteSpace(Output)?"アプリの既定値":Output;
 [JsonIgnore] public string IncludedExtensionsDisplay => Filters.IncludedExtensions.Count==0?"すべて":string.Join(", ",Filters.IncludedExtensions);
 [JsonIgnore] public string ExcludedDirsDisplay => Filters.ExcludedDirs.Count==0?"なし":string.Join(", ",Filters.ExcludedDirs);
 [JsonIgnore] public string ExcludedExtensionsDisplay => Filters.ExcludedExtensions.Count==0?"なし":string.Join(", ",Filters.ExcludedExtensions);
 [JsonIgnore] public string FormatDisplay => string.Join(" / ", new[]{Formats.Txt?"TXT":null,Formats.Html?"HTML":null,Formats.Csv?"CSV":null,Formats.Json?"JSON":null}.Where(x=>x!=null));
 public Preset DeepClone()=>new(){Default=Default,Quick=Quick,Order=Order,Favorite=Favorite,Name=Name,Memo=Memo,Source=Source,Output=Output,Filters=Filters.Clone(),Formats=Formats.Clone(),Organizer=Organizer.Clone()};
 public event PropertyChangedEventHandler? PropertyChanged;
 void Set<T>(ref T field,T value,[CallerMemberName]string? name=null){if(EqualityComparer<T>.Default.Equals(field,value))return;field=value;PropertyChanged?.Invoke(this,new(name));PropertyChanged?.Invoke(this,new(nameof(Badges)));}
}
