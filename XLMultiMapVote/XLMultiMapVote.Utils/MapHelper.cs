using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace XLMultiMapVote.Utils
{
    public static class MapHelper
    {
        // Existing map lists
        public static List<LevelInfo> combinedMapList = new List<LevelInfo>();
        public static string[] mapNames;

        // Properties to track current and last LevelInfo
        public static LevelInfo currentLevelInfo { get; set; }
        public static LevelInfo nextLevelInfo { get; set; }
        // Utility to check if the map has changed
        public static bool HasMapChanged()
        {
            return nextLevelInfo != null && currentLevelInfo == nextLevelInfo;
        }

        // Method to update current level
        public static void SetCurrentLevel()
        {
            if (currentLevelInfo != nextLevelInfo)
            {
                currentLevelInfo = nextLevelInfo;
            }
        }

        // Method to get map info by name
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

        // Get combined list of all maps
        public static List<LevelInfo> GetMaps()
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
        public static string[] GetMapNames()
        {
            mapNames = ConvertToString(GetMaps());
            return mapNames;
        }

        // Convert LevelInfo list to string names
        public static string[] ConvertToString(List<LevelInfo> info)
        {
            string[] names = new string[info.Count];

            for (int i = 0; i < info.Count; i++)
            {
                names[i] = info[i].name;
            }
            return names;
        }

        // Filter map names based on a search string
        public static string[] FilterArray(string[] mapNames, string searchString)
        {
            // Split the search string into lowercase words
            string[] searchWords = searchString.ToLower().Split(' ');

            // Filter the array based on the search string
            return mapNames.Where(s =>
            {
                // Check if all search words are contained in the string
                return searchWords.All(searchWord => s.ToLower().Contains(searchWord));
            }).ToArray();
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
