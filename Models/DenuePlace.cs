using System.Text.Json.Serialization;

namespace inegi.Models;

public sealed class DenuePlace
{
    [JsonPropertyName("CLEE")] public string? Clee { get; set; }
    [JsonPropertyName("Id")] public string? Id { get; set; }
    [JsonPropertyName("Nombre")] public string? Nombre { get; set; }
    [JsonPropertyName("Razon_social")] public string? RazonSocial { get; set; }
    [JsonPropertyName("Clase_actividad")] public string? ClaseActividad { get; set; }
    [JsonPropertyName("Estrato")] public string? Estrato { get; set; }
    [JsonPropertyName("Tipo_vialidad")] public string? TipoVialidad { get; set; }
    [JsonPropertyName("Calle")] public string? Calle { get; set; }
    [JsonPropertyName("Num_Exterior")] public string? NumExterior { get; set; }
    [JsonPropertyName("Num_Interior")] public string? NumInterior { get; set; }
    [JsonPropertyName("Colonia")] public string? Colonia { get; set; }
    [JsonPropertyName("CP")] public string? CP { get; set; }
    [JsonPropertyName("Ubicacion")] public string? Ubicacion { get; set; }
    [JsonPropertyName("Telefono")] public string? Telefono { get; set; }
    [JsonPropertyName("Correo_e")] public string? Correo { get; set; }
    [JsonPropertyName("Sitio_internet")] public string? SitioInternet { get; set; }
    [JsonPropertyName("Tipo")] public string? Tipo { get; set; }
    [JsonPropertyName("Longitud")] public string? LongitudStr { get; set; }
    [JsonPropertyName("Latitud")]  public string? LatitudStr  { get; set; }

    [JsonIgnore] public double? Longitud => Parse(LongitudStr);
    [JsonIgnore] public double? Latitud  => Parse(LatitudStr);
    [JsonIgnore] public double? DistanceMeters { get; set; }

    private static double? Parse(string? s) =>
        double.TryParse(s, System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : null;
}