using System;
using System.Linq;
using App.Scripts.Scenes.SceneChess.Features.ChessField.GridMatrix;
using UnityEngine;

namespace App.Scripts.Scenes.SceneChess.Features.GridNavigation.Utils
{
    public static class GridUtils
    {
        public static Vector2Int[] CreateDotsFromLines(Vector2Int line)
        {
            int[] intermediateCellsX = line.x != 0 ? Enumerable.Range(1, Math.Abs(line.x)).Select(x => line.x < 0 ? -x : x).ToArray() : new int[7];
            int[] intermediateCellsY = line.y != 0 ? Enumerable.Range(1, Math.Abs(line.y)).Select(y => line.y < 0 ? -y : y).ToArray() : new int[7];
            Vector2Int[] intermediateCells = intermediateCellsX.Zip(intermediateCellsY, (x, y) => new Vector2Int(x, y)).ToArray();
            return intermediateCells;
        }
        
        public static bool IsNodeAvailable(ChessGrid grid, Vector2Int node)
        {
            return IsNodeInGrid(grid, node) && IsNodeFree(grid, node);
        }
        
        private static bool IsNodeInGrid(ChessGrid grid, Vector2Int node)
        {
            return node.x >= 0 && node.x < grid.Size.x && node.y >= 0 && node.y < grid.Size.y;
        }
        
        private static bool IsNodeFree(ChessGrid grid, Vector2Int node)
        {
            return grid.Get(node) == null;
        }
    }
}