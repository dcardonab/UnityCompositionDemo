/*
 * Project Name: Unity Composition Demo
 * Script Name: SpawnerPlayerSphere.cs
 * Description: This script handles the generation of spheres when pressing the arrow keys.
 * Author: David Cardona
 * Date: July 17, 2024
 * License: MIT License
 */

using UnityEngine;

public class SpawnerPlayerSphere : MonoBehaviour
{
    [SerializeField] GameObject _playerSphere;

    [SerializeField] Transform[] _spawnPositions;
    [SerializeField] Material[] _sphereMaterials;
    int _colorID;

    [SerializeField] float _horizontalOffset = 13.0f;
    [SerializeField] float _verticalOffset = 4.0f;

    void Start()
    {
        _colorID = Shader.PropertyToID("_BaseColor");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Vector3 position = _spawnPositions[0].position;
            position.x += Random.Range(-_horizontalOffset, _horizontalOffset);
            position.y += Random.Range(-_verticalOffset, _verticalOffset);
            SpawnSphere(position, Vector3.back);
        }
        
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Vector3 position = _spawnPositions[1].position;
            position.x += Random.Range(-_horizontalOffset, _horizontalOffset);
            position.y += Random.Range(-_verticalOffset, _verticalOffset);
            SpawnSphere(position, Vector3.forward);
        }
        
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Vector3 position = _spawnPositions[2].position;
            position.z += Random.Range(-_horizontalOffset, _horizontalOffset);
            position.y += Random.Range(-_verticalOffset, _verticalOffset);
            SpawnSphere(position, Vector3.right);
        }
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Vector3 position = _spawnPositions[3].position;
            position.z += Random.Range(-_horizontalOffset, _horizontalOffset);
            position.y += Random.Range(-_verticalOffset, _verticalOffset);
            SpawnSphere(position, Vector3.left);
        }
    }

    void SpawnSphere(Vector3 position, Vector3 direction)
    {
        GameObject playerSphere = Instantiate(_playerSphere, position, Quaternion.identity, transform);

        Material material = _sphereMaterials[Random.Range(0, _sphereMaterials.Length)];
        playerSphere.GetComponent<Renderer>().material = material;
        
        TrailRenderer trailRenderer = playerSphere.GetComponent<TrailRenderer>(); 
        trailRenderer.startColor = material.GetColor(_colorID);
        
        float force = Random.Range(10.0f, 50.0f);
        playerSphere.GetComponent<Rigidbody>().AddForce(direction * force, ForceMode.Impulse);
    }
}
