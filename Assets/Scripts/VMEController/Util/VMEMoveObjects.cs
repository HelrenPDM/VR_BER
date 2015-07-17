using UnityEngine;
using System.Collections;

public class VMEMoveObjects : MonoBehaviour {

	public Transform[] Movables;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void MoveObjects()
	{
		if (Movables.Length != 0)
		{
			foreach (var item in Movables)
			{
				Debug.Log(transform.name + ".MoveObjects()");
				item.GetComponent<VMEMovableObject>().ToggleMove();
			}
		}
	}
}

