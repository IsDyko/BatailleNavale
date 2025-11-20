using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleShip
{
    /// <summary>
    /// Représente la grille de jeu avec gestion des bateaux et des tirs
    /// </summary>
    internal class Grille
    {
        public const int TAILLE_GRILLE = 10;
        private int[,] grille; // 0 = vide, 1+ = ID du bateau
        private bool[,] tirs; // false = non tiré, true = tiré
        public List<Bateau> Bateaux { get; private set; }
        private Random random;

        public Grille()
        {
            grille = new int[TAILLE_GRILLE, TAILLE_GRILLE];
            tirs = new bool[TAILLE_GRILLE, TAILLE_GRILLE];
            Bateaux = new List<Bateau>();
            random = new Random();
            InitialiserBateaux();
        }

        /// <summary>
        /// Initialise les bateaux avec placement aléatoire
        /// </summary>
        private void InitialiserBateaux()
        {
            // Définition des bateaux standards
            var typeBateaux = new List<(string nom, int taille)>
            {
                ("Porte-avions", 5),
                ("Croiseur", 4),
                ("Contre-torpilleur", 3),
                ("Sous-marin", 3),
                ("Torpilleur", 2)
            };

            int bateauId = 1;
            foreach (var (nom, taille) in typeBateaux)
            {
                Bateau bateau = new Bateau(nom, taille);
                PlacerBateauAleatoire(bateau, bateauId);
                Bateaux.Add(bateau);
                bateauId++;
            }
        }

        /// <summary>
        /// Place un bateau aléatoirement sur la grille
        /// </summary>
        private void PlacerBateauAleatoire(Bateau bateau, int bateauId)
        {
            bool place = false;
            int tentatives = 0;
            const int MAX_TENTATIVES = 1000;

            while (!place && tentatives < MAX_TENTATIVES)
            {
                tentatives++;
                bool horizontal = random.Next(2) == 0;
                int ligne = random.Next(TAILLE_GRILLE);
                int colonne = random.Next(TAILLE_GRILLE);

                if (PeutPlacerBateau(ligne, colonne, bateau.Taille, horizontal))
                {
                    PlacerBateau(ligne, colonne, bateau.Taille, horizontal, bateau, bateauId);
                    place = true;
                }
            }

            if (!place)
            {
                throw new Exception($"Impossible de placer le bateau {bateau.Nom}");
            }
        }

        /// <summary>
        /// Vérifie si un bateau peut être placé à une position donnée
        /// </summary>
        private bool PeutPlacerBateau(int ligne, int colonne, int taille, bool horizontal)
        {
            if (horizontal)
            {
                if (colonne + taille > TAILLE_GRILLE) return false;
                for (int c = colonne; c < colonne + taille; c++)
                {
                    if (grille[ligne, c] != 0) return false;
                }
            }
            else
            {
                if (ligne + taille > TAILLE_GRILLE) return false;
                for (int l = ligne; l < ligne + taille; l++)
                {
                    if (grille[l, colonne] != 0) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Place un bateau sur la grille
        /// </summary>
        private void PlacerBateau(int ligne, int colonne, int taille, bool horizontal, Bateau bateau, int bateauId)
        {
            if (horizontal)
            {
                for (int c = colonne; c < colonne + taille; c++)
                {
                    grille[ligne, c] = bateauId;
                    bateau.Positions.Add((ligne, c));
                }
            }
            else
            {
                for (int l = ligne; l < ligne + taille; l++)
                {
                    grille[l, colonne] = bateauId;
                    bateau.Positions.Add((l, colonne));
                }
            }
        }

        /// <summary>
        /// Traite un tir à une position donnée
        /// </summary>
        /// <returns>0 = déjà tiré, 1 = raté, 2 = touché, 3 = coulé</returns>
        public int Tirer(int ligne, int colonne)
        {
            if (tirs[ligne, colonne])
            {
                return 0; // Déjà tiré
            }

            tirs[ligne, colonne] = true;

            int bateauId = grille[ligne, colonne];
            if (bateauId == 0)
            {
                return 1; // Raté
            }

            // Touché
            Bateau bateau = Bateaux[bateauId - 1];
            bateau.Toucher(ligne, colonne);

            if (bateau.EstCoule())
            {
                return 3; // Coulé
            }

            return 2; // Touché
        }

        /// <summary>
        /// Vérifie si tous les bateaux sont coulés
        /// </summary>
        public bool TousBateauxCoules()
        {
            return Bateaux.All(b => b.EstCoule());
        }

        /// <summary>
        /// Retourne l'état d'une case (pour debugging)
        /// </summary>
        public bool CaseATire(int ligne, int colonne)
        {
            return tirs[ligne, colonne];
        }

        /// <summary>
        /// Retourne si une case contient un bateau (pour debugging)
        /// </summary>
        public bool CaseContientBateau(int ligne, int colonne)
        {
            return grille[ligne, colonne] > 0;
        }
    }
}