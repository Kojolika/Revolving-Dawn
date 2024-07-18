using Models.Characters;
using UnityEngine;

namespace Views
{
    public interface ICharacterView
    {
        Character Character { get; }
        Collider Collider { get; }
        HealthView HealthView { get; }
        Transform HealthViewLocation { get; }
        SpriteRenderer CharacterRenderer { get; }
        Transform transform { get; }
    }
}