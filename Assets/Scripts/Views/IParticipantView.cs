using Fight.Engine;
using UnityEngine;

namespace Views
{
    public interface IParticipantView : IHighlightable
    {
        ICombatParticipant Model     { get; }
        Collider           Collider  { get; }
        Transform          transform { get; }
        Renderer           Renderer  { get; }
    }
}