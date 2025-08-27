using System.Text.Json.Serialization;

namespace PlantTracker.Core.Constants;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Duration
{
    Unknown = 0,
    Annual = 1,
    Perennial = 2,
}