using UnityEngine;

public class LightIntensityControl : MonoBehaviour
{
    public enum Channel { Left, Right }
    public Channel ControlChannel = Channel.Left;
    
    Light _light;
    [Range(0.0f, 10.0f)] [SerializeField]  float _intensityMin = 0.0f;
    [Range(2000.0f, 20000.0f)] [SerializeField]  float _intensityMax = 20000.0f;
    [Range(0.5f, 20.0f)] [SerializeField] private float _interpolationExponent = 1.0f;

    void Start()
    {
        _light = GetComponent<Light>();
    }

    private void Update()
    {
        _light.intensity = Utilities.Map(
            ControlChannel == Channel.Left
                ? AudioManager.Instance.Left.Loudness
                : AudioManager.Instance.Right.Loudness,
            -80.0f, 0.0f,
            _intensityMin, _intensityMax,
            _interpolationExponent, 
            true
        );
    }
}
