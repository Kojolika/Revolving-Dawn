using Models.Characters;

namespace Fight.Events
{
    public class DrawCardEvent : BattleEvent<PlayerCharacter>
    {
        public DrawCardEvent(PlayerCharacter target, bool isCharacterAction = false) : base(target, isCharacterAction)
        {
            
        }

        public override void Execute(PlayerCharacter target, BattleEngine battleEngine)
        {
            throw new System.NotImplementedException();
        }

        public override string Log()
        {
            throw new System.NotImplementedException();
        }

        public override void Undo()
        {
            throw new System.NotImplementedException();
        }
    }
}