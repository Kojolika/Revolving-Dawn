using FightDamageCalc;
using Characters;

public class ResistProcessor : Processor
{
    public ResistProcessor(Processor nextProcessor) : base(nextProcessor)
    {
    }

    public override Number process(Number request, Character source,  Character target)
    {
        //effectively empty processor
        //no resistences are implemented yet
        return base.process(request, source, target);
    }
}