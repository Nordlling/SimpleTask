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

        private readonly Dictionary<int, string> wordsMap = new();
        
        public GridFillWords LoadModel(int index)
        {
            string row = ParseFile(levelsPackPath, index);
            ParseWords(row);
            Dictionary<string, List<int>> levelMap = ParseRow(row);
            int size = PerformGridSize(levelMap);
            if (size == 0)
            {
                return FillStub();
                // return null;
            }
            GridFillWords gridFillWords = new GridFillWords(new Vector2Int(size, size));

            FillGrid(levelMap, gridFillWords, size);
            return gridFillWords;
        }

        private void FillGrid(Dictionary<string, List<int>> levelMap, GridFillWords gridFillWords, int size)
        {
            foreach (KeyValuePair<string, List<int>> kvp in levelMap)
            {
                foreach (int idx in kvp.Value)
                {
                    gridFillWords.Set(idx / size, idx % size, new CharGridModel(kvp.Key[kvp.Value.IndexOf(idx)]));
                }
            }
        }

        private int PerformGridSize(Dictionary<string, List<int>> levelMap)
        {
            double gridLength = levelMap.Sum(kvp => kvp.Value.Count);
            double size = Math.Sqrt(gridLength);
            if (size % 1 != 0)
            {
                Debug.LogError("There are not enough indexes to fill in");
                return 0;
            }
            return (int)size;
        }

        private string ParseFile(string filePath, int index)
        {
            return ParseFile(filePath, new [] {index})[0];
        }

        private string[] ParseFile(string filePath, int[] indexes)
        {
            try
            {
                
                string[] lines = Resources.Load<TextAsset>(filePath).text.Split('\n');
                string[] targetLines = new string[indexes.Length];
                for (int i = 0; i < indexes.Length; i++)
                {
                    if (indexes[i] >= 0 && indexes[i] < lines.Length)
                    {
                        targetLines[i] = lines[indexes[i]];
                        Debug.Log(targetLines[i]);
                    }
                    else
                    {
                        Debug.LogError("String index unbound.");
                        return null;
                    }
                }
                
                return targetLines;
            }
            catch (IOException e)
            {
                Debug.LogError($"Parse file error: {e.Message}");
            }

            return null;
        }

        private void ParseWords(string row)
        {
            string pattern = @"(?<=^|\s)(\d+)(?=\s|$)";
            MatchCollection matches = Regex.Matches(row, pattern);
            int[] wordIndexes = new int[matches.Count];
            for (int i = 0; i < matches.Count; i++)
            {
                wordIndexes[i] = int.Parse(matches[i].Value);
            }

            string[] words = ParseFile(wordsListPath, wordIndexes);
            for (int i = 0; i < wordIndexes.Count(); i++)
            {
                wordsMap[wordIndexes[i]] = words[i];
            } 
        }

        private Dictionary<string, List<int>> ParseRow(string row)
        {
            string pattern = @"(\d+\s[\d;]+)";
            Dictionary<string, List<int>> resultMap = new Dictionary<string, List<int>>();

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

                    resultMap[wordsMap[key]] = intValues;
                }
            }
            return resultMap;
        }

        private GridFillWords FillStub()
        {
            GridFillWords gridFillWords = new GridFillWords(new Vector2Int(1, 1));
            gridFillWords.Set(0,0, new CharGridModel('X'));
            return gridFillWords;
        }
    }
}