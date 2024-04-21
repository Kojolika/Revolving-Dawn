namespace Testing 
{
    using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
  #region Unity Objects
  public class ScriptableObject { }
  public class GameObject { }
  public class MonoBehaviour : GameObject { }
  public class Sprite { }
  public class SerializedField : Attribute { }
  #endregion

  #region My Objects
    public interface IBuffable
    {
        List<IBuff> Buffs { get; }
    }
    public interface IHealth : IBuffable
    {
      void DealDamage(ulong amount);
      void Heal(ulong amount);
    }
    public interface IBuffDefinition
    {
      string Name { get; }
    }
    public interface IBattleEvent
    {
        void Execute();
        string Log();
    }
    public abstract class BattleEvent<S, T> : IBattleEvent
    {
        public S Source { get; private set; }
        public T Target { get; private set; }

        public BattleEvent(S source, T target)
        {
            Source = source;
            Target = target;
        }

        public abstract void Execute(S source, T target);
        public void Execute() => Execute(Source, Target);
        public abstract string Log();
    }
    public abstract class BattleEvent<T> : IBattleEvent
    {
        public T Target { get; private set; }

        public BattleEvent(T target)
        {
            Target = target;
        }

        public abstract void Execute(T target);
        public void Execute() => Execute(Target);
        public abstract string Log();
    }
    public interface IStackableBuff<T> : IBuff where T : IBattleEvent
    {
        ulong CurrentStackSize { get; }
        void OnEventTriggered(T StacklossEvent);
    }

    public interface IStackableBuffDefinition : IBuff
    {
        ulong AmountLostPerEvent { get; }
        ulong MaxStackSize { get; }
    }

    public interface ITriggerableBuff<T> : IBuff where T : IBattleEvent
    {
        void Apply(T triggeredByEvent);
    }
    public class DealDamageEvent : BattleEvent<IHealth>
    {
        public ulong Amount { get; set; }
        public DealDamageEvent(IHealth target, ulong amount) : base(target)
        {
            Amount = amount;
        }

        public override void Execute(IHealth target)
        {
            var relevantBuffs = target.Buffs
              .Where(buff => buff is ITriggerableBuff<DealDamageEvent>)
              .Select(buff => buff as ITriggerableBuff<DealDamageEvent>);

            foreach (var buff in relevantBuffs)
            {
                buff.Apply(this);
            }

            target.DealDamage(Amount);
        }

        public override string Log() => $"{Target} is dealt {Amount} damage";
    }
    public class TurnStarted : BattleEvent<Character>
    {
        public TurnStarted(Character target) : base(target) { }

        public override void Execute(Character target) { }

        public override string Log() => $"{Target.Name}'s turn started!";
    }
    public abstract class Character : IHealth
    {
        public abstract string Name { get; }
        public Health Health { get; set; }
        public List<IBuff> Buffs { get; set; }

        public void DealDamage(ulong amount) => Health.RemoveHealth(amount);
        public void Heal(ulong amount) => Health.AddHealth(amount);
    }
    public class Health
    {
      public ulong MaxHealth { get; private set; }
      public ulong CurrentHealth { get; private set; }

      public Health(ulong currentHealth, ulong maxHealth)
      {
        CurrentHealth = currentHealth;
        MaxHealth = maxHealth;
      }

      public void SetHealth(ulong amount) => CurrentHealth = Math.Min(MaxHealth, amount);


      public void AddHealth(ulong amount)
      {
        ulong finalHealth = default;
        try
        {
          checked
          {
            finalHealth = CurrentHealth + amount;
            finalHealth = Math.Min(MaxHealth, finalHealth);
          }
        }
        catch (OverflowException e)
        {
          finalHealth = Math.Min(MaxHealth, ulong.MaxValue);
        }
        CurrentHealth = finalHealth;
      }

      public void RemoveHealth(ulong amount)
      {
        ulong finalHealth;
        try
        {
          checked
          {
            finalHealth = CurrentHealth - amount;
          }
        }
        catch (OverflowException e)
        {
          finalHealth = 0;
        }
        CurrentHealth = finalHealth;
      }
    }
  #endregion

  public abstract class RuntimeModel<D> where D : ScriptableObject
  {
    public abstract D Definition { get; }
  }

  public class BlockDefinition : ScriptableObject, IStackableBuffDefinition
  {
    [SerializedField] string name;
    [SerializedField] ulong amountLostPerEvent;
    [SerializedField] ulong maxStackSize;

    [SerializedField] Sprite icon;
    
    public string Name => name;
    public ulong AmountLostPerEvent => amountLostPerEvent;
    public ulong MaxStackSize => maxStackSize;
  }

  public class BlockView : MonoBehaviour
  {
    
  }

  public interface IBuff
  {
    void Apply();
  }

  public abstract class Buff<D> : RuntimeModel<D>, IBuff where D : ScriptableObject 
  {

  }


  public class Block : Buff<BlockDefinition>, IStackableBuff<TurnStarted>, ITriggerableBuff<DealDamageEvent>
  {
    [SerializedField] BlockDefinition definition;

    public override BlockDefinition Definition => definition;
    public ulong CurrentStackSize { get; private set; }

    public Block(ulong currentStackSize) => CurrentStackSize = currentStackSize;

    public void Apply(DealDamageEvent dealDamageEvent)
        {
            try
            {
                checked
                {
                    dealDamageEvent.Amount -= CurrentStackSize;
                }
            }
            catch (OverflowException)
            {
                dealDamageEvent.Amount = 0;
            }
        }

        public void OnEventTriggered(TurnStarted StacklossEvent)
        {
            try
            {
                checked
                {
                    CurrentStackSize -= Definition.AmountLostPerEvent;
                }
            }
            catch (OverflowException)
            {
                CurrentStackSize = 0;
            }
        }
  }
}

}