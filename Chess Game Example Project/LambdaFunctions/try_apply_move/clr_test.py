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


def update_db(matchid, fen):
    boto3.resource('dynamodb').Table('ChessMatches').update_item(
        Key={
            'MatchId': matchid,
        },
        UpdateExpression='SET FEN = :fen',
        ExpressionAttributeValues={
            ':fen': fen
        }
    )


def lambda_handler(event, context):
    fen, alg = getfenalg(event['match_id'])
    if not fen and not alg:
        return {
            'statusCode': 404,
            'body': json.dumps('MatchID not found')
        }

    fr, tr, fc, tc = int(event['from_row']), int(event['to_row']), int(event['from_column']), int(event['to_column'])
    cpt, isc, ispq = int(event['chess_piece_type']), bool(event['is_capture']), bool(event['is_promotion_to_queen'])
    isd, isc, ism = bool(event['draw_offer_extended']), bool(event['is_check']), bool(event['is_check_mate'])
    kk, qk = bool(event['kingside_castle']), bool(event['queenside_castle'])
    a = TryApplyMove(fen, alg)
    res = not a.execute(fr, fc, tr, tc, cpt, isc, ispq, isd, isc, ism, kk, qk)
    update_db(event['match_id'], a.getfen())
    return {
        'statusCode': 200,
        'body': json.dumps(res)
    }
