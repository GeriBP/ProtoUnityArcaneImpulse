using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {
	public LayerMask groundRaycastMask;
	public float speed, jumpF, airSpeed, maxSpeed;

	private Rigidbody rb;
	private bool grounded = false;
	private bool canJump = true;
	[HideInInspector]
	public bool canWalk = true;
	private RaycastHit hit;
	private Vector3 input;

	void Start () {
		rb = GetComponent<Rigidbody>();
	}
	
	void Update () {
		input = (Input.GetAxis("Vertical") * transform.forward + Input.GetAxis("Horizontal") * transform.right).normalized;
		grounded = Grounded();
		if (grounded && canWalk)
		{
			Vector3 temp = Vector3.Cross(hit.normal, input);
			input = Vector3.Cross(temp, hit.normal);
			rb.velocity = input * speed;
		}
		else if(!grounded && canWalk)
		{
			rb.AddForce(input * airSpeed * Time.deltaTime, ForceMode.Acceleration);
		}

		if (Input.GetButton("Jump") && canJump && grounded)
		{
			StartCoroutine(JumpCr());
		}

		rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
	}

	bool Grounded()
	{
		return Physics.Raycast(transform.position, Vector2.down, out hit, 1.1f, groundRaycastMask);
	}

	IEnumerator JumpCr()
	{
		canJump = false;
		rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
		rb.AddForce(Vector3.up * jumpF, ForceMode.Impulse);
		yield return new WaitForSeconds(0.1f);
		canJump = true;
	}
}