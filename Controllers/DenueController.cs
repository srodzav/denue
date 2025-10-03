using inegi.Models;
using inegi.Services;
using Microsoft.AspNetCore.Mvc;

namespace inegi.Controllers;

[ApiController]
[Route("denue")]
public sealed class DenueController : ControllerBase
{
    private readonly IDenueClient _client;
    public DenueController(IDenueClient client) => _client = client;

    /// GET /denue/search?query=taqueria&lat=xx.xxxx&lng=yy.yyyy&radius=zzzz
    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<DenuePlace>>> Search(
        [FromQuery] string query,
        [FromQuery] double lat,
        [FromQuery] double lng,
        [FromQuery] int radius = 1000) // 1 km
    {
        try
        {
            var results = await _client.SearchNearbyAsync(query, lat, lng, radius, HttpContext.RequestAborted);

            // Ordenar por distancia (Haversine)
            foreach (var r in results)
            {
                if (r.Latitud is { } la && r.Longitud is { } lo)
                    r.DistanceMeters = Haversine(lat, lng, la, lo);
            }

            return Ok(results.Where(r => r.DistanceMeters.HasValue)
                .OrderBy(r => r.DistanceMeters)
                .ToList());
        }
        catch (HttpRequestException ex)
        {
            return Ok(Array.Empty<DenuePlace>());
        }
    }

    private static double Haversine(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371000; // metros
        double dLat = ToRad(lat2 - lat1);
        double dLon = ToRad(lon2 - lon1);
        double a = Math.Sin(dLat/2)*Math.Sin(dLat/2) +
                   Math.Cos(ToRad(lat1))*Math.Cos(ToRad(lat2)) *
                   Math.Sin(dLon/2)*Math.Sin(dLon/2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a));
        return R * c;
    }
    private static double ToRad(double deg) => deg * Math.PI / 180.0;
}