/*
 * Project Name: Unity Composition Demo
 * Script Name: DissolveManager.cs
 * Description: This script handles the Dissolve effect based on keyboard input.
 * Author: David Cardona
 * Date: July 17, 2024
 * License: MIT License
 */

using System.Collections;

using UnityEngine;

public class DissolveManager : MonoBehaviour
{
    bool _inCoroutine;
    int _dissolveAmountID;
    [SerializeField] float _transitionTime = 5.0f;
    Material _material;

    void Start()
    {
        _material = GetComponent<Renderer>().material;
        _dissolveAmountID = Shader.PropertyToID("_DissolveAmount");
        _inCoroutine = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D) && !_inCoroutine)
        {
            StartCoroutine(DissolveTransition());
        }
    }

    IEnumerator DissolveTransition()
    {
        _inCoroutine = true;

        float startValue = _material.GetFloat(_dissolveAmountID);
        float endValue = 1.0f - startValue;

        float elapsedTime = 0.0f;
        while (elapsedTime <= _transitionTime)
        {
            float value = Mathf.Lerp(startValue, endValue, elapsedTime / _transitionTime);
            _material.SetFloat(_dissolveAmountID, value);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        _material.SetFloat(_dissolveAmountID, endValue);

        _inCoroutine = false;
    }
}
