using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPC : MonoBehaviour
{
    [field: SerializeField] public bool isDie { get; protected set; } = false;
    [field: SerializeField] public Rigidbody rigid { get; protected set; }

    [field: Space]

    [field: SerializeField] public float moveSpeed { get; private set; } = 100f;
    [field: SerializeField] public float rotationSpeed { get; private set; } = 10f;
    [field: SerializeField] public float popingForce { get; private set; } = 100f;

    [field: Space]

    [field: SerializeField] protected string dieParticleKey {get; private set;}
    [field: SerializeField] protected Transform[] groundCheckPoints {get; private set;}
    [field: SerializeField] protected LayerMask groundCheckLayerMask;
    
    protected Vector3 moveDir;
    protected Quaternion lookRotation;
    protected Coroutine popingCoroutine;
    

    /// <summary>
    /// NPC 통통튀는 효과
    /// </summary>
    abstract public IEnumerator Poping();
    abstract public void Die();

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (rigid == null)
            rigid = GetComponent<Rigidbody>();
    }

    private void OnEnable() {
        popingCoroutine = StartCoroutine(Poping());
    }

    private void OnDisable()
    {
        StopCoroutine(popingCoroutine);
    }

    /// <summary>
    /// NPC 재배치
    /// </summary>
    virtual public void ResetNPC()
    {
        isDie = false;
        gameObject.SetActive(true);
        rigid.velocity = Vector3.zero;
    }

}
