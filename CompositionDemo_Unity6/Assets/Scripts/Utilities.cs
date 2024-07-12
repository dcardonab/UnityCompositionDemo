using System.Linq;

using UnityEngine;

public static class Utilities
{
    public static float Map(float value, float domainMin, float domainMax, float rangeMin, float rangeMax, float exponent = 1.0f, bool clamp = false)
    {
        if (Mathf.Approximately(domainMin, domainMax))
            return rangeMin;
        
        if (clamp)
            value = Mathf.Clamp(value, domainMin, domainMax);

        // Apply offset to value and domain to avoid negative values in exponential interpolation
        if (domainMin < 0)
        {
            float offset = Mathf.Abs(domainMin);
            value += offset;
            domainMin += offset;
            domainMax += offset;
        }
        
        // Calculate fraction of the value within the domain.
        float fraction = (value - domainMin) / (domainMax - domainMin);

        // If no exponent is provided, perform linear interpolation.
        if (Mathf.Approximately(exponent, 1.0f))
        {
            return rangeMin + fraction * (rangeMax - rangeMin);
        }
        
        // Ensure fraction is not negative
        fraction = Mathf.Max(fraction, 0.0f);

        float result = Mathf.Pow(fraction, exponent);

        switch (result)
        {
            case Mathf.Infinity:
                return rangeMax;
            case float.NaN:
                return rangeMin;
            default:
                return rangeMin + result * (rangeMax - rangeMin);
        }
    }
    
    public static float GetRootMeanSquared(float[] samples)
    {
        // In LINQ, the Select() method uses a lambda to transform each element.
        // Squared to calculate the root mean squared.
        float sum = samples.Select(sample => sample * sample).Sum();

        float rms = Mathf.Sqrt(sum / samples.Length); 

        // 10 is assumed as a reference value for computing dB
        return 20.0f * Mathf.Log10(rms * 10);
    }
}
