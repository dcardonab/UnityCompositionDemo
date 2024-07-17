/*
 * Project Name: Unity Composition Demo
 * Script Name: Utilities.cs
 * Description: This script contains several mathematical utilities used across the application.
 * Author: David Cardona
 * Date: July 17, 2024
 * License: MIT License
 */


using System.Linq;

using UnityEngine;
using Random = UnityEngine.Random;

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

    /// <summary>
    /// The algorithm randomly chooses an index from an input float array that
    /// contains normalized weights.
    /// </summary>
    /// <param name="weights"></param>
    /// <returns>(int) Index corresponding to the chosen weight.</returns>
    public static int WeightedRandom(float[] weights)
    {
        if (weights == null || weights.Length == 0)
            return -1;
        
        float r = Random.value;
        
        if (Mathf.Approximately(r, 1.0f))
            return Random.Range(0, weights.Length);
        
        float sum = 0.0f;

        for (int i = 0; i < weights.Length; i++)
        {
            float w = weights[i];
            
            if (float.IsNaN(w) || w <= 0.0f)
                continue;

            sum += w;

            // Subtract Epsilon as a tolerance to make floating point comparison more robust
            if (sum >= r - Mathf.Epsilon)
                return i;
        }

        // Function should never get here since the weights are normalized
        return -1;
    }

    public static float[] GetRow(float[,] matrix, int rowIndex)
    {
        int cols = matrix.GetLength(1);
        
        // Iterate over every column at the given row and return array
        return Enumerable.Range(0, cols)
            .Select(col => matrix[rowIndex, col])
            .ToArray();
    }
}
