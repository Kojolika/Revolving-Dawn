namespace Utils
{
    [System.Serializable]
    public class ReadOnly<T>
    {
        [UnityEngine.SerializeField]
        private T value;
        public T Value => value;

        public static implicit operator T(ReadOnly<T> instance)
        {
            return (instance.value);
        }
    }

}