# Validation de la première version

Vérifications réalisées le 2 novembre 2026, sous Linux, SDK .NET 10.0.403.

- Solution .NET : compilation sans avertissement ni erreur.
- Client WPF : compilation croisée `net10.0-windows` sans avertissement ni erreur. Aucune exécution de l’interface Windows.
- xUnit : 19 tests réussis, 0 échec, 0 ignoré. Référence de prix BSM, parité put-call, Greeks par différences finies, maturité/volatilité nulles, inversion de volatilité, entrées invalides, réconciliation P&L, couverture déterministe, absence d’anticipation des prochains spots, frais et import de marché.
- Recette HTTP : 30 contrôles réussis. Healthcheck, document OpenAPI, 18 comparaisons C#/Python avec CDF `erf` indépendante (tolérance absolue de prix 3e-5), calibration, scénarios, P&L, backtest, import, erreurs HTTP 400 et HTML/ressource Blazor.
- Chromium via Playwright : 5 parcours interactifs réussis, sans erreur JavaScript : import de snapshot, calibration, P&L, backtest et rejet d’un spot négatif. Le téléchargement et la sélection de quote n’ont pas de recette navigateur dédiée.

La recette a révélé un script Blazor manquant au premier démarrage. Le routage et le chargement du manifeste des assets ont été corrigés ; les contrôles ci-dessus ont été réexécutés. Les boutons attendent maintenant l’activation interactive avant de devenir utilisables.

La dépendance transitive Microsoft.OpenApi 2.0.0 initialement signalée par NuGet a été remplacée explicitement par 2.13.0 ; la compilation finale ne rapporte plus cet avertissement.

Le moteur utilise une approximation polynomiale de la CDF normale ; ce n’est pas une bibliothèque financière certifiée. Les cas extrêmes, données historiques réelles, recettes Windows, revue métier et exécution GitHub Actions restent à valider. Voir IMPLEMENTATION.md pour les critères de feuille de route encore ouverts.
