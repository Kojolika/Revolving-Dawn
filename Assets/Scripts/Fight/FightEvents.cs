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
            if (OnFightStarted != null)
            {
                OnFightStarted();
            }
        }
        ///////////////////////////////////////////////////////////////////////
        public delegate void EnemyDied(Enemy enemy);
        public static event EnemyDied OnEnemyDied;

        public static void TriggerEnemyDied(Enemy enemy)
        {
            Debug.Log("Enemy died");
            if (OnEnemyDied != null)
            {
                OnEnemyDied(enemy);
            }
        }
        ///////////////////////////////////////////////////////////////////////
        public delegate void PlayerDied(Player player);
        public static event PlayerDied OnPlayerDied;

        public static void TriggerPlayerDied(Player player)
        {
            Debug.Log("Player Died");
            if (OnPlayerDied != null)
            {
                OnPlayerDied(player);
            }
        }
        ///////////////////////////////////////////////////////////////////////
        public delegate void FightWon();
        public static event FightWon OnFightWon;

        public static void TriggerFightWon()
        {
            Debug.Log("Fight won");
            if (OnFightWon != null)
            {
                OnFightWon();
            }
        }
        ///////////////////////////////////////////////////////////////////////
        public delegate void FightLost();
        public static event FightLost OnFightLost;
        public static void TriggerFightLost()
        {
            Debug.Log("Fight Lost");
            if (OnFightLost != null)
            {
                OnFightLost();
            }
        }

        ///////////////////////////////////////////////////////////////////////
        public delegate void CharacterTurnStarted(Character character);
        public static event CharacterTurnStarted OnCharacterTurnStarted;

        public static void TriggerCharacterTurnStart(Character character)
        {
            Debug.Log("Calling turn start for: " + character);
            //trigger start of turn effects
            //No events yet, will for later effects (bleed,poison, etc.)
            if (OnCharacterTurnStarted != null)
            {
                OnCharacterTurnStarted(character);
            }
        }
        ///////////////////////////////////////////////////////////////////////
        public delegate void ChracterTurnAction(Character character);
        public static event ChracterTurnAction OnCharacterTurnAction;
        public static void TriggerCharacterTurnAction(Character character)
        {
            if (OnCharacterTurnAction != null)
            {
                OnCharacterTurnAction(character);
            }
        }
        ///////////////////////////////////////////////////////////////////////
        public delegate void CharacterTurnEnded(Character character);
        public static event CharacterTurnEnded OnCharacterTurnEnded;

        public static void TriggerCharacterTurnEnd(Character character)
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
