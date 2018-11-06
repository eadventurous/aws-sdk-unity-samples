using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AWSSDK.Examples.ChessGame;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace get_turn_color
{
    public class Function2
    {
        
        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {
            var t = Table.LoadTable(new AmazonDynamoDBClient(), "ChessMatches");
            QueryFilter filter = new QueryFilter("Id", QueryOperator.Equal, input.QueryStringParameters["match_id"]);
            var queryResult = t.Query(filter);
            var actualResultSet = await queryResult.GetNextSetAsync();
            string fen="",an="";
            foreach (var ar in actualResultSet)
            {
                fen = ar["FEN"];
                an = ar["AlgebraicNotation"];
                //exceptions will start even here, while querying an empty map
            }

            var row = Int32.Parse(input.QueryStringParameters["row"]);
            var column = Int32.Parse(input.QueryStringParameters["column"]);
            var b = new BoardState(fen, an);                           // how to move a ChessMove, or, more precisely, how to restore it?
            var getmoveresult = b.GetPossibleMoves(new BoardState.Coordinate(row,column));
            var res = new APIGatewayProxyResponse
            {
                Body = getmoveresult.ToString(),
                StatusCode = 200
            };
            return res;
        }
    }
}