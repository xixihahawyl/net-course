using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Client
{
   public class Chess
    {
        public Point _point;
        public int color;
        public Chess()
        { }
        public Chess(int x, int y, int color)
        {
            this.color = color;
            this._point.X = x;
            this._point.Y = y;
        }

        public override bool Equals(object obj)
        {
            return obj is Chess chess &&
                   EqualityComparer<Point>.Default.Equals(_point, chess._point) &&
                   color == chess.color;
        }

        public override int GetHashCode()
        {
            int hashCode = -1271543208;
            hashCode = hashCode * -1521134295 + _point.GetHashCode();
            hashCode = hashCode * -1521134295 + color.GetHashCode();
            return hashCode;
        }
    }
}
