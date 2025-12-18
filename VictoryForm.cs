///
/// ETML
/// Author: Diogo Martins | Thibault Gugler - FID1
/// Date: 11.12.25
/// 
/// Bataille navale Solo - Fenêtre de victoire
/// 

using System;
using System.Drawing;
using System.Windows.Forms;

namespace BattleShip
{
    public class VictoryForm : Form
    {
        /// <summary>
        /// Fenêtre de victoire affichée lorsque le joueur gagne la partie.
        /// </summary>
        public VictoryForm()
        {
            // Propriétés de base de la fenêtre
            Text = "EndGame";       // Titre de la fenêtre
            StartPosition = FormStartPosition.CenterParent; // Centre la fenêtre par rapport à la fenêtre principale
            FormBorderStyle = FormBorderStyle.FixedDialog;  // Empêche le redimensionnement
            MaximizeBox = false;    // Enleve le bouton maximiser
            MinimizeBox = false;    // Enleve le bouton minimiser
            Width = 320;            // Largeur de la fenêtre
            Height = 180;           // Hauteur de la fenêtre

            // Label de message
            Label labelWin = new Label
            {
                Text = "Victoire! Tous les bateaux ont été coulés!",
                AutoSize = false,                               // Ne redimensionne pas automatiquement
                TextAlign = ContentAlignment.MiddleCenter,      // Centre le texte
                Font = new Font("Segoe UI", 11, FontStyle.Bold),// Définition de la police
                Dock = DockStyle.Top,                           // Ancre en haut
                Height = 60                                     // Hauteur du label
            };

            // Bouton Rejouer dans la fenetre de victoire
            Button btnReplay = new Button
            {
                Text = "Rejouer",               // Texte du bouton
                Width = 100,                    // Largeur du bouton
                Height = 35,                    // Hauteur du bouton
                DialogResult = DialogResult.OK, // Résultat de dialogue OK
                Font = new Font("Segoe UI", 9, FontStyle.Regular)     // Définition de la police

            };
            btnReplay.Top = 70;                 // Position verticale
            btnReplay.Left = ((ClientSize.Width / 2) - btnReplay.Width) - 5; // Met l
            btnReplay.Anchor = AnchorStyles.Top;// Ancre en haut

            // Événement clic du bouton Rejouer
            btnReplay.Click += (sender, e) =>
            {
                DialogResult = DialogResult.OK;
                Close();
            };

            // Bouton Rejouer dans la fenetre de victoire
            Button btnExit = new Button
            {
                Text = "Quitter",               // Texte du bouton
                Width = 100,                    // Largeur du bouton
                Height = 35,                    // Hauteur du bouton
                DialogResult = DialogResult.OK, // Résultat de dialogue OK
                Font = new Font("Segoe UI", 9, FontStyle.Regular)     // Définition de la police

            };
            btnExit.Top = 70;                 // Position verticale
            btnExit.Left = ((ClientSize.Width + (btnExit.Width/5)) / 2) - 5; // Centre horizontalement
            btnExit.Anchor = AnchorStyles.Top;// Ancre en haut

            btnExit.Click += (sender, e) =>
            {
                DialogResult = DialogResult.OK;
                Application.Exit();
            };

            Controls.Add(labelWin);
            Controls.Add(btnReplay);
            Controls.Add(btnExit);

            // Touche Entrée = Rejouer
            AcceptButton = btnReplay;
        }
    }
}
