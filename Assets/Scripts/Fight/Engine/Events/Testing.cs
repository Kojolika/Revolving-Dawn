namespace Testing
{
  using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Program
{
  public static void Main()
  {

  }

  public interface IBattleEvent
  {
    void Execute();
  }

  public abstract class BattleEvent<S,T> : IBattleEvent
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
  }

  public class BattleStartedEvent : BattleEvent<Fightmanager>
  {
    public BattleStartedEvent(Fightmanager target) : base(target){} 
    public override void Execute(Fightmanager target)
    {
      Console.Write("Fight started!");
    }
  }

  public class DealDamageEvent : BattleEvent<IHealth>
  {
    public ulong Amount { get; private set; }
    public DealDamageEvent(IHealth target, ulong amount) : base(target)
    {
      Amount = amount;
    }

    public override void Execute(IHealth target)
    {
      target.DealDamage(Amount);
    }
  }

  public class TurnStarted : BattleEvent<Character>
  {
    public TurnStarted(Character target) : base(target) { }

    public override void Execute(Character target) { }
  }

  public class BattleEngine
  {
    public bool IsRunning { get; private set; }
    private Queue<IBattleEvent> battleEventQueue;

    public void Run()
    {
      battleEventQueue = new Queue<IBattleEvent>();
      IsRunning = true;
      EngineLoop();
    }

    public void Stop()
    {
      battleEventQueue.Clear();
      IsRunning = false;
    }

    async void EngineLoop()
    {
      while(IsRunning)
      {
        if(battleEventQueue.Count > 0)
        {
          battleEventQueue.Dequeue().Execute();
        }
        else
        {
          await Task.Delay(500);
        }
      }
    }

    public void AddEvent(IBattleEvent battleEvent) => battleEventQueue.Enqueue(battleEvent); 
  }

  public class Fightmanager
  {
    public BattleEngine Engine { get; private set; }

    public Fightmanager()
    {
      Engine = new BattleEngine();
    }

    public void StartCombat()
    {
      Engine.Run();
      Engine.AddEvent(new BattleStartedEvent(this));
    }
  }

  public interface IBuff
  {
    ulong MaxStackSize { get; }
    ulong CurrentStackSize { get; }
  }

  public interface IStackableBuff<T> : IBuff where T : IBattleEvent
  {
    T StacklossEvent { get; }
    ulong AmountLostPerEvent { get; }
  }

  public interface ITriggerableBuff<T> : IBuff where T : IBattleEvent
  {
    T TriggerByEvent { get; }
    void Apply();
  }

  public class Block : IStackableBuff<TurnStarted>, ITriggerableBuff<DealDamageEvent>
  {
    public ulong MaxStackSize { get; private set; }
    public ulong CurrentStackSize { get; private set; }

    public TurnStarted StacklossEvent { get; private set; }
    public ulong AmountLostPerEvent { get; private set; }

    public DealDamageEvent TriggerByEvent { get; private set; }

    public void Apply()
    {

    }
  }

  public interface IHealth
  {
    void DealDamage(ulong x);
    void Heal(ulong x);
    void GainBlock(ulong x);
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
      ulong finalHealth = default;
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

  public class Player
  {
    public PlayerHero Hero { get; private set; }
  }

  public abstract class Character : IHealth
  {
    public Health Health { get; set; }
    public List<IBuff> Buffs { get; set; }
    
    public void DealDamage(ulong amount)
    {
      
    }

    public void Heal(ulong amount)
    {

    }

    public void GainBlock(ulong amount)
    {

    }
  }

  public class PlayerHero : Character
  {
    public PlayerClass Class { get; private set; }

  }

  public enum PlayerClass
  {
    Warrior,
    Rogue,
    Priest,
    Mage
  }

  public abstract class Enemy : Character
  {

  }

  public class Slime : Enemy
  {

  }
}

}