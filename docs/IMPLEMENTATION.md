# État de réalisation

Cette livraison fournit un prototype fonctionnel transversal, pas la totalité des critères de la feuille de route initiale de 12 à 16 semaines.

| Lot | Réalisé | Travail restant |
|---|---|---|
| Cadrage | Conventions explicites, parcours synthétiques | Revue et validation avec un métier réel |
| Pricing | BSM, Greeks, cas limites, référence Python, tests | Corpus étendu sur extrêmes, validation indépendante métier |
| Marché | CSV, horodatage unique, SHA-256, volatilité implicite | Stockage immuable durable, fraîcheur, surface et arbitrage |
| API | REST, OpenAPI, validation, identifiants de pricing | Corrélation complète, authentification avant exposition |
| Blazor | Ticket, import, courbe de scénarios, P&L, backtest, JSON | Graphiques P&L/backtest, téléchargements CSV |
| P&L | Attribution locale et revalorisation exacte, résiduel | Transactions/cash-flows réels, attribution métier approuvée |
| Backtest | Delta discret, cash, dividendes approchés, frais | Données historiques ajustées, calendriers et délai d’exécution réaliste |
| WPF | Ticket call/put MVVM, API asynchrone, annulation | Recette Windows, scénarios et P&L dans le client |
| Exploitation | Scripts, dépendances verrouillées, CI et runbook | Packaging signé, recette sur machine Windows vierge, intégration SI |

Les tests ajoutés sont décrits dans VALIDATION.md. Le workflow CI est fourni mais son exécution sur GitHub n’est pas établie par les tests locaux.
