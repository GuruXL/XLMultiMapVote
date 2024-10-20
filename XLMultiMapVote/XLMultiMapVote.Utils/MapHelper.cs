using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace XLMultiMapVote.Utils
{
    public static class MapHelper
    {
        //private static List<LevelInfo> LevelList;
        //private static List<LevelInfo> ModLevelList;
        //private static List<LevelInfo> CommunityLevelList;

        public static List<LevelInfo> combinedMapList = new List<LevelInfo>();
        public static string[] mapNames;

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

        public static string [] GetMapNames()
        {
            mapNames = new string[] { };
            mapNames = ConvertToString(GetMaps());
            return mapNames;
        }

        public static string[] ConvertToString(List<LevelInfo> info)
        {
            string[] names = new string[info.Count];

            for(int i = 0; i < info.Count; i++)
            {
                names[i] = info[i].name;
            }
            return names;
        }
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
        /* FilterArray() old version
        public static string[] FilterArray(string[] filteredStrings, string searchstring)
        {
            string[] searchWords = searchstring.ToLower().Split(' ');

            return filteredStrings = mapNames.Where(s =>
            {
                bool containsAllWords = true;
                foreach (string searchWord in searchWords)
                {
                    if (!s.ToLower().Contains(searchWord))
                    {
                        containsAllWords = false;
                        break;
                    }
                }
                return containsAllWords;
            }).ToArray();
        }
        */

        public static string ChooseMapOnTie(Dictionary<int, int> voteIndex, string[] mapOptions)
        {
            // collect tied values
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
        /* ChooseMapOnTie() old version
        public string ChooseMapOnTie(Dictionary<int, int> voteIndex, string[] mapOptions)
        {
            // This function assumes there is already a tie and doesn't check it by itself
            int maxVotes = voteIndex.Values.Max();
            var tiedOptions = voteIndex.Where(pair => pair.Value == maxVotes).Select(pair => pair.Key).ToList();

            int randomIndex = UnityEngine.Random.Range(0, tiedOptions.Count);
            int chosenOptionIndex = tiedOptions[randomIndex];

            return mapOptions[chosenOptionIndex];
        }
        */
    }
}
