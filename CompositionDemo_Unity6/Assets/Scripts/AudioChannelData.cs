/*
 * Project Name: Unity Composition Demo
 * Script Name: AudioChannelData.cs
 * Description: This script defines a struct to store audio channel data.
 * Author: David Cardona
 * Date: July 17, 2024
 * License: MIT License
 */

using System.Linq;

using UnityEngine;

public struct AudioChannelData
{
    /********************* FIELDS *********************/
    int _channel;
    FFTWindow _fftWindowType;
    float[] _loudnessSamples;
    float[] _spectrumSamples;
    int[] _freqBandIndices;
    float[] _spectrumData;
    
    /********************* CONSTRUCTOR *********************/
    public AudioChannelData(int vectorSize, int channelNum, FFTWindow fftWindowType, float[] fftBandRangeFrequencies)
    { 
        _channel = channelNum;
        _fftWindowType = fftWindowType;
        _loudnessSamples = new float[vectorSize];
        _spectrumSamples = new float[vectorSize];

        _freqBandIndices = new int[fftBandRangeFrequencies.Length];

        int sampleRate = AudioSettings.outputSampleRate;
        int spectrumLength = _spectrumSamples.Length;
        
        // We multiply by 0.5 given the Nyquist-Shannon Theorem
        _freqBandIndices = fftBandRangeFrequencies
            .Select(
                frequency => Mathf.FloorToInt(
                    frequency / sampleRate * 0.5f * spectrumLength
                )
            ).ToArray();

        // We subtract one since the indices outline the bounds of each band
        _spectrumData = new float[_freqBandIndices.Length - 1];
    }
    
    /********************* GETTER PROPERTIES *********************/
    public float Loudness
    {
        get
        {
            AudioListener.GetOutputData(_loudnessSamples, _channel);
            return Utilities.GetRootMeanSquared(_loudnessSamples);
        }
    }
    
    public float[] SpectrumData
    {
        get
        {
            AudioListener.GetSpectrumData(_spectrumSamples, _channel, _fftWindowType);
            
            for (int i = 0; i < _freqBandIndices.Length - 1; i++)
                _spectrumData[i] = GetFrequencyBand(_freqBandIndices[i], _freqBandIndices[i + 1]);

            return _spectrumData;
        }
    }

    /********************* METHODS *********************/
    float GetFrequencyBand(int lowIndex, int highIndex)
    {
        // Return the Average of the specified band.
        // In LINQ, Skip() specifies the first index
        // and Take() specifies the number of iterations.
        return _spectrumSamples.Skip(lowIndex).Take(highIndex - lowIndex).Average();
    }
}
