using UnityEngine;
using System.Collections;

public class VMENextSpot : MonoBehaviour {

	public Transform nextSpot;
	public bool nextBody;

	// Use this for initialization
	void Start () {
		if (nextSpot == null)
		{
			Debug.LogWarning("Scene progression not set for " + this.gameObject.ToString());
		}
	}

	// Update is called once per frame
	void Update () {

	}
}