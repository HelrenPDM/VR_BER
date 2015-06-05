using UnityEngine;
using System.Collections;
using System;

public class VMECameraRig : MonoBehaviour {

	public float SimulationRate = 60.0f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 euler = transform.localRotation.eulerAngles;
		
		float rotateInfluence = SimulationRate * Time.deltaTime;

		#if !UNITY_EDITOR && UNITY_WEBGL
		euler.y += Input.GetAxis("Mouse X");
		euler.x += -(Input.GetAxis("Mouse Y"));
		#else
		euler.y += Input.GetAxis("Mouse X") * rotateInfluence * 3.25f;
		euler.x += -(Input.GetAxis("Mouse Y") * rotateInfluence * 3.25f);
		#endif
		transform.localRotation = Quaternion.Euler(euler.x, euler.y, 0.0f);
	}

	/// <summary>
	/// Updates the orientation.
	/// </summary>
	void LateUpdate()
	{
	}

}
