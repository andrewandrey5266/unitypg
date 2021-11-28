using System.Collections.Generic;
using System;

static class Generator
{
    static Random rnd = new Random();

    public static int[,] GetPuzzle(int xlength, int ylength, int figureN, int minSize, int maxSize, out int resultFigureN)
    {
        resultFigureN = figureN;
        int figureI;
        for (int I = 0; I < 1000; I++)
        {
            bool failedToBuild = false;

            int[,] area = new int[ylength, xlength];

            for (figureI = 1; figureI <= figureN; figureI++)
            {
                if(!SearchEmptySpot(area, out int y, out int x))
                {
                    resultFigureN = figureI - 1;
                    break;
                }
                area[y, x] = figureI;

                int figureSize = rnd.Next(minSize, maxSize + 1);

                //we start from i = 2, because in code below we found empty spot, and put first block there
                for (int i = 2; i <= figureSize; i++)
                {
                    if (GetNewYX(area, x, y, out (int, int) yx))
                    {
                        y = yx.Item1;
                        x = yx.Item2;

                        area[y, x] = figureI;
                    }
                    else
                    {
                        //if spot not found and figure that was build is less then min size, we fail this iteration
                        if(i - 1 < minSize)
                        {
                            failedToBuild = true;
                        }
                        //if figure size is >= minSize, we start to build another figure
                        break;
                    }
                }

                if (failedToBuild)
                {
                    break;
                }
            }

            if (!failedToBuild)
            {
                //if there's empty spot, we fail this iteration, and start from beginning
                foreach (var c in area)
                {
                    if (c == 0)
                    {
                        failedToBuild = true;
                        break;
                    }
                }
            }

            if (failedToBuild || GameManager.Settings.ExactAmountOfFigures &&  figureI != figureN)
            {
                continue;
            }

            return area;
        }

        return null;
    }

    static bool SearchEmptySpot(int[,] area, out int _y, out int _x)
    {
        _y = 0; _x = 0;
        for (int y = 0; y < area.GetLength(0); y++)
        {
            for (int x = 0; x < area.GetLength(1); x++)
            {
                if (area[y, x] == 0)
                {
                    _y = y; _x = x;
                    return true;
                }
            }
        }

        return false;
    }

    static bool GetNewYX(int[,] area, int x, int y, out (int, int) yx)
    {
        List<(int, int)> directions = new List<(int, int)>
        {
            (1, 0),
            (0, 1),
            (-1, 0),
            (0, -1)
        };
        yx = (0, 0);

        for (int r = 0; r < 4; r++)
        {
            int randomDir = rnd.Next(0, directions.Count);
            var dirVal = directions[randomDir];

            int newY = y + dirVal.Item1;
            int newX = x + dirVal.Item2;
            yx = (newY, newX);

            if (newX < 0 || newX >= area.GetLength(1) || newY < 0 || newY >= area.GetLength(0))
            {
                directions.Remove(dirVal);

                if (r == 3)
                {
                    return false;
                }
            }
            else
            {
                var cellValue = area[newY, newX];
                if (cellValue != 0)
                {
                    directions.Remove(dirVal);

                    if (r == 3)
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        return false;
    }
}