# quant_IT — Equity Quant Workbench

Projet de démonstration d’outils Front Office pour les dérivés actions : valorisation, données de marché, calibration, explication du P&L et backtesting.

Le projet traduit une mission IT Quant en parcours de réalisation. Il ne constitue pas un système de trading opérationnel ni une preuve d’expérience en salle de marché. Les premiers jeux de données sont synthétiques et aucune connexion à un courtier ou aucun passage d’ordre n’est prévu.

## Objectif

Permettre à un utilisateur métier de valoriser une option européenne, comprendre ses sensibilités, comparer deux situations de marché et expliquer la variation de valeur avec des calculs reproductibles.

## Stack cible

- C# / .NET 10 LTS : calculs, contrats et services ASP.NET Core REST.
- Blazor : interface web de démonstration.
- WPF avec MVVM : poste de travail Windows utilisant les mêmes services.
- Python : recherche, prototypage et validation indépendante des calculs.
- xUnit et pytest : tests du moteur et des références Python.

Le moteur et l’API doivent fonctionner sous Linux et Windows. WPF requiert Windows pour son exécution et ses tests d’interface. Les SDK et dépendances seront épinglés lors du premier lot d’implémentation.

## Documentation

- [Feuille de route](docs/ROADMAP.md) : lots, critères d’acceptation et planning indicatif.
- [Architecture cible](docs/ARCHITECTURE.md) : composants et contrats.
- [Premier sprint](docs/SPRINT-01.md) : tâches directement exécutables.

## État actuel

Dépôt initial de cadrage : aucun moteur, service ou écran n’est encore implémenté. Aucun résultat financier ni test applicatif n’est revendiqué. Dépôt GitHub : `kelian19/quant_IT`, indépendant de `business-b2b`.
