using System;
using System.Collections.Generic;

namespace BattleShip
{
    /// <summary>
    /// Représente un bateau dans le jeu de bataille navale
    /// </summary>
    internal class Bateau
    {
        public string Nom { get; set; }
        public int Taille { get; set; }
        public List<(int ligne, int colonne)> Positions { get; set; }
        public HashSet<(int ligne, int colonne)> Touches { get; set; }

        public Bateau(string nom, int taille)
        {
            Nom = nom;
            Taille = taille;
            Positions = new List<(int, int)>();
            Touches = new HashSet<(int, int)>();
        }

        /// <summary>
        /// Vérifie si le bateau est complètement coulé
        /// </summary>
        public bool EstCoule()
        {
            return Touches.Count == Taille;
        }

        /// <summary>
        /// Enregistre un tir sur le bateau
        /// </summary>
        public bool Toucher(int ligne, int colonne)
        {
            if (Positions.Contains((ligne, colonne)))
            {
                Touches.Add((ligne, colonne));
                return true;
            }
            return false;
        }
    }
}