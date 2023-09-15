using System;
using System.IO;
using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;
using UnityEngine;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel.ProviderWordLevel
{
    public class ProviderWordLevel : IProviderWordLevel
    {
        private const string levelsPath = "WordSearch/Levels";
        
        public LevelInfo LoadLevelData(int levelIndex)
        {
            try
            {
                string json = Resources.Load<TextAsset>($"{levelsPath}/{levelIndex}").text;
                LevelInfo levelInfo = JsonUtility.FromJson<LevelInfo>(json);
                return levelInfo;
            } 
            catch (Exception ex)
            {
                Debug.LogError($"Parse error: {ex.Message}");
                return null;
            }
        }
    }
}