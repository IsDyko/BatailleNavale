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
                    btnGame.Text = "";
                    btnGame.Width = width;
                    btnGame.Height = width;
                    btnGame.Left = j * (width + space);
                    btnGame.Top = i * (width + space);
                    this.Controls.Add(btnGame);
                }
            }
        }
    }
}
