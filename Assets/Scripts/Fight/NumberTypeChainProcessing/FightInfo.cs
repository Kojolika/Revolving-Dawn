namespace FightDamageCalc
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

    [System.Serializable]
    public class Number
    {
        [UnityEngine.SerializeField] float amount;
        [UnityEngine.SerializeField] FightInfo.NumberType type;

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

        public FightInfo.NumberType GetDamageType()
        {
            return type;
        }
    }
}