/*
 * Project Name: Unity Composition Demo
 * Script Name: QuitApplication.cs
 * Description: This script handles the behavior for quitting the application when pressing the Escape key.
 * Author: David Cardona
 * Date: July 17, 2024
 * License: MIT License
 */

using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class QuitApplication : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            QuitGame();
    }

    void QuitGame()
    {
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
