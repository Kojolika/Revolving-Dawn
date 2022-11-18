using UnityEngine;

namespace cards
{
    public abstract class Card : MonoBehaviour
    {
        public abstract void Play();
        public abstract int GetTarget();
        public abstract bool IsManaCharged();
        public abstract void LoadInfo(CardScriptableObject cardSO);
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

    public static class CardInfo{
        public static Vector3 DEFAULT_CARD_ROTATION = new Vector3(90f,90f,-90f);
        public static Vector3 DEFAULT_SCALE = new Vector3(0.2f,1f,0.3f);
        public static float CAMERA_DISTANCE = Camera.main.nearClipPlane + 7;
    }

}

