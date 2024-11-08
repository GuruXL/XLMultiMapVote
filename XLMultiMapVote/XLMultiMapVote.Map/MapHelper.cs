﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace XLMultiMapVote.Map
{
    public static class MapHelper
    {
        private static List<LevelInfo> combinedMapList = new List<LevelInfo>();
        public static string[] mapNames { get; private set; }
        public static LevelInfo currentLevelInfo { get; private set; }
        public static LevelInfo nextLevelInfo { get; private set; }

        private static bool _isVoteInProgress = false;
        private static bool _hasMapChangedByVote = false;
        public static bool isVoteInProgress
        {
            get { return _isVoteInProgress; }
            private set { _isVoteInProgress = value; }
        }
        public static bool hasMapChangedByVote
        {
            get { return _hasMapChangedByVote; }
            private set { _hasMapChangedByVote = value; }
        }
        public static void Set_isVoteInProgress(bool status)
        {
            _isVoteInProgress = status;
        }
        public static void Set_hasMapChangedByVote(bool status)
        {
            _hasMapChangedByVote = status;
        }
        public static bool HasMapChanged()
        {
            return nextLevelInfo != null && currentLevelInfo == nextLevelInfo;
        }
        public static void SetCurrentLevel(LevelInfo level, bool setNextLevel)
        {
            if (currentLevelInfo != level)
            {
                currentLevelInfo = level;
            }
            if (setNextLevel)
            {
                SetNextLevel(level);
            }
        }
        public static void SetNextLevel(LevelInfo level)
        {
            if (nextLevelInfo != level)
            {
                nextLevelInfo = level;
            }
        }

        public static LevelInfo GetMapInfo(string votedMap)
        {
            if (combinedMapList?.Count > 0)
            {
                foreach (LevelInfo level in combinedMapList)
                {
                    if (level.name == votedMap)
                    {
                        return level;
                    }
                }
            }
            return null;
        }

        private static List<LevelInfo> GetMaps()
        {
            combinedMapList.Clear();

            if (LevelManager.Instance.Levels != null)
            {
                combinedMapList.AddRange(LevelManager.Instance.Levels.ToList());
            }
            if (LevelManager.Instance.CommunityLevels != null)
            {
                combinedMapList.AddRange(LevelManager.Instance.CommunityLevels.ToList());
            }
            if (LevelManager.Instance.ModLevels != null)
            {
                combinedMapList.AddRange(LevelManager.Instance.ModLevels.ToList());
            }

            return combinedMapList;
        }

        // Get names of maps as a string array
        private static string[] GetMapNames()
        {
            mapNames = ConvertToString(GetMaps());
            return mapNames;
        }

        // Convert LevelInfo list to string names
        private static string[] ConvertToString(List<LevelInfo> info)
        {
            string[] names = new string[info.Count];

            for (int i = 0; i < info.Count; i++)
            {
                names[i] = info[i].name;
            }
            return names;
        }
        private static string[] FilterArray(string[] mapNames, string searchString)
        {
            // Split the search string into lowercase words
            string[] searchWords = searchString.ToLower().Split(' ');

            // Filter the array based on the search string
            var filteredResults = mapNames.Where(s =>
            {
                // Check if all search words are contained in the string
                return searchWords.All(searchWord => s.ToLower().Contains(searchWord));
            }).ToList();

            // Sort the results for more accurate matches
            filteredResults.Sort((a, b) =>
            {
                string aLower = a.ToLower();
                string bLower = b.ToLower();

                // Prioritize strings that start with the searchString
                bool aStartsWith = aLower.StartsWith(searchString.ToLower());
                bool bStartsWith = bLower.StartsWith(searchString.ToLower());

                if (aStartsWith && !bStartsWith)
                    return -1;
                if (!aStartsWith && bStartsWith)
                    return 1;

                // If both start or both do not start with the search string, prioritize by the index of the first occurrence
                int aIndex = aLower.IndexOf(searchString.ToLower());
                int bIndex = bLower.IndexOf(searchString.ToLower());

                if (aIndex != bIndex)
                    return aIndex.CompareTo(bIndex);

                // If both have the same index, fall back to alphabetical order
                return string.Compare(aLower, bLower, System.StringComparison.Ordinal);
            });

            return filteredResults.ToArray();
        }
        public static string[] FilterMaps(string mapName)
        {
            string[] mapList;

            if (!string.IsNullOrEmpty(mapName))
            {
                mapList = FilterArray(GetMapNames(), mapName);
            }
            else
            {
                mapList = GetMapNames();
            }

            return mapList;
        }
        // Choose a map on a tie from vote index and options
        public static string ChooseMapOnTie(Dictionary<int, int> voteIndex, string[] mapOptions)
        {
            // Collect tied values
            int maxVotes = int.MinValue;
            List<int> tiedOptions = new List<int>();

            foreach (var pair in voteIndex)
            {
                if (pair.Value > maxVotes)
                {
                    // New maximum found, reset the tied list
                    maxVotes = pair.Value;
                    tiedOptions.Clear();
                    tiedOptions.Add(pair.Key);
                }
                else if (pair.Value == maxVotes)
                {
                    // Add to the list of tied options
                    tiedOptions.Add(pair.Key);
                }
            }

            // Randomly choose one of the tied options
            int randomIndex = Random.Range(0, tiedOptions.Count);
            int chosenOptionIndex = tiedOptions[randomIndex];

            return mapOptions[chosenOptionIndex];
        }
    }
}
