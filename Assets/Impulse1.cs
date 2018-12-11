using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impulse1 : MonoBehaviour {
	public float pushF, pullFPlayer, pullFObjects, maxDist, minDistPull, sFriction, dFriction;
	public LayerMask mask;
	public GameObject indicator;
	public Transform objectPos;
	public PhysicMaterial playerPhysMat;
	public static Impulse1 instance;

	private Rigidbody rb, targetRb;
	private float myMass;
	private RaycastHit hit;
	private Transform camTransform;
	private PulledObj objS;
	private Move moveS;

	private void Awake()
	{
		instance = this;
	}

	void Start () {
		rb = GetComponent<Rigidbody>();
		camTransform = Camera.main.transform;
		myMass = rb.mass;
		playerPhysMat.staticFriction = sFriction;
		playerPhysMat.dynamicFriction = dFriction;
		moveS = FindObjectOfType<Move>();
	}
	
	void Update () {
		Physics.Raycast(camTransform.position, camTransform.forward, out hit, mask);
		float dist = Vector3.Distance(hit.point, transform.position);

		if (hit.transform != null && dist < maxDist)
		{
			indicator.SetActive(true);
			indicator.transform.position = hit.point;
			targetRb = hit.transform.GetComponent<Rigidbody>();
			HandlePowerInput(dist);
		}
		else
		{
			indicator.SetActive(false);
		}
	}

	private void HandlePowerInput(float dist)
	{
		if (Input.GetButtonDown("Push"))
		{
			if (targetRb == null)
			{
				NoFriction();
				rb.AddForce(-camTransform.forward * pushF, ForceMode.Impulse);
			}
			else
			{
				if (objS != null)
				{
					objS.CancelPull();
				}
				targetRb.AddForce(camTransform.forward * pushF, ForceMode.Impulse);
			}
		}
		else if (Input.GetButtonDown("Pull"))
		{
			objS = null;
			if (targetRb == null)
			{
				NoFriction();
				rb.AddForce(camTransform.forward * pullFPlayer, ForceMode.Impulse);
			}
		}
		else if (Input.GetButton("Pull"))
		{
			if (targetRb != null && objS == null)
			{
				if (dist > minDistPull)
				{
					targetRb.AddForce(-camTransform.forward * pullFObjects, ForceMode.Force);
				}
				else
				{
					targetRb.velocity = Vector3.zero;
					objS = targetRb.GetComponent<PulledObj>();
					objS.GetPulled();
				}
			}
		}

		if (Input.GetButtonUp("Pull"))
		{
			if(objS != null) objS.CancelPull();
			objS = null;
		}
	}

	private void NoFriction()
	{
		StopAllCoroutines();
		StartCoroutine(NoFrictionCr());
	}

	IEnumerator NoFrictionCr()
	{
		playerPhysMat.staticFriction = 0f;
		playerPhysMat.dynamicFriction = 0f;
		moveS.canWalk = false;
		yield return new WaitForSeconds(1f);
		playerPhysMat.staticFriction = sFriction;
		playerPhysMat.dynamicFriction = dFriction;
		moveS.canWalk = true;
	}
}
 