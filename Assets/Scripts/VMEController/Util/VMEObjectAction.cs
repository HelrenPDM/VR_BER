using UnityEngine;
using System.Collections;

/// <summary>
/// VME object action for generic raycast handling.
/// </summary>
[ExecuteInEditMode]
public class VMEObjectAction : MonoBehaviour {

	[Tooltip("Function of a behaviour bound script to execute.")]
	public bool HasLockMethod;
	public string[] m_LockMethodName;
	public bool HasEnterMethod;
	public string[] m_EnterMethodName;
	public bool HasExitMethod;
	public string[] m_ExitMethodName;

	public bool Active { get; private set; }
	public bool Lock { get; private set; }
	public bool Hover { get; private set; }

	// Use this for initialization
	void Start () {
		bool Active = false;
		bool Lock = false;
		bool Hover = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Execute given method upon focus lock.
	/// </summary>
	public void LockAction()
	{
		#if DEBUG
		Debug.Log(transform.name + ".LockAction()");
		#endif
		if (HasLockMethod)
		{
			foreach (string method in m_LockMethodName)
			{
				gameObject.SendMessage(method);
			}
		}
	}

	/// <summary>
	/// Execute given method upon enter.
	/// </summary>
	public void EnterAction()
	{
		#if DEBUG
		Debug.Log(transform.name + ".EnterAction()");
		#endif
		if (HasEnterMethod)
		{
			foreach (string method in m_EnterMethodName)
			{
				gameObject.SendMessage(method);
			}
		}
	}

	/// <summary>
	/// Execute given method upon exit.
	/// </summary>
	public void ExitAction()
	{
		#if DEBUG
		Debug.Log(transform.name + ".ExitAction()");
		#endif
		if (HasExitMethod)
		{
			foreach (string method in m_ExitMethodName)
			{
				gameObject.SendMessage(method);
			}
		}
	}
}