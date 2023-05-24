using Chess2.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Net.NetworkInformation;
using System.Globalization;

namespace Chess2
{
    public class Piece
    {
        String color; //white = 0, black = 1
        String type; //pawn = 0, knight = 1, bishop = 2, rook = 3, queen = 4, king = 5
        Button position;

        public String _type { get { return type; } set { type = value; } }
        public String _color { get { return color; } set { color = value; } }
        public Button _position { get { return position; } set { position = value; } }

        public Piece(String color, String type, Button position)
        {
            this.color = color;
            this.type = type;
            this.position = position;
            chessImagery();
        }

        private void chessImagery()
        {
            if (color == "white" && type == "pawn") { position.Image = Properties.Resources.Chess_plt45_svg; position.Image.Tag = "wp"; }
            if (color == "black" && type == "pawn") { position.Image = Properties.Resources.Chess_pdt45_svg; position.Image.Tag = "bp"; }
            if (color == "white" && type == "knight") { position.Image = Properties.Resources.Chess_nlt45_svg; position.Image.Tag = "wn"; }
            if (color == "black" && type == "knight") { position.Image = Properties.Resources.Chess_ndt45_svg; position.Image.Tag = "bn"; }
            if (color == "white" && type == "bishop") { position.Image = Properties.Resources.Chess_blt45_svg; position.Image.Tag = "wb"; }
            if (color == "black" && type == "bishop") { position.Image = Properties.Resources.Chess_bdt45_svg; position.Image.Tag = "bb"; }
            if (color == "white" && type == "rook") { position.Image = Properties.Resources.Chess_rlt45_svg; position.Image.Tag = "wr"; }
            if (color == "black" && type == "rook") { position.Image = Properties.Resources.Chess_rdt45_svg; position.Image.Tag = "br"; }
            if (color == "white" && type == "queen") { position.Image = Properties.Resources.Chess_qlt45_svg; position.Image.Tag = "wq"; }
            if (color == "black" && type == "queen") { position.Image = Properties.Resources.Chess_qdt45_svg; position.Image.Tag = "bq"; }
            if (color == "white" && type == "king") { position.Image = Properties.Resources.Chess_klt45_svg; position.Image.Tag = "wk"; }
            if (color == "black" && type == "king") { position.Image = Properties.Resources.Chess_kdt45_svg; position.Image.Tag = "bk"; }
        }

        public (int, int) kingPosition(Button[,] tile)
        {
            (int x, int y) k_coords = tile.Cast<Button>().Select((button, index) => new { button, index }).Where(pair => pair.button.Image != null && ((color == "white" && pair.button.Image.Tag == "wk") || (color == "black" && pair.button.Image.Tag == "bk"))).Select(pair => ((pair.index / 8), (pair.index % 8))).FirstOrDefault();

            return k_coords;
        }

        public void movePiece(Button[,] tile, int x, int y)
        {
            isLegalMove = isTileAttacked_0(tile, kingPosition(tile).Item1, kingPosition(tile).Item2) ? false : true;

            switch (type)
            {
                case "pawn":
                    movePawn(tile, x, y);
                    break;

                case "knight":
                    moveKnight(tile, x, y);
                    break;

                case "bishop":
                    moveBishop(tile, x, y);
                    break;

                case "rook":
                    moveRook(tile, x, y);
                    break;

                case "queen":
                    moveQueen(tile, x, y);
                    break;

                case "king":
                    moveKing(tile, x, y);
                    break;
            }
        }

        public bool isFirstTurn = true;

        public bool isQueenSideCastlingPossible = true;

        public bool isKingSideCastlingPossible = true;

        public static bool isStatusCheck = false;

        bool isLegalMove;

        public static Button? enPassantTile { get; set; }

