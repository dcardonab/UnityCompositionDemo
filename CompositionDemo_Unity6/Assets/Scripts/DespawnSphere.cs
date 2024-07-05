using UnityEngine;

public class DespawnSphere : MonoBehaviour
{
    [SerializeField] float _destroyHeight = -10.0f;
    Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HeightCheck();
        VelocityCheck();
    }

    void HeightCheck()
    {
        if (transform.position.y < _destroyHeight)
            Destroy(gameObject);
    }

    void VelocityCheck()
    {
        if (Mathf.Approximately(transform.position.y, 0.5f) && Mathf.Approximately(_rb.linearVelocity.magnitude, 0))
            Destroy(gameObject);
    }
}