# Sprint 1 — du cadrage à un calcul vérifiable

Durée cible : deux semaines, à ajuster selon disponibilité. Dépendances : SDK .NET 10 LTS, Python 3.12 ou version compatible, environnement Windows pour la future UI WPF. Aucun fournisseur de données nécessaire.

1. Écrire trois parcours : valoriser un call, appliquer un choc de volatilité, comparer deux marchés. Définir entrées, sorties et cas d’erreur.
2. Figer conventions et unités ; préciser delta, gamma, vega par point de volatilité, rho par point de taux et theta par jour.
3. Créer la solution, les projets Domain/Pricing et tests xUnit ; épingler SDK et dépendances sans ajouter WPF au build Linux.
4. Créer le modèle d’option européenne et un snapshot synthétique. Refuser prix du sous-jacent ou strike non positifs et valeurs non finies ; traiter explicitement expiration et volatilité nulle.
5. Implémenter Black–Scholes–Merton call/put avec rendement de dividende continu.
6. Produire une référence Python indépendante et un corpus de cas, dont maturités courtes et options loin du strike.
7. Tester prix, parité, bornes et sensibilités numériques ; fixer et justifier les tolérances.
8. Ajouter CI Linux et Windows pour le moteur. Documenter une commande de reproduction et une sortie attendue.

Terminé lorsque les tests financiers passent sur les deux systèmes, qu’un calcul est reproductible depuis un checkout propre et que les hypothèses et limites sont écrites. Une compilation seule ne valide pas la valorisation. Aucun résultat de test n’existe encore à ce stade de cadrage.
