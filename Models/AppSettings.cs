using System.Text.Json.Serialization;

namespace DirectoryStructureGenerator.PresetManager.Models;

public sealed class AppSettings
{
    [JsonPropertyName("presetFilePath")]
    public string PresetFilePath { get; set; } = "Config\\presets.csv";
}