        bool isEnPassantLegal(Button[,] tile, int x, int y)
        {
            Button lefthandside = new(), righthandside = new();

            for (int i = -1; x + i >= 0; i--)
                if (tile[x + i, y].Image != null && tile[x + i, y] != enPassantTile)
                { lefthandside = tile[x + i, y]; break; }

            for (int i = 1; x + i < 8; i++)
                if (tile[x + i, y].Image != null && tile[x + i, y] != enPassantTile)
                { righthandside = tile[x + i, y]; break; }

            if (lefthandside.Image != null && lefthandside.Enabled == true && (lefthandside.Image.Tag == "wk" || lefthandside.Image.Tag == "bk") && righthandside.Image != null && righthandside.Enabled == false && (righthandside.Image.Tag == "wq" || righthandside.Image.Tag == "bq" || righthandside.Image.Tag == "wr" || righthandside.Image.Tag == "br")) return false;
            else if (righthandside.Image != null && righthandside.Enabled == true && (righthandside.Image.Tag == "wk" || righthandside.Image.Tag == "bk") && lefthandside.Image != null && lefthandside.Enabled == false && (lefthandside.Image.Tag == "wq" || lefthandside.Image.Tag == "bq" || lefthandside.Image.Tag == "wr" || lefthandside.Image.Tag == "br")) return false;
            else return true;
        }

        private void movePawn(Button[,] tile, int x, int y)
        {
            int d = (color == "white") ? -1 : 1; // d stands for direction on y axis

            if (!isStatusCheck)
            {
                if (isLegalMove || (a == 0 && (b == -1 || b == 1)))
                {
                    if (isFirstTurn)
                        oneWayMove(tile, x, y, 0, d, 2);
                    else
                        oneWayMove(tile, x, y, 0, d, 1);
                }

                if (isLegalMove || a == 1 && b == d)
                {
                    if (x + 1 < 8 && tile[x + 1, y + d].Image != null && tile[x + 1, y + d].Enabled == false)
                        oneWayMove(tile, x, y, 1, d, 1);
                }
                if (isLegalMove || a == -1 && b == d)
                {
                    if (x - 1 >= 0 && tile[x - 1, y + d].Image != null && tile[x - 1, y + d].Enabled == false)
                        oneWayMove(tile, x, y, -1, d, 1);
                }

                if (isLegalMove || (a == 1 && b == d || a == -1 && b == -d))
                {
                    if (position == tile[x, (7 + d) / 2] && x + 1 < 8 && tile[x + 1, (7 + d) / 2] == enPassantTile && isEnPassantLegal(tile, x, y))
                        oneWayMove(tile, x, (7 + d) / 2, 1, d, 1);
                }
                if (isLegalMove || (a == -1 && b == d || a == 1 && b == -d))
                {
                    if (position == tile[x, (7 + d) / 2] && x - 1 >= 0 && tile[x - 1, (7 + d) / 2] == enPassantTile && isEnPassantLegal(tile, x, y))
                        oneWayMove(tile, x, (7 + d) / 2, -1, d, 1);
                }
            }
            else for (int i = 0; i < checkStatusPossibleMoves.Item1.Count; i++) if (checkStatusPossibleMoves.Item1[i] == position) oneWayMove(tile, x, y, checkStatusPossibleMoves.Item2[i] - x, checkStatusPossibleMoves.Item3[i] - y, 1);
        }

        private void moveKnight(Button[,] tile, int x, int y)
        {
            if (!isStatusCheck)
            {
                if (isLegalMove)
                {
                    oneWayMove(tile, x, y, 1, 2, 1);
                    oneWayMove(tile, x, y, 1, -2, 1);
                    oneWayMove(tile, x, y, -1, 2, 1);
                    oneWayMove(tile, x, y, -1, -2, 1);
                    oneWayMove(tile, x, y, 2, 1, 1);
                    oneWayMove(tile, x, y, 2, -1, 1);
                    oneWayMove(tile, x, y, -2, 1, 1);
                    oneWayMove(tile, x, y, -2, -1, 1);
                }
            }
            else for (int i = 0; i < checkStatusPossibleMoves.Item1.Count; i++) if (checkStatusPossibleMoves.Item1[i] == position) oneWayMove(tile, x, y, checkStatusPossibleMoves.Item2[i] - x, checkStatusPossibleMoves.Item3[i] - y, 1);
        }

