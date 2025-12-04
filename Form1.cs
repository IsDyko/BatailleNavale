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
        const int WIDTH = 50;
        const int SPACE = 2;
        const int GRID_SIZE = 10;
        const int SHIP_SPACE = 1;
        int[,] grid;
        bool[,] shots;
        Button[,] gridButton;
        Random random;
        List<Ship> ships { get; set; }
        HashSet<(int line, int column)> hit { get; set; }
        Timer victoryTimer;
        int animationStep = 0;


        /// <summary>
        /// Constructeur
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            victoryTimer = new Timer();
            victoryTimer.Interval = 50; // vitesse de l’animation (50 ms)
            victoryTimer.Tick += VictoryTimer_Tick;


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

            public Ship(string name, int size, int id)
            {
                Name = name;
                Size = size;
                Id = id;
            }
        }

        /// <summary>
        /// Création de la grille de jeu
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
                    btnGame.Tag = (i, j);
                    btnGame.Left = j * (WIDTH + SPACE);
                    btnGame.Top = i * (WIDTH + SPACE);
                    btnGame.BackColor = Color.LightBlue;
                    btnGame.Click += btnGame_Click;
                    panelGrille.Controls.Add(btnGame);
                    gridButton[i, j] = btnGame;
                }
            }
        }

        /// <summary>
        /// Initialisation des bateaux
        /// </summary>
        private void InitializeShips()
        {
            var shiptypes = new List<(string name, int size)>
            {
                ("Porte-avions", 5),
                ("Croiseur", 4),
                ("Contre-torpilleur", 3),
                ("Sous-marin", 3),
                ("Torpilleur", 2)
            };

            int shipId = 1;
            foreach (var ship in shiptypes)
            {
                ships.Add(new Ship(ship.name, ship.size, shipId));
                PlaceShipOnGrid(shipId, ship.size);
                shipId++;
            }
        }

        /// <summary>
        /// Placement aléatoire des bateaux sur la grille
        /// </summary>
        /// <param name="shipId"></param>
        /// <param name="size"></param>
        void PlaceShipOnGrid(int shipId, int size)
        {
            bool placed = false;

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
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="size"></param>
        /// <param name="horizontal"></param>
        /// <returns></returns>
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
                        // Vérifie si une case est occupée
                        if (horizontal)
                        {
                            if (c >= col && c < col + size && r == row) continue; // Ignore les cases du bateau
                        }
                        else
                        {
                            if (r >= row && r < row + size && c == col) continue; // Ignore les cases du bateau
                        }
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

        private bool IsSunk(int shipId)
        {
            // Compte les cases des bateaux
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

            if (!shots[line, column])
            {
                shots[line, column] = true;

                if (grid[line, column] > 0)
                {
                    btn.BackColor = Color.Red;
                    btn.Text = "X";

                    // On enregistre le tir comme "touché"
                    hit.Add((line, column));

                    // Récupérer l'id du bateau touché
                    int shipId = grid[line, column];

                    // Vérifier s'il est coulé
                    if (IsSunk(shipId))
                    {
                        var ship = ships.First(s => s.Id == shipId);

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
                        if (Victory())
                        {
                            playWinSound();
                            victoryTimer.Start();
                        }
                    }
                }
                else
                {
                    btn.BackColor = Color.White;
                    btn.Text = "O";
                }
            }
        }

        private bool Victory()
        {
            return ships.All(ship => IsSunk(ship.Id));
        }

        private void VictoryTimer_Tick(object sender, EventArgs e)
        {
            // Animation progressive : chaque tick colore 1 ligne
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

                // Après animation → afficher la fenêtre de victoire
                VictoryForm victory = new VictoryForm();
                if (victory.ShowDialog() == DialogResult.OK)
                {
                    RestartGame();
                }
            }
        }
        private void RestartGame()
        {
            // Reset grid data
            Array.Clear(grid, 0, grid.Length);
            Array.Clear(shots, 0, shots.Length);
            hit.Clear();

            // Reset boutons
            for (int i = 0; i < GRID_SIZE; i++)
            {
                for (int j = 0; j < GRID_SIZE; j++)
                {
                    gridButton[i, j].BackColor = Color.LightBlue;
                    gridButton[i, j].Text = "";
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

        private void playSimpleSound()
        {
            SoundPlayer player = new SoundPlayer(@"D:\Code\BattleShip\explosion-01.wav");
            player.Play();
        }

        private void playWinSound()
        {
            SoundPlayer player1 = new SoundPlayer(@"D:\Code\BattleShip\victory.wav");
            player1.Play();
        }
    }
}
