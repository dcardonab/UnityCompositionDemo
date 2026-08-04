# Envelop

Envelop is an interactive audiovisual instrument built in Unity 6. It treats a composition as a space of possible outcomes rather than a fixed sequence of events: there is no timeline and no score, only a set of relationships and a performer acting within them.

Almost everything in the piece is generated procedurally. A single audio sample is the only audio asset in the project. Musical material is produced by retuning that sample through mathematical relationships across a twelve-tone temperament system, so pitch, harmony, and register all emerge from one recording. The performer launches spheres into an enclosure, and what sounds depends on where they enter, how they collide, and how long they survive; spheres are destroyed once they come to rest or leave the enclosure.

The visual layer is driven by the same audio it accompanies. Audio-reactive shading built with [Shader Graph](https://docs.unity3d.com/Packages/com.unity.shadergraph@17.0/manual/index.html) responds to sound as it is generated, and switching camera perspective also switches the audio between 2D and 3D spatialization.

🎥 [Envelop — Video Demo](https://vimeo.com/1215017253)

## Controls
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
