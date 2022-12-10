
namespace fightDamageCalc
{
    public class FightInfo
    {
        public enum NumberType
        {
            None,
            Attack,
            Block,
            Heal
        }

    }

    public class Number
    {
        float amount;
        FightInfo.NumberType type;
        public Number(float number, FightInfo.NumberType type)
        {
            this.amount = number;
            this.type = type;
        }

        public float Amount
        {
            get => amount;
            set => amount = value;
        }
        public FightInfo.NumberType getType()
        {
            return type;
        }
    }
}
