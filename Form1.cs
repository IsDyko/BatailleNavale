///
/// ETML
/// Author: Diogo Martins | Thibault Gugler - FID1
/// Date: 08.01.26
/// 
/// Bataille navale Solo - Code principal
/// 

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;

namespace BattleShip
{
    public partial class Form1 : Form
    {
        // Constantes pour la taille des boutons et de la grille
        const int WIDTH = 50;
        const int SPACE = 2;
        const int GRID_SIZE = 10;

        int[,] grid;            // Grille de jeu (stocke ID du bateau ou 0)
        bool[,] shots;          // Grille des tirs (true si déjà tiré)
        Button[,] gridButton;   // Tableau des boutons de la grille
        Random random;          // Générateur de nombres aléatoires
        List<ShipList> ships { get; set; }                  // Liste des bateaux
        HashSet<(int line, int column)> hit { get; set; }   // Ensemble des positions touchées
        Timer victoryTimer;     // Timer pour l'animation de victoire
        int animationStep = 0;  // Étape de l'animation de victoire
        int elapsedSeconds = 0; // Temps écoulé en secondes
        Timer gameTimer;        // Timer pour le chronomètre de jeu

        /// <summary>
        /// Constructeur principal de la fenêtre
        /// </summary>
        public Form1()
        {
            // Initialisation des composants de la fenêtre
            InitializeComponent();

            victoryTimer = new Timer();                     // Initialisation du timer de victoire
            victoryTimer.Interval = 50;                     // Vitesse de l’animation (50 ms)
            victoryTimer.Tick += VictoryTimer_Tick;         // Événement à chaque tick

            gameTimer = new Timer();                        // Initialisation du timer de jeu
            gameTimer.Interval = 1000;                      // Intervalle de 1 seconde
            gameTimer.Tick += GameTimer_Tick;               // Événement à chaque tick

            elapsedSeconds = 0;                             // Initialisation du temps écoulé
            timerLabel.Text = FormatTime(elapsedSeconds);   // Affichage initial du temps
            gameTimer.Start();                              // Démarre le timer de jeu

            gridButton = new Button[GRID_SIZE, GRID_SIZE];  // Tableau des boutons de la grille
            grid = new int[GRID_SIZE, GRID_SIZE];           // Grille de jeu
            shots = new bool[GRID_SIZE, GRID_SIZE];         // Grille des tirs
            random = new Random();                          // Générateur de nombres aléatoires
            ships = new List<ShipList>();                   // Liste des bateaux
            hit = new HashSet<(int line, int column)>();    // Ensemble des positions touchées

            // Création de la grille
            createGrid();

            // Initialisation et placement des bateaux
            InitializeShips();
        }

        /// <summary>
        /// Modèle de bateau
        /// </summary>
        public class ShipList
        {
            public string Name { get; set; }
            public int Size { get; set; }
            public int Id { get; set; }

            // Constructeur des bateaux en fonctions de leurs propriétés
            public ShipList(string name, int size, int id)
            {
                Name = name; // Nom du bateau
                Size = size; // Taille du bateau
                Id = id;     // Identifiant du bateau
            }
        }

        /// <summary>
        /// Création de la grille de jeu (boutons dynamiques)
        /// </summary>
        private void createGrid()
        {
            // Pour chaque colonne
            for (int i = 0; i < GRID_SIZE; i++)
            {
                // Pour chaque ligne
                for (int j = 0; j < GRID_SIZE; j++)
                {
                    Button btnGame = new Button();      // Création d'un nouveau bouton  
                    btnGame.Text = "";                  // Texte vide        
                    btnGame.Width = WIDTH;              // Définition de la taille
                    btnGame.Height = WIDTH;             // Définition de la taille
                    btnGame.Tag = (i, j);               // Stocke la position dans le Tag
                    btnGame.Left = j * (WIDTH + SPACE); // Positionnement dans le panel
                    btnGame.Top = i * (WIDTH + SPACE);  // Positionnement dans le panel
                    btnGame.BackColor = Color.LightBlue;// Définit la couleur par défaut
                    btnGame.Click += btnGame_Click;     // Événement click
                    panelGrille.Controls.Add(btnGame);  // Ajoute le bouton au panel
                    gridButton[i, j] = btnGame;         // Stocke le bouton dans le tableau
                }
            }
        }

