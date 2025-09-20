using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Models.Map
{
    [JsonObject(IsReference = false)]
    public struct Coordinate : IEquatable<Coordinate>, IComparable<Coordinate>
    {
        public int X;
        public int Y;

        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static float Distance(Coordinate c1, Coordinate c2)
        {
            return Mathf.Sqrt(Mathf.Pow(c1.X - c2.X, 2) + Mathf.Pow(c1.Y - c2.Y, 2));
        }

        public static Coordinate operator +(Coordinate a, Coordinate b)
        {
            return new Coordinate(a.X + b.X, a.Y + b.Y);
        }

        public static Coordinate operator -(Coordinate a, Coordinate b)
        {
            return new Coordinate(a.X - b.X, a.Y - b.Y);
        }

        public static bool operator ==(Coordinate a, Coordinate b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Coordinate a, Coordinate b)
        {
            return !(a == b);
        }

        public bool Equals(Coordinate other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is Coordinate coordinate && Equals(coordinate);
        }

        public override int GetHashCode()
        {
            return (x: X, y: Y).GetHashCode();
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }

        public int CompareTo(Coordinate other)
        {
            var subtractedCoord = other - this;
            var combinedValue   = subtractedCoord.X + subtractedCoord.Y;
            return combinedValue;
        }
    }
}