using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPanel : MonoBehaviour
{
    [SerializeField] private float bounceForce;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem particle;

    private bool bounceReady = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<NPC>(out NPC npc))
        {
            Bounce(npc.gameObject);
        }
    }

    private void Bounce(GameObject target)
    {
        bounceReady = false;
        animator.SetTrigger("Trigger");

        var movedir = target.GetComponentInChildren<Rigidbody>().velocity;

        target.GetComponentInChildren<Rigidbody>().velocity = Vector3.zero;

        target.GetComponentInChildren<Rigidbody>().AddForce((movedir.normalized * 3) + Vector3.up * bounceForce, ForceMode.VelocityChange);

        StartCoroutine(reload());

        IEnumerator reload()
        {
            yield return new WaitForSeconds(1);

            bounceReady = true;
        }

        particle.Play();
        SoundManager.instance.PlaySFX("pop!");
    }
}
