using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using EquityQuant.Domain;
using EquityQuant.Pricing;
namespace EquityQuant.Application;

public record MarketQuote(DateTimeOffset AsOf, string Symbol, double Spot, double Rate, double Dividend, double Strike, double Years, bool IsCall, double Price, double ImpliedVolatility);
public record MarketSnapshot(string Id, MarketQuote[] Quotes, string Source = "CSV utilisateur — provenance et licence à vérifier");
public static class MarketData
{
    public static MarketSnapshot Import(string csv)
    {
        if (string.IsNullOrWhiteSpace(csv) || csv.Length > 1_000_000) throw new ArgumentException("CSV vide ou supérieur à 1 Mo.");
        var lines = csv.Replace("\r", "").Trim().Split('\n');
        if (lines[0] != "asOf,symbol,spot,rate,dividend,strike,years,isCall,price" || lines.Length < 2 || lines.Length > 1001) throw new ArgumentException("En-tête invalide ou nombre de quotes hors de 1–1000.");
        var quotes = new List<MarketQuote>(); var keys = new HashSet<string>();
        foreach (var line in lines.Skip(1))
        {
            var c = line.Split(',');
            if (c.Length != 9) throw new ArgumentException("Chaque ligne doit contenir 9 colonnes ; décimales avec point, pas de virgules dans les symboles.");
            double N(int i) { if (!double.TryParse(c[i], NumberStyles.Float, CultureInfo.InvariantCulture, out var v) || !double.IsFinite(v)) throw new ArgumentException("Valeur numérique invalide."); return v; }
            if (!DateTimeOffset.TryParseExact(c[0], "yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var at) || string.IsNullOrWhiteSpace(c[1]) || c[1].Length > 32 || !bool.TryParse(c[7], out var call)) throw new ArgumentException("Horodatage UTC ISO, symbole et booléen requis.");
            var x = new OptionInput(N(2), N(5), N(6), N(3), N(4), .2, call); double price = N(8);
            var key = $"{c[0]}|{c[1]}|{x.Strike:R}|{x.Years:R}|{call}";
            if (!keys.Add(key)) throw new ArgumentException("Quote dupliquée.");
            quotes.Add(new(at, c[1], x.Spot, x.Rate, x.Dividend, x.Strike, x.Years, call, price, BlackScholes.ImpliedVolatility(x, price)));
        }
        if (quotes.Select(q => q.AsOf).Distinct().Count() != 1) throw new ArgumentException("Un snapshot doit avoir un unique horodatage.");
        return new(Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(csv))), quotes.ToArray());
    }
}
