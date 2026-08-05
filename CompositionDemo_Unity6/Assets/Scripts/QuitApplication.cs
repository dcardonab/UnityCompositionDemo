/*
 * Project Name: Envelop
 * Script Name: QuitApplication.cs
 * Description: This script handles the behavior for quitting the application when pressing the Escape key.
 * Author: David Cardona
 * Date: July 17, 2024
 * License: MIT License
 */

using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class QuitApplication : MonoBehaviour
{
    void Update()
    {
    #if UNITY_WEBGL
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    #elif UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Escape))
            EditorApplication.isPlaying = false;
    #else
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    #endif
    }
}
