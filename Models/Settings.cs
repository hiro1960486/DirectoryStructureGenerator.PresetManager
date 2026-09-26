using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace DirectoryStructureGenerator.PresetManager.Models;
public sealed class FilterSettings {
 [JsonPropertyName("excluded_dirs")] public List<string> ExcludedDirs { get; set; } = new();
 [JsonPropertyName("excluded_extensions")] public List<string> ExcludedExtensions { get; set; } = new();
 [JsonPropertyName("included_extensions")] public List<string> IncludedExtensions { get; set; } = new();
 [JsonPropertyName("patterns")] public List<string> Patterns { get; set; } = new();
 [JsonPropertyName("include_hidden")] public bool IncludeHidden { get; set; }
 [JsonPropertyName("include_empty_dirs")] public bool IncludeEmptyDirs { get; set; }
 [JsonPropertyName("follow_symlinks")] public bool FollowSymlinks { get; set; }
 [JsonPropertyName("max_depth")] public int MaxDepth { get; set; } = -1;
 public FilterSettings Clone() => new() { ExcludedDirs=new(ExcludedDirs), ExcludedExtensions=new(ExcludedExtensions), IncludedExtensions=new(IncludedExtensions), Patterns=new(Patterns), IncludeHidden=IncludeHidden, IncludeEmptyDirs=IncludeEmptyDirs, FollowSymlinks=FollowSymlinks, MaxDepth=MaxDepth };
}
public sealed class FormatSettings {
 [JsonPropertyName("txt")] public bool Txt { get; set; } = true;
 [JsonPropertyName("html")] public bool Html { get; set; } = true;
 [JsonPropertyName("csv")] public bool Csv { get; set; } = true;
 [JsonPropertyName("json")] public bool Json { get; set; }
 public FormatSettings Clone()=>new(){Txt=Txt,Html=Html,Csv=Csv,Json=Json};
}
public sealed class OrganizerSettings {
 [JsonPropertyName("default_destination")] public string DefaultDestination { get; set; } = "";
 [JsonPropertyName("naming_template")] public string NamingTemplate { get; set; } = "{name}";
 [JsonPropertyName("custom_template")] public string CustomTemplate { get; set; } = "{name}";
 [JsonPropertyName("keep_subfolders")] public bool KeepSubfolders { get; set; } = true;
 [JsonPropertyName("collision")] public string Collision { get; set; } = "number";
 public OrganizerSettings Clone()=>new(){DefaultDestination=DefaultDestination,NamingTemplate=NamingTemplate,CustomTemplate=CustomTemplate,KeepSubfolders=KeepSubfolders,Collision=Collision};
}
