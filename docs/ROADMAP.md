# Feuille de route — IT Quant Equity Markets

## Résultat attendu

Une démonstration de bout en bout : charger un marché daté, sélectionner une option, calculer prix et Greeks, appliquer des chocs, expliquer le P&L entre deux dates, puis reproduire un backtest de couverture delta. Chaque résultat conserve ses paramètres, sa version de modèle et la provenance des données.

Hypothèse de planning : un développeur à temps plein ayant les bases de C#, Python et mathématiques financières. Les 12 à 16 semaines ci-dessous sont une estimation, à réviser après le premier sprint. L’apprentissage de WPF, la disponibilité de Windows et l’accès aux données peuvent allonger le calendrier. Les lots se terminent sur leurs critères d’acceptation, pas sur la seule date.

## Périmètre initial

Options européennes call/put sur une action ou un indice, taux et rendement de dividende continus constants, modèle Black–Scholes–Merton. Positions simples, devise unique et données synthétiques. Conventions explicites : temps ACT/365F, taux en décimal, volatilité annualisée, prix par unité de sous-jacent ; multiplicateur et quantité appliqués à la position.

Les options américaines, dividendes discrets, produits exotiques, modèles stochastiques, multi-devises, données temps réel et intégrations SI réelles sont des extensions ultérieures. Les hypothèses du modèle devront être visibles dans les interfaces.

## Lots et critères de sortie

| Lot | Durée indicative | Livrables | Critères d’acceptation |
|---|---|---|---|
| 0 — Cadrage métier | Semaine 1 | Trois parcours utilisateur, dictionnaire de données, conventions, exemples chiffrés et backlog | Besoins distingués des hypothèses ; cas limites et tolérances définis ; démonstration ciblée sans données confidentielles |
| 1 — Socle et pricing | Semaines 2–3 | Solution .NET, moteur C# pur, référence Python, tests et CI Linux/Windows | Prix de référence concordants selon tolérances documentées ; parité put-call, bornes, maturité nulle et volatilité nulle testées ; Greeks comparés à des différences finies |
| 2 — Marché et calibration | Semaines 4–5 | Import CSV, snapshots immuables, solveur de volatilité implicite, diagnostics par maturité/strike | Quotes invalides rejetées ; prix hors bornes signalés ; convergence bornée ; reconstruction des prix d’entrée ; erreurs affichées et unités cohérentes |
| 3 — Services REST | Semaine 6 | API de pricing, Greeks, scénarios et snapshots ; OpenAPI ; journalisation structurée | Tests d’intégration HTTP ; erreurs structurées ; identifiant de calcul ; validation des entrées ; aucune trace de secret ; résultats identiques à ceux du moteur |
| 4 — Interface Blazor | Semaines 7–8 | Ticket d’option, tableau de Greeks, courbes de scénarios, comparaison de marchés, exports | Parcours complet avec données synthétiques ; états d’erreur visibles ; date et version du marché affichées ; export permettant de reproduire le calcul |
| 5 — Explication de P&L | Semaines 9–10 | Revalorisation exacte et attribution delta/gamma/vega/theta/rho ; résiduel ; rapport | Total exact = somme des contributions + résiduel à tolérance près ; cas isolés par facteur ; convention temporelle et unités testées ; écarts non masqués |
| 6 — Backtesting | Semaines 11–12 | Couverture delta historique, registre des trades simulés et du cash, coûts, graphiques | Absence de données futures ; exécutions décalées après décision ; frais et financement inclus ; benchmark sans couverture ; reproduction déterministe ; métriques définies |
| 7 — Client WPF | Semaines 13–14 | Shell MVVM, ticket de pricing, scénarios et P&L consommant l’API | Tests sur Windows ; interface non bloquée pendant les calculs ; annulation et erreurs réseau ; concordance des résultats avec Blazor |
| 8 — Exploitation et démonstration | Semaines 15–16 | Runbook, diagnostics, packaging, CI, documentation et scénario de démonstration | Installation reproductible sur machine vierge ; parcours de recette ; test de reprise après incident ; versions identifiables ; limites documentées |

## Validation financière

- Conserver des cas de référence indépendants du code C# : formules analytiques, référence Python et valeurs publiées avec source et convention.
- Fixer des tolérances absolues et relatives par mesure avant l’implémentation ; étudier spécialement les maturités courtes et options très hors de la monnaie.
- Tester les propriétés : parité put-call, monotonie et bornes de prix. Vérifier les Greeks numériquement avec plusieurs tailles de pas.
- La calibration initiale désigne l’inversion des prix en volatilités implicites. Une surface interpolée exige ensuite des contrôles d’arbitrage calendrier et butterfly ; elle ne sera pas présentée comme validée sans ces contrôles.
- Le P&L expliqué est une approximation locale. Utiliser la revalorisation complète comme référence et conserver les termes croisés et effets d’ordre supérieur dans le résiduel explicite.
- Pour le backtest, documenter dividendes, splits, calendrier, prix d’exécution, rééquilibrage, coûts et financement. Séparer calibration et évaluation temporellement. Les données synthétiques valident le mécanisme, pas la rentabilité d’une stratégie.

## Besoins métier à explorer au premier lot

1. Quels instruments, conventions et volumes le trader utilise-t-il ?
2. Quels prix font référence et quelles erreurs sont acceptables ?
3. À quel instant les marchés sont-ils figés et quelles quotes sont obsolètes ?
4. Le P&L couvre-t-il uniquement la variation de valeur ou aussi trades, cash-flows et frais ?
5. Quelles opérations doivent rester disponibles lorsque l’API ne répond pas ?
6. Quels fuseaux horaires, langues et formats doivent être partagés entre équipes ?

Ces questions cadrent une future intégration métier ; elles ne bloquent pas la démonstration synthétique.

## Harmonisation et support

Centraliser les conventions financières et contrats de données, pas les préférences d’affichage. Conserver les instants en UTC et expliciter les calendriers de marché. Versionner les API et modèles ; partager les mêmes jeux de référence entre zones. Prévoir diagnostics corrélés, journal des changements, guide de résolution des incidents, procédure de retour à une version connue et responsabilité de validation des modèles.

## Priorités de livraison

Premier jalon : lots 0 à 4, une option valorisée et analysée dans Blazor. Deuxième jalon : P&L et backtesting reproductibles. Troisième jalon : client WPF et exploitation. Authentification d’entreprise, habilitations, audit persistant, licences de données et intégration SI feront l’objet d’un cadrage avant tout usage réel. Aucun déploiement de production n’est prévu dans cette feuille de route.
