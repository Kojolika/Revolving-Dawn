using System;
using Tooling.Logging;

namespace Tooling.StaticData.EditorUI
{
    /// <summary>
    /// Defines custom callbacks for types that are drawn by the <see cref="EditorWindow"/>
    /// when an instance of that types value is changed in the editor window.
    /// <remarks>Types inheriting this must have an open constructor (no parameters) and should not be instantiated directly.</remarks>
    /// </summary>
    public interface ICallback
    {
        Type CallbackType { get; }
        void OnValueChanged(object oldValue, object newValue);
    }

    public interface ICallback<in T> : ICallback
    {
        Type ICallback.CallbackType => typeof(T);
        protected void OnValueChanged(T oldValue, T newValue);

        void ICallback.OnValueChanged(object oldValue, object newValue)
        {
            MyLogger.Log($"OnValueChanged for type; {oldValue.GetType()}");
            OnValueChanged((T)oldValue, (T)newValue);
        }
    }
}