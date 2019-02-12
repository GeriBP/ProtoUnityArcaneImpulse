using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageMove : MonoBehaviour
{
    public float actionRange, moveSpeed, shootRange, fireRate, bulletSpeed;
    public GameObject gunP, fireball;
    public float maxHp, damageFromImpact;

    private float currHp;
    public LayerMask mask, maskFloor;
    private Transform target;
    private Rigidbody rb;
    private RaycastHit hit;
    private bool chase = false;
    private bool grounded = false;
    private bool canShoot = true;
    private float groundDist, distance;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        groundDist = transform.localScale.y + 0.01f;
        target = FindObjectOfType<Move>().transform;
        currHp = maxHp;
    }


    void Update()
    {
        grounded = IsGrounded();
        Vector3 dir = (target.position - transform.position).normalized;
        distance = Vector3.Distance(target.position, transform.position);
        if (grounded && Physics.Raycast(transform.position, dir, out hit, actionRange, mask))
        {
            if (hit.transform.CompareTag("Player"))
            {
                chase = true;
            }
            else
            {
                chase = false;
            }
        }

        if (chase && grounded)
        {
            if (distance < shootRange)
            {
                if (canShoot)
                {
                    StartCoroutine(HandleShoot());
                }
            }
            else
            {
                Vector3 vel = dir * moveSpeed;
                vel.y = rb.velocity.y;
                rb.velocity = vel;
            }
        }
        else if (!chase && grounded)
        {
            rb.velocity = Vector3.zero;
        }
        dir.y = 0.0f;
        transform.forward = dir;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundDist, maskFloor);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.CompareTag("Object"))
        {
            ChangeHp(-damageFromImpact);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FireBall"))
        {
            ChangeHp(-damageFromImpact);
            Destroy(other.gameObject);
        }
    }

    void ChangeHp(float d)
    {
        currHp = Mathf.Clamp(currHp + d, 0f, maxHp);
        if (currHp <= 0f)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator HandleShoot()
    {
        canShoot = false;
        GameObject b = Instantiate(fireball, gunP.transform.position, Quaternion.identity);
        b.GetComponent<Rigidbody>().AddForce((target.position - transform.position).normalized * bulletSpeed, ForceMode.Impulse);
        Destroy(b, 3f);
        yield return new WaitForSeconds(fireRate);
        canShoot = true;
    }
}
