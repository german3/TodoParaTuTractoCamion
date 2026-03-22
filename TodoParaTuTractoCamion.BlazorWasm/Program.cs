using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TodoParaTuTractoCamion.BlazorWasm;
using TodoParaTuTractoCamion.BlazorWasm.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Leer la URL de la API desde configuración
var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
                 ?? "https://localhost:7045/"; // fallback para desarrollo

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });
builder.Services.AddScoped<CartService>();



await builder.Build().RunAsync();
