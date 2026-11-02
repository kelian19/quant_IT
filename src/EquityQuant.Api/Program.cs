using EquityQuant.Domain;
using EquityQuant.Pricing;
using EquityQuant.Application;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
var app = builder.Build();
app.Use(async (ctx, next) =>
{
    try { await next(ctx); }
    catch (ArgumentException e) { await Results.Problem(e.Message, statusCode: 400, title: "Paramètres invalides").ExecuteAsync(ctx); }
});
app.MapOpenApi();
app.MapPost("/api/v1/market/import", (CsvImport x) => MarketData.Import(x.Csv));
app.MapGet("/health", () => new { status = "ok", model = "BSM-1" });
app.MapPost("/api/v1/pricing", (OptionInput x) => new { calculationId = Guid.NewGuid(), asOf = DateTimeOffset.UtcNow, result = BlackScholes.Calculate(x) });
app.MapPost("/api/v1/calibration", (CalibrationRequest x) => new { volatility = BlackScholes.ImpliedVolatility(x.Option, x.MarketPrice) });
app.MapPost("/api/v1/pnl-explain", (PnlRequest x) => Analytics.Explain(x));
app.MapPost("/api/v1/backtest", (BacktestRequest x) => Analytics.Backtest(x));
app.MapPost("/api/v1/scenarios", (OptionInput x) => Enumerable.Range(-5, 11).Select(i => { var shocked = x with { Spot = x.Spot * (1 + i * .02) }; return new { shock = i * .02, result = BlackScholes.Calculate(shocked) }; }).ToArray());
app.Run();

record CsvImport(string Csv);
