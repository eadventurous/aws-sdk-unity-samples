using System.Collections.Generic;

namespace AWSSDK.Examples.ChessGame
{
    public interface IBoard
    {
        ChessData.ChessPieceColor GetTurnColor();

        ChessData.ChessMove GetPreviousMove();

        IEnumerable<ChessData.ChessMove> GetPossibleMoves(ChessData.Coordinate coordinate);

        bool TryApplyMove(ChessData.ChessMove newMove);

        string GetForsythEdwardsNotation();

        ChessData.ChessPiece GetPieceAtCoordinate(ChessData.Coordinate coordinate);
    }
}