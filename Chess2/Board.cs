using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Chess2
{
    internal class Board
    {
        public Button[,] tile;
        Form1 form;
        List<Piece> blackPieces;
        List<Piece> whitePieces;

        public Board(Form1 form)
        {
            tile = new Button[8, 8];
            this.form = form;
            blackPieces = new List<Piece>();
            whitePieces = new List<Piece>();
        }
        public void generateBoard()
        {
            tile = new Button[8, 8];
            for (int i = 0; i < tile.GetLength(0); i++)
            {
                for (int j = 0; j < tile.GetLength(1); j++)
                {
                    tile[i, j] = new Button();
                    tile[i, j].Size = new Size(80, 80);
                    tile[i, j].Location = new Point(80 * i, 80 * j);
                    if ((i + j) % 2 == 0) tile[i, j].BackColor = Color.Gray;
                    else tile[i, j].BackColor = Color.LightGray;
                    tile[i, j].FlatStyle = FlatStyle.Flat;
                    tile[i, j].FlatAppearance.BorderSize = 0;
                    tile[i, j].Enabled = false;
                    tile[i, j].TabStop = false;
                    int a = i, b = j;
                    tile[i, j].Click += (s, args) =>
                    {
                        // Handle the button click event here
                        boardHandler(a, b);
                    };

                    form.Controls.Add(tile[i, j]);
                }
            }
            
            generatePieces();

        }

        public void generatePieces()
        {
            blackPieces.Clear();
            whitePieces.Clear();

            for (int i = 0; i < 8; i++)
            {
                blackPieces.Add(new Piece("black", "pawn", tile[i, 1]));
                whitePieces.Add(new Piece("white", "pawn", tile[i, 6]));
            }

            blackPieces.Add(new Piece("black", "rook", tile[0, 0]));
            whitePieces.Add(new Piece("white", "rook", tile[0, 7]));

            blackPieces.Add(new Piece("black", "knight", tile[1, 0]));
            whitePieces.Add(new Piece("white", "knight", tile[1, 7]));

            blackPieces.Add(new Piece("black", "bishop", tile[2, 0]));
            whitePieces.Add(new Piece("white", "bishop", tile[2, 7]));

            blackPieces.Add(new Piece("black", "queen", tile[3, 0]));
            whitePieces.Add(new Piece("white", "queen", tile[3, 7]));

            blackPieces.Add(new Piece("black", "king", tile[4, 0]));
            whitePieces.Add(new Piece("white", "king", tile[4, 7]));

            blackPieces.Add(new Piece("black", "bishop", tile[5, 0]));
            whitePieces.Add(new Piece("white", "bishop", tile[5, 7]));

            blackPieces.Add(new Piece("black", "knight", tile[6, 0]));
            whitePieces.Add(new Piece("white", "knight", tile[6, 7]));

            blackPieces.Add(new Piece("black", "rook", tile[7, 0]));
            whitePieces.Add(new Piece("white", "rook", tile[7, 7]));
        }
        
        public List<Piece> getWhitePieces { get => whitePieces; }
        public List<Piece> getBlackPieces { get => blackPieces; }
        int index;
        private void boardHandler(int a, int b)
        {
            int control = 0;
            
            for (int i = 0; i < pieces.Count; i++)
            {
                if (pieces[i]._position == tile[a, b])
                {
                    control = 1;
                    if(index < pieces.Count)
                        pieces[index].clearPossibleMove();
                    index = i;
                    x = a; y = b;
                    pieces[i].movePiece(tile, a, b);
                    break;
                }
            }

            if (control == 0)
            {
                for (int i = 0; true; i++)
                {
                    if (pieces[i]._position == tile[x, y])
                    {
                        if (tile[a, b].BackgroundImage != null)
                        {
                            pieceCapture(tile[a, b]);
                        }
                        pieces[i]._position = tile[a, b];
                        pieces[i]._position.Image = tile[x, y].Image;
                        tile[x, y].Image = null;
                        pieces[i].clearPossibleMove(tile[x, y], tile[a, b]);

                        if (Piece.enPassantTile != null && pieces[i]._type == "pawn" && enPx == a && (pieces[i]._color == "white" && enPy == b + 1 || pieces[i]._color == "black" && enPy == b - 1))
                        {
                            pieceCapture(tile[enPx, enPy]);
                            tile[enPx, enPy].Image = null;
                        }

                        if (pieces[i]._type == "pawn" && Math.Abs(y - b) == 2)
                        {
                            Piece.enPassantTile = tile[a, b];
                            enPx = a; enPy = b;
                        }
                        else Piece.enPassantTile = null;

                        if (pieces[i]._type == "pawn" && (b == 0 || b == 7))
                        {
                            PawnPromotion pawnpromotion = new(pieces[i]);
                            pawnpromotion.ShowDialog();
                        }

                        if (pieces[i]._type == "rook" && x == 0 && pieces[i].isFirstTurn == true)
                        {
                            pieces.FirstOrDefault(obj => obj._type == "king").isQueenSideCastlingPossible = false;
                        }

                        if (pieces[i]._type == "rook" && x == 7 && pieces[i].isFirstTurn == true)
                        {
                            pieces.FirstOrDefault(obj => obj._type == "king").isKingSideCastlingPossible = false;
                        }

                        pieces[i].isFirstTurn = false;

                        if (pieces[i]._type == "king" && x == 4 && a == 2)
                        {
                            pieces.FirstOrDefault(obj => obj._position == tile[0, y])._position = tile[3, y];
                            tile[3, y].Image = tile[0, y].Image;
                            tile[0, y].Image = null;
                            tile[0, y].Enabled = false;
                        }

                        if (pieces[i]._type == "king" && x == 4 && a == 6)
                        {
                            pieces.FirstOrDefault(obj => obj._position == tile[7, y])._position = tile[5, y];
                            tile[5, y].Image = tile[7, y].Image;
                            tile[7, y].Image = null;
                            tile[7, y].Enabled = false;
                        }

                        Piece.checkStatusPossibleMoves.Item1.Clear(); Piece.checkStatusPossibleMoves.Item2.Clear(); Piece.checkStatusPossibleMoves.Item3.Clear();
                        Piece.threateningTiles.Item1.Clear(); Piece.threateningTiles.Item2.Clear();

                        bool isCheck = isStatusCheck();

                        Piece.isStatusCheck = isCheck;

                        if (isCheck == true)
                            possibleMoves();

                        bool isCheckmate = false;
                        if (isCheck == true && Piece.checkStatusPossibleMoves.Item1.Count == 0 )
                            isCheckmate = isStatusCheckmate();

                        for (int j = 0; j < pieces.Count; j++)
                        {
                            pieces[j]._position.Enabled = false;
                        }

                        turnEnded?.Invoke(this, new TurnEndedEventArgs { isStatusCheck = isCheck, isStatusCheckmate = isCheckmate });

                        break;
                    }
                }
            }
        }

        int x, y;

        int enPx, enPy;

        public event EventHandler<TurnEndedEventArgs> turnEnded;

        public void colorHandler(object sender, string color)
        {
            if (color == "white")
                pieces = whitePieces;
            else if (color == "black")
                pieces = blackPieces;
        }
        private List<Piece> pieces;

        private void pieceCapture(Button tile)
        {
            if (pieces[0]._color == "black")
            {
                for (int j = 0; j < whitePieces.Count; j++)
                {
                    if (whitePieces[j]._position == tile)
                    {
                        tile.BackgroundImage = null;
                        whitePieces.RemoveAt(j);
                        break;
                    }
                }
            }
            else if (pieces[0]._color == "white")
            {
                for (int j = 0; j < blackPieces.Count; j++)
                {
                    if (blackPieces[j]._position == tile)
                    {
                        tile.BackgroundImage = null;
                        blackPieces.RemoveAt(j);
                        break;
                    }
                }
            }
        }

        bool isStatusCheck()
        {
            if (pieces[0]._color == "black")
            {
                return whitePieces.FirstOrDefault(obj => obj._type == "king").isTileAttacked_1(tile, whitePieces[0].kingPosition(tile).Item1, whitePieces[0].kingPosition(tile).Item2) ? true : false;
            }
            else /*if (pieces[0]._color == "white")*/
            {
                return blackPieces.FirstOrDefault(obj => obj._type == "king").isTileAttacked_1(tile, blackPieces[0].kingPosition(tile).Item1, blackPieces[0].kingPosition(tile).Item2) ? true : false;
            }
        }

        void possibleMoves()
        {
            if (pieces[0]._color == "black")
            {
                for (int i = 0; i < Piece.threateningTiles.Item1.Count; i++) whitePieces[0].isTileAttacked_2(tile, Piece.threateningTiles.Item1[i], Piece.threateningTiles.Item2[i]);
            }
            else /*if (pieces[0]._color == "white")*/
            {
                for (int i = 0; i < Piece.threateningTiles.Item1.Count; i++) blackPieces[0].isTileAttacked_2(tile, Piece.threateningTiles.Item1[i], Piece.threateningTiles.Item2[i]);
            }
        }

        bool isStatusCheckmate()
        {
            Piece king;
            int kingX, kingY;

            if (pieces[0]._color == "black")
            {
                king = whitePieces.FirstOrDefault(obj => obj._type == "king");
                kingX = whitePieces[0].kingPosition(tile).Item1; kingY = whitePieces[0].kingPosition(tile).Item2;
            }
            else /*if (pieces[0]._color == "white")*/
            {
                king = blackPieces.FirstOrDefault(obj => obj._type == "king");
                kingX = blackPieces[0].kingPosition(tile).Item1; kingY = blackPieces[0].kingPosition(tile).Item2;
            }

            int adjacentTiles = 0;

            if (kingX == 0 || (tile[kingX - 1, kingY].Image != null && tile[kingX - 1, kingY].Enabled == false) || king.isTileAttacked_1(tile, kingX - 1, kingY)) adjacentTiles++;
            if (kingX == 7 || (tile[kingX + 1, kingY].Image != null && tile[kingX + 1, kingY].Enabled == false) || king.isTileAttacked_1(tile, kingX + 1, kingY)) adjacentTiles++;
            if (kingY == 0 || (tile[kingX, kingY - 1].Image != null && tile[kingX, kingY - 1].Enabled == false) || king.isTileAttacked_1(tile, kingX, kingY - 1)) adjacentTiles++;
            if (kingY == 7 || (tile[kingX, kingY + 1].Image != null && tile[kingX, kingY + 1].Enabled == false) || king.isTileAttacked_1(tile, kingX, kingY + 1)) adjacentTiles++;
            if ((kingX == 0 || kingY == 0) || (tile[kingX - 1, kingY - 1].Image != null && tile[kingX - 1, kingY - 1].Enabled == false) || king.isTileAttacked_1(tile, kingX - 1, kingY - 1)) adjacentTiles++;
            if ((kingX == 7 || kingY == 0) || (tile[kingX + 1, kingY - 1].Image != null && tile[kingX + 1, kingY - 1].Enabled == false) || king.isTileAttacked_1(tile, kingX + 1, kingY - 1)) adjacentTiles++;
            if ((kingX == 7 || kingY == 7) || (tile[kingX + 1, kingY + 1].Image != null && tile[kingX + 1, kingY + 1].Enabled == false) || king.isTileAttacked_1(tile, kingX + 1, kingY + 1)) adjacentTiles++;
            if ((kingX == 0 || kingY == 7) || (tile[kingX - 1, kingY + 1].Image != null && tile[kingX - 1, kingY + 1].Enabled == false) || king.isTileAttacked_1(tile, kingX - 1, kingY + 1)) adjacentTiles++;

            if (adjacentTiles == 8) return true;
            else return false;
        }

        bool isStatusStalemate()
        {
            Piece.stalemateCheck = true;
            Piece.isMovePossible = false;

            if (pieces[0]._color == "black")
                foreach (var i in whitePieces)
                {
                    var coords = (from x in Enumerable.Range(0, 8) from y in Enumerable.Range(0, 8) where i._position == tile[x, y] select Tuple.Create(x, y)).FirstOrDefault();
                    i.movePiece(tile, coords.Item1, coords.Item2);
                    if (Piece.isMovePossible == true) return false;
                }
            else /*if (pieces[0]._color == "white")*/
                foreach (var i in blackPieces)
                {
                    var coords = (from x in Enumerable.Range(0, 8) from y in Enumerable.Range(0, 8) where i._position == tile[x, y] select Tuple.Create(x, y)).FirstOrDefault();
                    i.movePiece(tile, coords.Item1, coords.Item2);
                    if (Piece.isMovePossible == true) return false;
                }

            Piece.stalemateCheck = false;

            return true;
        }

        public void restartGame()
        {
            index = 0;
            x = 0; y = 0;
            enPx = 0; enPy = 0;
            pieces.Clear();

            Piece.isStatusCheck = false;
            Piece.enPassantTile = null;
        }
    }

    public class TurnEndedEventArgs : EventArgs
    {
        public bool isStatusCheck { get; set; }
        public bool isStatusCheckmate { get; set; }
        public bool isStatusStalemate { get; set; }
    }
}