using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {
	public Camera visor;
	public float verticalSpeed, horizontalSpeed;

	private bool canLook = true;

	void Start () {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}
	
	void Update () {
		if (canLook)
		{
			float tempx = visor.transform.localEulerAngles.x;
			float tempy = visor.transform.localEulerAngles.y;
			float tempz = visor.transform.localEulerAngles.z;

			visor.transform.Rotate(-verticalSpeed * Input.GetAxis("Mouse Y") * Time.deltaTime, 0.0f, 0.0f);
			transform.Rotate(0.0f, horizontalSpeed * Input.GetAxis("Mouse X") * Time.deltaTime, 0.0f);

			if (visor.transform.localEulerAngles.x > 85.0f && visor.transform.localEulerAngles.x < 200.0f)
			{
				visor.transform.localEulerAngles = new Vector3(tempx, tempy, tempz);
			}
			else if (visor.transform.localEulerAngles.x > 200.0f && visor.transform.localEulerAngles.x < 281.0f)
			{
				visor.transform.localEulerAngles = new Vector3(tempx, tempy, tempz);
			}
		}
	}
}