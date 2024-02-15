using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunter : NPC
{
    [Space]

    [SerializeField] private Sheep currentTarget = null;
    private Sheep originCurrentTarget = null;
    [SerializeField] private Collider coll;
    [SerializeField] private LayerMask obstacleLayerMask;

    private void Start()
    {
        if (rigid == null)
        {
            rigid = GetComponent<Rigidbody>();
        }

        if (coll == null)
        {
            coll = GetComponent<BoxCollider>();
        }

        originCurrentTarget = currentTarget;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.enableDrawFence)
            return;



        if (currentTarget == null)
        {
            FindNextTarget();
        }
        else
        {
            if (currentTarget.isDie)
                currentTarget = null;
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.enableDrawFence)
            return;

        //rigid.velocity = (moveDir * moveForce);

        //print(rigid.velocity.sqrMagnitude);

        if (rigid.velocity.sqrMagnitude > 4f && currentTarget != null)
        {
            lookRotation = Quaternion.LookRotation(new Vector3(currentTarget.transform.position.x, 0, currentTarget.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z));

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public override void Die()
    {
        ParticleGenerator.instance.GenenrateParticle(dieParticleKey, transform.position, 3f);

        isDie = true;
        gameObject.SetActive(false);
    }

    public void FindNextTarget()
    {
        currentTarget = StageManager.instance.GetClosestSheep(transform.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (GameManager.enableDrawFence)
            return;

        if (collision.gameObject.CompareTag("Sheep"))
        {
            if (collision.gameObject.TryGetComponent<Sheep>(out Sheep sheep))
            {
                if (!Physics.Raycast(coll.bounds.center, (sheep.coll.bounds.center - coll.bounds.center).normalized, Vector3.Distance(coll.bounds.center, sheep.coll.bounds.center), obstacleLayerMask))
                    sheep.Die();
            }
        }

        // if (collision.gameObject.CompareTag("Obstacle"))
        // {
        //     rigid.AddForce(-(rigid.velocity.normalized) * 500f);
        // }
    }

    override public IEnumerator Poping()
    {
        while (true)
        {
            yield return new WaitForSeconds(.5f);

            if (Util.GroundCheck(groundCheckPoints, groundCheckLayerMask))
            {
                if (!GameManager.enableDrawFence && currentTarget != null)
                {
                    rigid.AddForce(Vector3.up * popingForce);

                    moveDir = (currentTarget.transform.position - transform.position).normalized;
                    rigid.AddForce(moveDir * moveSpeed);
                }
                else if (!GameManager.instance.stageClear)
                {
                    rigid.AddForce(Vector3.up * popingForce);
                }
            }
        }
    }

    public override void ResetNPC()
    {
        base.ResetNPC();

        currentTarget = originCurrentTarget;
    }
}
