using System;
using System.Collections.Generic;
using System.Linq;
using App.Scripts.Libs.Factory;
using App.Scripts.Scenes.SceneWordSearch.Features.Level.Models.Level;

namespace App.Scripts.Scenes.SceneWordSearch.Features.Level.BuilderLevelModel
{
    public class FactoryLevelModel : IFactory<LevelModel, LevelInfo, int>
    {
        public LevelModel Create(LevelInfo value, int levelNumber)
        {
            var model = new LevelModel();

            model.LevelNumber = levelNumber;

            model.Words = value.words;
            model.InputChars = BuildListChars(value.words);

            return model;
        }

        private List<char> BuildListChars(List<string> words)
        {
            List<Dictionary<char, int>> wordLetterCountsList = words
                .Select(word => word.GroupBy(c => c)
                    .ToDictionary(group => group.Key, group => group.Count()))
                .ToList();

            Dictionary<char, int> letterMaxCounts = wordLetterCountsList
                .SelectMany(dict => dict)
                .GroupBy(pair => pair.Key)
                .ToDictionary(group => group.Key, group => group.Max(pair => pair.Value));
            
            List<char> listChars = letterMaxCounts
                .SelectMany(kvp => Enumerable.Repeat(kvp.Key, kvp.Value)).ToList();
            
            return listChars;
        }
    }
}