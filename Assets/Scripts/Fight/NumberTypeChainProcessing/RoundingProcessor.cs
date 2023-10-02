using FightDamageCalc;
using Characters;

public class RoundingProcessor : Processor
{
    public RoundingProcessor(Processor nextProcessor) : base(nextProcessor)
    {
    }

    public override Number process(Number request, Character source,  Character target)
    {
        double num = request.Amount;
        request.Amount = (float)System.Math.Round(num);
        return base.process(request, source, target);
    }

}