﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

namespace AWSSDK.Examples.ChessGame
{
    public class Board: IBoard
    {
        private int _id;
        
        public Board(int matchId)
        {
            var requestParams = new Dictionary<string, string>();
            requestParams.Add("matchId", matchId.ToString());
            _id = int.Parse(GetResponse("get_board_id", requestParams));
            
        }

        private static readonly string GatewayUri = "";

        public string GetResponse(string func, Dictionary<String, String> requestParams)
        {
            string additionalUri = func;
            if (requestParams != null)
            {
                additionalUri += "/?";
                foreach (var key in requestParams.Keys)
                {
                    additionalUri += key + "=" + requestParams[key];
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

        public ChessData.ChessMove GetPreviousMove()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ChessData.ChessMove> GetPossibleMoves(ChessData.Coordinate coordinate)
        {
            throw new System.NotImplementedException();
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
            request.Add("kingsideCastle", newMove.KingsideCastle.ToString());
            request.Add("queenside_castle", newMove.QueensideCastle.ToString());
            return bool.Parse(GetResponse("try_apply_move", request));
        }

        public string GetForsythEdwardsNotation()
        {
            throw new System.NotImplementedException();
        }

        public ChessData.ChessPiece GetPieceAtCoordinate(ChessData.Coordinate coordinate)
        {
            throw new System.NotImplementedException();
        }
    }
}