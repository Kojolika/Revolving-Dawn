using fightDamageCalc;
using characters;
public class DeBuffProcessor : Processor
{
    public DeBuffProcessor(Processor nextProcessor) : base(nextProcessor)
    {
    }

    public override Number process(Number request, Character target)
    {
        if(target.gameObject.TryGetComponent<Affects>(out Affects affects))
        {
            foreach(var affect in affects.list)
            {
                if(affect.AffectType == AffectType.Debuff)
                {
                    request = affect.process(request);
                }
            }
            return base.process(request, target);
        }
        else
        {
            return base.process(request, target);
        }
    }

}