        private void moveBishop(Button[,] tile, int x, int y)
        {
            if (!isStatusCheck)
            {
                if (isLegalMove || (a == -1 && b == -1 || a == 1 && b == 1))
                {
                    oneWayMove(tile, x, y, 1, 1, 7); // x, y
                    oneWayMove(tile, x, y, -1, -1, 7); // -x, -y
                }
                if (isLegalMove || (a == 1 && b == -1 || a == -1 && b == 1))
                {
                    oneWayMove(tile, x, y, 1, -1, 7); // x, -y
                    oneWayMove(tile, x, y, -1, 1, 7); // -x, y
                }
            }
            else for (int i = 0; i < checkStatusPossibleMoves.Item1.Count; i++) if (checkStatusPossibleMoves.Item1[i] == position) oneWayMove(tile, x, y, checkStatusPossibleMoves.Item2[i] - x, checkStatusPossibleMoves.Item3[i] - y, 1);
        }

        private void moveRook(Button[,] tile, int x, int y)
        {
            if (!isStatusCheck)
            {
                if (isLegalMove || ((a == -1 || a == 1) && b == 0))
                {
                    oneWayMove(tile, x, y, -1, 0, 7); // leftward
                    oneWayMove(tile, x, y, 1, 0, 7); // rightward
                }
                if (isLegalMove || (a == 0 && (b == -1 || b == 1)))
                {
                    oneWayMove(tile, x, y, 0, -1, 7); // upward
                    oneWayMove(tile, x, y, 0, 1, 7); // downward
                }
            }
            else for (int i = 0; i < checkStatusPossibleMoves.Item1.Count; i++) if (checkStatusPossibleMoves.Item1[i] == position) oneWayMove(tile, x, y, checkStatusPossibleMoves.Item2[i] - x, checkStatusPossibleMoves.Item3[i] - y, 1);
        }

        private void moveQueen(Button[,] tile, int x, int y)
        {
            if (!isStatusCheck)
            {
                if (isLegalMove || ((a == -1 || a == 1) && b == 0))
                {
                    oneWayMove(tile, x, y, -1, 0, 7); // leftward
                    oneWayMove(tile, x, y, 1, 0, 7); // rightward
                }
                if (isLegalMove || (a == 0 && (b == -1 || b == 1)))
                {
                    oneWayMove(tile, x, y, 0, -1, 7); // upward
                    oneWayMove(tile, x, y, 0, 1, 7); // downward
                }
                if (isLegalMove || (a == -1 && b == -1 || a == 1 && b == 1))
                {
                    oneWayMove(tile, x, y, 1, 1, 7); // x, y
                    oneWayMove(tile, x, y, -1, -1, 7); // -x, -y
                }
                if (isLegalMove || (a == 1 && b == -1 || a == -1 && b == 1))
                {
                    oneWayMove(tile, x, y, 1, -1, 7); // x, -y
                    oneWayMove(tile, x, y, -1, 1, 7); // -x, y
                }
            }
            else for (int i = 0; i < checkStatusPossibleMoves.Item1.Count; i++) if (checkStatusPossibleMoves.Item1[i] == position) oneWayMove(tile, x, y, checkStatusPossibleMoves.Item2[i] - x, checkStatusPossibleMoves.Item3[i] - y, 1);
        }

