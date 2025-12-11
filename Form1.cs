///
/// ETML
/// Author: Diogo Martins | Thibault Gugler - FID1
/// Date: 11.12.25
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
        List<Ship> ships { get; set; }                      // Liste des bateaux
        HashSet<(int line, int column)> hit { get; set; }   // Ensemble des positions touchées
        Timer victoryTimer;     // Timer pour l'animation de victoire
        int animationStep = 0;  // Étape de l'animation de victoire
        int elapsedSeconds = 0;
        Timer gameTimer;

        /// <summary>
        /// Constructeur principal de la fenêtre
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            victoryTimer = new Timer();                 // Initialisation du timer de victoire
            victoryTimer.Interval = 50;                 // Vitesse de l’animation (50 ms)
            victoryTimer.Tick += VictoryTimer_Tick;     // Événement à chaque tick

            gameTimer = new Timer();
            gameTimer.Interval = 1000;
            gameTimer.Tick += GameTimer_Tick;

            elapsedSeconds = 0;
            timerLabel.Text = FormatTime(elapsedSeconds);
            gameTimer.Start();

            gridButton = new Button[GRID_SIZE, GRID_SIZE];
            grid = new int[GRID_SIZE, GRID_SIZE];
            shots = new bool[GRID_SIZE, GRID_SIZE];
            random = new Random();
            ships = new List<Ship>();
            hit = new HashSet<(int line, int column)>();

            createGrid();
            InitializeShips();
        }

        /// <summary>
        /// Modèle de bateau
        /// </summary>
        public class Ship
        {
            public string Name { get; set; }
            public int Size { get; set; }
            public int Id { get; set; }

            // Constructeur des bateaux en fonctions de leurs propriétés
            public Ship(string name, int size, int id)
            {
                Name = name;
                Size = size;
                Id = id;
            }
        }

        /// <summary>
        /// Création de la grille de jeu (boutons dynamiques)
        /// </summary>
        private void createGrid()
        {
            for (int i = 0; i < GRID_SIZE; i++)
            {
                for (int j = 0; j < GRID_SIZE; j++)
                {
                    Button btnGame = new Button();
                    btnGame.Text = "";
                    btnGame.Width = WIDTH;
                    btnGame.Height = WIDTH;
                    btnGame.Tag = (i, j);               // Stocke la position dans le Tag
                    btnGame.Left = j * (WIDTH + SPACE);
                    btnGame.Top = i * (WIDTH + SPACE);
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

            // Définition du nom des bateaux avec leurs tailles
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
                ships.Add(new Ship(ship.name, ship.size, shipId));
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

            while (!placed)
            {
                bool horizontal = random.Next(2) == 0;
                int row = random.Next(GRID_SIZE);
                int col = random.Next(GRID_SIZE);

                if (CanPlaceShip(row, col, size, horizontal))
                {
                    for (int i = 0; i < size; i++)
                    {
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
            int startRow = row - 1;
            int endRow = horizontal ? row + 1 : row + size;
            int startCol = col - 1;
            int endCol = horizontal ? col + size : col + 1;

            // Parcours de la zone autour du bateau (y compris diagonales)
            for (int r = startRow; r <= endRow; r++)
            {
                for (int c = startCol; c <= endCol; c++)
                {
                    // Vérifie que la case est dans la grille
                    if (r >= 0 && r < GRID_SIZE && c >= 0 && c < GRID_SIZE)
                    {
                        // Ignore les cases du bateau lui-même
                        if (horizontal)
                        {
                            if (c >= col && c < col + size && r == row) continue;
                        }
                        else
                        {
                            if (r >= row && r < row + size && c == col) continue;
                        }
                        // Si une case autour est occupée, placement impossible
                        if (grid[r, c] != 0) return false;
                    }
                }
            }
            // Vérifie que le bateau ne sort pas de la grille
            if (horizontal)
            {
                if (col + size > GRID_SIZE) return false;
            }
            else
            {
                if (row + size > GRID_SIZE) return false;
            }
            // Vérifie que les cases du bateau sont libres
            for (int i = 0; i < size; i++)
            {
                int r = horizontal ? row : row + i;
                int c = horizontal ? col + i : col;
                if (grid[r, c] != 0) return false;
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
            Ship ship = ships.First(s => s.Id == shipId);

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
                shots[line, column] = true;

                // Si un bateau est touché
                if (grid[line, column] > 0)
                {
                    btn.BackColor = Color.Red;
                    btn.Text = "X";

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
            // Reset grid data
            Array.Clear(grid, 0, grid.Length);
            Array.Clear(shots, 0, shots.Length);
            hit.Clear();

            elapsedSeconds = 0;
            timerLabel.Text = FormatTime(elapsedSeconds);
            gameTimer.Start();

            // Reset boutons
            for (int i = 0; i < GRID_SIZE; i++)
            {
                for (int j = 0; j < GRID_SIZE; j++)
                {
                    gridButton[i, j].BackColor = Color.LightBlue; // Remet la couleur par défaut
                    gridButton[i, j].Text = ""; // Vide le texte si il y a du texte
                }
            }

            // Reset labels
            label2.Text = "Porte-avions: 1";
            label3.Text = "Croiseur: 1";
            label4.Text = "Contre-Torpilleur: 1";
            label5.Text = "Sous-marin: 1";
            label6.Text = "Torpilleur: 1";

            // Reset ships list + re-placement
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

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            elapsedSeconds++;
            timerLabel.Text = FormatTime(elapsedSeconds);
        }

        private string FormatTime(int seconds)
        {
            int minutes = seconds / 60;
            int secs = seconds % 60;
            return $"{minutes:00}:{secs:00}";
        }

    }
}
