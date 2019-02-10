using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sensitivity : MonoBehaviour {
	public Slider sensitivity;
	public GameObject sliderGO;
	public float minSens, maxSens;

	private bool settings = false;
	private CameraMove camMS;

	void Start () {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		camMS = FindObjectOfType<CameraMove>();
		sliderGO.SetActive(false);
	}
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.P))
		{
			settings = !settings;
			if (!settings)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				Time.timeScale = 1f;
				sliderGO.SetActive(false);
			}
			else
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				Time.timeScale = 0f;
				sliderGO.SetActive(true);
			}
		}
	}

	public void UpdateSensitivity()
	{
		camMS.verticalSpeed = minSens + (sensitivity.value * (maxSens - minSens));
		camMS.horizontalSpeed = camMS.verticalSpeed;
	}
}
