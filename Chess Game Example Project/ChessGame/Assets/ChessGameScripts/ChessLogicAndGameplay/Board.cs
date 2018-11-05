using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

namespace AWSSDK.Examples.ChessGame
{
    public class Board: IBoard
    {
        private int _id;
        public ChessData.ChessMove PreviousMove { get; private set; }
        
        public Board(string matchId)
        {
            var requestParams = new Dictionary<string, string>();
            requestParams.Add("matchId", matchId);
            _id = int.Parse(GetResponse("get_board_id", requestParams));
            
        }

        private static readonly string GatewayUri = "";

        public string GetResponse(string func, Dictionary<String, String> requestParams)
        {
            string additionalUri = func + "/?" + "board_id=" + _id.ToString();
            if (requestParams != null)
            {
                foreach (var key in requestParams.Keys)
                {
                    additionalUri += "&" + key + "=" + requestParams[key];
                }
            }
            using (UnityWebRequest www = UnityWebRequest.Get(GatewayUri+additionalUri))
            {
                www.Send();
                
                while(!www.isDone){}

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.Log(www.error);
                    throw new Exception(www.error);
                }

                // Show results as text
                Debug.Log(www.downloadHandler.text);
                return www.downloadHandler.text;
            }
        }
        
        public ChessData.ChessPieceColor GetTurnColor()
        {
            var color = int.Parse(GetResponse("get_turn_color", null));
            return (ChessData.ChessPieceColor) color;
        }

        private ChessData.ChessMove ParseMove(string json)
        {
            var parsed = JSON.Parse(json);
            var pieceType = (ChessData.ChessPieceType)int.Parse(parsed["chess_piece_type"]);
            var isCapture = bool.Parse(parsed["is_capture"]);
            var isPromotioToQueen = bool.Parse(parsed["is_promotion_to_queen"]);
            var drawOfferExtended = bool.Parse(parsed["draw_offer_extended"]);
            var isCheck = bool.Parse(parsed["is_check"]);
            var isCheckMate = bool.Parse(parsed["is_check_mate"]);
            var kingsideCastle = bool.Parse(parsed["kingsideCastle"]);
            var queensideCastle = bool.Parse(parsed["queenside_castle"]);
            var fromRow = int.Parse(parsed["from"]["row"]);
            var fromColumn = int.Parse(parsed["from"]["column"]);
            var fromCoord = new ChessData.Coordinate(fromRow, fromColumn);
            var toRow = int.Parse(parsed["to"]["row"]);
            var toColumn = int.Parse(parsed["to"]["column"]);
            var toCoord = new ChessData.Coordinate(toRow, toColumn);
            var move = new ChessData.ChessMove(fromCoord, toCoord, pieceType, isCapture,
                isPromotioToQueen, drawOfferExtended, isCheck, isCheckMate,
                kingsideCastle, queensideCastle);
            return move;
        }

        /*public ChessData.ChessMove GetPreviousMove()
        {
            return ParseMove(GetResponse("get_previous_move", null));
        }*/

        public IEnumerable<ChessData.ChessMove> GetPossibleMoves(ChessData.Coordinate coordinate)
        {
            var request = new Dictionary<string, string>();
            request.Add("row", coordinate.Row.ToString());
            request.Add("column", coordinate.Column.ToString());
            var parsed = JSON.Parse(GetResponse("get_piece_at_coordinate", request));
            var list = new LinkedList<ChessData.ChessMove>();
            foreach (var move in parsed["moves"].AsArray)
            {
                list.AddLast(ParseMove(move.Value));
            }
            return list;
        }

        public bool TryApplyMove(ChessData.ChessMove newMove)
        {
            var request = new Dictionary<string, string>();
            request.Add("from_row", newMove.From.Row.ToString());
            request.Add("to_row", newMove.To.Row.ToString());
            request.Add("from_column", newMove.From.Column.ToString());
            request.Add("to_column", newMove.To.Column.ToString());
            request.Add("chess_piece_type", ((int)newMove.PieceType).ToString());
            request.Add("is_capture", newMove.IsCapture.ToString());
            request.Add("is_promotion_to_queen", newMove.IsPromotionToQueen.ToString());
            request.Add("draw_offer_extended", newMove.DrawOfferExtended.ToString());
            request.Add("is_check", newMove.IsCheck.ToString());
            request.Add("is_check_mate", newMove.IsCheckMate.ToString());
            request.Add("kingside_castle", newMove.KingsideCastle.ToString());
            request.Add("queenside_castle", newMove.QueensideCastle.ToString());
            var status = bool.Parse(GetResponse("try_apply_move", request));
            if (status)
                PreviousMove = newMove;
            return status;
        }

        public string GetForsythEdwardsNotation()
        {
            return GetResponse("get_forsyth_edward_notation", null);
        }

        public ChessData.ChessPiece GetPieceAtCoordinate(ChessData.Coordinate coordinate)
        {
            var request = new Dictionary<string, string>();
            request.Add("row", coordinate.Row.ToString());
            request.Add("column", coordinate.Column.ToString());
            var parsed = JSON.Parse(GetResponse("get_piece_at_coordinate", request));
            var color = (ChessData.ChessPieceColor)int.Parse(parsed["color"]);
            var type = (ChessData.ChessPieceType)int.Parse(parsed["type"]);
            return new ChessData.ChessPiece(color, type);
        }
    }
}