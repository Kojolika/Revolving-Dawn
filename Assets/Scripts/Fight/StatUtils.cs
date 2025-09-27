using System;
using System.Collections.Generic;
using System.Linq;
using Common.Util;
using Fight.Engine;
using Tooling.StaticData.Data;
using UnityEngine.Assertions;

namespace Fight
{
    public static class StatUtils
    {
        // TODO: Add a common key static class, create tests to make sure these common keys are defined in the static data base
        public const string HealthKey = "Health";

        public static float? GetMaxHealth(ICombatParticipant target)
        {
            var maxHealthStat = GetMaxStat(HealthKey);
            return target.GetStat(maxHealthStat);
        }

        public static float? GetMaxHealth(IEnumerable<StatAmount> stats)
        {
            return stats.OrEmptyIfNull().FirstOrDefault(i => i.Stat.Name == $"Max{HealthKey}")?.Amount ?? 0;
        }

        public static float? GetHealth(ICombatParticipant target)
        {
            var healthStat = StaticDatabase.Instance.GetInstance<Stat>(HealthKey);
            return target.GetStat(healthStat);
        }

        public static void SetHealth(ICombatParticipant target, float health)
        {
            var healthStat = StaticDatabase.Instance.GetInstance<Stat>(HealthKey);
            target.SetStat(healthStat, health);
        }

        public static void SetMaxHealth(ICombatParticipant target, float health)
        {
            var maxHealthStat = GetMaxStat(HealthKey);
            target.SetStat(maxHealthStat, health);
        }

        public static Stat GetMaxStat(string statKey)
        {
            return StaticDatabase.Instance.GetInstance<Stat>($"Max{statKey}");
        }

        public static void AddStat(ICombatParticipant target, Stat stat, float amount)
        {
            float currentAmount = target.GetStat(stat) ?? 0f;

            // We have a naming convention where a stat name can be prefixed with Max to allow max stats on characters on an individual basis
            // This allows flexibility
            float max = GetMaxStat(stat.Name) is { } maxStat &&
                        target.GetStat(maxStat) is { } maxStatAmount
                ? maxStatAmount
                : float.NegativeInfinity;

            float newAmount = MathF.Max(currentAmount + amount, max);

            Assert.IsTrue(newAmount >= currentAmount);

            target.SetStat(stat, newAmount);
        }
    }
}