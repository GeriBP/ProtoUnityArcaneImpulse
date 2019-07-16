using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Impulse1 : MonoBehaviour {
	public float pushFObjects, pushFPlayer, pullFPlayer, pullFObjects, maxDist, minDistPull, maxObjSpeed;
	public LayerMask mask;
	public GameObject indicator;
	public Transform objectPos;
	public PhysicMaterial playerPhysMat;
	public static Impulse1 instance;
    [Header("Power Stuff")]
    public float pushCooldown;
    public float pullCooldown;
    private bool canPush = true;
    private bool canPull = true;
    private Image canPushImage, canPullImage; 

    private Rigidbody rb, targetRb;
	private RaycastHit hit;
	private Transform camTransform;
	private PulledObj objS;
	private Move moveS;
    private bool drainMana = false;
    private float timeToRecharge = 0f;


	private void Awake()
	{
		instance = this;
        canPushImage = GameObject.Find("CanPushImage").GetComponent<Image>();
        canPullImage = GameObject.Find("CanPullImage").GetComponent<Image>();
        canPushImage.color = Color.yellow;
        canPullImage.color = Color.green;
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
			indicator.SetActive(false);
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
		if (Input.GetButtonDown("Push") && canPush)
		{
            if (objS != null)
			{
				objS.CancelPull();
				targetRb.AddForce(camTransform.forward * pushFObjects, ForceMode.Impulse);
			}
			else if (targetRb == null)
			{
				NoFriction();
				rb.AddForce(-camTransform.forward * pushFPlayer, ForceMode.Impulse);
			}
			else
			{
				targetRb.AddForce(camTransform.forward * pushFObjects, ForceMode.Impulse);
			}
            StartCoroutine(PushToggle());
        }
		else if (Input.GetButtonDown("Pull") && canPull)
		{
            objS = null;
			if (targetRb == null)
			{
				NoFriction();
				rb.AddForce(camTransform.forward * pullFPlayer, ForceMode.Impulse);
                StartCoroutine(PullToggle());
			}
		}
		else if (Input.GetButton("Pull") && canPull)
		{
			if (targetRb != null && objS == null)
			{
                drainMana = true;
                if (Vector3.Distance(targetRb.transform.position, objectPos.position) > minDistPull)
				{
					targetRb.AddForce(-camTransform.forward * pullFObjects, ForceMode.Force);
					targetRb.velocity = Vector3.ClampMagnitude(targetRb.velocity, maxObjSpeed);
                }
				else
				{
					targetRb.velocity = Vector3.zero;
					objS = targetRb.GetComponent<PulledObj>();
                    if (objS != null) objS.GetPulled();
                    else objS = null;
				}
			}
		}

		if (Input.GetButtonUp("Pull"))
		{
			if(objS != null) objS.CancelPull();
			objS = null;
            drainMana = false;
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

    IEnumerator PushToggle()
    {
        canPush = false;
        canPushImage.color = Color.black;
        yield return new WaitForSeconds(pushCooldown);
        canPush = true;
        canPushImage.color = Color.yellow;
    }

    IEnumerator PullToggle()
    {
        canPull = false;
        canPullImage.color = Color.black;
        yield return new WaitForSeconds(pullCooldown);
        canPull = true;
        canPullImage.color = Color.green;
    }
}
 