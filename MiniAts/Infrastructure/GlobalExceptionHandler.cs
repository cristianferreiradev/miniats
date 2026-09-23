using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace MiniAts.Infrastructure;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Erro não tratado: {Mensagem}", exception.Message);

        var problema = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Erro interno do servidor",
            Detail = "Ocorreu um erro ao processar a requisição.",
            Instance = httpContext.Request.Path
        };

        problema.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = problema.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problema, cancellationToken);

        return true;
    }
}