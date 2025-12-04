using System;
using System.Drawing;
using System.Windows.Forms;

namespace BattleShip
{
    public class VictoryForm : Form
    {
        public VictoryForm()
        {
            // Propriétés de base de la fenêtre
            Text = "EndGame";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Width = 320;
            Height = 180;

            // Label de message
            Label labelWin = new Label
            {
                Text = "Victoire ! Tous les bateaux ont été coulés !",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 11, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 60
            };

            // Bouton Rejouer
            Button btnReplay = new Button
            {
                Text = "Rejouer",
                Width = 100,
                Height = 35,
                DialogResult = DialogResult.OK
            };
            btnReplay.Top = 70;
            btnReplay.Left = (ClientSize.Width - btnReplay.Width) / 2;
            btnReplay.Anchor = AnchorStyles.Top;

            // Quand on clique → fermer avec OK
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
