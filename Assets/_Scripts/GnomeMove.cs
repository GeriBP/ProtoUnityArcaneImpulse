using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GnomeMove : MonoBehaviour
{
    public float actionRange, moveSpeed, explodeDmg;
    public GameObject explosion;

    public LayerMask mask, maskFloor;
    private Transform target;
    private Rigidbody rb;
    private RaycastHit hit;
    private bool chase = false;
    private bool grounded = false;
    private float spawnT, groundDist;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        groundDist = (transform.localScale.y / 2.0f) + 0.01f;
        target = FindObjectOfType<Move>().transform;
        spawnT = Time.time;
    }

    
    void Update()
    {
        grounded = IsGrounded();
        Vector3 dir = (target.position - transform.position).normalized;
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
            Vector3 vel = dir * moveSpeed;
            vel.y = rb.velocity.y;
            rb.velocity = vel;
            transform.forward = -dir;
        }
        else if(!chase && grounded)
        {
            rb.velocity = Vector3.zero;
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundDist, maskFloor);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.CompareTag("Player"))
        {
            col.gameObject.GetComponent<PlayerHp>().ChangeHp(-explodeDmg);
            Die();
        }
        else if (col.transform.CompareTag("Object"))
        {
            Die();
        }
        if (spawnT + 2.0f < Time.time)
        {
            Die();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FireBall"))
        {
            Die();
            Destroy(other.gameObject);
        }
    }


    void Die()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
