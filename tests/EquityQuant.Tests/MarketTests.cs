using EquityQuant.Application;
namespace EquityQuant.Tests;

public class MarketTests
{
    const string Csv = "asOf,symbol,spot,rate,dividend,strike,years,isCall,price\n2026-11-02T09:00:00Z,SYNTH,100,0.05,0,100,1,true,10.450583572";
    [Fact] public void CalibrateAndHash() { var a = MarketData.Import(Csv); Assert.Single(a.Quotes); Assert.InRange(a.Quotes[0].ImpliedVolatility, .19999, .20001); Assert.Equal(a.Id, MarketData.Import(Csv).Id); }
    [Fact] public void RejectInvalidQuote() { Assert.Throws<ArgumentException>(() => MarketData.Import(Csv.Replace("10.450583572", "200"))); }
    [Fact] public void RejectDuplicates() { Assert.Throws<ArgumentException>(() => MarketData.Import(Csv + "\n" + Csv.Split('\n')[1])); }
}
