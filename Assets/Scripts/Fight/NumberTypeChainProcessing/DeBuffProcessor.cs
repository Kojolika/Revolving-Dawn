using FightDamageCalc;
using Characters;
public class DeBuffProcessor : Processor
{
    public DeBuffProcessor(Processor nextProcessor) : base(nextProcessor)
    {
    }

    public override Number process(Number request, Character source,  Character target)
    {
        if(target && target.gameObject.TryGetComponent<Affects>(out Affects affects))
        {
            foreach(var affect in affects.list)
            {
                if(affect.AffectType == AffectType.Debuff && affect.AffectsOtherCharactersAbilities)
                {
                    request = affect.process(request);
                }
            }
        }
        if(source && source.gameObject.TryGetComponent<Affects>(out Affects source_affects))
        {
            foreach(var affect in source_affects.list)
            {
                if(affect.AffectType == AffectType.Debuff && !affect.AffectsOtherCharactersAbilities)
                {
                    request = affect.process(request);
                }
            }

            return base.process(request, source, target);
        }
        else
        {
            return base.process(request, source, target);
        }
    }

}