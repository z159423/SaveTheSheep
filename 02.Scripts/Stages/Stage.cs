using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    [SerializeField] private Sheep[] sheeps;
    [SerializeField] private Hunter[] hunters;
    [SerializeField] private Gunner[] gunners;

    [SerializeField] private List<objectOriginTransform> allMoveableObjects = new List<objectOriginTransform>();

    [SerializeField] public float fenceGagueThisStage = 30f;


    //public bool openedStage = false;
    public int score { get; private set; }

    [field: SerializeField] public StageData stageData { get; set; }


    private void Start()
    {
        sheeps = GetComponentsInChildren<Sheep>();
        hunters = GetComponentsInChildren<Hunter>();
        gunners = GetComponentsInChildren<Gunner>();

        var objects = GetComponentsInChildren<Rigidbody>();

        for (int i = 0; i < objects.Length; i++)
        {
            objectOriginTransform objectOriginTransform = new objectOriginTransform(objects[i].transform, objects[i].transform.position, objects[i].transform.rotation);
            allMoveableObjects.Add(objectOriginTransform);
        }
    }

    public void LoadStage()
    {
        for (int i = 0; i < sheeps.Length; i++)
        {
            sheeps[i].ResetNPC();
        }

        for (int i = 0; i < hunters.Length; i++)
        {
            hunters[i].ResetNPC();
        }

        for (int i = 0; i < gunners.Length; i++)
        {
            gunners[i].ResetNPC();
        }

        for (int i = 0; i < allMoveableObjects.Count; i++)
        {
            allMoveableObjects[i].ResetObject();
        }

        gameObject.SetActive(true);
    }

    public void CloseStage()
    {
        gameObject.SetActive(false);
    }


    public Sheep GetClosestSheep(Vector3 position)
    {
        float closestDist = 1000000;
        Sheep closestSheep = null;

        for (int i = 0; i < sheeps.Length; i++)
        {
            if (!sheeps[i].isDie)
            {
                var dist = Vector3.Distance(position, sheeps[i].transform.position);

                if (closestDist > dist)
                {
                    closestDist = dist;
                    closestSheep = sheeps[i];
                }
            }
        }

        return closestSheep;

    }

    public Hunter GetClosestWolf(Vector3 position)
    {
        float closestDist = 1000000;
        Hunter closestHunter = null;

        for (int i = 0; i < hunters.Length; i++)
        {
            if (!hunters[i].isDie)
            {
                var dist = Vector3.Distance(position, hunters[i].transform.position);

                if (closestDist > dist)
                {
                    closestDist = dist;
                    closestHunter = hunters[i];
                }
            }
        }

        return closestHunter;
    }

    class objectOriginTransform
    {
        public Transform objectTransform;
        public Vector3 originPostion;
        public Quaternion originRotation;

        public objectOriginTransform(Transform trans, Vector3 position, Quaternion rotation)
        {
            objectTransform = trans;
            originPostion = position;
            originRotation = rotation;
        }

        public void ResetObject()
        {
            objectTransform.transform.position = originPostion;
            objectTransform.transform.rotation = originRotation;

            if (objectTransform.TryGetComponent<Rigidbody>(out Rigidbody rigid))
            {
                rigid.velocity = Vector3.zero;
            }
        }
    }
}
