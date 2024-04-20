using Models.Buffs;

namespace Models
{
    public interface IHealth : IBuffable
    {
      void DealDamage(ulong amount);
      void Heal(ulong amount);
    }
}