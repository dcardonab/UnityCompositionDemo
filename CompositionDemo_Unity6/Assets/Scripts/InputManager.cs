/*
 * Project Name: Unity Composition Demo
 * Script Name: InputManager.cs
 * Description: This script handles retrieving and storing the Mouse input for usage in the Rotate.cs script.
 * Author: David Cardona
 * Date: July 17, 2024
 * License: MIT License
 */

using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    [HideInInspector] public float MousePosition;
    [SerializeField] float _mouseSensitivity = 5.0f;
    
    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        
        // Confine cursor to the screen
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }
    
    void Update()
    {
        MousePosition = Utilities.Map(
            Input.mousePosition.x,
            0, Screen.width,
            -1.0f, 1.0f,
            clamp: true
        ) * _mouseSensitivity;
    }
}
