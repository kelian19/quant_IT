using EquityQuant.Domain;
using EquityQuant.Pricing;
using EquityQuant.Application;
namespace EquityQuant.Tests;

public class FinancialTests
{
    static void Near(double a, double b, double tol = 1e-4) => Assert.True(Math.Abs(a - b) < tol, $"{a} != {b}");
    [Fact] public void PublishedReference() { var p = BlackScholes.Calculate(new()); Near(p.Price, 10.450583572); Near(p.Delta, .636830651); Near(p.Gamma, .018762017); Near(p.Vega, .375240346); }
    [Theory][InlineData(80)][InlineData(100)][InlineData(120)] public void Parity(double spot) { var x = new OptionInput(spot, 100, 1, .05, .02, .3); Near(BlackScholes.Calculate(x).Price - BlackScholes.Calculate(x with { IsCall = false }).Price, spot * Math.Exp(-.02) - 100 * Math.Exp(-.05)); }
    [Fact] public void GreeksFiniteDifferences() { var x = new OptionInput(); double P(OptionInput a) => BlackScholes.Calculate(a).Price; var g = BlackScholes.Calculate(x); double h = .01; Near(g.Delta, (P(x with { Spot = 100 + h }) - P(x with { Spot = 100 - h })) / (2 * h)); Near(g.Gamma, (P(x with { Spot = 100 + h }) - 2 * P(x) + P(x with { Spot = 100 - h })) / (h * h), 2e-4); Near(g.Vega, (P(x with { Volatility = .2001 }) - P(x with { Volatility = .1999 })) / .0002 * .01); Near(g.Rho, (P(x with { Rate = .0501 }) - P(x with { Rate = .0499 })) / .0002 * .01); Near(g.Theta, (P(x with { Years = .9999 }) - P(x with { Years = 1.0001 })) / .0002 / 365); }
    [Fact] public void Limits() { Near(BlackScholes.Calculate(new(120, 100, 0)).Price, 20); Near(BlackScholes.Calculate(new(100, 100, 1, .05, 0, 0)).Price, 100 - 100 * Math.Exp(-.05)); }
    [Theory][InlineData(.1)][InlineData(.2)][InlineData(.8)] public void CalibrationRoundTrip(double vol) { var x = new OptionInput(100, 110, 2, .03, .01, vol); Near(BlackScholes.ImpliedVolatility(x, BlackScholes.Calculate(x).Price), vol, 1e-7); }
    [Fact] public void InvalidInputs() { Assert.Throws<ArgumentException>(() => BlackScholes.Calculate(new(Spot: double.NaN))); Assert.Throws<ArgumentException>(() => BlackScholes.ImpliedVolatility(new(), 101)); Assert.Throws<ArgumentException>(() => BlackScholes.Calculate(new(Years: -1))); }
    [Fact] public void PnlReconciles() { var a = new OptionInput(); var p = Analytics.Explain(new(a, a with { Spot = 103, Volatility = .22, Years = .99, Rate = .06, Dividend = .01 }, 20)); Near(p.Exact, p.Delta + p.Gamma + p.Vega + p.Theta + p.Rho + p.Dividend + p.Residual, 1e-10); }
    [Fact] public void UnchangedPnl() { var x = new OptionInput(); Near(Analytics.Explain(new(x, x)).Exact, 0); }
    [Fact] public void BacktestDeterministic() { var x = new BacktestRequest([100, 101, 99, 102]); var a = Analytics.Backtest(x); var b = Analytics.Backtest(x); Near(a.TerminalError, b.TerminalError, 1e-12); Assert.Equal(4, a.Steps.Length); Assert.True(a.TotalCosts > 0); Near(a.Steps[^1].HedgeValue - a.Steps[^1].Option, a.TerminalError); }
    [Fact] public void DecisionsDoNotUseFuture() { var a = Analytics.Backtest(new([100, 101, 102, 103])); var b = Analytics.Backtest(new([100, 101, 90, 80])); Assert.Equal(a.Steps[0], b.Steps[0]); Assert.Equal(a.Steps[1], b.Steps[1]); }
    [Fact] public void ZeroVolHedgeReplicates() { var a = Analytics.Backtest(new([120, 121, 122], 100, 0, 0, 0, 1.0 / 252, 0)); Near(a.TerminalError, 0, 1e-10); }
    [Fact] public void CostsReduceHedge() { var spots = new double[] { 100, 102, 99, 101 }; var free = Analytics.Backtest(new(spots, CostBps: 0)); var costly = Analytics.Backtest(new(spots, CostBps: 10)); Assert.True(costly.TerminalError < free.TerminalError); }
}
