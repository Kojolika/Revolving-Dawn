using Models.Buffs;

namespace Models
{
    public interface IHealth 
    {
      void DealDamage(ulong amount);
      void Heal(ulong amount);
    }
}