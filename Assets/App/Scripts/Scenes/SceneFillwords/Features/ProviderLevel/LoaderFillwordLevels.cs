using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace App.Scripts.Scenes.SceneFillwords.Features.ProviderLevel
{
    public class LoaderFillwordLevels
    {
        private const string levelsPackPath = "Fillwords/pack_0";
        private const string wordsListPath = "Fillwords/words_list";

        private static List<Dictionary<string, int[]>> _levels = new();

        public static List<Dictionary<string, int[]>> GetLevels()
        {
            if (_levels.Count == 0)
            {
                LoadLevels();
            }

            return _levels;
        }

        private static void LoadLevels()
        {
            string[] levelRows = ParseFile(levelsPackPath);
            string[] words = ParseFile(wordsListPath);
            _levels = ConvertStringDataToLevelDictionary(levelRows, words);
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

        private static List<Dictionary<string, int[]>> ConvertStringDataToLevelDictionary(string[] levelRows, string[] words)
        {
            List<Dictionary<string, int[]>> levels = new();
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
                    
                    levels.Add(level);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Convert string '{levelRow}' to object error: {ex.Message}");
                }
            }

            return levels;
        }
        
    }
}