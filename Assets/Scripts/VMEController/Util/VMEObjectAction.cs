using UnityEngine;
using System.Collections;

public class VMETriggerAction : MonoBehaviour {

	[Tooltip("Function of a behaviour bound script to execute.")]
	public string m_MethodName = "Execute";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Execute this instance.
	/// </summary>
	public void ExecuteAction()
	{
		Debug.Log(transform.name + ".ExecuteAction()");
		gameObject.SendMessage(m_MethodName);
	}
}