        private void moveKing(Button[,] tile, int x, int y)
        {
            bool xm1_y = false, x1_y = false, x_ym1 = false, x_y1 = false, xm1_ym1 = false, x1_ym1 = false, x1_y1 = false, xm1_y1 = false, qsc = false, ksc = false;

            if (!isTileAttacked_0(tile, x - 1, y)) xm1_y = true;
            if (!isTileAttacked_0(tile, x + 1, y)) x1_y = true;
            if (!isTileAttacked_0(tile, x, y - 1)) x_ym1 = true;
            if (!isTileAttacked_0(tile, x, y + 1)) x_y1 = true;
            if (!isTileAttacked_0(tile, x - 1, y - 1)) xm1_ym1 = true;
            if (!isTileAttacked_0(tile, x + 1, y - 1)) x1_ym1 = true;
            if (!isTileAttacked_0(tile, x + 1, y + 1)) x1_y1 = true;
            if (!isTileAttacked_0(tile, x - 1, y + 1)) xm1_y1 = true;

            if (isFirstTurn && isQueenSideCastlingPossible && tile[x - 1, y].Image == null && tile[x - 2, y].Image == null && tile[x - 3, y].Image == null && !isTileAttacked_0(tile, x, y) && !isTileAttacked_0(tile, x - 1, y) && !isTileAttacked_0(tile, x - 2, y)) qsc = true;
            if (isFirstTurn && isKingSideCastlingPossible && tile[x + 1, y].Image == null && tile[x + 2, y].Image == null && !isTileAttacked_0(tile, x, y) && !isTileAttacked_0(tile, x + 1, y) && !isTileAttacked_0(tile, x + 2, y)) ksc = true;

            if (xm1_y == true) oneWayMove(tile, x, y, -1, 0, 1); // leftward
            if (x1_y == true) oneWayMove(tile, x, y, 1, 0, 1); // rightward
            if (x_ym1 == true) oneWayMove(tile, x, y, 0, -1, 1); // upward
            if (x_y1 == true) oneWayMove(tile, x, y, 0, 1, 1); // downward
            if (xm1_ym1 == true) oneWayMove(tile, x, y, -1, -1, 1); // -x, -y
            if (x1_ym1 == true) oneWayMove(tile, x, y, 1, -1, 1); // x, -y
            if (x1_y1 == true) oneWayMove(tile, x, y, 1, 1, 1); // x, y
            if (xm1_y1 == true) oneWayMove(tile, x, y, -1, 1, 1); // -x, y

            if (qsc == true) oneWayMove(tile, x, y, -2, 0, 1); // queenside castling
            if (ksc == true) oneWayMove(tile, x, y, 2, 0, 1); // kingside castling
        }

        public static bool stalemateCheck = false;
        public static bool isMovePossible = false;

        private void oneWayMove(Button[,] tile, int x, int y, int a, int b, int s)
        {
            for (int i = 1; i <= s; i++)
            {
                if ((x + (i * a) >= 0 && x + (i * a) < 8) && (y + (i * b) >= 0 && y + (i * b) < 8) && tile[x + (i * a), y + (i * b)].Enabled == false && (tile[x + (i * a), y + (i * b)].Image == null || (tile[x + (i * a), y + (i * b)].Image != null && (type != "pawn" || (type == "pawn" && a != 0)))))
                {
                    possibleMoveTile.Add(tile[x + (i * a), y + (i * b)]);
                    tile[x + (i * a), y + (i * b)].Enabled = true;

                    if (tile[x + (i * a), y + (i * b)].Image != null)
                    {
                        tile[x + (i * a), y + (i * b)].BackgroundImage = tile[x + (i * a), y + (i * b)].Image;
                        tile[x + (i * a), y + (i * b)].BackgroundImageLayout = ImageLayout.Center;
                        tile[x + (i * a), y + (i * b)].Image = Properties.Resources.Chess_possible_move;
                        break;
                    }
                    else
                        tile[x + (i * a), y + (i * b)].Image = Properties.Resources.Chess_possible_move;
                }
                else break;
            }
        }

        List<Button> possibleMoveTile = new List<Button>();

        public void clearPossibleMove(Button start, Button end)
        {
            foreach (var item in possibleMoveTile)
            {
                if (item != end)
                {
                    item.Enabled = false;
                    item.Image = null;
                    if (item.BackgroundImage != null)
                    {
                        item.Image = item.BackgroundImage;
                        item.BackgroundImage = null;
                    }
                }
            }
            start.Enabled = false;
            possibleMoveTile.Clear();
        }
        public void clearPossibleMove()
        {
            foreach (var item in possibleMoveTile)
            {
                item.Enabled = false;
                item.Image = null;
                if (item.BackgroundImage != null)
                {
                    item.Image = item.BackgroundImage;
                    item.BackgroundImage = null;
                }
            }
            possibleMoveTile.Clear();
        }

