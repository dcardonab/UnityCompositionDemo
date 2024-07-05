using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    float[] _noteMultipliers = new float[12];
    
    Dictionary<string, int[]> _scales = new() {
        {"Ionian", new []{0, 2, 4, 5, 7, 9, 11}},
        {"Dorian", new []{0, 2, 3, 5, 7, 9, 10}},
        {"Phrygian", new []{0, 1, 3, 5, 7, 8, 10}},
        {"Lydian", new []{0, 2, 4, 6, 7, 9, 11}},
        {"Mixolydian", new []{0, 2, 4, 5, 7, 9, 10}},
        {"Aeolian", new []{0, 2, 3, 5, 7, 8, 10}},
        {"Locrian", new []{0, 1, 3, 5, 6, 8, 10}},
        {"Whole Tone", new []{0, 2, 4, 6, 8, 10}},
        {"1st Pentatonic", new []{0, 3, 5, 7, 10}},
        {"2nd Pentatonic", new []{0, 2, 4, 7, 9}},
        {"3rd Pentatonic", new []{0, 2, 5, 7, 10}},
        {"4th Pentatonic", new []{0, 3, 5, 8, 10}},
        {"5th Pentatonic", new []{0, 2, 5, 7, 9}}
    };

    public int[] CurrentScale;
    
    int _scaleIndex;
    public int ScaleIndex
    {
        get => _scaleIndex;
        set
        {
            int index = value;
            
            if (index >= _scales.Count)
                index = 0;
            else if (index < 0)
                index = _scales.Count - 1;
            
            _scaleIndex = index;
            CurrentScale = _scales.ElementAt(ScaleIndex).Value;
        }
    }
    
    bool _inCoroutine;
    public float TransitionTime = 2.0f;
    
    AudioSource _source;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
        
        InitNoteMultipliers();
    }
    
    void Start()
    {
        _source = GetComponent<AudioSource>();
        ScaleIndex = 1;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
            ScaleIndex++;
        
        if (Input.GetKeyDown(KeyCode.S))
            ScaleIndex--;

        if (Input.GetKeyDown(KeyCode.M) && !_inCoroutine)
        {
            _inCoroutine = true;
            StartCoroutine(TransitionPlaybackMode());
        }
    }

    void InitNoteMultipliers()
    {
        float twelfthRoot = Mathf.Pow(2.0f, 1.0f / 12.0f);

        for (int i = 0; i < _noteMultipliers.Length; i++)
            _noteMultipliers[i] = Mathf.Pow(twelfthRoot, i);
    }

    public void PlayNote(int note, float multiplier)
    {
        _source.pitch = _noteMultipliers[CurrentScale[note % CurrentScale.Length]] * multiplier;
        _source.PlayOneShot(_source.clip);
    }

    IEnumerator TransitionPlaybackMode()
    {
        float initialValue = _source.spatialBlend;
        float destinationValue = 1.0f - _source.spatialBlend;
        
        float elapsedTime = 0.0f;

        while (elapsedTime <= TransitionTime)
        {
            _source.spatialBlend = Mathf.Lerp(initialValue, destinationValue, elapsedTime / TransitionTime);
            
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }

        _source.spatialBlend = Mathf.Round(_source.spatialBlend);

        _inCoroutine = false;
    }
}
