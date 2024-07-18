using Models.Characters;
using UnityEngine;

namespace Views
{
    public interface ICharacterView
    {
        Character Character { get; }
        Collider Collider { get; }
        HealthView HealthView { get; }
    }
}