using UnityEngine;

public class Rotate : MonoBehaviour
{
    enum Direction : int
    {
        Counterclockwise = -1,
        Clockwise = 1
    }
    [SerializeField] Direction _direction = Direction.Clockwise;
    
    [SerializeField] float _speed = 5.0f;
    
    void Update()
    {
        transform.Rotate(0, _speed * (int)_direction * Time.deltaTime, 0);
    }
}
