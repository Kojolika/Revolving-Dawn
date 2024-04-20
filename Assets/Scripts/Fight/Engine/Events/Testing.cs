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
    string Log();
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

  public class BattleStartedEvent : BattleEvent<Fightmanager>
  {
    public BattleStartedEvent(Fightmanager target) : base(target){} 
    public override void Execute(Fightmanager target){ }
    public override string Log() => "Fight started!";
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
      var relevantBuffs = target.Buffs
        .Where(buff => buff is ITriggerableBuff<DealDamageEvent>)
        .Select(buff => buff as ITriggerableBuff<DealDamageEvent>);
        
      foreach(var buff in relevantBuffs)
      {
        buff.Apply(this);
      }

      target.DealDamage(Amount);
    }

    public override string Log() => $"{Target} is dealt {Amount} damage";
  }

  public class DealDamageFromCharacterEvent : DealDamageEvent
  {
    public Character Source { get; private set; }
    public DealDamageFromCharacterEvent(Character source, IHealth target, ulong amount) : base(target, amount)
    {
      Source = source;
    }
    public override string Log() => $"Character {Source} deals {Amount} damage to {Target}";
  }

  public class TurnStarted : BattleEvent<Character>
  {
    public TurnStarted(Character target) : base(target) { }

    public override void Execute(Character target) { }

    public override string Log() => $"{Target.Name}'s turn started!";
  }

  public class BattleEngine
  {
    public bool IsRunning { get; private set; }
    private Queue<IBattleEvent> battleEventQueue;
    public event Action<IBattleEvent> EventOccurred;

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
          var latestEvent = battleEventQueue.Dequeue();
          latestEvent.Execute();
          Console.Write(latestEvent.Log());
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

    public void StartCombat(PlayerHero player, List<Enemy> enemies)
    {
      Engine.Run();
      Engine.AddEvent(new BattleStartedEvent(this));
    }
  }

  public interface IBuff
  {
    string Name { get; }
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
    void Apply(T triggeredByEvent);
  }

  public class Block : IStackableBuff<TurnStarted>, ITriggerableBuff<DealDamageEvent>
  {
    public string Name => "Block";
    public ulong MaxStackSize { get; private set; }
    public ulong CurrentStackSize { get; private set; }

    public TurnStarted StacklossEvent { get; private set; }
    public ulong AmountLostPerEvent { get; private set; }

    public void Apply(DealDamageEvent triggeredByEvent)
    {
      
    }
  }

  public interface IHealth
  {
    List<IBuff> Buffs { get; }
    void DealDamage(ulong amount);
    void Heal(ulong amount);
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
    public abstract string Name { get; }
    public Health Health { get; set; }
    public List<IBuff> Buffs { get; set; }

    public void DealDamage(ulong amount) => Health.RemoveHealth(amount);
    public void Heal(ulong amount) => Health.AddHealth(amount);
  }

  public class PlayerHero : Character
  {
    public override string Name => Enum.GetName(typeof(PlayerClass), Class);
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
    public override string Name => "Slime";
  }
}

}