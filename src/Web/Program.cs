using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AfipFacturacion.Services;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddControllers();

// Configuración para WsaaClient
builder.Services.AddScoped<WsaaClient>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var cuit = configuration["AfipSettings:Cuit"];
    var certPath = configuration["AfipSettings:CertificatePath"];
    var certificado = new X509Certificate2(certPath);
    var service = configuration["AfipSettings:Service"];
    return new WsaaClient(cuit, certificado, service);
});

// Configuración para AfipWsfeClient
builder.Services.AddScoped<AfipWsfeClient>(serviceProvider =>
{
    var wsaaClient = serviceProvider.GetRequiredService<WsaaClient>();
    var (token, sign) = wsaaClient.Authenticate();
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var cuit = configuration["AfipSettings:Cuit"];
    var isProduction = bool.Parse(configuration["AfipSettings:Production"]);
    return new AfipWsfeClient(cuit, token, sign, isProduction);
});

// Agregar servicios de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
