using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour
{
    [field: SerializeField] public float pushForce { get; private set; } = 50f;

    [Space]

    [field: SerializeField] private Transform pushDirStart;
    [field: SerializeField] private Transform pushDirEnd;

    private List<Rigidbody> pushRigid = new List<Rigidbody>();

    private void FixedUpdate()
    {
        try
        {
            for (int i = 0; i < pushRigid.Count; i++)
            {
                if (pushRigid[i] != null)
                    pushRigid[i].AddForce((pushDirEnd.position - pushDirStart.position).normalized * pushForce, ForceMode.VelocityChange);
            }
        }
        catch (System.Exception e)
        {
            pushRigid.Clear();
        }

        pushRigid.Clear();
    }

    private void OnTriggerStay(Collider other)
    {
        if (GameManager.enableDrawFence)
            return;

        if (other.gameObject.CompareTag("Sheep"))
        {
            if (!pushRigid.Contains(other.GetComponent<Rigidbody>()))
            {
                pushRigid.Add(other.GetComponent<Rigidbody>());
                //other.GetComponent<Rigidbody>().AddForce((pushDirEnd.position - pushDirStart.position).normalized * pushForce);
            }
        }

        if (other.gameObject.CompareTag("Obstacle"))
        {
            if (other.GetComponentInParent<Rigidbody>() != null)
            {
                if (!pushRigid.Contains(other.GetComponentInParent<Rigidbody>()))
                {
                    pushRigid.Add(other.GetComponentInParent<Rigidbody>());
                }
                //other.GetComponentInParent<Rigidbody>().AddForce((pushDirEnd.position - pushDirStart.position).normalized * pushForce);
            }
        }
    }
}
