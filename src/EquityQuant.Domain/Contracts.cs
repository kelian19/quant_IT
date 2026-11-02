namespace EquityQuant.Domain;

public record OptionInput(double Spot = 100, double Strike = 100, double Years = 1,
    double Rate = .05, double Dividend = 0, double Volatility = .2, bool IsCall = true);
public record PriceResult(double Price, double Delta, double Gamma, double Vega, double Theta, double Rho,
    string Model = "BSM-1", string Units = "prix/unité; vega et rho par point (1%); theta par jour ACT/365F");
public record CalibrationRequest(OptionInput Option, double MarketPrice);
public record PnlRequest(OptionInput Before, OptionInput After, double Quantity = 1);
public record PnlResult(double Exact, double Delta, double Gamma, double Vega, double Theta, double Rho, double Dividend, double Residual);
public record BacktestRequest(double[] Spots, double Strike = 100, double Rate = .05,
    double Dividend = 0, double Volatility = .2, double StepYears = 1.0 / 252, double CostBps = 1, bool IsCall = true);
public record HedgeStep(int Day, double Spot, double Option, double Delta, double Cash, double HedgeValue, double Costs);
public record BacktestResult(HedgeStep[] Steps, double TerminalError, double UnhedgedPnl, double TotalCosts,
    string Convention = "Option vendue, couverture delta rééquilibrée après observation; taux/dividendes continus; trajectoire synthétique, sans prédiction de performance");
