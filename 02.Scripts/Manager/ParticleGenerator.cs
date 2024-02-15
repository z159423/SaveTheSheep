using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleGenerator : MonoBehaviour
{
    public static ParticleGenerator instance;

    public ObjectPool dieParticlePool = new ObjectPool();

    [SerializeField] private Transform parent;

    [field: SerializeField] public ParticlePool[] particlePool { get; set; }

    private void Awake()
    {
        instance = this;
    }

    public void CoroutineHelper(ObjectPool pool, GameObject obj, float waitTime)
    {
        StartCoroutine(enqueue());

        IEnumerator enqueue()
        {
            yield return new WaitForSeconds(waitTime);

            pool.EnqueueObject(obj);
        }
    }

    public void GenenrateParticle(string key, Vector3 positon, float deleteTime)
    {
        for (int i = 0; i < particlePool.Length; i++)
        {
            if (key.Equals(particlePool[i].key))
            {
                var particle = particlePool[i].pool.DequeueObject(positon);

                StartCoroutine(enqueue());

                IEnumerator enqueue()
                {
                    yield return new WaitForSeconds(deleteTime);

                    particlePool[i].pool.EnqueueObject(particle);
                }

                break;
            }
        }
    }
}


[System.Serializable]
public class ParticlePool
{
    [field: SerializeField] public string key { get; private set; }
    [field: SerializeField] public ObjectPool pool = new ObjectPool();
}
