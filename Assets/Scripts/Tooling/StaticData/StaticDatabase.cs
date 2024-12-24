using System;
using System.Collections.Generic;

namespace Tooling.StaticData
{
    public class StaticDatabase
    {
        private readonly Dictionary<Type, Dictionary<string, StaticData>> staticDataDictionary = new();
    }
}