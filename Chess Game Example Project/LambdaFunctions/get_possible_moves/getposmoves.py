import clr
import json
import boto3
from boto3.dynamodb.conditions import Key, Attr

clr.AddReference("TestClassLibrary")

from TestClassLibrary import TryApplyMove

import json
def getfenalg(matchid):
    ddb = boto3.resource('dynamodb')
    table = ddb.Table('ChessMatches')
    response = table.query(
        KeyConditionExpression=Key('MatchId').eq(matchid)
    )
    items = response['Items']
    if not items:
        return None, None
    else:
        return items[0]['FEN'], items[0]['AlgorithmicNotation']

def lambda_handler(event, context):
    row, column = event['row'], event['column']
    fen, alg = getfenalg(event['match_id'])
    if not fen and not alg:
        return {
            'statusCode': 404,
            'body': json.dumps('MatchID not found')
        }
    a = TryApplyMove(fen, alg)
    res = a.get_possible_moves(row,column)
    return {
        'statusCode': 200,
        'body': res
    }
