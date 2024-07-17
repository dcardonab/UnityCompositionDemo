/*
 * Project Name: Unity Composition Demo
 * Script Name: CameraBehavior.cs
 * Description: This script handles the rotation behavior for object, handling rotation in both local and world space.
 * Author: David Cardona
 * Date: July 17, 2024
 * License: MIT License
 */

using UnityEngine;

public class Rotate : MonoBehaviour
{
    enum Direction
    {
        Counterclockwise = -1,
        Clockwise = 1
    }
    [SerializeField] Direction _direction = Direction.Clockwise;

    enum TransformSpace
    {
        Self,
        World
    }

    [SerializeField] private TransformSpace _transformationSpace = TransformSpace.Self;
    
    [SerializeField] float _speed = 5.0f;

    void Update()
    {
        transform.Rotate(
            0,
            InputManager.Instance.MousePosition * _speed * (int)_direction * Time.deltaTime,
            0,
            _transformationSpace == TransformSpace.Self ? UnityEngine.Space.Self : UnityEngine.Space.World);
    }
}
