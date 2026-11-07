using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
namespace Taburai.Store.Exceptions;

public sealed class ApiExceptionHandler : IExceptionHandler
{
  public async ValueTask<bool> TryHandleAsync(HttpContext ctx, Exception ex, CancellationToken ct)
  {
    var (status, title) = ex switch
    {
      NotFoundException => (404, ex.Message),
      ConflictException => (409, ex.Message),
      BadRequestException => (400, ex.Message),
      SqlException { Number: 2627 or 2601 } => (409, "Duplicate value"),
      SqlException { Number: 547 } => (409, "Operation violates a database constraint (data is referenced or invalid)"),
      _ => (0, "")
    };

    if (status == 0) return false; // error tak terduga tetap 500

    ctx.Response.StatusCode = status;
    await ctx.Response.WriteAsJsonAsync(new ProblemDetails { Status = status, Title = title }, ct);
    return true;
  }
}