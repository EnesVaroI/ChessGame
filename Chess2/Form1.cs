using System.Security.Cryptography.X509Certificates;
using System.Timers;
using System.Windows.Forms;

namespace Chess2
{
    public partial class Form1 : Form
    {
        Board board;
        Gameplay gameplay;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            board = new Board(this);
            board.generateBoard();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Restart")
                restart();
            
            gameplay = new Gameplay(this, board);
            ColorEventSubscription(gameplay, board);
            TurnEndSubscription(gameplay, board);
            button1.Text = "Restart";

            timer1.Enabled = true;
            gameplay.whiteTurn();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            gameplay.timerHandler();
        }

        private void ColorEventSubscription(Gameplay gameplay, Board board)
        {
            gameplay.whiteEvent += board.colorHandler;
            gameplay.blackEvent += board.colorHandler;
        }

        private void TurnEndSubscription(Gameplay gameplay, Board board)
        {
            board.turnEnded += gameplay.turnHandler;
        }

        private void restart()
        {
            gameplay.whiteEvent -= board.colorHandler;
            gameplay.blackEvent -= board.colorHandler;
            board.turnEnded -= gameplay.turnHandler;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board.tile[i, j].Image = null;
                    board.tile[i, j].BackgroundImage = null;
                    board.tile[i, j].Enabled = false;
                }
            }

            whiteTimer.Text = "00:00";
            blackTimer.Text = "00:00";
            status.Text = "Normal";
            status.ForeColor = Color.MediumBlue;
            status.Location = new Point(45, status.Location.Y);
            winner.Text = "N/A";
            winner.Location = new Point(61, winner.Location.Y);

            board.restartGame();
            board.generatePieces();
        }

        public Label whiteTimer { get => label3; }
        public Label blackTimer { get => label4; }
        public Label status { get => label5; }
        public Label winner { get => label6; }
        public System.Windows.Forms.Timer timer { get => timer1; }
    }
}