using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleShip
{
    public partial class Form1 : Form
    {
        private Grille grille;
        private int nombreTirs;
        private int nombreTouches;
        private Label lblStats;
        private Button btnRestart;
        private Button[,] boutonsGrille;

        //Creation des objets
        public Form1()
        {
            InitializeComponent();
            this.Text = "Bataille Navale - Mode Solo";
            boutonsGrille = new Button[Grille.TAILLE_GRILLE, Grille.TAILLE_GRILLE];
            
            CreerInterface();
            NouvellePartie();
        }

        /// <summary>
        /// Crée l'interface de jeu avec la grille et les contrôles
        /// </summary>
        private void CreerInterface()
        {
            int width = 50;
            int space = 2;

            // Création de la grille de boutons
            for (int i = 0; i < Grille.TAILLE_GRILLE; i++)
            {
                for (int j = 0; j < Grille.TAILLE_GRILLE; j++)
                {
                    Button btnGame = new Button();
                    btnGame.Text = "";
                    btnGame.Width = width;
                    btnGame.Height = width;
                    btnGame.Tag = (i, j);
                    btnGame.Left = j * (width + space);
                    btnGame.Top = i * (width + space);
                    btnGame.BackColor = Color.LightBlue;
                    btnGame.Click += btnGame_Click;
                    panelGrille.Controls.Add(btnGame);
                    boutonsGrille[i, j] = btnGame;
                }
            }

            // Label pour les statistiques
            lblStats = new Label();
            lblStats.Location = new Point(112, 20);
            lblStats.Size = new Size(400, 30);
            lblStats.Font = new Font(lblStats.Font.FontFamily, 10, FontStyle.Bold);
            this.Controls.Add(lblStats);

            // Bouton Nouvelle Partie
            btnRestart = new Button();
            btnRestart.Text = "Nouvelle Partie";
            btnRestart.Location = new Point(550, 20);
            btnRestart.Size = new Size(150, 30);
            btnRestart.Click += btnRestart_Click;
            this.Controls.Add(btnRestart);
        }

        /// <summary>
        /// Démarre une nouvelle partie
        /// </summary>
        private void NouvellePartie()
        {
            grille = new Grille();
            nombreTirs = 0;
            nombreTouches = 0;
            
            // Réinitialiser l'apparence de tous les boutons
            for (int i = 0; i < Grille.TAILLE_GRILLE; i++)
            {
                for (int j = 0; j < Grille.TAILLE_GRILLE; j++)
                {
                    boutonsGrille[i, j].BackColor = Color.LightBlue;
                    boutonsGrille[i, j].Text = "";
                    boutonsGrille[i, j].Enabled = true;
                }
            }

            MettreAJourStats();
        }

        /// <summary>
        /// Met à jour l'affichage des statistiques
        /// </summary>
        private void MettreAJourStats()
        {
            int bateauxCoules = grille.Bateaux.Count(b => b.EstCoule());
            lblStats.Text = $"Tirs: {nombreTirs} | Touches: {nombreTouches} | Bateaux coulés: {bateauxCoules}/{grille.Bateaux.Count}";
        }

        /// <summary>
        /// Gère le clic sur un bouton de la grille
        /// </summary>
        void btnGame_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            var (ligne, colonne) = ((int, int))btn.Tag;

            // Traiter le tir
            int resultat = grille.Tirer(ligne, colonne);

            switch (resultat)
            {
                case 0: // Déjà tiré
                    MessageBox.Show("Vous avez déjà tiré sur cette case !", "Attention");
                    return;

                case 1: // Raté
                    btn.BackColor = Color.White;
                    btn.Text = "○";
                    nombreTirs++;
                    break;

                case 2: // Touché
                    btn.BackColor = Color.Orange;
                    btn.Text = "X";
                    nombreTirs++;
                    nombreTouches++;
                    MessageBox.Show("Touché !", "Résultat");
                    break;

                case 3: // Coulé
                    btn.BackColor = Color.Red;
                    btn.Text = "X";
                    nombreTirs++;
                    nombreTouches++;
                    
                    // Trouver le bateau coulé
                    Bateau bateauCoule = grille.Bateaux.FirstOrDefault(b => b.EstCoule() && b.Positions.Contains((ligne, colonne)));
                    string message = bateauCoule != null ? $"Coulé ! Vous avez coulé le {bateauCoule.Nom} !" : "Coulé !";
                    
                    // Marquer toutes les positions du bateau coulé en rouge
                    if (bateauCoule != null)
                    {
                        foreach (var (l, c) in bateauCoule.Positions)
                        {
                            boutonsGrille[l, c].BackColor = Color.Red;
                        }
                    }
                    
                    MessageBox.Show(message, "Résultat");
                    break;
            }

            btn.Enabled = false;
            MettreAJourStats();

            // Vérifier la victoire
            if (grille.TousBateauxCoules())
            {
                MessageBox.Show($"Félicitations ! Vous avez gagné !\n\nStatistiques finales:\nNombre de tirs: {nombreTirs}\nPrécision: {(nombreTouches * 100 / nombreTirs):F1}%", 
                    "Victoire !", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Information);
                
                // Désactiver tous les boutons
                for (int i = 0; i < Grille.TAILLE_GRILLE; i++)
                {
                    for (int j = 0; j < Grille.TAILLE_GRILLE; j++)
                    {
                        boutonsGrille[i, j].Enabled = false;
                    }
                }
            }
        }

        /// <summary>
        /// Gère le clic sur le bouton Nouvelle Partie
        /// </summary>
        void btnRestart_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Voulez-vous vraiment commencer une nouvelle partie ?", 
                "Nouvelle Partie", 
                MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                NouvellePartie();
            }
        }
    }
}
