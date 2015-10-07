using UnityEngine;
using System.Collections;

public class VMELoadScene : MonoBehaviour
{
	public GameObject m_GlobalManager;
	public string SceneName;
	public string SceneSpot;
	public bool UseBody;

	// Use this for initialization
	void Start ()
	{
		m_GlobalManager = FindObjectOfType<GlobalManager> ().gameObject;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void LoadScene ()
	{
		if (SceneSpot == "EXIT") {
			Debug.Log ("Exiting");
			Application.Quit ();
		} else {
			m_GlobalManager.transform.GetComponent<GlobalManager> ().VMEStart = SceneSpot;
			m_GlobalManager.transform.GetComponent<GlobalManager> ().UseBody = UseBody;
			Application.LoadLevel (SceneName);
		}
	}
}
