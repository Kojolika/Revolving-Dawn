using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Tooling.StaticData.Data;
using UnityEngine;

namespace Models.Map
{
    public class NodeDefinition
    {
        public Coordinate       Coord;
        public NodeEvent        Event;
        public NodeEventLogic   EventLogic;
        public List<Coordinate> NextNodes;
        public List<Coordinate> PreviousNodes;
        public int              Level = int.MaxValue;

        [JsonIgnore] public int NumberOfEdges => NextNodes?.Count ?? 0 + PreviousNodes?.Count ?? 0;


        [Serializable, JsonObject(IsReference = false)]
        public struct Coordinate : IEquatable<Coordinate>, IComparable<Coordinate>
        {
            public int x;
            public int y;

            public Coordinate(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public static float Distance(Coordinate c1, Coordinate c2)
            {
                return Mathf.Sqrt(Mathf.Pow(c1.x - c2.x, 2) + Mathf.Pow(c1.y - c2.y, 2));
            }

            public static Coordinate operator +(Coordinate a, Coordinate b)
            {
                return new Coordinate(a.x + b.x, a.y + b.y);
            }

            public static Coordinate operator -(Coordinate a, Coordinate b)
            {
                return new Coordinate(a.x - b.x, a.y - b.y);
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
                return x == other.x && y == other.y;
            }

            public override bool Equals(object obj)
            {
                return obj is Coordinate coordinate && Equals(coordinate);
            }

            public override int GetHashCode()
            {
                return (x, y).GetHashCode();
            }

            public override string ToString()
            {
                return $"({x},{y})";
            }

            public int CompareTo(Coordinate other)
            {
                var subtractedCoord = other - this;
                var combinedValue   = subtractedCoord.x + subtractedCoord.y;
                return combinedValue;
            }
        }
    }
}