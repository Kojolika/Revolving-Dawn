using Characters;

namespace FightDamageCalc
{
    public class ProcessingChain
    {
        Processor chain;
        public ProcessingChain()
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
                                new RoundingProcessor(null)
                            )
                        )
                    );
        }

        public Number Process(Number request, Character source, Character target)
        {
            Number copy = new Number(request.Amount, request.GetDamageType()); //can add a bool in the future if needed where I would want to change the original request value
            return chain.process(copy, source, target);
        }


    }
}
