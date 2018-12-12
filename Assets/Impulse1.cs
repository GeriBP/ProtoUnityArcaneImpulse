using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Impulse1 : MonoBehaviour {
	public float pushF, pullFPlayer, pullFObjects, maxDist, minDistPull, maxObjSpeed;
	public LayerMask mask;
	public GameObject indicator;
	public Transform objectPos;
	public PhysicMaterial playerPhysMat;
	public static Impulse1 instance;

	private Rigidbody rb, targetRb;
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
		moveS = FindObjectOfType<Move>();
	}
	
	void Update () {
		Physics.Raycast(camTransform.position, camTransform.forward, out hit, mask);
		float dist = Vector3.Distance(hit.point, transform.position);

		if (hit.transform != null && dist < maxDist && objS == null)
		{
			indicator.SetActive(true);
			indicator.transform.position = hit.point;
			targetRb = hit.transform.GetComponent<Rigidbody>();
			HandlePowerInput(dist);
		}
		else if (objS != null)
		{
			indicator.SetActive(true);
			indicator.transform.position = objS.transform.position;
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
			if (objS != null)
			{
				objS.CancelPull();
				targetRb.AddForce(camTransform.forward * pushF, ForceMode.Impulse);
			}
			else if (targetRb == null)
			{
				NoFriction();
				rb.AddForce(-camTransform.forward * pushF, ForceMode.Impulse);
			}
			else
			{
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
				if (Vector3.Distance(targetRb.transform.position, objectPos.position) > minDistPull)
				{
					targetRb.AddForce(-camTransform.forward * pullFObjects, ForceMode.Force);
					targetRb.velocity = Vector3.ClampMagnitude(targetRb.velocity, maxObjSpeed);
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
		StartCoroutine(CantWalkCR());
	}

	IEnumerator CantWalkCR()
	{
		moveS.canWalk = false;
		yield return new WaitForSeconds(1f);
		moveS.canWalk = true;
	}
}
 