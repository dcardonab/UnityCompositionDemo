/*
 * Project Name: Unity Composition Demo
 * Script Name: NoteGridGenerator.cs
 * Description: This script generates the central tile grid based on a given number of rows and columns.
 * Author: David Cardona
 * Date: July 17, 2024
 * License: MIT License
 */

using UnityEngine;

public class NoteGridGenerator : MonoBehaviour
{
    [Header("Spacing")]
    [SerializeField] float _xOffset = -13.0f;
    [SerializeField] float _yOffset = 1.0f;
    [SerializeField] float _spacing = 2.0f;

    [Header("Grid Size")]
    [SerializeField] int _numColumns = 7;
    [SerializeField] int _numRows = 14;

    [Header("Tiles")]
    [SerializeField] Transform _noteGridContainer;
    [SerializeField] GameObject _noteTile;
    [SerializeField] Material[] _tileMaterials;
    
    void Start()
    {
        for (int x = 0; x < _numColumns; x++)
        {
            for (int y = 0; y < _numRows; y++)
            {
                GameObject tile = Instantiate(
                    _noteTile,
                    new Vector3(x * _spacing + _xOffset, y * _spacing + _yOffset, 0),
                    Quaternion.identity,
                    _noteGridContainer
                );
                int index = Random.Range(0, _tileMaterials.Length);
                NotePlayer notePlayer = tile.GetComponent<NotePlayer>();
                notePlayer.NoteIndex = index;
                notePlayer.Multiplier = Mathf.Pow(2, Random.Range(-2, 3));
                tile.GetComponent<Renderer>().material = _tileMaterials[index];
            }
        }
    }
}
