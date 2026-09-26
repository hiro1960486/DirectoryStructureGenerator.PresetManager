using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using DirectoryStructureGenerator.PresetManager.Models;
namespace DirectoryStructureGenerator.PresetManager.Services;
public sealed class PresetCsvService {
 static readonly JsonSerializerOptions J=new(){PropertyNameCaseInsensitive=true,WriteIndented=false};
 static readonly string[] Headers={"default","quick","order","favorite","name","memo","source","output","filters_json","formats_json","organizer_json"};
 public List<Preset> Load(string path){if(!File.Exists(path))return new();var rows=CsvCodec.Parse(File.ReadAllText(path,Encoding.UTF8));if(rows.Count<2)return new();var index=rows[0].Select((v,i)=>(v:v.Trim().TrimStart('\uFEFF'),i)).ToDictionary(x=>x.v,x=>x.i,StringComparer.OrdinalIgnoreCase);string V(List<string> r,string n)=>index.TryGetValue(n,out int i)&&i<r.Count?r[i]:"";return rows.Skip(1).Where(r=>r.Any(x=>!string.IsNullOrWhiteSpace(x))).Select(r=>new Preset{Default=Bool(V(r,"default")),Quick=Bool(V(r,"quick")),Order=int.TryParse(V(r,"order"),out int o)?o:0,Favorite=Bool(V(r,"favorite")),Name=V(r,"name"),Memo=V(r,"memo"),Source=V(r,"source"),Output=V(r,"output"),Filters=Deserialize<FilterSettings>(V(r,"filters_json")),Formats=Deserialize<FormatSettings>(V(r,"formats_json")),Organizer=Deserialize<OrganizerSettings>(V(r,"organizer_json"))}).ToList();}
 public void Save(string path,IEnumerable<Preset> presets){Directory.CreateDirectory(Path.GetDirectoryName(path)!);var rows=new List<IEnumerable<string>>{Headers};rows.AddRange(presets.OrderBy(p=>p.Order).Select(p=>new[]{p.Default.ToString().ToLowerInvariant(),p.Quick.ToString().ToLowerInvariant(),p.Order.ToString(),p.Favorite.ToString().ToLowerInvariant(),p.Name,p.Memo,p.Source,p.Output,JsonSerializer.Serialize(p.Filters,J),JsonSerializer.Serialize(p.Formats,J),JsonSerializer.Serialize(p.Organizer,J)}));AtomicWrite(path,CsvCodec.Write(rows));}
 static void AtomicWrite(string path,string content){var temp=path+".tmp";File.WriteAllText(temp,content,new UTF8Encoding(true));if(File.Exists(path)){var backup=path+".previous_"+DateTime.Now.ToString("yyyyMMdd_HHmmssfff");File.Replace(temp,path,backup,true);}else File.Move(temp,path);}
 static bool Bool(string s)=>bool.TryParse(s,out bool value)&&value;
 static T Deserialize<T>(string s) where T:new(){try{return string.IsNullOrWhiteSpace(s)?new T():JsonSerializer.Deserialize<T>(s,J)??new T();}catch{return new T();}}
}
