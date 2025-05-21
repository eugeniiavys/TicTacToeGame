using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe.Logic
{
    public struct PositionPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public PositionPoint(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public override bool Equals(object obj)
        {
            if (obj is PositionPoint point)
            {
                return X == point.X && Y == point.Y && Z == point.Z;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }
        public override string? ToString()
        {
            return $"{X}_{Y}_{Z}";
        }
        public static bool operator ==(PositionPoint left, PositionPoint right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(PositionPoint left, PositionPoint right)
        {
            return !left.Equals(right);
        }    
    }
}
