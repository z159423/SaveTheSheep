using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : NPC
{

    private Vector3 rotationTargetPosition;

    [field: SerializeField] public Collider coll { get; private set; }

    protected override void Start()
    {
        base.Start();

        if (coll == null)
        {
            coll = GetComponent<BoxCollider>();
        }
    }


    private void FixedUpdate()
    {
        if (rigid.velocity.sqrMagnitude > 2f)
        {
            lookRotation = Quaternion.LookRotation(new Vector3(rotationTargetPosition.x, 0, rotationTargetPosition.z) - new Vector3(transform.position.x, 0, transform.position.z));

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public override void Die()
    {
        isDie = true;

        ParticleGenerator.instance.GenenrateParticle(dieParticleKey, transform.position, 3f);

        gameObject.SetActive(false);

        StageManager.instance.StageFailed();

        SoundManager.instance.GenerateAudioAndPlaySFX("sheepDie", Vector3.zero);
    }

    override public IEnumerator Poping()
    {
        while (true)
        {
            yield return null;

            if (GameManager.instance.stageClear)
                yield return new WaitForSeconds(Random.Range(0.3f, 0.4f));
            else
                yield return new WaitForSeconds(Random.Range(0.4f, 0.6f));

            if (Util.GroundCheck(groundCheckPoints, groundCheckLayerMask))
            {
                if (GameManager.instance.stageClear)
                    rigid.AddForce(Vector3.up * popingForce * 3);
                else
                    rigid.AddForce(Vector3.up * popingForce);

                rotationTargetPosition = transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            }

            if (GameManager.gameStart)
            {

            }
        }
    }


}
