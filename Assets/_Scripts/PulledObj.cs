using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulledObj : MonoBehaviour {
	private Rigidbody rb;
	private Transform target;
	private bool isPulled;
	public float attraction;

	void Start () {
		rb = GetComponent<Rigidbody>();
		target = Impulse1.instance.objectPos;
	}
	
	public void GetPulled()
	{
		if (!isPulled)
		{
			isPulled = true;
			StartCoroutine(PullObj());
		}
	}

	public void CancelPull()
	{
		isPulled = false;
	}

	IEnumerator PullObj()
	{
		rb.useGravity = false;
		float dist = 0f;
		while (isPulled && dist < Impulse1.instance.minDistPull + 0.5f)
		{
			dist = Vector3.Distance(target.position, transform.position);
			if (Vector3.Distance(target.position, transform.position) > 0.05f)
			{
				Vector3 dir = (target.position - transform.position).normalized;
				rb.MovePosition(transform.position + (dir * attraction * Mathf.Clamp(dist/ (Impulse1.instance.minDistPull + 0.5f), 0f, 1f)));
			}
			yield return null;
		}
		rb.useGravity = true;
	}
}