        /// <summary>
        /// Initialisation des bateaux et placement sur la grille
        /// </summary>
        private void InitializeShips()
        {
            int shipId = 1; // Initialisation de l'ID des bateaux

            // Définition des types de bateaux (nom, taille)
            var shiptypes = new List<(string name, int size)>
            {
                ("Porte-avions", 5),
                ("Croiseur", 4),
                ("Contre-torpilleur", 3),
                ("Sous-marin", 3),
                ("Torpilleur", 2)
            };

            // Placement de chaque bateau
            foreach (var ship in shiptypes)
            {
                // Ajoute le bateau à la liste
                ships.Add(new ShipList(ship.name, ship.size, shipId));
                // Place le bateau sur la grille
                PlaceShipOnGrid(shipId, ship.size);
                shipId++;
            }
        }

        /// <summary>
        /// Placement aléatoire d'un bateau sur la grille
        /// </summary>
        /// <param name="shipId">Identifiant du bateau</param>
        /// <param name="size">Taille du bateau</param>
        void PlaceShipOnGrid(int shipId, int size)
        {
            bool placed = false; // Indicateur de placement

            // Tente de placer le bateau jusqu'à ce que ce soit réussi
            while (!placed)
            {
                bool horizontal = random.Next(2) == 0;  // Orientation aléatoire
                int row = random.Next(GRID_SIZE);       // Ligne de départ aléatoire
                int col = random.Next(GRID_SIZE);       // Colonne de départ aléatoire

                // Vérifie si le bateau peut être placé à cette position
                if (CanPlaceShip(row, col, size, horizontal))
                {
                    // Place le bateau sur la grille
                    for (int i = 0; i < size; i++)
                    {
                        // Remplit les cases du bateau avec son ID
                        if (horizontal)
                            grid[row, col + i] = shipId;
                        else
                            grid[row + i, col] = shipId;
                    }
                    placed = true;
                }
            }
        }

        /// <summary>
        /// Vérifie si un bateau peut être placé à la position donnée
        /// </summary>
        /// <param name="row">Ligne de départ</param>
        /// <param name="col">Colonne de départ</param>
        /// <param name="size">Taille du bateau</param>
        /// <param name="horizontal">Orientation</param>
        /// <returns>True si le placement est possible</returns>
        private bool CanPlaceShip(int row, int col, int size, bool horizontal)
        {
            int startRow = row - 1; // Ligne de départ pour la vérification
            int endRow = horizontal ? row + 1 : row + size; // Ligne de fin pour la vérification
            int startCol = col - 1; // Colonne de départ pour la vérification
            int endCol = horizontal ? col + size : col + 1; // Colonne de fin pour la vérification

            // Parcours de la zone autour du bateau (y compris diagonales)
            for (int r = startRow; r <= endRow; r++)
            {
                // Parcours des colonnes
                for (int c = startCol; c <= endCol; c++)
                {
                    // Vérifie que la case est dans la grille
                    if (r >= 0 && r < GRID_SIZE && c >= 0 && c < GRID_SIZE)
                    {
                        // Si une case autour est occupée, placement impossible
                        if (grid[r, c] != 0) return false;
                    }
                }
            }

            // Vérifie que le bateau ne sort pas de la grille
            if (horizontal)
            {
                // Vérifie la limite droite
                if (col + size > GRID_SIZE) return false;
            }
            else
            {
                // Vérifie la limite basse
                if (row + size > GRID_SIZE) return false;
            }

            return true;
        }

        /// <summary>
        /// Vérifie si un bateau est coulé
        /// </summary>
        /// <param name="shipId">Identifiant du bateau</param>
        /// <returns>True si toutes les cases du bateau sont touchées</returns>
        private bool IsSunk(int shipId)
        {
            // Compte les cases touchées du bateau
            int touchedParts = hit.Count(pos => grid[pos.line, pos.column] == shipId);

            // Retrouve quel bateau est-ce
            ShipList ship = ships.First(s => s.Id == shipId);

            // Retourne vrai si toutes les parties du bateau sont touchées
            return touchedParts >= ship.Size;
        }

        /// <summary>
        /// Gestion du click sur les boutons de la grille
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnGame_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            var (line, column) = ((int, int))btn.Tag;

