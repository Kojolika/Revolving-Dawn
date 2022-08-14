namespace utils
{
    public class Computations
    {
        public float Normalize(float num, float oldMin, float oldMax, float newMin, float newMax)
        {
            return newMin + (num - oldMin) / (oldMax - oldMin) * (newMax - newMin);
        }
    }
}
