using System.Net;
using System.Text.Json;

namespace EventosVivos.API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (KeyNotFoundException ex)
        {
            await EscribirRespuesta(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (ArgumentException ex)
        {
            await EscribirRespuesta(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            await EscribirRespuesta(context, HttpStatusCode.Conflict, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado");
            await EscribirRespuesta(context, HttpStatusCode.InternalServerError, "Ocurrió un error inesperado");
        }
    }

    private static async Task EscribirRespuesta(HttpContext context, HttpStatusCode statusCode, string mensaje)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var respuesta = JsonSerializer.Serialize(new
        {
            status = (int)statusCode,
            error = mensaje,
            timestamp = DateTime.UtcNow
        });

        await context.Response.WriteAsync(respuesta);
    }
}