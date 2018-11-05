using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace AWSSDK.Examples.ChessGame
{
    public class Board: IBoard
    {
        private int _id;
        
        public Board(int matchId)
        {
            var requestParams = new Dictionary<string, string>();
            requestParams.Add("matchId", matchId.ToString());
            _id = int.Parse(GetResponse(requestParams));
            
        }

        private static readonly string GatewayUri = "";

        public string GetResponse(Dictionary<String, String> requestParams)
        {
            string additionalUri = "?";
            foreach (var key in requestParams.Keys)
            {
                additionalUri += key + "=" + requestParams[key];
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
            throw new System.NotImplementedException();
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
            throw new System.NotImplementedException();
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