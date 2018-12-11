using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {
	public LayerMask groundRaycastMask;
	public float speed, jumpF, airSpeed;

	private Rigidbody rb;
	private bool grounded = false;
	private bool canJump = true;
	private RaycastHit hit;

	void Start () {
		rb = GetComponent<Rigidbody>();
	}
	
	void Update () {
		Vector3 input = (Input.GetAxisRaw("Vertical") * transform.forward + Input.GetAxisRaw("Horizontal") * transform.right).normalized;
		grounded = Grounded();
		if (grounded)
		{
			Vector3 temp = Vector3.Cross(hit.normal, input);
			input = Vector3.Cross(temp, hit.normal);
			rb.velocity = input * speed;
		}
		else
		{
			rb.AddForce(input * airSpeed * Time.deltaTime, ForceMode.Acceleration);
		}

		if (Input.GetButton("Jump") && canJump && grounded)
		{
			StartCoroutine(JumpCr());
		}
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