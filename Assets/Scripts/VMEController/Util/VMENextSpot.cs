using UnityEngine;
using System.Collections;

public class VMENextSpot : MonoBehaviour {

	public Transform nextSpot;
	public bool nextBody;
	public Vector3 lookAt;

	// Use this for initialization
	void Start () {
		if (nextSpot == null)
		{
			Debug.LogWarning("Scene progression not set for " + this.gameObject.ToString());
		}
		if (lookAt == null)
		{
			lookAt = Vector3.zero;
		}
	}

	// Update is called once per frame
	void Update () {

	}
}