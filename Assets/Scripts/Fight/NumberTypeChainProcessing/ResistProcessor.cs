using fightDamageCalc;
using characters;

public class ResistProcessor : Processor
{
    public ResistProcessor(Processor nextProcessor) : base(nextProcessor)
    {
    }

    public override Number process(Number request, Character target)
    {
        //effectively empty processor
        //no resistences are implemented yet
        return base.process(request, target);
    }
}