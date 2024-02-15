using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPool
{
    Queue<GameObject> pool = new Queue<GameObject>();

    [SerializeField] private GameObject objectPrefabs;
    [SerializeField] private Transform objectParent;

    public GameObject DequeueObject(Vector3 position)
    {
        if(pool.Count > 0)
        {
            var obj = pool.Dequeue();
            obj.transform.position = position;
            obj.SetActive(true);
            return obj;
        }
        else
        {
            var obj = GameObject.Instantiate(objectPrefabs, objectParent);
            obj.transform.position = position;
            return obj;
        }
    }

    public void EnqueueObject(GameObject obj)
    {
        MonoBehaviour.print("enQueue");

        obj.SetActive(false);
        pool.Enqueue(obj);
    }

}
