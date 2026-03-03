using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Valora.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(ILogger<WeatherForecastController> logger) : ControllerBase
{
    // 1. Criamos um campo privado para armazenar o Logger

    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    // 2. Injetamos o Logger via Construtor (O Serilog entra aqui automaticamente)

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        // 3. Adicionamos um Log Estruturado
        // Diferente de string.Format, aqui usamos {Chaves}. 
        // O Serilog guarda "QuantidadeDias" como um dado pesquisável no JSON, e não apenas texto solto.
        logger.LogInformation("Gerando previsão do tempo para os próximos {QuantidadeDias} dias. Solicitado às {Hora}", 5, DateTime.Now.ToLongTimeString());

        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }
}