namespace TransportSystems.Backend.Core.Infrastructure.Business.Extension
{
    public static class MathExtension
    {
        public static bool InRange(this float value, float min, float max)
        {
            return value >= min && value <= max;
        }
    }
}