        int a, b;

        public bool isTileAttacked_0(Button[,] tile, int x, int y)
        {
            if (adjacentTileControl_0(tile, x, y, -1, -1, 7)) return true;
            if (adjacentTileControl_0(tile, x, y, 0, -1, 7)) return true;
            if (adjacentTileControl_0(tile, x, y, 1, -1, 7)) return true;
            if (adjacentTileControl_0(tile, x, y, 1, 0, 7)) return true;
            if (adjacentTileControl_0(tile, x, y, 1, 1, 7)) return true;
            if (adjacentTileControl_0(tile, x, y, 0, 1, 7)) return true;
            if (adjacentTileControl_0(tile, x, y, -1, 1, 7)) return true;
            if (adjacentTileControl_0(tile, x, y, -1, 0, 7)) return true;

            if (adjacentTileControl_0(tile, x, y, 1, 2, 1)) return true;
            if (adjacentTileControl_0(tile, x, y, 1, -2, 1)) return true;
            if (adjacentTileControl_0(tile, x, y, -1, 2, 1)) return true;
            if (adjacentTileControl_0(tile, x, y, -1, -2, 1)) return true;
            if (adjacentTileControl_0(tile, x, y, 2, 1, 1)) return true;
            if (adjacentTileControl_0(tile, x, y, 2, -1, 1)) return true;
            if (adjacentTileControl_0(tile, x, y, -2, 1, 1)) return true;
            if (adjacentTileControl_0(tile, x, y, -2, -1, 1)) return true;

            if (adjacentTileControl_0(tile, x, y, -1, -1, 1)) return true;
            if (adjacentTileControl_0(tile, x, y, 0, -1, 1)) return true;
            if (adjacentTileControl_0(tile, x, y, 1, -1, 1)) return true;
            if (adjacentTileControl_0(tile, x, y, 1, 0, 1)) return true;
            if (adjacentTileControl_0(tile, x, y, 1, 1, 1)) return true;
            if (adjacentTileControl_0(tile, x, y, 0, 1, 1)) return true;
            if (adjacentTileControl_0(tile, x, y, -1, 1, 1)) return true;
            if (adjacentTileControl_0(tile, x, y, -1, 0, 1)) return true;

            return false;
        }

        bool adjacentTileControl_0(Button[,] tile, int x, int y, int a, int b, int s)
        {
            for (int i = 1; i <= s; i++)
            {
                if ((x + (i * a) >= 0 && x + (i * a) < 8) && (y + (i * b) >= 0 && y + (i * b) < 8) && (tile[x + (i * a), y + (i * b)].Enabled == false || tile[x + (i * a), y + (i * b)] == position))
                {
                    if (tile[x + (i * a), y + (i * b)].Image != null && tile[x + (i * a), y + (i * b)] != position)
                    {
                        if ((Math.Abs(a) + Math.Abs(b) == 2 && s == 7) && (tile[x + (i * a), y + (i * b)].Image.Tag == "wb" || tile[x + (i * a), y + (i * b)].Image.Tag == "wq" || tile[x + (i * a), y + (i * b)].Image.Tag == "bb" || tile[x + (i * a), y + (i * b)].Image.Tag == "bq"))
                        {
                            this.a = a; this.b = b; return true;
                        }
                        else if ((Math.Abs(a + b) == 1 && s == 7) && (tile[x + (i * a), y + (i * b)].Image.Tag == "wr" || tile[x + (i * a), y + (i * b)].Image.Tag == "wq" || tile[x + (i * a), y + (i * b)].Image.Tag == "br" || tile[x + (i * a), y + (i * b)].Image.Tag == "bq"))
                        {
                            this.a = a; this.b = b; return true;
                        }
                        else if ((Math.Abs(a) + Math.Abs(b) == 3 && s == 1) && (tile[x + (i * a), y + (i * b)].Image.Tag == "wn" || tile[x + (i * a), y + (i * b)].Image.Tag == "bn"))
                        {
                            return true;
                        }
                        else if (((Math.Abs(a) + Math.Abs(b) == 2 || Math.Abs(a) + Math.Abs(b) == 1) && (s == 1)) && ((color == "white" && tile[x + (i * a), y + (i * b)].Image.Tag == "bk") || (color == "black" && tile[x + (i * a), y + (i * b)].Image.Tag == "wk")))
                        {
                            return true;
                        }
                        else if ((Math.Abs(a) == 1 && s == 1) && ((color == "white" && b == -1 && tile[x + (i * a), y + (i * b)].Image.Tag == "bp") || (color == "black" && b == 1 && tile[x + (i * a), y + (i * b)].Image.Tag == "wp")))
                        {
                            return true;
                        }
                        else break;
                    }
                }
                else break;
            }
            return false;
        }

