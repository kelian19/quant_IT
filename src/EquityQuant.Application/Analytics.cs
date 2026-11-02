using EquityQuant.Domain;
using EquityQuant.Pricing;
namespace EquityQuant.Application;

public static class Analytics
{
    public static PnlResult Explain(PnlRequest req)
    {
        var a = req.Before; var b = req.After; var g = BlackScholes.Calculate(a); var end = BlackScholes.Calculate(b);
        if (a.Strike != b.Strike || a.IsCall != b.IsCall || b.Years > a.Years || !double.IsFinite(req.Quantity) || Math.Abs(req.Quantity) > 1e9)
            throw new ArgumentException("Même contrat requis, temps décroissant et quantité finie limitée à 1 milliard.");
        double ds = b.Spot - a.Spot, n = req.Quantity;
        double delta = g.Delta * ds, gamma = .5 * g.Gamma * ds * ds, vega = g.Vega * (b.Volatility - a.Volatility) * 100,
            theta = g.Theta * (a.Years - b.Years) * 365, rho = g.Rho * (b.Rate - a.Rate) * 100;
        double div = BlackScholes.Calculate(a with { Dividend = b.Dividend }).Price - g.Price;
        double exact = end.Price - g.Price, res = exact - delta - gamma - vega - theta - rho - div;
        return new(exact * n, delta * n, gamma * n, vega * n, theta * n, rho * n, div * n, res * n);
    }
    public static BacktestResult Backtest(BacktestRequest x)
    {
        if (x.Spots is null || x.Spots.Length < 2 || x.Spots.Length > 5000 || x.Spots.Any(s => !double.IsFinite(s) || s <= 0) || !double.IsFinite(x.StepYears) || x.StepYears <= 0 || !double.IsFinite(x.CostBps) || x.CostBps < 0 || x.CostBps > 1000)
            throw new ArgumentException("Trajectoire de 2 à 5 000 spots positifs, pas temporel positif et coût entre 0 et 1 000 bps requis.");
        int count = x.Spots.Length; double dt = x.StepYears;
        OptionInput Input(int i) => new(x.Spots[i], x.Strike, (count - 1 - i) * dt, x.Rate, x.Dividend, x.Volatility, x.IsCall);
        var first = BlackScholes.Calculate(Input(0)); double holdings = first.Delta;
        double costs = Math.Abs(holdings) * x.Spots[0] * x.CostBps / 10000;
        double cash = first.Price - holdings * x.Spots[0] - costs;
        var steps = new List<HedgeStep> { new(0, x.Spots[0], first.Price, holdings, cash, cash + holdings * x.Spots[0], costs) };
        for (int i = 1; i < count; i++)
        {
            // Previous holdings earn dividends over the interval; no future spot is used in decisions.
            cash = cash * Math.Exp(x.Rate * dt) + holdings * x.Spots[i - 1] * (Math.Exp(x.Dividend * dt) - 1);
            var p = BlackScholes.Calculate(Input(i)); double target = i == count - 1 ? 0 : p.Delta;
            double trade = target - holdings, fee = Math.Abs(trade) * x.Spots[i] * x.CostBps / 10000;
            cash -= trade * x.Spots[i] + fee; holdings = target; costs += fee;
            steps.Add(new(i, x.Spots[i], p.Price, p.Delta, cash, cash + holdings * x.Spots[i], fee));
        }
        var last = steps[^1];
        return new(steps.ToArray(), last.HedgeValue - last.Option, first.Price * Math.Exp(x.Rate * (count - 1) * dt) - last.Option, costs);
    }
}
