namespace Fight.Events
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

    }
  }

  public interface IHealth
  {
    void DealDamage(ulong x);
    void Heal(ulong x);
    void GainBlock(ulong x);
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
}
}