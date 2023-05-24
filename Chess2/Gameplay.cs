using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess2
{
    public enum GameStatus { Normal, White_Check, Black_Check, White_Checkmate, Black_Checkmate, Stalemate }
    internal class Gameplay
    {
        Form1 form;
        List<Piece> whitePieces;
        List<Piece> blackPieces;
        GameStatus status = GameStatus.Normal;

        public Gameplay(Form1 form, Board board)
        {
            this.form = form;
            whitePieces = board.getWhitePieces;
            blackPieces = board.getBlackPieces;
        }

        public event EventHandler<String> whiteEvent;
        public event EventHandler<String> blackEvent;

        public bool whiteTurn()
        {
            for (int i = 0; i < whitePieces.Count; i++)
            {
                whitePieces[i]._position.Enabled = true;
            }

            whiteEvent?.Invoke(this, "white");

            return false;

        }
        public bool blackTurn()
        {
            for (int i = 0; i < blackPieces.Count; i++)
            {
                blackPieces[i]._position.Enabled = true;
            }

            blackEvent?.Invoke(this, "black");

            return false;

        }

        public void turnHandler(object sender, TurnEndedEventArgs e)
        {
            counter++;
            if (counter % 2 == 0)
            { status = e.isStatusCheck ? (e.isStatusCheckmate ? GameStatus.Black_Checkmate : GameStatus.Black_Check) : GameStatus.Normal; sidebarHandler(); whiteTurn(); }

            else if (counter % 2 == 1)
            { status = e.isStatusCheck ? (e.isStatusCheckmate ? GameStatus.White_Checkmate : GameStatus.White_Check) : GameStatus.Normal; sidebarHandler(); blackTurn(); }

            if (status == GameStatus.White_Checkmate || status == GameStatus.Black_Checkmate || status == GameStatus.Stalemate)
                form.timer.Enabled = false;
        }

        int counter = 0;

        int whiteTimer = 0, blackTimer = 0;

        public void timerHandler()
        {
            if (counter % 2 == 0)
            {
                whiteTimer++;
                form.whiteTimer.Text = TimeSpan.FromSeconds(whiteTimer).ToString(@"mm\:ss");
            }
            else if (counter % 2 == 1)
            {
                blackTimer++;
                form.blackTimer.Text = TimeSpan.FromSeconds(blackTimer).ToString(@"mm\:ss");
            }
        }

        private void sidebarHandler()
        {
            if (status == GameStatus.Normal)
            {
                form.status.Text = "Normal";
                form.status.ForeColor = Color.MediumBlue;
                form.status.Location = new Point(45, form.status.Location.Y);
            }
            else if (status == GameStatus.White_Check)
            {
                form.status.Text = "Check";
                form.status.ForeColor = Color.Crimson;
                form.status.Location = new Point(51, form.status.Location.Y);
            }
            else if (status == GameStatus.Black_Check)
            {
                form.status.Text = "Check";
                form.status.ForeColor = Color.Crimson;
                form.status.Location = new Point(51, form.status.Location.Y);
            }
            else if (status == GameStatus.White_Checkmate)
            {
                form.status.Text = "Checkmate";
                form.status.ForeColor = Color.Firebrick;
                form.status.Location = new Point(29, form.status.Location.Y);

                form.winner.Text = "White";
                form.winner.Location = new Point(53, form.winner.Location.Y);
            }
            else if (status == GameStatus.Black_Checkmate)
            {
                form.status.Text = "Checkmate";
                form.status.ForeColor = Color.Firebrick;
                form.status.Location = new Point(29, form.status.Location.Y);

                form.winner.Text = "Black";
                form.winner.Location = new Point(56, form.winner.Location.Y);
            }
            else if (status == GameStatus.Stalemate)
            {
                form.status.Text = "Stalemate";
                form.status.ForeColor = Color.Goldenrod;
                form.status.Location = new Point(34, form.status.Location.Y);
            }
        }
    }
}
