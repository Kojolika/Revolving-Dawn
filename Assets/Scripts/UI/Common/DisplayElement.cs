using Data;
using UnityEngine;

namespace UI.Common
{
    public abstract class DisplayElement<T> : MonoBehaviour, IPopulateData<T>
    {
        public abstract void Populate(T data);
    }
}