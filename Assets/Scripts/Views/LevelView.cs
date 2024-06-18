using System.Collections.Generic;
using UnityEngine;

namespace Views
{
    public class LevelView : MonoBehaviour
    {
        [SerializeField] List<Transform> EnemySpawns;
        [SerializeField] Transform PlayerSpawn;
    }
}