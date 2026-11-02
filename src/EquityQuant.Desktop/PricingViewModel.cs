using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows.Input;
using EquityQuant.Domain;
namespace EquityQuant.Desktop;
public sealed class PricingViewModel : INotifyPropertyChanged {
 public event PropertyChangedEventHandler? PropertyChanged;
 public bool IsCall { get; set; } = true;
    public double Spot{get;set;}=100; public double Strike{get;set;}=100; public double Years{get;set;}=1;
 public double Volatility{get;set;}=20;public double Rate{get;set;}=5;public double Dividend{get;set;}=0;
 string output=""; public string Output{get=>output;set{output=value;PropertyChanged?.Invoke(this,new(nameof(Output)));}}
 readonly HttpClient client=new(){BaseAddress=new Uri("http://127.0.0.1:5080"),Timeout=TimeSpan.FromSeconds(15)};
 CancellationTokenSource? cts; bool busy;
 public ICommand Calculate{get;} public ICommand Cancel{get;}
 public PricingViewModel(){Calculate=new Command(()=>{if(!busy)_=Run();});Cancel=new Command(()=>cts?.Cancel());}
 async Task Run(){busy=true;cts=new();Output="Calcul en cours…";try{
 var req=new OptionInput(Spot,Strike,Years,Rate/100,Dividend/100,Volatility/100);
 using var response=await client.PostAsJsonAsync("/api/v1/pricing",req,cts.Token);
 if(!response.IsSuccessStatusCode){Output="Requête refusée : "+await response.Content.ReadAsStringAsync(cts.Token);return;}
 var envelope=await response.Content.ReadFromJsonAsync<Envelope>(cts.Token);
 var p=envelope!.Result;Output=$"Prix : {p.Price:F6}\nDelta : {p.Delta:F6} · Gamma : {p.Gamma:F6}\nVega : {p.Vega:F6} · Theta : {p.Theta:F6} · Rho : {p.Rho:F6}\n{p.Units}";
 }catch(OperationCanceledException){Output="Calcul annulé ou délai dépassé.";}catch(HttpRequestException){Output="API indisponible. Démarrer EquityQuant.Api sur le port 5080.";}catch(Exception e){Output="Erreur : "+e.Message;}finally{busy=false;cts.Dispose();cts=null;}}
 record Envelope(PriceResult Result);
 sealed class Command(Action action):ICommand{public event EventHandler? CanExecuteChanged{add{}remove{}}public bool CanExecute(object? p)=>true;public void Execute(object? p)=>action();}
}
