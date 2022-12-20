using fightDamageCalc;
using characters;
using System.Collections.Generic;

public class BuffProcessor : Processor
{
    public BuffProcessor(Processor nextProcessor) : base(nextProcessor)
    {
    }

    public override Number process(Number request, Character target)
    {
        if(target.gameObject.TryGetComponent<Affects>(out Affects affects))
        {
            foreach(var affect in affects.list)
            {
                if(affect.AffectType == AffectType.Buff)
                {
                    request = affect.process(request);
                }
            }

            return base.process(request,target);
        }
        else
        {
            return base.process(request,target);
        }
    }

}