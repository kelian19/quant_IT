# Architecture cible

```text
Blazor ─┐
        ├── REST / OpenAPI ── ASP.NET Core ── Moteur C#
WPF ────┘                         │
                           Snapshots de marché

Python ── références de calcul et expériences hors production
```

## Composants prévus

- `src/EquityQuant.Domain` : instruments, positions, conventions et identifiants de marché.
- `src/EquityQuant.Pricing` : fonctions déterministes, Greeks, volatilité implicite et scénarios ; aucune dépendance UI ou réseau.
- `src/EquityQuant.Application` : orchestration des calculs, attribution et backtests.
- `src/EquityQuant.Api` : validation des requêtes, contrats versionnés, OpenAPI et diagnostics.
- `src/EquityQuant.Web` : interface Blazor.
- `src/EquityQuant.Desktop` : WPF/MVVM, compilation et recette Windows.
- `research/` : références Python et expériences reproductibles.
- `tests/` : tests unitaires, propriétés financières et intégration.
- `data/synthetic/` : fixtures fictives versionnées, sans données personnelles ni licence restrictive.

Ces chemins sont proposés ; leur code sera créé pendant les lots correspondants.

## Contrats à figer

Une demande de pricing contient instrument, date de valorisation et snapshot de marché. Une réponse contient prix, Greeks avec unités, hypothèses, avertissements, version du modèle et identifiant de calcul. Utiliser `double` pour les calculs numériques et définir explicitement les arrondis d’affichage ; ne pas arrondir les résultats intermédiaires.

API proposée : `POST /api/v1/pricing`, `POST /api/v1/scenarios`, `POST /api/v1/pnl-explain` et `GET /health`. Les imports et backtests longs demanderont ultérieurement des tâches annulables avec statut et limites de ressources. Les requêtes de calcul ne déclenchent aucune opération de marché.

Pour le prototype, snapshots JSON/CSV immuables et résultats locaux suffisent. Ajouter une base de données seulement lorsque la persistance multi-utilisateur et les volumes le justifient. Les processus et services devront être redémarrés dans chaque nouvel environnement.
