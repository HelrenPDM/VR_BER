using UnityEngine;
using System.Collections;

public class VMEModifyObjectMaterial : MonoBehaviour {
	
	public Material ReplacementMat;

	public Transform[] Targets;

	private MeshRenderer[] m_Store;
	private bool m_Alternator;
	private Material[] tmpmats;

	// Use this for initialization
	void Start () {
		m_Alternator = false;

		// store material list
		for (int i = 0; i < Targets.Length; i++)
		{
			if (Targets[i] != null)
			{
				// store in MeshRenderer list
				m_Store = new MeshRenderer[Targets.Length];
				// create copy
				m_Store[i] = GameObject.Instantiate<MeshRenderer>(Targets[i].GetComponent<MeshRenderer>());
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
				tmpmats = new Material[Targets[i].GetComponent<MeshRenderer>().materials.Length];
				for (int j = 0; j < tmpmats.Length; j++) {
					tmpmats[j] = ReplacementMat;	
				}
				Targets[i].GetComponent<MeshRenderer>().sharedMaterials = tmpmats;
			}
		}
	}

	/// <summary>
	/// Reset this material.
	/// </summary>
	private void Reset()
	{
		Debug.Log(transform.name + ".Reset()");
		if (m_Store.Length > 0 && Targets.Length > 0)
		{
			for (int i = 0; i < Targets.Length; i++)
			{
				if (m_Store[i] != null)
				{
					Targets[i].GetComponent<MeshRenderer>().materials = m_Store[i].materials;
				}
			}
		}
	}
}
