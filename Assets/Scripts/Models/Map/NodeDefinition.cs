using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Models.Map
{
    [Serializable]
    public class NodeDefinition
    {
        [JsonProperty("x")]
        public int Y;

        [JsonProperty("y")]
        public int X;

        [JsonProperty("isBoss")]
        public bool IsBoss = false;

        [JsonProperty("nextNodes")]
        public List<Coordinate> NextNodes;


        [Serializable]
        public struct Coordinate : IEquatable<Coordinate>, IComparable<Coordinate>
        {
            public int x;
            public int y;

            public static readonly Coordinate Invalid = new Coordinate(int.MinValue, int.MinValue);

            public Coordinate(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public static Coordinate operator +(Coordinate a, Coordinate b) => new Coordinate(a.x + b.x, a.y + b.y);

            public static Coordinate operator -(Coordinate a, Coordinate b) => new Coordinate(a.x - b.x, a.y - b.y);

            public static bool operator ==(Coordinate a, Coordinate b) => ReferenceEquals(a, b) || (!ReferenceEquals(a, null) && a.Equals(b));

            public static bool operator !=(Coordinate a, Coordinate b) => !(a == b);

            public override bool Equals(object obj) => obj is Coordinate coordinate && Equals(coordinate);

            public override int GetHashCode() => (x, y).GetHashCode();

            public override string ToString() => $"({x},{y})";

            public bool Equals(Coordinate other)
                => !ReferenceEquals(other, null)
                    && x == other.x
                    && y == other.y;

            public int CompareTo(Coordinate other)
            {
                var subtractedCoord = other - this;
                var combinedValue = subtractedCoord.x + subtractedCoord.y;
                return combinedValue;
            }
        }
    }
}