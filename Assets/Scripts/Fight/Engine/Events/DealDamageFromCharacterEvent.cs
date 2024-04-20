using Models;
using Models.Characters;

namespace Fight.Events
{
    public class DealDamageFromCharacterEvent : DealDamageEvent
    {
        public Character Source { get; private set; }
        public DealDamageFromCharacterEvent(Character source, IHealth target, ulong amount) : base(target, amount)
        {
            Source = source;
        }
        public override string Log() => $"Character {Source} deals {Amount} damage to {Target}";
    }
}