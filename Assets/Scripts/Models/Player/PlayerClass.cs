using UnityEngine;

namespace Models.Player
{
    [CreateAssetMenu(fileName = "New Player Class", menuName = "Player/Classes")]
    public class PlayerClass : ScriptableObject
    {
        public string Name { get; private set; }
    }
}