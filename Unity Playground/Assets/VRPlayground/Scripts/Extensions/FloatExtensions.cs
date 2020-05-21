namespace VRPlayground.Extensions
{
    public static class FloatExtensions
    {
        public static float Map(this float OldValue, float OldMin, float OldMax, float NewMin, float NewMax)
        {
            float OldRange = OldMax - OldMin;
            float NewRange = NewMax - NewMin;
            return (OldValue - OldMin) * NewRange / OldRange + NewMin;
        }
    }
}
