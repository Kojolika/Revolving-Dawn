using Models.Characters;
using UnityEngine;

namespace Views
{
    public interface ICharacterView
    {
        Character CharacterModel { get; }
        Collider Collider { get; }
        HealthView HealthView { get; }
        Transform HealthViewLocation { get; }
        Transform transform { get; }
    }
}