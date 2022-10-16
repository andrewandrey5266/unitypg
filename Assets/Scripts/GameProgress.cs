using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public static class GameProgress
{
        public static string ComposeInfo(string savedProgress, int[,] currentPuzzle, int numberOfMoves,
                TimeSpan levelTime)
        {
                List<ProgressInfo> progressInfoList =
                        JsonConvert.DeserializeObject<List<ProgressInfo>>(savedProgress) ?? new List<ProgressInfo>();

                progressInfoList.Add(new ProgressInfo
                {
                        CurrentPuzzle = currentPuzzle,
                        NumberOfMoves = numberOfMoves,
                        LevelTime = levelTime
                });

                string progressInfo = JsonConvert.SerializeObject(progressInfoList);

                return progressInfo;
        }
        
        private class ProgressInfo
        { 
                public int[,] CurrentPuzzle { get; set; }
                public int NumberOfMoves { get; set; }
                public TimeSpan LevelTime { get; set; }
        }
}

