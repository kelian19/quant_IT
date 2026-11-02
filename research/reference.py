"""Independent BSM reference: standard-library erf CDF, no shared C# implementation."""
import math

def price(spot, strike, years, rate, dividend, volatility, call=True):
    if years == 0:
        return max((1 if call else -1) * (spot-strike), 0)
    if volatility == 0:
        return max((1 if call else -1) * (spot*math.exp(-dividend*years)-strike*math.exp(-rate*years)), 0)
    n = lambda x: (1+math.erf(x/math.sqrt(2)))/2
    d1=(math.log(spot/strike)+(rate-dividend+volatility**2/2)*years)/(volatility*math.sqrt(years))
    d2=d1-volatility*math.sqrt(years)
    sign=1 if call else -1
    return sign*(spot*math.exp(-dividend*years)*n(sign*d1)-strike*math.exp(-rate*years)*n(sign*d2))

if __name__ == '__main__':
    assert abs(price(100,100,1,.05,0,.2)-10.450583572185565)<1e-10
    print('Référence Python BSM vérifiée.')
