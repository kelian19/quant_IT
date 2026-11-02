using EquityQuant.Domain;
namespace EquityQuant.Pricing;

public static class BlackScholes
{
    static double Pdf(double x) => Math.Exp(-x * x / 2) / Math.Sqrt(2 * Math.PI);
    static double Cdf(double x)
    {
        double t = 1 / (1 + .2316419 * Math.Abs(x));
        double p = Pdf(x) * t * (.319381530 + t * (-.356563782 + t * (1.781477937 + t * (-1.821255978 + t * 1.330274429))));
        return x >= 0 ? 1 - p : p;
    }
    public static void Validate(OptionInput x)
    {
        if (x is null) throw new ArgumentException("Option requise.");
        double[] a = [x.Spot, x.Strike, x.Years, x.Rate, x.Dividend, x.Volatility];
        if (a.Any(v => !double.IsFinite(v)) || x.Spot <= 0 || x.Strike <= 0 || x.Years < 0 || x.Years > 100 || x.Volatility < 0 || x.Volatility > 5 || Math.Abs(x.Rate) > 1 || Math.Abs(x.Dividend) > 1)
            throw new ArgumentException("Entrées invalides : spot/strike positifs, maturité 0–100 ans, volatilité 0–500 %, taux/dividende entre −100 et 100 % ; valeurs finies.");
    }
    public static PriceResult Calculate(OptionInput x)
    {
        Validate(x); double s = x.Spot, k = x.Strike, t = x.Years, r = x.Rate, q = x.Dividend, v = x.Volatility;
        double sign = x.IsCall ? 1 : -1;
        if (t == 0) return new(Math.Max(sign * (s - k), 0), s == k ? sign * .5 : (sign * (s - k) > 0 ? sign : 0), 0, 0, 0, 0, "BSM-1", "À expiration : payoff ; delta conventionnel, autres Greeks nuls.");
        double dq = Math.Exp(-q * t), dr = Math.Exp(-r * t), f = s * dq - k * dr;
        if (v == 0)
        {
            bool itm = sign * f > 0;
            return new(Math.Max(sign * f, 0), itm ? sign * dq : 0, 0, 0, itm ? sign * (q * s * dq - r * k * dr) / 365 : 0, itm ? sign * k * t * dr * .01 : 0, "BSM-1", "Volatilité nulle : dérivées hors du point de rupture ; au point de rupture Greeks conventionnels nuls.");
        }
        double root = Math.Sqrt(t), d1 = (Math.Log(s / k) + (r - q + v * v / 2) * t) / (v * root), d2 = d1 - v * root;
        double price = sign * (s * dq * Cdf(sign * d1) - k * dr * Cdf(sign * d2));
        double theta = -s * dq * Pdf(d1) * v / (2 * root) - sign * r * k * dr * Cdf(sign * d2) + sign * q * s * dq * Cdf(sign * d1);
        return new(price, sign * dq * Cdf(sign * d1), dq * Pdf(d1) / (s * v * root), s * dq * Pdf(d1) * root * .01, theta / 365, sign * k * t * dr * Cdf(sign * d2) * .01);
    }
    public static double ImpliedVolatility(OptionInput x, double price)
    {
        Validate(x);
        if (!double.IsFinite(price) || x.Years == 0) throw new ArgumentException("Prix fini et maturité strictement positive requis.");
        double lower = Calculate(x with { Volatility = 0 }).Price;
        double upper = x.IsCall ? x.Spot * Math.Exp(-x.Dividend * x.Years) : x.Strike * Math.Exp(-x.Rate * x.Years);
        if (price < lower || price >= upper) throw new ArgumentException("Prix hors des bornes d’arbitrage ou volatilité non finie.");
        if (price == lower) return 0;
        double lo = 0, hi = 5;
        if (Calculate(x with { Volatility = hi }).Price < price) throw new ArgumentException("Volatilité au-delà de la limite de 500 %.");
        for (int i = 0; i < 100; i++) { double mid = (lo + hi) / 2, p = Calculate(x with { Volatility = mid }).Price; if (Math.Abs(p - price) < 1e-9) return mid; if (p < price) lo = mid; else hi = mid; }
        throw new ArgumentException("Calibration sans convergence.");
    }
}
