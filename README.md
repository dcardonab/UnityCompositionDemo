# Unity Composition Demo

This project demonstrates an immersive audiovisual instrument implemented in Unity. The majority of the project relies on procedural generation, with the exception of a single audio sample, which is used as the only audio clip. Musical generation is based on mathematical relationships to playback this sample in different tonal centers that adhere to a twelve-tone temperament system. Additionally, the project showcases audio-reactive graphics that leverage the [Shader Graph](https://docs.unity3d.com/Packages/com.unity.shadergraph@17.0/manual/index.html) Unity tool.

## Requirements

This project is making use of [Adaptive Probe Volumes](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@17.0/manual/probevolumes-concept.html), a feature introduced in Unity 6. The project was developed using version 6000.0.4f1, and is unlikely to work properly in previous Unity versions. However, it should work smoothly with newer versions of Unity 6.

## Opening the Project
* Download the Unity 6 preview from the [Unity Download Archive](https://unity.com/releases/editor/archive).
* Clone this repository.
* In the Unity Hub, click on the **Add** button and open the root folder of the Unity project, titled *CompositionDemo_Unity6*.
* Once Unity opens, navigate to **Assets > Scenes** in the Project pane, and double click on the *CompositionDemo* scene.

## Player Controls
* Arrow Keys
    * Spheres launched from different directions
    * Spheres are destroyed when still or when outside enclosure
* W / S
    * Change musical scale used
* M
    * Change camera perspective
    * Switches between 2D and 3D audio
* D
    * Dissolve effect to hide enclosure
* Mouse X-Axis
    * Rotation rate
