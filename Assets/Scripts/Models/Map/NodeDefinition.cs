using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Models.Map
{
    [Serializable]
    public class NodeDefinition
    {
        [JsonProperty("coordinate")]
        public Coordinate Coord;

        [JsonProperty("event")]
        public INodeEvent Event;

        [JsonProperty("next_nodes")]
        public List<Coordinate> NextNodes;

        [JsonProperty("previous_nodes")]
        public List<Coordinate> PreviousNodes;

        [JsonProperty("level")]
        public int Level = int.MaxValue;


        [Serializable]
        public struct Coordinate : IEquatable<Coordinate>, IComparable<Coordinate>
        {
            public int x;
            public int y;

            public Coordinate(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public static float Distance(Coordinate c1, Coordinate c2) => Mathf.Sqrt(Mathf.Pow(c1.x - c2.x, 2) + Mathf.Pow(c1.y - c2.y, 2));

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