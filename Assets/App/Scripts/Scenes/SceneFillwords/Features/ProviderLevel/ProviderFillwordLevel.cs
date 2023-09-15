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

        private static readonly List<Dictionary<string, List<int>>> _levels = new();

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
            foreach (var levelRow in levelRows)
            {
                Dictionary<string, List<int>> levelDictionary = ConvertWordsToLevelDictionary(levelRow, words);
                _levels.Add(levelDictionary);
            }
        }
        
        public GridFillWords LoadModel(int index)
        {
            try
            {
                Dictionary<string, List<int>> currentLevel = _levels[index - 1];
                int size = CalculateGridSize(currentLevel);
                GridFillWords gridFillWords = new GridFillWords(new Vector2Int(size, size));
                FillGrid(currentLevel, gridFillWords, size);
                return gridFillWords;
            }
            catch (Exception ex)
            {
                Debug.LogError($"LoadModel error: {ex.Message}");
                return null;
            }
        }

        private void FillGrid(Dictionary<string, List<int>> currentLevel, GridFillWords gridFillWords, int size)
        {
            foreach (KeyValuePair<string, List<int>> kvp in currentLevel)
            {
                foreach (int idx in kvp.Value)
                {
                    gridFillWords.Set(idx / size, idx % size, new CharGridModel(kvp.Key[kvp.Value.IndexOf(idx)]));
                }
            }
        }

        private int CalculateGridSize(Dictionary<string, List<int>> currentLevel)
        {
            double gridLength = currentLevel.Sum(kvp => kvp.Value.Count);
            double size = Math.Sqrt(gridLength);
            if (size == 0 || size % 1 != 0)
            {
                throw new Exception("Incorrect grid size error");
            }
            return (int)size;
        }

        private Dictionary<string, List<int>> ConvertWordsToLevelDictionary(string row, string[] _words)
        {
            Dictionary<string, List<int>> levelDictionary = new();
            string pattern = @"(\d+\s[\d;]+)";

            try
            {
                MatchCollection matches = Regex.Matches(row, pattern);
                string[] rowArray = new string[matches.Count];

                for (int i = 0; i < matches.Count; i++)
                {
                    rowArray[i] = matches[i].Value;
                }

                foreach (string input in rowArray)
                {
                    string[] parts = input.Split(" ");

                    if (parts.Length == 2)
                    {
                        int key = int.Parse(parts[0]);
                        string[] values = parts[1].Split(';');
                        List<int> intValues = new List<int>();

                        foreach (string value in values)
                        {
                            if (int.TryParse(value, out int intValue))
                            {
                                intValues.Add(intValue);
                            }
                        }

                        levelDictionary[_words[key]] = intValues;
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.LogError($"Convert string to object error: {ex.Message}");
            }

            return levelDictionary;
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