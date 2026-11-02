# Exploitation locale

## Prérequis et démarrage

Utiliser le checkout existant ; aucune worktree supplémentaire nécessaire. SDK .NET épinglé par `global.json`. NuGet doit pouvoir accéder à api.nuget.org. Aucun secret ni compte client nécessaire.

Exécuter les commandes du README depuis la racine. Les dépendances et outputs de build restent sur disque ; les processus API/Blazor sont à redémarrer dans chaque session. Après une restauration de snapshot cloud, ne pas supposer que les services sont encore actifs.

Sur cette machine cloud, le SDK est installé sous `/workspace/.dotnet`. Avant les commandes :

```sh
export PATH="/workspace/.dotnet:$PATH"
export DOTNET_CLI_HOME=/workspace/.dotnet-cli
export NUGET_PACKAGES=/workspace/.nuget/packages
```

## Vérification et arrêt

`GET /health` doit renvoyer `status: ok`. L’application web doit afficher un prix initial proche de 10,450584. Lancer `python3 scripts/smoke.py` pour valider les requêtes et références. Les logs ASP.NET sortent sur le terminal. Ctrl+C dans chaque terminal arrête le service correspondant ; ne pas terminer les processus d’autres projets.

## Incidents

- SDK absent : installer la version officielle épinglée, sans désactiver TLS ni les vérifications NuGet.
- Port occupé : arrêter uniquement une instance de ce projet ou choisir d’autres ports et adapter les clients/tests.
- WPF « API indisponible » : vérifier le port 5080 et le healthcheck ; l’interface ne se bloque pas pendant la requête et peut annuler.
- HTTP 400 : lire le détail Problem Details, corriger unités/valeurs ; un CSV invalide ne produit aucun snapshot partiel.
- Calibration rejetée : vérifier bornes d’arbitrage, maturité et plafond de volatilité 500 % ; ne pas remplacer une erreur par une volatilité inventée.
- Mauvais résultat après modification de modèle : relancer tests et référence Python ; revenir au dernier commit validé via une branche dédiée, en préservant les modifications locales.

API et interface sont destinées à un usage local. Toute exposition réseau demande un cadrage d’authentification, d’habilitations et de données. Ne jamais mettre de données client ou secrets dans les fixtures.
