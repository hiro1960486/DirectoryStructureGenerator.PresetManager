using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using DirectoryStructureGenerator.PresetManager.Models;
namespace DirectoryStructureGenerator.PresetManager.Services;
public sealed class MatchResult { public required Preset Preset {get;init;} public double Score {get;init;} public bool Exact {get;init;} public required IReadOnlyList<string> Differences {get;init;} public string ScoreText=>$"{Score:F0}%一致"; public string DifferencesText=>Differences.Count==0?"差分なし":"差分: "+string.Join(", ",Differences); }
public sealed class PresetCompareService {
 static readonly JsonSerializerOptions J=new(){PropertyNamingPolicy=JsonNamingPolicy.CamelCase};
 public List<MatchResult> Find(Preset target,IEnumerable<Preset> all)=>all.Where(p=>!ReferenceEquals(p,target)).Select(p=>Compare(target,p)).OrderByDescending(x=>x.Score).ToList();
 MatchResult Compare(Preset a,Preset b){var d=new List<string>();if(N(a.Source)!=N(b.Source))d.Add("source");if(N(a.Output)!=N(b.Output))d.Add("output");if(S(a.Filters)!=S(b.Filters))d.Add("filters_json");if(S(a.Formats)!=S(b.Formats))d.Add("formats_json");if(S(a.Organizer)!=S(b.Organizer))d.Add("organizer_json");return new(){Preset=b,Score=(5-d.Count)/5d*100,Exact=d.Count==0,Differences=d};}
 static string N(string? s)=>(s??"").Trim().Replace('\\','/'); static string S<T>(T value)=>JsonSerializer.Serialize(value,J);
}
