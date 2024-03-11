using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace XLMultiMapVote.Utils
{
    public static class MapHelper
    {
        //private static List<LevelInfo> LevelList;
        //private static List<LevelInfo> ModLevelList;
        //private static List<LevelInfo> CommunityLevelList;

        public static List<LevelInfo> combinedMapList = new List<LevelInfo>();
        public static string[] mapNames;

        public static string [] GetMaps()
        {
            combinedMapList.Clear();
            mapNames = new string[] {};

            if (LevelManager.Instance.Levels != null)
            {
                combinedMapList.AddRange(LevelManager.Instance.Levels.ToList());
            }
            if (LevelManager.Instance.ModLevels != null)
            {
                combinedMapList.AddRange(LevelManager.Instance.ModLevels.ToList());
            }
            if (LevelManager.Instance.CommunityLevels != null)
            {
                combinedMapList.AddRange(LevelManager.Instance.CommunityLevels.ToList());
            }

            mapNames = ConvertToString(combinedMapList);
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
    }
}
