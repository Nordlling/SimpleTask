using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using App.Scripts.Scenes.SceneFillwords.Features.FillwordModels;
using UnityEngine;

namespace App.Scripts.Scenes.SceneFillwords.Features.ProviderLevel
{
    public class ProviderFillwordLevel : IProviderFillwordLevel
    {
        private const string levelsPackPath = "Fillwords/pack_0";
        private const string wordsListPath = "Fillwords/words_list";

        private static readonly List<Dictionary<string, int[]>> _levels = new();

        public ProviderFillwordLevel()
        {
            if (_levels.Count == 0)
            {
                Init();
            }
        }

        private void Init()
        {
            string[] words = ParseFile(wordsListPath);
            string[] levelRows = ParseFile(levelsPackPath);
            ConvertStringDataToLevelDictionary(levelRows, words);
        }
        
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

        private void ConvertStringDataToLevelDictionary(string[] levelRows, string[] words)
        {
            const string levelWordPattern = @"(\d+\s[\d;]+)";

            foreach (var levelRow in levelRows)
            {
                Dictionary<string, int[]> level = new();
                try
                {
                    MatchCollection matches = Regex.Matches(levelRow, levelWordPattern);
                    string[] levelWords = matches.Select(match => match.Value).ToArray();
                    
                    foreach (string levelWord in levelWords)
                    {
                        string[] parts = levelWord.Split(" ");

                        if (parts.Length != 2)
                        {
                            continue;
                        }
                        
                        int wordIndex = int.Parse(parts[0]);
                        int[] gridPositions = parts[1].Split(';').Select(int.Parse).ToArray();

                        string word = words[wordIndex].Trim();
                        level[word] = gridPositions;
                    }
                    
                    _levels.Add(level);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Convert string '{levelRow}' to object error: {ex.Message}");
                }
            }
        }
        
        private static string[] ParseFile(string filePath)
        {
            try
            {
                string[] lines = Resources.Load<TextAsset>(filePath).text.Split('\n');
                return lines;
            }
            catch (IOException ex)
            {
                Debug.LogError($"Parse file error: {ex.Message}");
            }
            
            return null;
        }
    }
}