using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTest : MonoBehaviour
{
    public float speed, jumpSpeed, gravity, groundDist, reduceForceAmount;
    public LayerMask mask;

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;
    private RaycastHit hit;

    private Vector3 totalForces = Vector3.zero;
    private Vector3 jumpForce = Vector3.zero;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleMovement();
 
        JumpHandle();

        totalForces = Vector3.MoveTowards(totalForces, Vector3.zero, reduceForceAmount * Time.deltaTime);

        //Debug.Log(moveDirection + " | " + jumpForce + " | " + totalForces);
        controller.Move((moveDirection + jumpForce + totalForces) * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.P))
        {
            totalForces += new Vector3(0f, 0f, 20f);
        }
    }

    private void HandleMovement()
    {
        moveDirection = (Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward).normalized;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundDist, mask))
        {
            Vector3 temp = Vector3.Cross(hit.normal, moveDirection);
            moveDirection = Vector3.Cross(temp, hit.normal);
        }
        moveDirection = moveDirection * speed;
    }

    private void JumpHandle()
    {
        if (controller.isGrounded)
        {
            if (Input.GetButton("Jump"))
            {
                jumpForce = new Vector3(0f, jumpSpeed, 0f);
            }
            else jumpForce = Vector3.zero;
        }
        else jumpForce.y -= gravity * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        totalForces += other.GetComponent<FVolume>().force; 
    }
}