        public static (List<Button>, List<int>, List<int>) checkStatusPossibleMoves = (new List<Button>(), new List<int>(), new List<int>());
        public static (List<int>, List<int>) threateningTiles = (new List<int>(), new List<int>());

        bool adjacentTileControl_1(Button[,] tile, int x, int y, int a, int b, int s)
        {
            for (int i = 1; i <= s; i++)
            {
                if ((x + (i * a) >= 0 && x + (i * a) < 8) && (y + (i * b) >= 0 && y + (i * b) < 8) && (tile[x + (i * a), y + (i * b)].Enabled == true || tile[x + (i * a), y + (i * b)].Image == null || tile[x + (i * a), y + (i * b)] == position))
                {
                    if (tile[x + (i * a), y + (i * b)].Enabled == true)
                    {
                        if ((Math.Abs(a) + Math.Abs(b) == 2 && s == 7) && (tile[x + (i * a), y + (i * b)].Image.Tag == "wb" || tile[x + (i * a), y + (i * b)].Image.Tag == "wq" || tile[x + (i * a), y + (i * b)].Image.Tag == "bb" || tile[x + (i * a), y + (i * b)].Image.Tag == "bq"))
                        {
                            for (int j = i; j > 0; j--) { threateningTiles.Item1.Add(x + (j * a)); threateningTiles.Item2.Add(y + (j * b)); }
                            return true;
                        }
                        else if ((Math.Abs(a + b) == 1 && s == 7) && (tile[x + (i * a), y + (i * b)].Image.Tag == "wr" || tile[x + (i * a), y + (i * b)].Image.Tag == "wq" || tile[x + (i * a), y + (i * b)].Image.Tag == "br" || tile[x + (i * a), y + (i * b)].Image.Tag == "bq"))
                        {
                            for (int j = i; j > 0; j--) { threateningTiles.Item1.Add(x + (j * a)); threateningTiles.Item2.Add(y + (j * b)); }
                            return true;
                        }
                        else if ((Math.Abs(a) + Math.Abs(b) == 3 && s == 1) && (tile[x + (i * a), y + (i * b)].Image.Tag == "wn" || tile[x + (i * a), y + (i * b)].Image.Tag == "bn"))
                        {
                            for (int j = i; j > 0; j--) { threateningTiles.Item1.Add(x + (j * a)); threateningTiles.Item2.Add(y + (j * b)); }
                            return true;
                        }
                        else if ((Math.Abs(a) == 1 && s == 1) && ((color == "white" && b == -1 && tile[x + (i * a), y + (i * b)].Image.Tag == "bp") || (color == "black" && b == 1 && tile[x + (i * a), y + (i * b)].Image.Tag == "wp")))
                        {
                            for (int j = i; j > 0; j--) { threateningTiles.Item1.Add(x + (j * a)); threateningTiles.Item2.Add(y + (j * b)); }
                            return true;
                        }
                        else break;
                    }
                }
                else break;
            }
            return false;
        }

