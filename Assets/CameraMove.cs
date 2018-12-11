using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {
	public Camera visor;
	public float verticalSpeed, horizontalSpeed, viewRange;
	private float rotX, rotY;

	private bool canLook = true;

	void Start () {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		rotX = 0f;
		rotY = 0f;
	}
	
	void Update () {
		if (canLook)
		{
			rotX += Input.GetAxis("Mouse X") * horizontalSpeed * Time.deltaTime;
			rotY += Input.GetAxis("Mouse Y") * verticalSpeed * Time.deltaTime;
			rotY = Mathf.Clamp(rotY, -viewRange, viewRange);

			visor.transform.localRotation = Quaternion.Euler(-rotY, 0f, 0f);
			transform.rotation = Quaternion.Euler(0f, rotX, 0f);
		}
	}
}