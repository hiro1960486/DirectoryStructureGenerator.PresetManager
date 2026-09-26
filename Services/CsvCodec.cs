using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DirectoryStructureGenerator.PresetManager.Services;
public static class CsvCodec {
 public static List<List<string>> Parse(string text){var rows=new List<List<string>>();var row=new List<string>();var cell=new StringBuilder();bool quoted=false;for(int i=0;i<text.Length;i++){char c=text[i];if(quoted){if(c=='"'&&i+1<text.Length&&text[i+1]=='"'){cell.Append('"');i++;}else if(c=='"')quoted=false;else cell.Append(c);}else if(c=='"')quoted=true;else if(c==','){row.Add(cell.ToString());cell.Clear();}else if(c=='\r'){}else if(c=='\n'){row.Add(cell.ToString());cell.Clear();rows.Add(row);row=new();}else cell.Append(c);}if(cell.Length>0||row.Count>0){row.Add(cell.ToString());rows.Add(row);}return rows;}
 public static string Write(IEnumerable<IEnumerable<string>> rows)=>string.Join("\r\n",rows.Select(r=>string.Join(",",r.Select(Escape))))+"\r\n";
 static string Escape(string? value){value??="";return value.IndexOfAny(new[]{',','"','\r','\n'})>=0?"\""+value.Replace("\"","\"\"")+"\"":value;}
}
