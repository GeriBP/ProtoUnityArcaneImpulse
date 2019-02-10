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
    [Header("Mana Bar Stuff")]
    public float maxMana;
    public float pushCost, pullPlayerCost, pullObjCostIni, pullObjCostHold, manaRecoveryTime, manaRecoveryAmount;

    private float currMana;
    private Image manaBar;
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
        manaBar = GameObject.Find("ManaBar").GetComponent<Image>();
        manaBar.fillAmount = 1.0f;
    }

	void Start () {
        currMana = maxMana;
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

        if (drainMana)
        {
            currMana -= pullObjCostHold * Time.deltaTime;
            timeToRecharge = Time.timeSinceLevelLoad + manaRecoveryTime;
        }
        else if (timeToRecharge < Time.timeSinceLevelLoad)
        {
            currMana += manaRecoveryAmount * Time.deltaTime;
        }
        currMana = Mathf.Clamp(currMana, 0f, maxMana);
        manaBar.fillAmount = currMana/maxMana;
    }

	private void HandlePowerInput(float dist)
	{
		if (Input.GetButtonDown("Push") && currMana >= pushCost)
		{
            currMana -= pushCost;
            timeToRecharge = Time.timeSinceLevelLoad + manaRecoveryTime;
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
		}
		else if (Input.GetButtonDown("Pull") && currMana >= pullPlayerCost)
		{
            currMana -= pullPlayerCost;
            timeToRecharge = Time.timeSinceLevelLoad + manaRecoveryTime;
            objS = null;
			if (targetRb == null)
			{
				NoFriction();
				rb.AddForce(camTransform.forward * pullFPlayer, ForceMode.Impulse);
			}
		}
		else if (Input.GetButton("Pull") && currMana >= pullObjCostIni)
		{
			if (targetRb != null && objS == null)
			{
                if(!drainMana) currMana -= pullObjCostIni;
                timeToRecharge = Time.timeSinceLevelLoad + manaRecoveryTime;
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
					objS.GetPulled();
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
}
 