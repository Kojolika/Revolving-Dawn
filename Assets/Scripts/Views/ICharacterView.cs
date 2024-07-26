using Models.Characters;
using UnityEngine;

namespace Views
{
    public interface ICharacterView
    {
        Character CharacterModel { get; }
        Collider Collider { get; }
        Transform transform { get; }
        Renderer Renderer { get; }
    }
}