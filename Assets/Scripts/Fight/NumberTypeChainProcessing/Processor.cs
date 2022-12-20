using fightDamageCalc;
using characters;

public abstract class Processor 
{
    private Processor nextProcessor;

    public Processor(Processor nextProcessor)
    {
        this.nextProcessor = nextProcessor;
    }

    public virtual Number process(Number request, Character target)
    {
        if(nextProcessor != null)
        {
            return nextProcessor.process(request, target);
        }
        else return request;
    }
}