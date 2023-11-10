using UnityEngine;

namespace UI.Menus.Common
{
    /// <summary>
    /// Used to reference the gameObject of a menu.
    /// </summary>
    public interface IMenuHandle
    {
        // ReSharper disable once InconsistentNaming
        // Needs to be same name as a monobehaviour gameObject property
        GameObject gameObject { get; }
    }
}