        public bool isTileAttacked_1(Button[,] tile, int x, int y)
        {
            int attackingPiece = 0;

            if (adjacentTileControl_1(tile, x, y, -1, -1, 7)) attackingPiece++;
            if (adjacentTileControl_1(tile, x, y, 0, -1, 7)) attackingPiece++;
            if (adjacentTileControl_1(tile, x, y, 1, -1, 7)) attackingPiece++;
            if (adjacentTileControl_1(tile, x, y, 1, 0, 7)) attackingPiece++;
            if (adjacentTileControl_1(tile, x, y, 1, 1, 7)) attackingPiece++;
            if (adjacentTileControl_1(tile, x, y, 0, 1, 7)) attackingPiece++;
            if (adjacentTileControl_1(tile, x, y, -1, 1, 7)) attackingPiece++;
            if (adjacentTileControl_1(tile, x, y, -1, 0, 7)) attackingPiece++;

            if (adjacentTileControl_1(tile, x, y, 1, 2, 1)) attackingPiece++;
            if (adjacentTileControl_1(tile, x, y, 1, -2, 1)) attackingPiece++;
            if (adjacentTileControl_1(tile, x, y, -1, 2, 1)) attackingPiece++;
            if (adjacentTileControl_1(tile, x, y, -1, -2, 1)) attackingPiece++;
            if (adjacentTileControl_1(tile, x, y, 2, 1, 1)) attackingPiece++;
            if (adjacentTileControl_1(tile, x, y, 2, -1, 1)) attackingPiece++;
            if (adjacentTileControl_1(tile, x, y, -2, 1, 1)) attackingPiece++;
            if (adjacentTileControl_1(tile, x, y, -2, -1, 1)) attackingPiece++;

            if (adjacentTileControl_1(tile, x, y, -1, -1, 1)) attackingPiece++;
            if (adjacentTileControl_1(tile, x, y, 1, -1, 1)) attackingPiece++;
            if (adjacentTileControl_1(tile, x, y, 1, 1, 1)) attackingPiece++;
            if (adjacentTileControl_1(tile, x, y, -1, 1, 1)) attackingPiece++;

            if (attackingPiece == 0) { return false; }
            else if (attackingPiece == 1) { return true; }
            else /*if (attackingPiece == 2)*/ { threateningTiles.Item1.Clear(); threateningTiles.Item2.Clear(); return true; }
        }

