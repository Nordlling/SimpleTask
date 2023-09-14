using System.Collections.Generic;
using App.Scripts.Scenes.SceneChess.Features.ChessField.Types;
using App.Scripts.Scenes.SceneChess.Features.GridNavigation.Types;
using UnityEngine;

namespace App.Scripts.Scenes.SceneChess.Features.GridNavigation.Navigator
{
    public class UnitMovesInfo
    {
        public Dictionary<ChessUnitType, Vector2Int[]> UnitMoves { get; private set; }
        public Dictionary<ChessUnitType, MoveType> UnitMoveTypes { get; private set; }
        
        private readonly Vector2Int[] knightPoints =
        {
            new(1, 2),
            new(2, 1),
            new(2, -1),
            new(1, -2),
            new(-1, -2),
            new(-2, -1),
            new(-2, 1),
            new(-1, 2)
        };
        
        private readonly Vector2Int[] kingPoints =
        {
            new(1, 1),
            new(1, 0),
            new(1, -1),
            new(0, -1),
            new(-1, -1),
            new(-1, 0),
            new(-1, 1),
            new(0, 1)
        };
        
        private readonly Vector2Int[] ponPoints =
        {
            new(0, 1),
            new(0, -1)
        };
        
        private readonly Vector2Int[] rookLines =
        {
            new(7, 0),
            new(0, -7),
            new(-7, 0),
            new(0, 7)
        };
        
        private readonly Vector2Int[] bishopLines =
        {
            new(7, 7),
            new(7, -7),
            new(-7, -7),
            new(-7, 7)
        };
        
        private readonly Vector2Int[] queenLines =
        {
            new(7, 7),
            new(7, 0),
            new(7, -7),
            new(0, -7),
            new(-7, -7),
            new(-7, 0),
            new(-7, 7),
            new(0, 7)
        };

        public UnitMovesInfo()
        {
            InitUnitPoints(); 
        }

        private void InitUnitPoints()
        {
            UnitMoves = new Dictionary<ChessUnitType, Vector2Int[]>
            {
                { ChessUnitType.Pon, ponPoints },
                { ChessUnitType.King, kingPoints },
                { ChessUnitType.Queen, queenLines },
                { ChessUnitType.Rook, rookLines },
                { ChessUnitType.Knight, knightPoints },
                { ChessUnitType.Bishop, bishopLines }
            };
            
            UnitMoveTypes = new Dictionary<ChessUnitType, MoveType>
            {
                { ChessUnitType.Pon, MoveType.Dots },
                { ChessUnitType.King, MoveType.Dots },
                { ChessUnitType.Queen, MoveType.Lines },
                { ChessUnitType.Rook, MoveType.Lines },
                { ChessUnitType.Knight, MoveType.Dots },
                { ChessUnitType.Bishop, MoveType.Lines }
            };
        }
    }
}