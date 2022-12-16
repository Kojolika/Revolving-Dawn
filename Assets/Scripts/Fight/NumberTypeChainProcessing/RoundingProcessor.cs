using fightDamageCalc;
using characters;

public class RoundingProcessor : Processor
{
    public RoundingProcessor(Processor nextProcessor) : base(nextProcessor)
    {
    }

    public override Number process(Number request, Character target)
    {
        double num = request.Amount;
        request.Amount = (float)System.Math.Round(num);
        return base.process(request, target);
    }

}