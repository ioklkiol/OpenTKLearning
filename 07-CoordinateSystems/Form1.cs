using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenTKTutorial07
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            using (Game game = new Game())
            {
                game.Run(60);
            }
            Environment.Exit(0);
        }
    }
}
