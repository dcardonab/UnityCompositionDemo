using System;
using System.Collections;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    enum VisualizationMode { Overhead = 1, Center = -1 }
    VisualizationMode _visualizationMode = VisualizationMode.Overhead;

    [SerializeField] Transform _cameraOffsetPoint;
    Camera _cam;
    [SerializeField] float _centerFOV = 80.0f;
    [SerializeField] float _overheadFOV = 60.0f;
    
    bool _inCoroutine;

    void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && !_inCoroutine)
        {
            _inCoroutine = true;
            if (_visualizationMode == VisualizationMode.Overhead)
                StartCoroutine(TransitionToCenterVisualizationMode());
            else
                StartCoroutine(TransitionToOverheadVisualizationMode());
        }
    }

    void LateUpdate()
    {
        if (!_inCoroutine)
        {
            if (_visualizationMode == VisualizationMode.Overhead)
            {
                transform.LookAt(transform.parent); 
            }
            else
            {
                transform.LookAt(_cameraOffsetPoint);
            }
        }
    }

    IEnumerator TransitionToCenterVisualizationMode()
    {
        Vector3 initPosition = transform.position;
        Quaternion initRotation = transform.rotation;

        float elapsedTime = 0.0f;

        while (elapsedTime <= AudioManager.Instance.TransitionTime)
        {
            float currentTransitionPoint = elapsedTime / AudioManager.Instance.TransitionTime;
            
            transform.position = Vector3.Lerp(initPosition, transform.parent.position, currentTransitionPoint);
            transform.rotation = Quaternion.Lerp(initRotation, _cameraOffsetPoint.rotation * Quaternion.Euler(0, 180, 0), currentTransitionPoint);
            _cam.fieldOfView = Mathf.Lerp(_overheadFOV, _centerFOV, currentTransitionPoint);
            
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }

        transform.position = transform.parent.position;
        transform.LookAt(_cameraOffsetPoint);
        _cam.fieldOfView = _centerFOV;

        _visualizationMode = VisualizationMode.Center;

        _inCoroutine = false;
    }
    
    IEnumerator TransitionToOverheadVisualizationMode()
    {
        Vector3 initPosition = transform.position;
        Quaternion initRotation = transform.rotation;

        float elapsedTime = 0.0f;

        while (elapsedTime <= AudioManager.Instance.TransitionTime)
        {
            float currentTransitionPoint = elapsedTime / AudioManager.Instance.TransitionTime;
            
            transform.position = Vector3.Lerp(initPosition, _cameraOffsetPoint.position, currentTransitionPoint);
            transform.rotation = Quaternion.Lerp(initRotation, _cameraOffsetPoint.rotation, currentTransitionPoint);
            _cam.fieldOfView = Mathf.Lerp(_centerFOV, _overheadFOV, currentTransitionPoint);
            
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }

        transform.position = _cameraOffsetPoint.position;
        transform.rotation = _cameraOffsetPoint.rotation;
        _cam.fieldOfView = _overheadFOV;
        
        _visualizationMode = VisualizationMode.Overhead;

        _inCoroutine = false;
    }
}
