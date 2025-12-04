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

        //Creation des objets
        public Form1()
        {
            InitializeComponent();
            int width = 50;
            int space = 2;

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Button btnGame = new Button();
                    btnGame.Text = "X";
                    btnGame.Width = width;
                    btnGame.Height = width;
                    btnGame.Tag = (i, j);
                    btnGame.Left = j * (width + space);
                    btnGame.Top = i * (width + space);
                    btnGame.Click += btnGame_Click;
                    panelGrille.Controls.Add(btnGame);
                }
            }
        }
        void btnGame_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            var (ligne, colonne) = ((int, int))btn.Tag;

            MessageBox.Show($"Bouton à la ligne {ligne}, {colonne} a été pressé");
        }
    }
}
