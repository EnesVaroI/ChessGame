using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess2
{
    public partial class PawnPromotion : Form
    {
        Piece pawn;
        public PawnPromotion(Piece pawn)
        {
            InitializeComponent();
            this.pawn = pawn;
        }

        private void PawnPromotion_Load(object sender, EventArgs e)
        {
            if (pawn._color == "white")
            {
                button1.Image = Properties.Resources.Chess_qlt45_svg;
                button2.Image = Properties.Resources.Chess_rlt45_svg;
                button3.Image = Properties.Resources.Chess_blt45_svg;
                button4.Image = Properties.Resources.Chess_nlt45_svg;
            }
            else if (pawn._color == "black")
            {
                button1.Image = Properties.Resources.Chess_qdt45_svg;
                button2.Image = Properties.Resources.Chess_rdt45_svg;
                button3.Image = Properties.Resources.Chess_bdt45_svg;
                button4.Image = Properties.Resources.Chess_ndt45_svg;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pawn._type = "queen";

            if (pawn._color == "white") { pawn._position.Image = Properties.Resources.Chess_qlt45_svg; pawn._position.Image.Tag = "wq"; }
            else if (pawn._color == "black") { pawn._position.Image = Properties.Resources.Chess_qdt45_svg; pawn._position.Image.Tag = "bq"; }

            control = true;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pawn._type = "rook";

            if (pawn._color == "white") { pawn._position.Image = Properties.Resources.Chess_rlt45_svg; pawn._position.Image.Tag = "wr"; }
            else if (pawn._color == "black") { pawn._position.Image = Properties.Resources.Chess_rdt45_svg; pawn._position.Image.Tag = "br"; }

            control = true;
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pawn._type = "bishop";

            if (pawn._color == "white") { pawn._position.Image = Properties.Resources.Chess_blt45_svg; pawn._position.Image.Tag = "wb"; }
            else if (pawn._color == "black") { pawn._position.Image = Properties.Resources.Chess_bdt45_svg; pawn._position.Image.Tag = "bb"; }

            control = true;
            Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            pawn._type = "knight";

            if (pawn._color == "white") { pawn._position.Image = Properties.Resources.Chess_nlt45_svg; pawn._position.Image.Tag = "wn"; }
            else if (pawn._color == "black") { pawn._position.Image = Properties.Resources.Chess_ndt45_svg; pawn._position.Image.Tag = "bn"; }

            control = true;
            Close();
        }

        private void PawnPromotion_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!control)
                e.Cancel = true;
        }

        bool control = false;
    }
}
