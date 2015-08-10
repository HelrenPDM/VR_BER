using UnityEngine;
using System.Collections;

public class VMEModifyObjectMaterial : MonoBehaviour {
	
	public Material ReplacementMat;

	public Transform[] Targets;

	private Renderer[] m_Store;
	private bool m_Alternator;

	// Use this for initialization
	void Start () {
		m_Alternator = false;

		// store in MeshRenderer list
		this.m_Store = new Renderer[Targets.Length];

		// store original material list
		for (int i = 0; i < Targets.Length; i++)
		{
			if (Targets[i] != null)
			{
				// create copy
				this.m_Store[i] = GameObject.Instantiate<Renderer>(Targets[i].GetComponentInChildren<Renderer>());
				Debug.Log("Stored " + m_Store[i].name);
				m_Store[i].enabled = false;
			}
			else
			{
				Debug.LogWarning("No material set for MeshRenderer material " + i);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Switchs the material.
	/// </summary>
	public void SwitchMaterial()
	{
		Debug.Log(transform.name + ".SwitchMaterial()");

		if (this.m_Alternator)
		{
			this.Reset();
			this.m_Alternator = false;
		}
		else
		{
			this.Replace();
			this.m_Alternator = true;
		}
	}

	/// <summary>
	/// Replace this material.
	/// </summary>
	private void Replace()
	{
		Debug.Log(transform.name + ".Replace()");
		if (ReplacementMat != null)
		{
			for (int i = 0; i < Targets.Length; i++)
			{
				Material[] tmpmats = new Material[Targets[i].GetComponentInChildren<Renderer>().materials.Length];
				for (int j = 0; j < tmpmats.Length; j++) {
					tmpmats[j] = ReplacementMat;
				}
				Targets[i].GetComponentInChildren<Renderer>().materials = tmpmats;
			}
		}
	}

	/// <summary>
	/// Reset this material.
	/// </summary>
	private void Reset()
	{
		Debug.Log(transform.name + ".Reset()");
		if (this.m_Store != null && Targets.Length > 0)
		{
			for (int i = 0; i < Targets.Length; i++)
			{
				Debug.Log("Resetting " + Targets[i].name + " from " + this.m_Store[i].name);
				Targets[i].GetComponentInChildren<Renderer>().materials = this.m_Store[i].materials;
			}
		}
	}
}