        void adjacentTileControl_2(Button[,] tile, int x, int y, int a, int b, int s)
        {
            for (int i = 1; i <= s; i++)
            {
                if ((x + (i * a) >= 0 && x + (i * a) < 8) && (y + (i * b) >= 0 && y + (i * b) < 8) && (tile[x + (i * a), y + (i * b)].Enabled == false))
                {
                    if (tile[x + (i * a), y + (i * b)].Image != null)
                    {
                        if ((Math.Abs(a) + Math.Abs(b) == 2 && s == 7) && (tile[x + (i * a), y + (i * b)].Image.Tag == "wb" || tile[x + (i * a), y + (i * b)].Image.Tag == "wq" || tile[x + (i * a), y + (i * b)].Image.Tag == "bb" || tile[x + (i * a), y + (i * b)].Image.Tag == "bq"))
                        {
                            checkStatusPossibleMoves.Item1.Add(tile[x + (i * a), y + (i * b)]);
                            checkStatusPossibleMoves.Item2.Add(x);
                            checkStatusPossibleMoves.Item3.Add(y);
                        }
                        else if ((Math.Abs(a + b) == 1 && s == 7) && (tile[x + (i * a), y + (i * b)].Image.Tag == "wr" || tile[x + (i * a), y + (i * b)].Image.Tag == "wq" || tile[x + (i * a), y + (i * b)].Image.Tag == "br" || tile[x + (i * a), y + (i * b)].Image.Tag == "bq"))
                        {
                            checkStatusPossibleMoves.Item1.Add(tile[x + (i * a), y + (i * b)]);
                            checkStatusPossibleMoves.Item2.Add(x);
                            checkStatusPossibleMoves.Item3.Add(y);
                        }
                        else if ((Math.Abs(a) + Math.Abs(b) == 3 && s == 1) && (tile[x + (i * a), y + (i * b)].Image.Tag == "wn" || tile[x + (i * a), y + (i * b)].Image.Tag == "bn"))
                        {
                            checkStatusPossibleMoves.Item1.Add(tile[x + (i * a), y + (i * b)]);
                            checkStatusPossibleMoves.Item2.Add(x);
                            checkStatusPossibleMoves.Item3.Add(y);
                        }
                        else if (((tile[x, y].Image == null && a == 0 && (s == 1 || s == 2)) || (Math.Abs(a) == 1 && s == 1)) && ((color == "white" && b == 1 && tile[x + (i * a), y + (i * b)].Image.Tag == "wp") || (color == "black" && b == -1 && tile[x + (i * a), y + (i * b)].Image.Tag == "bp")))
                        {
                            checkStatusPossibleMoves.Item1.Add(tile[x + (i * a), y + (i * b)]);
                            checkStatusPossibleMoves.Item2.Add(x);
                            checkStatusPossibleMoves.Item3.Add(y);
                        }
                        else if ((Math.Abs(a) == 1 && b == 0 && s == 1) && (tile[x + (i * a), y + (i * b)].Image.Tag == "wp" || tile[x + (i * a), y + (i * b)].Image.Tag == "bp"))
                        {
                            checkStatusPossibleMoves.Item1.Add(tile[x + (i * a), y + (i * b)]);
                            checkStatusPossibleMoves.Item2.Add(x);
                            if (color == "white") checkStatusPossibleMoves.Item3.Add(y - 1); else checkStatusPossibleMoves.Item3.Add(y + 1);
                        }
                        else break;
                    }
                }
                else break;
            }
        }

        public void isTileAttacked_2(Button[,] tile, int x, int y)
        {
            adjacentTileControl_2(tile, x, y, -1, -1, 7);
            adjacentTileControl_2(tile, x, y, 0, -1, 7);
            adjacentTileControl_2(tile, x, y, 1, -1, 7);
            adjacentTileControl_2(tile, x, y, 1, 0, 7);
            adjacentTileControl_2(tile, x, y, 1, 1, 7);
            adjacentTileControl_2(tile, x, y, 0, 1, 7);
            adjacentTileControl_2(tile, x, y, -1, 1, 7);
            adjacentTileControl_2(tile, x, y, -1, 0, 7);

            adjacentTileControl_2(tile, x, y, 1, 2, 1);
            adjacentTileControl_2(tile, x, y, 1, -2, 1);
            adjacentTileControl_2(tile, x, y, -1, 2, 1);
            adjacentTileControl_2(tile, x, y, -1, -2, 1);
            adjacentTileControl_2(tile, x, y, 2, 1, 1);
            adjacentTileControl_2(tile, x, y, 2, -1, 1);
            adjacentTileControl_2(tile, x, y, -2, 1, 1);
            adjacentTileControl_2(tile, x, y, -2, -1, 1);

            int d = (color == "white") ? 1 : -1;

            if (y == (7 + d) / 2)
                adjacentTileControl_2(tile, x, y, 0, d, 2);
            else /*if(y == (7 - 7 * d) / 2 || y == (7 - 5 * d) / 2 || y == (7 - 3 * d) / 2 || y == (7 * -d) / 2 || y == (7 + 3 * d) / 2)*/
                adjacentTileControl_2(tile, x, y, 0, d, 1);

            if (tile[x, y].Image != null)
                adjacentTileControl_2(tile, x, y, 1, d, 1);
            if (tile[x, y].Image != null)
                adjacentTileControl_2(tile, x, y, -1, d, 1);

            if (tile[x, y] == enPassantTile)
                adjacentTileControl_2(tile, x, y, 1, 0, 1);
            if (tile[x, y] == enPassantTile)
                adjacentTileControl_2(tile, x, y, -1, 0, 1);
        }
    }
}