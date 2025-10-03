using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using inegi.Models;
using Microsoft.Extensions.Options;

namespace inegi.Services;

public interface IDenueClient
{
    Task<IReadOnlyList<DenuePlace>> SearchNearbyAsync(string query, double lat, double lng, int radiusMeters, CancellationToken ct = default);
}

public sealed class DenueClient : IDenueClient
{
    private readonly HttpClient _http;
    private readonly string _token;
    private readonly JsonSerializerOptions _json;

    public DenueClient(HttpClient http, IOptions<InegiOptions> options)
    {
        _http = http;
        _token = options.Value.DenueToken ?? string.Empty;

        _json = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    public async Task<IReadOnlyList<DenuePlace>> SearchNearbyAsync(
        string query, double lat, double lng, int radiusMeters, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_token))
            throw new InvalidOperationException("Configura Inegi:DenueToken.");

        // La doc dice: radio máx. 5,000 m
        if (radiusMeters < 1 || radiusMeters > 5000)
            throw new ArgumentOutOfRangeException(nameof(radiusMeters), "Radio entre 1 y 5000 m.");

        var q = Uri.EscapeDataString(query);
        var url = $"Buscar/{q}/{lat.ToString(System.Globalization.CultureInfo.InvariantCulture)},{lng.ToString(System.Globalization.CultureInfo.InvariantCulture)}/{radiusMeters}/{_token}";

        using var resp = await _http.GetAsync(url, ct);
        if (!resp.IsSuccessStatusCode)
            return Array.Empty<DenuePlace>();

        var data = await resp.Content.ReadFromJsonAsync<List<DenuePlace>>(_json, ct)
                   ?? new List<DenuePlace>();
        return data;
    }
}