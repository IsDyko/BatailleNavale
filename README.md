# Bataille Navale (Battleship) - Mode Solo

Un jeu de bataille navale solo développé en C# avec Windows Forms.

## Description

Ce jeu implémente une version solo classique de la bataille navale où le joueur doit deviner l'emplacement des navires placés aléatoirement sur une grille 10x10. Le placement des navires est généré automatiquement par le système.

## Fonctionnalités

### Gameplay
- **Grille interactive** : Grille de 10x10 boutons cliquables
- **Placement aléatoire** : Les navires sont placés automatiquement au début de chaque partie
- **Retour visuel** :
  - 🔵 Bleu clair : Case d'eau non explorée
  - ⚪ Blanc avec "○" : Tir manqué
  - 🟠 Orange avec "X" : Navire touché
  - 🔴 Rouge avec "X" : Navire coulé
- **Statistiques** : Affichage en temps réel des tirs, touches et bateaux coulés
- **Détection de victoire** : Message automatique quand tous les bateaux sont coulés avec statistiques finales

### Navires
Le jeu inclut 5 navires standard :
- Porte-avions (5 cases)
- Croiseur (4 cases)
- Contre-torpilleur (3 cases)
- Sous-marin (3 cases)
- Torpilleur (2 cases)

### Interface
- **Bouton "Nouvelle Partie"** : Redémarre le jeu avec un nouveau placement aléatoire
- **Affichage des statistiques** : Nombre de tirs, nombre de touches, bateaux coulés

## Prérequis

- Windows OS
- .NET Framework 4.7.2 ou supérieur
- Visual Studio 2017 ou supérieur (pour compiler le projet)

## Installation et Compilation

1. Clonez le dépôt :
   ```bash
   git clone https://github.com/IsDyko/BatailleNavale.git
   ```

2. Ouvrez le fichier `BattleShip.sln` dans Visual Studio

3. Compilez le projet (Build > Build Solution) ou appuyez sur `Ctrl+Shift+B`

4. Exécutez l'application (Debug > Start Debugging) ou appuyez sur `F5`

## Comment Jouer

1. Lancez l'application
2. Les navires sont automatiquement placés sur la grille (invisibles pour le joueur)
3. Cliquez sur les boutons de la grille pour tirer
4. Observez les couleurs pour comprendre le résultat :
   - Un message "Touché !" apparaît quand vous touchez un navire
   - Un message "Coulé !" apparaît avec le nom du navire quand vous le coulez complètement
5. Continuez jusqu'à couler tous les navires
6. Un message de victoire s'affiche avec vos statistiques finales
7. Cliquez sur "Nouvelle Partie" pour recommencer

## Structure du Code

### Fichiers principaux

- **Program.cs** : Point d'entrée de l'application
- **Form1.cs** : Interface utilisateur et logique du jeu
- **Grille.cs** : Gestion de la grille de jeu et placement aléatoire des navires
- **Bateau.cs** : Classe représentant un navire avec ses propriétés

### Architecture

```
BattleShip
├── Bateau.cs           # Classe Ship (propriétés et méthodes)
├── Grille.cs           # Classe Grid (placement aléatoire, détection de tirs)
├── Form1.cs            # Interface et contrôleur de jeu
├── Form1.Designer.cs   # Code généré pour l'interface
└── Program.cs          # Point d'entrée
```

## Améliorations Futures Possibles

- Ajout de niveaux de difficulté
- Sauvegarde des meilleurs scores
- Effets sonores
- Mode multijoueur
- Thèmes visuels personnalisables
- Animation des tirs

## Licence

Ce projet est un projet éducatif open source.

## Auteur

Développé pour le dépôt IsDyko/BatailleNavale
