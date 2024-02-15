using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gunner : NPC
{
    [field: SerializeField] public bool gunReady { get; private set; } = true;
    [field: SerializeField] public float gunReloadingTime { get; private set; } = 1f;
    [field: SerializeField] public float gunFireDistance { get; private set; } = 1f;
    [field: SerializeField] public float knockbackForce { get; private set; } = 400;


    [field: SerializeField] public ParticleSystem gunFireParticle { get; private set; }
    [field: SerializeField] public Transform rayStart { get; private set; }
    [field: SerializeField] public Transform firePos { get; private set; }
    [field: SerializeField] public Transform fireDir { get; private set; }
    [field: SerializeField] public LayerMask obstacleMask { get; private set; }
    [field: SerializeField] public Light gunFireLight { get; private set; }




    private Hunter currentTarget = null;

    private void FixedUpdate()
    {
        if (!GameManager.gameStart)
            return;

        if (currentTarget != null)
        {
            if (currentTarget.isDie)
                currentTarget = null;
        }

        if (currentTarget != null)
        {


            if (Vector3.Distance(rayStart.position, currentTarget.transform.position + Vector3.up) < gunFireDistance && gunReady)
            {
                if (!Physics.Raycast(rayStart.position, (currentTarget.transform.position - rayStart.position).normalized, 1, obstacleMask))
                    GunShot();
            }

            if (rigid.velocity.sqrMagnitude > 4f)
            {
                lookRotation = Quaternion.LookRotation(new Vector3(currentTarget.transform.position.x, 0, currentTarget.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z));

                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }


        }
        else
            FindNextTarget();
    }

    public override void Die()
    {
        throw new System.NotImplementedException();
    }

    override public IEnumerator Poping()
    {
        while (true)
        {
            yield return new WaitForSeconds(.5f);

            print(0);

            if (Util.GroundCheck(groundCheckPoints, groundCheckLayerMask))
            {
                print(1);
                if (GameManager.gameStart && currentTarget != null)
                {
                    print(1);
                    rigid.AddForce(Vector3.up * popingForce);

                    moveDir = (currentTarget.transform.position - transform.position).normalized;
                    rigid.AddForce(moveDir * moveSpeed);
                }
                else if (!GameManager.instance.stageClear)
                {
                    print(2);
                    rigid.AddForce(Vector3.up * popingForce);
                }
                else
                {
                    print(3);
                    rigid.AddForce(Vector3.up * popingForce);
                }
            }
        }
    }

    public void GunShot()
    {
        StartCoroutine(fire());

        IEnumerator fire()
        {
            gunReady = false;

            // 총 발사
            gunFireParticle.Play();
            currentTarget.Die();

            rigid.AddForce((-transform.forward * 1.5f) * knockbackForce);

            gunFireLight.range = 3f;
            StartCoroutine(GunFireLight());

            yield return new WaitForSeconds(gunReloadingTime);

            gunReady = true;
        }

        IEnumerator GunFireLight()
        {
            while (gunFireLight.range > 0)
            {
                gunFireLight.range -= 0.1f;
                yield return null;
            }
        }

        SoundManager.instance.PlaySFX("shotgun");
    }

    public void FindNextTarget()
    {
        currentTarget = StageManager.instance.GetClosestWolf(transform.position);
    }

    public override void ResetNPC()
    {
        base.ResetNPC();

        gunReady = true;
    }
}
