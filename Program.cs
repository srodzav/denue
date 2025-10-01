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
    c.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddControllers();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowFrontend", p =>
        p.WithOrigins("http://localhost:63342", "https://denue.sebastianrdz.com")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseCors("AllowFrontend");

app.MapGet("/", () => Results.Text("HEALTHY", "text/plain"));

app.Run();

public sealed class InegiOptions
{
    public string DenueToken { get; set; } = string.Empty;
}