using System;
using System.Collections.Generic;
using System.Linq;
using App.Scripts.Scenes.SceneFillwords.Features.FillwordModels;
using UnityEngine;

namespace App.Scripts.Scenes.SceneFillwords.Features.ProviderLevel
{
    public class ProviderFillwordLevel : IProviderFillwordLevel
    {
        private readonly List<Dictionary<string, int[]>> _levels = LoaderFillwordLevels.GetLevels();

        public GridFillWords LoadModel(int index)
        {
            try
            {
                Dictionary<string, int[]> currentLevel = _levels[index - 1];
                int size = CalculateGridSize(currentLevel);
                GridFillWords gridFillWords = FillGrid(currentLevel, size);
                
                if (!IsGridValid(gridFillWords))
                {
                    Debug.LogError($"LoadModel error: Incorrect level data");
                    return null;
                }
                
                return gridFillWords;
            }
            catch (Exception ex)
            {
                Debug.LogError($"LoadModel error: {ex.Message}");
                return null;
            }
        }

        private GridFillWords FillGrid(Dictionary<string, int[]> currentLevel, int size)
        {
            GridFillWords gridFillWords = new GridFillWords(new Vector2Int(size, size));
            foreach (KeyValuePair<string, int[]> kvp in currentLevel)
            {
                foreach (int idx in kvp.Value)
                {
                    if (kvp.Value.Length != kvp.Key.Length)
                    {
                        throw new Exception("FillGrid error: Mismatched letters and cells");
                    }
                    gridFillWords.Set(idx / size, idx % size, new CharGridModel(kvp.Key[Array.IndexOf(kvp.Value, idx)]));
                }
            }
            return gridFillWords;
        }
        
        private bool IsGridValid(GridFillWords gridFillWords)
        {
            for (int i = 0; i < gridFillWords.Size.x; i++)
            {
                for (int j = 0; j < gridFillWords.Size.y; j++)
                {
                    if (gridFillWords.Get(i, j) == null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private int CalculateGridSize(Dictionary<string, int[]> currentLevel)
        {
            double gridLength = currentLevel.Sum(kvp => kvp.Value.Length);
            double size = Math.Sqrt(gridLength);
            if (size == 0 || size % 1 != 0)
            {
                throw new Exception("Incorrect grid size error");
            }
            return (int)size;
        }
    }
}