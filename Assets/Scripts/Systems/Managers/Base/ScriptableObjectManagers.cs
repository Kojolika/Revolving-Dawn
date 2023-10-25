using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Systems.Managers.Base
{
    [Serializable]
    public class ScriptableObjectManagers : MonoBehaviour
    {
        [SerializeField] private List<AbstractSOManager> scriptableObjectManagers = new List<AbstractSOManager>();
        public List<AbstractSOManager> SOManagers => scriptableObjectManagers;
    }
}