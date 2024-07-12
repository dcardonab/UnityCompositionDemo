using System;
using UnityEngine;
using UnityEngine.Pool;

public class SpawnerParticleSystem : MonoBehaviour
{
    public static SpawnerParticleSystem Instance;
    
    ObjectPool<NoteTileParticleSystem> _particleSystemPool;
    [SerializeField] NoteTileParticleSystem _particleSystemPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
        
        _particleSystemPool = new ObjectPool<NoteTileParticleSystem>(
            CreatePooledParticleSystem,
            OnGetParticleSystem,
            OnReleaseParticleSystem,
            OnDestroyParticleSystem,
            false,
            200,
            1000
        );
    }

    NoteTileParticleSystem CreatePooledParticleSystem()
    {
        NoteTileParticleSystem instance = Instantiate(_particleSystemPrefab, Vector3.zero, Quaternion.identity);
        instance.Disable += ReleaseParticleSystem;
        instance.gameObject.SetActive(false);

        return instance;
    }

    void OnGetParticleSystem(NoteTileParticleSystem instance)
    {
        instance.gameObject.SetActive(true);
        instance.transform.SetParent(transform, true);
    }

    void OnReleaseParticleSystem(NoteTileParticleSystem instance)
    {
        instance.gameObject.SetActive(false);
    }

    void OnDestroyParticleSystem(NoteTileParticleSystem instance)
    {
        Destroy(instance.gameObject);
    }

    void ReleaseParticleSystem(NoteTileParticleSystem instance)
    {
        _particleSystemPool.Release(instance);
    }

    public NoteTileParticleSystem GetParticleSystem()
    {
        return _particleSystemPool.Get();
    }
}
