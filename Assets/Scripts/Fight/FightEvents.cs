using characters;
using UnityEngine;

namespace fight
{
    public static class FightEvents
    {
        ///////////////////////////////////////////////////////////////////////
        public delegate void EnemyDiedEffects(Enemy enemy);
        public static event EnemyDiedEffects TriggerEnemyDiedEffects;

        public static void OnEnemyDied(Enemy enemy)
        {
            if (TriggerEnemyDiedEffects != null)
            {
                TriggerEnemyDiedEffects(enemy);
            }
        }
        ///////////////////////////////////////////////////////////////////////
        public delegate void PlayerDiedEffects(Player player);
        public static event PlayerDiedEffects TriggerPlayerDiedEffects;

        public static void OnPlayerDied(Player player)
        {
            if (TriggerPlayerDiedEffects != null)
            {
                TriggerPlayerDiedEffects(player);
            }
        }
        ///////////////////////////////////////////////////////////////////////
        public delegate void FightWon();
        public static event FightWon TriggerFightWonEffects;

        public static void OnFightWon()
        {
            Debug.Log("Fight won");
            if (TriggerFightWonEffects != null)
            {
                TriggerFightWonEffects();
            }
        }

        ///////////////////////////////////////////////////////////////////////
        public delegate void PlayerTurnStarted();
        public static event PlayerTurnStarted OnPlayerTurnStarted;

        public static void TriggerPlayerTurnStarted()
        {
            //trigger start of turn effects
            //No events yet, will for later effects (bleed,poison, etc.)
            if (OnPlayerTurnStarted != null)
            {
                OnPlayerTurnStarted();
            }
        }
        ///////////////////////////////////////////////////////////////////////
        public delegate void PlayerTurnEnded();
        public static event PlayerTurnEnded OnPlayerTurnEnded;

        public static void TriggerPlayerTurnEnded()
        {
            //trigger end of turn effects
            //No events yet, will for later effects (bleed,poison, etc.)
            if (OnPlayerTurnEnded != null)
            {
                OnPlayerTurnEnded();
            }
        }

    }
}
