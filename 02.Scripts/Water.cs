using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.enableDrawFence)
            return;

        if(other.gameObject.TryGetComponent<NPC>(out NPC npc))
        {
            npc.Die();
        }
    }
}
