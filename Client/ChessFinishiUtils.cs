using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Client
{
    public class ChessFinishiUtils
    {
        private const int MAX_COUNT_IN_LINE = 4;

        public static bool checkWin(List<Chess> chesses)
        {
            foreach (Chess chess in chesses)
            {
                int x = chess._point.X;
                int y = chess._point.Y;
                int _color = chess.color;
                bool isWin = ChessFinishiUtils.checkHorizontal(x, y, _color, chesses);
                if (isWin)
                {
                    return true;
                }
                isWin = ChessFinishiUtils.checkVertical(x, y, _color, chesses);
                if (isWin)
                {
                    return true;
                }
                isWin = ChessFinishiUtils.checkMainDiagonal(x, y, _color, chesses);
                if (isWin)
                {
                    return true;
                }
                isWin = ChessFinishiUtils.checkMinorDiagonal(x, y, _color, chesses);
                if (isWin)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool checkVertical(int x, int y, int color, List<Chess> chesses)
        {
            int count = 1;
            string colors = "";
            for (int i = 1; i < MAX_COUNT_IN_LINE; i++)
            {
                if (chesses.Contains(new Chess(x, y - i, color)))
                {
                    colors += color.ToString();
                    count++;
                }
                else
                {
                    break;
                }
            }
            if (count == MAX_COUNT_IN_LINE)
            {
                using (StreamWriter sw = new StreamWriter("c.txt"))
                {
                     sw.WriteLine(colors);
                }
                return true;
            }
            count=1;
            colors = "";
            for (int i = 1; i < MAX_COUNT_IN_LINE; i++)
            {
                if (chesses.Contains(new Chess(x, y + i, color)))
                {
                    colors += color.ToString();
                    count++;
                }
                else
                {
                    break;
                }
            }
            if (count == MAX_COUNT_IN_LINE)
            {
                using (StreamWriter sw = new StreamWriter("c.txt"))
                {
                    sw.WriteLine(colors);
                }
                return true;
            }
            return false;
        }

        public static bool checkHorizontal(int x, int y, int color, List<Chess> chesses)
        {
            int count = 1;
            for (int i = 1; i < MAX_COUNT_IN_LINE; i++)
            {
                if (chesses.Contains(new Chess(x - i, y, color)))
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            if (count == MAX_COUNT_IN_LINE)
            {
                return true;
            }
            count=1;
            for (int i = 1; i < MAX_COUNT_IN_LINE; i++)
            {
                if (chesses.Contains(new Chess(x + i, y, color)))
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            if (count == MAX_COUNT_IN_LINE)
            {
                return true;
            }
            return false;
        }

        public static bool checkMainDiagonal(int x, int y, int color, List<Chess> chesses)
        {
            int count = 1;
            for (int i = 1; i < MAX_COUNT_IN_LINE; i++)
            {
                if (chesses.Contains(new Chess(x - i, y + i, color)))
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            if (count == MAX_COUNT_IN_LINE)
            {
                return true;
            }
            count=1;
            for (int i = 1; i < MAX_COUNT_IN_LINE; i++)
            {
                if (chesses.Contains(new Chess(x + i, y - i, color)))
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            if (count == MAX_COUNT_IN_LINE)
            {
                return true;
            }
            return false;
        }

        public static bool checkMinorDiagonal(int x, int y, int color, List<Chess> chesses)
        {
            int count = 1;
            for (int i = 1; i < MAX_COUNT_IN_LINE; i++)
            {
                if (chesses.Contains(new Chess(x - i, y - i, color)))
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            if (count == MAX_COUNT_IN_LINE)
            {
                return true;
            }
            count=1;
            for (int i = 1; i < MAX_COUNT_IN_LINE; i++)
            {
                if (chesses.Contains(new Chess(x + i, y + i, color)))
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            if (count == MAX_COUNT_IN_LINE)
            {
                return true;
            }
            return false;
        }
    }
}
