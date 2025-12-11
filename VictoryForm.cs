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
                Font = new Font("Arial", 11, FontStyle.Bold),   // Définition de la police
                Dock = DockStyle.Top,                           // Ancre en haut
                Height = 60                                     // Hauteur du label
            };

            // Bouton Rejouer dans la fenetre de victoire
            Button btnReplay = new Button
            {
                Text = "Rejouer",               // Texte du bouton
                Width = 100,                    // Largeur du bouton
                Height = 35,                    // Hauteur du bouton
                DialogResult = DialogResult.OK  // Résultat de dialogue OK
            };
            btnReplay.Top = 70;                 // Position verticale
            btnReplay.Left = (ClientSize.Width - btnReplay.Width) / 2; // Centre horizontalement
            btnReplay.Anchor = AnchorStyles.Top;// Ancre en haut

            // Événement clic du bouton Rejouer
            btnReplay.Click += (sender, e) =>
            {
                DialogResult = DialogResult.OK;
                Close();
            };

            Controls.Add(labelWin);
            Controls.Add(btnReplay);

            // Touche Entrée = Rejouer
            AcceptButton = btnReplay;
        }
    }
}