            // Vérifie si la case a déjà été tirée
            if (!shots[line, column])
            {
                shots[line, column] = true; // Marque la case comme tirée

                // Si un bateau est touché
                if (grid[line, column] > 0)
                {
                    btn.BackColor = Color.Red; // Change la couleur en rouge
                    btn.Text = "X";            // Affiche un X

                    // Enregistre le tir comme "touché"
                    hit.Add((line, column));

                    // Récupère l'id du bateau touché
                    int shipId = grid[line, column];

                    // Vérifie s'il est coulé
                    if (IsSunk(shipId))
                    {
                        var ship = ships.First(s => s.Id == shipId);

                        // Met à jour le label correspondant et joue un son
                        switch (shipId)
                        {
                            case 1:
                                label2.Text = "Porte-avions: X";
                                playSimpleSound();
                                break;
                            case 2:
                                label3.Text = "Croiseur: X";
                                playSimpleSound();
                                break;
                            case 3:
                                label4.Text = "Contre-Torpilleur: X";
                                playSimpleSound();
                                break;
                            case 4:
                                label5.Text = "Torpilleur: X";
                                playSimpleSound();
                                break;
                            case 5:
                                label6.Text = "Sous-marin: X";
                                playSimpleSound();
                                break;
                            default:
                                break;
                        }
                        // Si tous les bateaux sont coulés, victoire
                        if (Victory())
                        {
                            // Joue le son de victoire
                            playWinSound();
                            // Démarre l'animation de victoire
                            victoryTimer.Start();
                        }
                    }
                }
                else
                {
                    // Si loupé
                    btn.BackColor = Color.White;
                }
            }
        }

        /// <summary>
        /// Vérifie la condition de victoire (tous les bateaux coulés)
        /// </summary>
        private bool Victory()
        {
            // Retourne vrai si tous les bateaux sont coulés
            return ships.All(ship => IsSunk(ship.Id));
        }

        /// <summary>
        /// Animation de victoire (colorie la grille ligne par ligne)
        /// </summary>
        private void VictoryTimer_Tick(object sender, EventArgs e)
        {
            // Animation progressive
            if (animationStep < GRID_SIZE)
            {
                for (int col = 0; col < GRID_SIZE; col++)
                {
                    gridButton[animationStep, col].BackColor = Color.LimeGreen;
                }
                animationStep++;
            }
            else
            {
                victoryTimer.Stop();
                animationStep = 0;

                gameTimer.Stop();

                // Après animation / afficher la fenêtre de victoire
                VictoryForm victory = new VictoryForm();
                if (victory.ShowDialog() == DialogResult.OK)
                {
                    RestartGame();
                }
            }
        }

        /// <summary>
        /// Réinitialise la partie
        /// </summary>
        private void RestartGame()
        {
            // Réinitialise les données de la grille
            Array.Clear(grid, 0, grid.Length);
            Array.Clear(shots, 0, shots.Length);
            hit.Clear();

            elapsedSeconds = 0;
            timerLabel.Text = FormatTime(elapsedSeconds);
            gameTimer.Start();

            // Réinitialise les boutons
            for (int i = 0; i < GRID_SIZE; i++)
            {
                for (int j = 0; j < GRID_SIZE; j++)
                {
                    gridButton[i, j].BackColor = Color.LightBlue; // Remet la couleur par défaut
                    gridButton[i, j].Text = ""; // Vide le texte si il y a du texte
                }
            }

            // Réinitialise les labels
            label2.Text = "Porte-avions: 1";
            label3.Text = "Croiseur: 1";
            label4.Text = "Contre-Torpilleur: 1";
            label5.Text = "Sous-marin: 1";
            label6.Text = "Torpilleur: 1";

            // Réinitialise la liste des bateaux et leur placement
            ships.Clear();
            InitializeShips();
        }

        /// <summary>
        /// Joue un son d'explosion simple
        /// </summary>
        private void playSimpleSound()
        {
            SoundPlayer player = new SoundPlayer(@"./../../explosion-01.wav");
            player.Play();
        }

        /// <summary>
        /// Joue le son de victoire
        /// </summary>
        private void playWinSound()
        {
            SoundPlayer player1 = new SoundPlayer(@"./../../victory.wav");
            player1.Play();
        }

        /// <summary>
        /// Gère le tick du timer de jeu (incrémente le temps)
        /// </summary>
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            elapsedSeconds++;
            timerLabel.Text = FormatTime(elapsedSeconds);
        }

        /// <summary>
        /// Formate le temps en mm:ss
        /// </summary>
        private string FormatTime(int seconds)
        {
            int minutes = seconds / 60;
            int secs = seconds % 60;
            return $"{minutes:00}:{secs:00}";
        }

    }
}
