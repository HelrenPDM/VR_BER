using UnityEngine;
using System.Collections;
using System;

public class FlightPoint : MonoBehaviour {

	/// <summary>
	/// Gets or sets the rotation speed.
	/// </summary>
	/// <value>The rotation speed.</value>
	public float RotationSpeed { get; set; }

	// Use this for initialization
	void Start () {
		RotationSpeed = 60f;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (Vector3.up, RotationSpeed * Time.deltaTime, Space.World);
	}
}
