using characters;

namespace fightDamageCalc
{
    public class Chain
    {
        Processor chain;
        public Chain()
        {
            BuildChain();
        }

        void BuildChain()
        {
            //Creates an ordering for how affects are applied to damage/heal/block numbers
            //The ordering is Buffs -> Debuffs -> Resistances
            //Example: Orignal Attack: 10
            //         Buff +5 attack => 15
            //         Debuff -33% attack => 10
            //         Final Attack: 10
            //Resistances are possibly not going to be added but
            //Not implementing the ResistProcessor.process method is the same as not having them
            //so here they are to remind me of the idea
            chain = new BuffProcessor(
                    new DeBuffProcessor(
                    new ResistProcessor(
                    new RoundingProcessor(
                    null))));
        }

        public Number process(Number request, Character target)
        {
            return chain.process(request, target);
        }
   

}
}