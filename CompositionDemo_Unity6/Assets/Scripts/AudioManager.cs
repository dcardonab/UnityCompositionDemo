/*
 * Project Name: Unity Composition Demo
 * Script Name: AudioManager.cs
 * Description: This script contains the main audio behavior, including scale selection syntax, generation of a transition matrix for running first order Markov chains, pooling of audio sources, and note playback behavior.
 * Author: David Cardona
 * Date: July 17, 2024
 * License: MIT License
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    private float[] _noteMultipliers = new float[12];

    [Header("Music Generation")]
    public int[] CurrentScale;
    
    Constants.Scale _selectedScale;
    Constants.Scale SelectedScale
    {
        get => _selectedScale;
        set
        {
            Constants.Scale index = value;
            
            if ((int)index >= Constants.Scales.Count)
                index = 0;
            else if ((int)index < 0)
                index = Constants.Scales.ElementAt(Constants.Scales.Count - 1).Key;
            
            _selectedScale = index;
            CurrentScale = Constants.Scales[_selectedScale];
            
            if (_noteGenerationMode == Constants.NoteGenerationMode.Markov)
                // Generate transition matrix every time the scale is updated
                _transitionMatrix = GenerateTransitionMatrix();
        }
    }

    float[,] _transitionMatrix;
    int _currentNote;
    [SerializeField] Constants.NoteGenerationMode _noteGenerationMode = Constants.NoteGenerationMode.Markov;
    
    [Header("Interpolation")]
    public float TransitionTime = 2.0f;
    bool _inTransition;

    [Header("AudioSource Settings")]
    [SerializeField] AudioClip _audioClip;
    [SerializeField] AudioMixerGroup _mixerGroup;
    
    [Header("AudioSource Pool")]
    public ObjectPool<AudioSource> AudioSourcePool;
    List<AudioSource> _activeAudioSources = new List<AudioSource>();
    [SerializeField] int MaxAudioSources = 1000;
    
    [SerializeField] AudioSource _audioSourcePrefab;
    
    [Header("Spectrum Data")]
    [SerializeField] int _numFftBands = 16;
    float[] _fftBandRangeFrequencies;
    [SerializeField] int _vectorSize = 1024;
    [SerializeField] FFTWindow _fftWindowType = FFTWindow.BlackmanHarris;
    
    public AudioChannelData Left; 
    public AudioChannelData Right; 
    
    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        AudioSourcePool = new ObjectPool<AudioSource>(
            CreatePooledAudioSource,
            OnGetAudioSource,
            OnReleaseAudioSource,
            OnDestroyAudioSource,
            false,
            MaxAudioSources,
            MaxAudioSources);
        
        InitNoteMultipliers();
        InitFftBandRangeFrequencies();
        
        SelectedScale = Constants.Scale.Dorian;
        
        // Choose first note at random
        _currentNote = Random.Range(0, _transitionMatrix.GetLength(0));

        Left = new AudioChannelData(_vectorSize, 0, _fftWindowType, _fftBandRangeFrequencies);
        Right = new AudioChannelData(_vectorSize, 1, _fftWindowType, _fftBandRangeFrequencies);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
            SelectedScale++;
        
        if (Input.GetKeyDown(KeyCode.S))
            SelectedScale--;

        if (Input.GetKeyDown(KeyCode.M) && !_inTransition)
        {
            _inTransition = true;
            foreach (AudioSource source in _activeAudioSources)
            {
                StartCoroutine(TransitionPlaybackMode(source));
            }
        }
    }

    /********************* INITS *********************/
    void InitNoteMultipliers()
    {
        // The note multipliers are computed using the 12th root of the
        // scale step.
        float twelfthRoot = Mathf.Pow(2.0f, 1.0f / 12.0f);

        _noteMultipliers = _noteMultipliers
            .Select((_, i) => Mathf.Pow(twelfthRoot, i))
            .ToArray();
    }

    void InitFftBandRangeFrequencies()
    {
        float freqMin = 20.0f;
        float nyquistFreq = AudioSettings.outputSampleRate * 0.5f;
        
        // Ratio for each step in logarithmic scaling
        float ratio = Mathf.Pow(nyquistFreq / freqMin, 1.0f / _numFftBands);

        _fftBandRangeFrequencies = new float[_numFftBands + 1];
        _fftBandRangeFrequencies[0] = freqMin;

        _fftBandRangeFrequencies = _fftBandRangeFrequencies
            .Select(
                (freq, i) => i == 0
                    ? freq
                    : _fftBandRangeFrequencies[i - 1] * ratio)
            .ToArray();
    }

    float[,] GenerateTransitionMatrix()
    {
        int rows = CurrentScale.Length;
        int cols = CurrentScale.Length;
        
        float[,] transitionMatrix = new float[rows, cols];

        for (int r = 0; r < rows; r++)
        {
            // Compute sum for normalizing probabilities to prevent normalizing
            // during weighted random function when choosing notes.
            float sum = 0;
            for (int c = 0; c < cols; c++)
            {
                transitionMatrix[r, c] = Random.value;
                sum += transitionMatrix[r, c];
            }

            // Normalize row.
            if (sum > 0)
            {
                for (int c = 0; c < cols; c++)
                   transitionMatrix[r, c] /= sum;
            }
            
            // If the sum is 0, assign uniform distribution
            else
            {
                for (int c = 0; c < cols; c++)
                    transitionMatrix[r, c] = 1.0f / cols;
            }
        }
        
        return transitionMatrix;
    }
    
    /********************* METHODS *********************/
    public IEnumerator PlayNote(NotePlayer tilePlayer, int sourceIndex, int note, float multiplier, Collider other)
    {
        AudioSource source = tilePlayer.Sources[sourceIndex];

        // Control loudness based on the velocity of the colliding object
        source.volume = Utilities.Map(
            other.attachedRigidbody.linearVelocity.magnitude,
            0.1f, 100.0f,
            0.1f, 1.0f,
            1.5f,
            true
        );
        
        if (_noteGenerationMode == Constants.NoteGenerationMode.Markov)
        {
            // Assign current note
            source.pitch = _noteMultipliers[CurrentScale[_currentNote]] * multiplier;
            
            // Retrieve note for next iteration
            _currentNote = ChooseNextNote();
        }
        else
            source.pitch = _noteMultipliers[CurrentScale[note % CurrentScale.Length]] * multiplier;
        
        source.PlayOneShot(source.clip);
        
        yield return new WaitWhile(() => source.isPlaying || _inTransition);

        tilePlayer.RemoveSource(sourceIndex);
    }

    int ChooseNextNote()
    {
        // Retrieve probabilities corresponding to the next note
        float[] nextNoteProbabilities = Utilities.GetRow(_transitionMatrix, _currentNote);
        
        // Pick next note. Next note corresponds to index in the scale.
        return Utilities.WeightedRandom(nextNoteProbabilities);
    }
    
    IEnumerator TransitionPlaybackMode(AudioSource source)
    {
        float initialValue = source.spatialBlend;
        float destinationValue = 1.0f - source.spatialBlend;
        
        float elapsedTime = 0.0f;
        while (elapsedTime <= TransitionTime)
        {
            source.spatialBlend = Mathf.Lerp(initialValue, destinationValue, elapsedTime / TransitionTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    
        // Set final value and mark transition as completed.
        source.spatialBlend = Mathf.Round(source.spatialBlend);
        _inTransition = false;
    }

    /********************* POOL METHODS *********************/
    AudioSource CreatePooledAudioSource()
    {
        AudioSource instance = Instantiate(_audioSourcePrefab, Vector3.zero, Quaternion.identity);
        instance.gameObject.SetActive(false);
        return instance;
    }
    
    void OnGetAudioSource(AudioSource instance)
    {
        instance.gameObject.SetActive(true);
        instance.transform.SetParent(transform, true);
        _activeAudioSources.Add(instance);
    }

    void OnReleaseAudioSource(AudioSource instance)
    {
        instance.gameObject.SetActive(false);
    }

    void OnDestroyAudioSource(AudioSource instance)
    {
        Destroy(instance.gameObject);
    }

    public void ReleaseAudioSource(AudioSource instance)
    {
        AudioSourcePool.Release(instance);
        _activeAudioSources.Remove(instance);
    }
}
