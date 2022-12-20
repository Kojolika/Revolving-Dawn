using UnityEngine;
using characters;
using System.Collections.Generic;
using TMPro;

namespace cards
{
    public abstract class Card : MonoBehaviour
    {
        public abstract void Play(List<Character> targets);
        public abstract int GetTarget();
        public abstract bool IsManaCharged();
        public virtual void LoadInfo(CardScriptableObject cardSO){}
        public Player currentPlayer;
    }

    enum Targeting
    {
        Friendly,
        Enemy,
        RandomEnemy,
        AllEnemies,
        All,
        None
    }

    public static class CardInfo
    {
        public static Vector3 DEFAULT_CARD_ROTATION = new Vector3(90f,90f,-90f);
        public static Vector3 DEFAULT_SCALE = new Vector3(0.2f,1f,0.3f);
        public static float CAMERA_DISTANCE = Camera.main.nearClipPlane + 7;
        public static TMP_FontAsset DEFAULT_FONT = Resources.Load<TMP_FontAsset>("DeterminationSansWebRegular-369X SDF");

    }

}

