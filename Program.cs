using inegi.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Options para token
builder.Services.Configure<InegiOptions>(
    builder.Configuration.GetSection("Inegi"));

// HttpClient
builder.Services.AddHttpClient<IDenueClient, DenueClient>(c =>
{
    c.BaseAddress = new Uri("https://www.inegi.org.mx/app/api/denue/v1/consulta/");
    c.Timeout = TimeSpan.FromSeconds(12);
    c.DefaultRequestHeaders.UserAgent.ParseAdd("denue-demo/1.0 (+https://denue.sebastianrdz.com)");
    c.DefaultRequestHeaders.Accept.ParseAdd("application/json");
});

builder.Services.AddControllers();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowFrontend", p =>
        p.SetIsOriginAllowed(origin =>
        {
            if (string.IsNullOrEmpty(origin)) return false;
            if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri)) return false;

            if (uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase))
                return uri.Port == 63342 || uri.Port == 5173 || uri.Port == 5163;

            return uri.Host.Equals("denue.sebastianrdz.com", StringComparison.OrdinalIgnoreCase)
                   || uri.Host.EndsWith(".sebastianrdz.com", StringComparison.OrdinalIgnoreCase);
        })
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.MapControllers();
app.MapControllers().RequireCors("AllowFrontend");

app.MapGet("/", () => Results.Text("HEALTHY", "text/plain"));

app.Run();

public sealed class InegiOptions
{
    public string DenueToken { get; set; } = string.Empty;
}