# quant_IT — Equity Quant Workbench

Application de démonstration pour les dérivés actions : valorisation Black–Scholes–Merton, Greeks, volatilité implicite, scénarios, explication de P&L et backtest de couverture delta.

## Démarrer

Installer le SDK **.NET 10.0.403** (épinglé dans `global.json`), puis depuis ce dépôt :

```sh
dotnet restore --locked-mode
dotnet build --no-restore
dotnet test --no-build
```

Dans deux terminaux :

```sh
dotnet run --no-build --project src/EquityQuant.Api --urls http://127.0.0.1:5080
dotnet run --no-build --project src/EquityQuant.Web --urls http://127.0.0.1:5081
```

Ouvrir le port local 5081 dans un navigateur sur sa machine. Sous Linux/macOS, `scripts/dev.sh` démarre les deux services après restauration ; Ctrl+C les arrête. Ces services locaux ne publient rien.

Sous Windows, démarrer l’API puis le client WPF :

```powershell
dotnet run --project src/EquityQuant.Desktop
```

Le client WPF est un ticket de pricing call/put européen ; Blazor propose l’ensemble des analyses. WPF est compilé depuis Linux avec ciblage Windows, mais son interface doit encore être exécutée et validée sur Windows.

## Fonctions disponibles

- Call/put européens, dividende continu, prix et delta/gamma/vega/theta/rho.
- Scénarios de spot entre −10 % et +10 %.
- Calibration de volatilité implicite par dichotomie bornée.
- Import CSV strict de quotes horodatées, validation globale, identification SHA-256 du contenu et calibration par quote.
- P&L de position constante : delta, gamma, vega, theta, rho, effet dividende et résiduel explicite.
- Backtest de couverture d’une option vendue : financement, dividendes, rééquilibrages et frais ; données synthétiques dans l’interface.
- API REST avec erreurs structurées et document OpenAPI sous `/openapi/v1.json`.
- Téléchargement et copie du ticket calculé au format JSON pour reproduction via l’API.

Les imports restent en mémoire. Un snapshot identifie un contenu ; il n’est ni stocké durablement ni automatiquement utilisé par le ticket de pricing. Le bouton Charger reprend les paramètres d’une quote dans le ticket. L’interface Blazor exécute le même moteur côté serveur ; le client WPF utilise l’API REST.

## Vérifier

Avec API et application web démarrées :

```sh
python3 research/reference.py
python3 scripts/smoke.py
# Recette navigateur optionnelle : pip install playwright, puis playwright install chromium
python3 scripts/browser_smoke.py
```

`CHROMIUM_PATH` permet de sélectionner un Chromium existant. Voir [la recette et les limites](docs/VALIDATION.md).

## Hypothèses et limites

Données fictives, pas de passage d’ordre, pas de connexion courtier. Options européennes, taux et rendement de dividende constants, devise unique. Temps du pricing en années ACT/365F ; backtest en pas de 1/252 année par défaut, à renseigner explicitement pour une autre convention. Greeks par unité : vega/rho pour un mouvement de 1 point de pourcentage, theta par jour. La couverture est discrète ; le résiduel n’est pas une garantie de performance.

Les décisions de couverture utilisent le spot courant, puis leurs effets sont mesurés au point suivant ; l’exécution au prix observé suppose une liquidité parfaite hors frais paramétrés. Les dividendes du backtest sont approximés sur le spot du début d’intervalle. Pas de calendrier de bourse, splits, surface sans arbitrage, ajustement historique ou gestion de trades intermédiaires. Aucun usage réel validé. Les endpoints sont locaux et sans authentification.

## Documents

- [Feuille de route](docs/ROADMAP.md)
- [Architecture](docs/ARCHITECTURE.md)
- [Exploitation et dépannage](docs/RUNBOOK.md)
- [État de réalisation](docs/IMPLEMENTATION.md)
