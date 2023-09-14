using System.Collections.Generic;
using System.Linq;
using App.Scripts.Scenes.SceneChess.Features.ChessField.GridMatrix;
using App.Scripts.Scenes.SceneChess.Features.ChessField.Types;
using App.Scripts.Scenes.SceneChess.Features.GridNavigation.Types;
using App.Scripts.Scenes.SceneChess.Features.GridNavigation.Utils;
using UnityEngine;

namespace App.Scripts.Scenes.SceneChess.Features.GridNavigation.Navigator
{
    public class ChessGridNavigator : IChessGridNavigator
    {
        private readonly Dictionary<ChessUnitType, Vector2Int[]> _unitMoves;
        private readonly Dictionary<ChessUnitType, MoveType> _unitMoveTypes;

        private Vector2Int[] _availableMoves;

        public ChessGridNavigator()
        {
            UnitMovesInfo unitMovesInfo = new UnitMovesInfo();
            _unitMoves = unitMovesInfo.UnitMoves;
            _unitMoveTypes = unitMovesInfo.UnitMoveTypes;
        }

        public List<Vector2Int> FindPath(ChessUnitType unit, Vector2Int from, Vector2Int to, ChessGrid grid)
        {
            _availableMoves = _unitMoves[unit];

            MoveType currentMoveType = _unitMoveTypes[unit];
            Queue<Vector2Int> queue = new();
            List<Vector2Int> visited = new();
            Dictionary<Vector2Int, Vector2Int> parent = new();

            queue.Enqueue(from);

            while (queue.Count != 0)
            {
                Vector2Int currentNode = queue.Dequeue();

                if (currentNode == to)
                {
                    List<Vector2Int> result = new();
                    while (currentNode != from)
                    {
                        result.Add(currentNode);
                        currentNode = parent[currentNode];
                    }

                    result.Reverse();

                    return result.Any() ? result : null;
                }

                switch (currentMoveType)
                {
                    case MoveType.Dots:
                        CheckDots(grid, currentNode, visited, queue, parent);
                        break;
                    case MoveType.Lines:
                        CheckLines(grid, currentNode, visited, queue, parent);
                        break;
                    default:
                        return null;
                }
            }

            return null;
        }

        private void CheckDots(ChessGrid grid, Vector2Int currentNode, List<Vector2Int> visited,
            Queue<Vector2Int> queue, Dictionary<Vector2Int, Vector2Int> parent)
        {
            foreach (Vector2Int dot in _availableMoves)
            {
                TryRegisterNode(out bool availableNodeVisited, grid, currentNode, visited, queue, parent, dot);
            }
        }

        private void CheckLines(ChessGrid grid, Vector2Int currentNode, List<Vector2Int> visited,
            Queue<Vector2Int> queue, Dictionary<Vector2Int, Vector2Int> parent)
        {
            foreach (Vector2Int line in _availableMoves)
            {
                Vector2Int[] intermediateDots = GridUtils.CreateDotsFromLines(line);
                foreach (var dot in intermediateDots)
                {
                    if (!TryRegisterNode(out bool availableNodeVisited, grid, currentNode, visited, queue, parent, dot) && !availableNodeVisited)
                    {
                        break;
                    }
                }
            }
        }

        private bool TryRegisterNode(out bool availableNodeVisited, ChessGrid grid, Vector2Int currentNode, List<Vector2Int> visited,
            Queue<Vector2Int> queue, Dictionary<Vector2Int, Vector2Int> parent, Vector2Int move)
        {
            Vector2Int newNode = new Vector2Int(currentNode.x + move.x, currentNode.y + move.y);
            availableNodeVisited = false;

            if (GridUtils.IsNodeAvailable(grid, newNode))
            {
                if (visited.Contains(newNode))
                {
                    availableNodeVisited = true;
                    return false;
                }
                queue.Enqueue(newNode);
                visited.Add(newNode);
                parent[newNode] = currentNode;
                return true;
            }

            return false;
        }
    }
}