using System.Collections.Generic;
using Systems.Managers.Base;
using UnityEngine;

namespace Systems.Managers
{
    public class MySceneManager : ScriptableObject, IManager
    {
        public List<Object> dontDestroyOnLoadObjects = new List<Object>();

        public void AddObjectToNotDestroyOnLoad(Object obj)
        {
            
        }
    }
}