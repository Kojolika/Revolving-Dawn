using System.Collections.Generic;
using Systems.Managers.Base;
using UnityEngine;

namespace GameLoop.Startup
{
    public class Startup : MonoBehaviour
    {
        [SerializeField] private List<IManager> scriptAbleObjectManagers;
        private void Awake()
        {
            Managers.InitializeManagers(scriptAbleObjectManagers);
        }
    }
}