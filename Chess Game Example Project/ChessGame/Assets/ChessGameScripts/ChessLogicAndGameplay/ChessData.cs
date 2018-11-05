using System;
using System.Collections.Generic;

namespace AWSSDK.Examples.ChessGame
{
    //common provider of all general chess types and constants
    public abstract class ChessData
    {
        public const string InitialGameFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
        public const string InitialLongAlgNotation = "";
        public enum ChessPieceType
        {
            None = 0,
            Rook = 1,
            Knight = 2,
            Bishop = 3,
            Queen = 4,
            King = 5,
            Pawn = 6
        }
        public enum ChessPieceColor
        {
            None = 0,
            White = 1, 
            Black = 2
        }
        // The directions that a rook can move.
        public static readonly List<int[]> RookDirections = new List<int[]> { new int[2] { 0, 1 }, new int[2] { 0, -1 }, new int[2] { -1, 0 }, new int[2] { 1, 0 } };
        // The directions that a bishop can move.
        public static readonly List<int[]> BishopDirections = new List<int[]> { new int[2] { 1, 1 }, new int[2] { 1, -1 }, new int[2] { -1, 1 }, new int[2] { -1, -1 } };
        // The directions that a queen can move, as well as the translational movement of a king.
        public static readonly List<int[]> QueenDirectionsKingTranslations = new List<int[]> { new int[2] { 0, 1 }, new int[2] { 0, -1 }, new int[2] { -1, 0 }, new int[2] { 1, 0 }, new int[2] { 1, 1 }, new int[2] { 1, -1 }, new int[2] { -1, 1 }, new int[2] { -1, -1 } };
        // The distances in x,y that a rook can move.
        public static readonly List<int[]> KnightTranslations = new List<int[]> { new int[2] { 2, 1 }, new int[2] { 1, 2 }, new int[2] { -2, 1 }, new int[2] { 1, -2 }, new int[2] { 2, -1 }, new int[2] { -1, 2 }, new int[2] { -2, -1 }, new int[2] { -1, -2 } };
        // Note that the keys are all uppercase, so lowercase letters should be cast to upper to retrieve the piece type
        private static readonly Dictionary<char, ChessPieceType> CharToChessPieceType = new Dictionary<char, ChessPieceType>{
            { 'P', ChessPieceType.Pawn   },
            { 'N', ChessPieceType.Knight },
            { 'B', ChessPieceType.Bishop },
            { 'Q', ChessPieceType.Queen  },
            { 'R', ChessPieceType.Rook   },
            { 'K', ChessPieceType.King   }
        };
        
        public struct ChessPiece
        {
            private ChessPieceColor color;
            private ChessPieceType type;
            public ChessPieceColor Color
            {
                get { return color; }
            }
            public ChessPieceType Type
            {
                get { return type; }
            }
            public ChessPiece(ChessPieceColor color, ChessPieceType type)
            {
                this.color = color;
                this.type = type;
            }
        }

        public struct Coordinate
        {
            private int row;
            private int column;
            public int Row { get { return row; } }
            public int Column { get { return column; } }
            public Coordinate(int row, int column)
            {
                this.row = row;
                this.column = column;
            }
            // Make a new coordinate based on a translation of another coordinate
            public Coordinate(Coordinate originalCoordinate, int rowTranslation, int columnTranslation)
            {
                this.row = originalCoordinate.Row + rowTranslation;
                this.column = originalCoordinate.Column + columnTranslation;
            }
            // Indexed by the column to retrieve the file letter.
            private const string ColumnToFile = "abcdefgh";
            public override string ToString()
            {
                return IsInBoardBounds() ? ColumnToFile[column] + (row + 1).ToString() : "-";
            }

            // Check if the coordinate is within the bounds of the board (i.e. not beyond the 8th row/column now before the 1st).
            public bool IsInBoardBounds()
            {
                return column >= 0 && column <= 7 && row >= 0 && row <= 7;
            }

            public override bool Equals(object obj)
            {
                return (obj is Coordinate) && ((Coordinate)obj).Row == row && ((Coordinate)obj).Column == column;
            }
        }

        public struct ChessMove
        {
            private Coordinate from;
            private Coordinate to;
            private ChessPieceType pieceType;
            private bool isCapture;
            private bool isPromotionToQueen;
            private bool drawOfferExtended;
            private bool isCheck;
            private bool isCheckMate;
            private bool kingsideCastle;
            private bool queensideCastle;
            public Coordinate From { get { return from; } }
            public Coordinate To { get { return to; } }
            public ChessPieceType PieceType { get { return pieceType; } }
            public bool IsCapture { get { return isCapture; } }
            public bool IsPromotionToQueen { get { return isPromotionToQueen; } }
            public bool DrawOfferExtended { get { return drawOfferExtended; } }
            public bool IsCheck { get { return isCheck; } }
            public bool IsCheckMate { get { return isCheckMate; } }
            public bool KingsideCastle { get { return kingsideCastle; } }
            public bool QueensideCastle { get { return queensideCastle; } }
            
            private const string QueensideCastlingString = "0-0-0";
            private const string KingsideCastlingString = "0-0";

            public ChessMove(Coordinate from, Coordinate to, ChessPieceType pieceType, bool isCapture, bool isPromotionToQueen, bool drawOfferExtended, bool isCheck, bool isCheckMate, bool kingsideCastle, bool queensideCastle)
            {
                this.from = from;
                this.to = to;
                this.pieceType = pieceType;
                this.isCapture = isCapture;
                this.isPromotionToQueen = isPromotionToQueen;
                this.drawOfferExtended = drawOfferExtended;
                this.isCheck = isCheck;
                this.isCheckMate = isCheckMate;
                this.kingsideCastle = kingsideCastle;
                this.queensideCastle = queensideCastle;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is ChessMove))
                {
                    return false;
                }
                var m = (ChessMove)obj;
                return m.From.Equals(from) &&
                    m.To.Equals(to) &&
                    m.PieceType == pieceType &&
                    m.IsCapture == isCapture &&
                    m.IsPromotionToQueen == isPromotionToQueen &&
                    m.DrawOfferExtended == drawOfferExtended &&
                    m.IsCheck == isCheck &&
                    m.IsCheckMate == isCheckMate &&
                    m.KingsideCastle == kingsideCastle &&
                    m.QueensideCastle == queensideCastle;
            }
            private const string ChessPieceToCapitalChar = " RNBQKP";
            public string ToLongAlgebraicNotation()
            {

                string notation = "";
                if (kingsideCastle)
                {
                    notation += KingsideCastlingString;
                }
                else if (queensideCastle)
                {
                    notation += QueensideCastlingString;
                }
                else
                {
                    if (pieceType == ChessPieceType.None)
                    {
                        // If there is no piece type, there has not yet been a move.
                        return "";
                    }
                    // Pawn type is implied if no letter is shown
                    else if (pieceType != ChessPieceType.Pawn)
                    {
                        
                        notation += ChessPieceToCapitalChar[(int)pieceType];
                    }
                    notation += from.ToString();
                    if (isCapture)
                    {
                        notation += 'x';
                    }
                    else
                    {
                        notation += '-';
                    }
                    notation += to.ToString();
                }
                if (isPromotionToQueen)
                {
                    notation += 'Q';
                }
                if (drawOfferExtended)
                {
                    notation += '=';
                }
                if (isCheck)
                {
                    notation += '+';
                }
                if (isCheckMate)
                {
                    notation += '#';
                }
                return notation;
            }
        }
    }
}