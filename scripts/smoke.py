"""API and SSR checks against running services; uses only local synthetic requests."""
import json, math, sys, urllib.request, urllib.error
from pathlib import Path
sys.path.insert(0,str(Path(__file__).resolve().parents[1]/'research'))
from reference import price
API='http://127.0.0.1:5080'
checks=0

def get(path):
    return urllib.request.urlopen(API+path,timeout=10).read()

def post(path,body):
    req=urllib.request.Request(API+'/api/v1/'+path,data=json.dumps(body).encode(),headers={'Content-Type':'application/json'})
    return json.load(urllib.request.urlopen(req,timeout=10))

assert json.loads(get('/health'))['status']=='ok'
assert '/api/v1/pricing' in json.loads(get('/openapi/v1.json'))['paths']
checks+=2
for s in (70,100,140):
 for call in (True,False):
  for vol in (.1,.2,.8):
   x=dict(spot=s,strike=100,years=1,rate=.05,dividend=.02,volatility=vol,isCall=call)
   actual=post('pricing',x)['result']['price']
   assert abs(actual-price(s,100,1,.05,.02,vol,call))<3e-5,(x,actual)
   checks+=1
x=dict(spot=100,strike=100,years=1,rate=.05,dividend=0,volatility=.2,isCall=True)
p=post('pricing',x)['result']['price']
assert abs(post('calibration',dict(option=x,marketPrice=p))['volatility']-.2)<1e-7
assert len(post('scenarios',x))==11
pnl=post('pnl-explain',dict(before=x,after={**x,'spot':103,'years':.99},quantity=10))
assert abs(pnl['exact']-sum(pnl[k] for k in ('delta','gamma','vega','theta','rho','dividend','residual')))<1e-8
assert len(post('backtest',dict(spots=[100,101,99,102]))['steps'])==4
csv='asOf,symbol,spot,rate,dividend,strike,years,isCall,price\n2026-11-02T09:00:00Z,SYNTH,100,0.05,0,100,1,true,10.450583572'
assert len(post('market/import',dict(csv=csv))['quotes'])==1
checks+=5
for endpoint,body in [('pricing',{**x,'spot':-1}),('calibration',dict(option=x,marketPrice=200)),('backtest',dict(spots=[])),('market/import',dict(csv='bad'))]:
 try:
  post(endpoint,body)
  raise AssertionError('Invalid request accepted')
 except urllib.error.HTTPError as e:
  assert e.code==400
  assert json.load(e)['title']=='Paramètres invalides'
 checks+=1
html=urllib.request.urlopen('http://127.0.0.1:5081',timeout=10).read().decode()
assert 'Equity Quant Workbench' in html and '10,450' in html and 'blazor.web.js' in html
assert urllib.request.urlopen('http://127.0.0.1:5081/_framework/blazor.web.js',timeout=10).status==200
checks+=1
print(f'{checks} contrôles HTTP/référence Python/HTML réussis. Interactions navigateur non testées.')
