using characters;
using UnityEngine;

namespace fight
{
    public static class FightEvents
    {
        public delegate void FightStarted();
        public static event FightStarted OnFightStarted;

        public static void TriggerFightStarted()
        {
            if(OnFightStarted != null)
            {
                OnFightStarted();
            }
        }
        ///////////////////////////////////////////////////////////////////////
        public delegate void EnemyDiedEffects(Enemy enemy);
        public static event EnemyDiedEffects OnEnemyDiedEffects;

        public static void TriggerEnemyDied(Enemy enemy)
        {
            if (OnEnemyDiedEffects != null)
            {
                OnEnemyDiedEffects(enemy);
            }
        }
        ///////////////////////////////////////////////////////////////////////
        public delegate void PlayerDiedEffects(Player player);
        public static event PlayerDiedEffects OnPlayerDiedEffects;

        public static void TriggerPlayerDied(Player player)
        {
            if (OnPlayerDiedEffects != null)
            {
                OnPlayerDiedEffects(player);
            }
        }
        ///////////////////////////////////////////////////////////////////////
        public delegate void FightWon();
        public static event FightWon OnFightWonEffects;

        public static void TriggerFightWon()
        {
            Debug.Log("Fight won");
            if (OnFightWonEffects != null)
            {
                OnFightWonEffects();
            }
        }

        ///////////////////////////////////////////////////////////////////////
        public delegate void CharacterTurnStarted(Character character);
        public static event CharacterTurnStarted OnCharacterTurnStarted;

        public static void TriggerCharacterTurnStarted(Character character)
        {
            //trigger start of turn effects
            //No events yet, will for later effects (bleed,poison, etc.)
            if (OnCharacterTurnStarted != null)
            {
                OnCharacterTurnStarted(character);
            }
        }
        ///////////////////////////////////////////////////////////////////////
        public delegate void CharacterTurnEnded(Character character);
        public static event CharacterTurnEnded OnCharacterTurnEnded;

        public static void TriggerCharacterTurnEnded(Character character)
        {
            //trigger end of turn effects
            //No events yet, will for later effects (bleed,poison, etc.)
            if (OnCharacterTurnEnded != null)
            {
                OnCharacterTurnEnded(character);
            }
        }

    }
}
