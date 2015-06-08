//
//  VMEPlayerController.cs
//
//  Author:
//       Stephan Gensch <stgensch@netzwerkcafe.org>
//
//  Copyright (c) 2015 Stephan Gensch
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Controls the player's movement in virtual reality using a set of visual observations, like rays hitting objects and tilt information.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class VMEPlayerController : MonoBehaviour {

	#region PlayerController variables
	private bool  SkipMouseRotation = true;
	private bool  HaltUpdateMovement = false;

	// prefab handles
	protected CharacterController m_PlayerController = null;
	protected Camera m_CameraRig = null;
	protected NavMeshAgent m_Agent = null;
	#endregion

	// visual ray tracking handle
	public VMECameraRayTracker CenterRayTracker = new VMECameraRayTracker();

	// for input modes
	public bool MoveMode = false;
	public bool ScriptedMove = false;

	private Transform ScriptTarget = null;

	// movement parameters
	private float SimulationRate = 60f;

	// GUI stuff
	private TextMesh m_FocusObjectText = null;
	private TextMesh m_DebugText = null;
	private TextMesh m_MoveModeText = null;
	private GazePointer m_FocusGazePointer = null;

	// Use this for initialization
	void Start () {

		m_FocusObjectText = GameObject.Find("FocusObjectText3D").GetComponent<TextMesh>();
		m_DebugText = GameObject.Find("DebugText3D").GetComponent<TextMesh>();
		m_MoveModeText = GameObject.Find("MoveModeText3D").GetComponent<TextMesh>();
		m_FocusGazePointer = GameObject.Find("FocusGazePointer").GetComponent<GazePointer>();

		DebugInGUI("");

		#if UNITY_ANDROID && !UNITY_STANDALONE
		m_PlayerController = GameObject.Find ("SceneCharacter").GetComponent<CharacterController>();
		if(m_PlayerController == null)
			Debug.LogWarning("VMEPlayerController: No CharacterController attached.");
		#else
		m_PlayerController = gameObject.GetComponent<CharacterController>();
		if(m_PlayerController == null)
			Debug.LogWarning("VMEPlayerController: No CharacterController attached.");
		#endif

		CheckNavMeshAgent();

		#if UNITY_STANDALONE
		m_CameraRig = GameObject.Find("OVRCameraRig").GetComponent<OVRCameraRig>().leftEyeCamera;
		#elif UNITY_ANDROID
		m_CameraRig = GameObject.Find("Main Camera Left").GetComponent<Camera>();
		#else
		Camera[] CameraRigs = transform.GetComponentsInChildren<Camera>();
		
		if(CameraRigs.Length == 0)
			Debug.LogWarning("VMEPlayerController: No Camera attached.");
		else if (CameraRigs.Length > 1)
		{
			Debug.LogWarning("VMEPlayerController: More then 1 Camera attached.");
			m_CameraRig = CameraRigs[0]; //still take the first
		}
		else
			m_CameraRig = CameraRigs[0];
		
		if(!m_CameraRig.isActiveAndEnabled)
			Debug.LogWarning("VMEPlayerController: No Camera attached.");
		#endif

		Debug.Log ("CameraRig set to " + m_CameraRig.name);
		CenterRayTracker.Init ();
		CenterRayTracker.SetCameraController (ref m_CameraRig);
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	protected void Update ()
	{
		MoveModeInGUI(this.MoveMode);
		// Check for an object ray hit in center viewpath (x=0.5f,y=0.5f)
		CenterRayTracker.FireRay ();

		if (CenterRayTracker.TargetObject != null)
		{
			if (CenterRayTracker.TargetObject.layer == 8)
			{
				FocusObjectInGUI(CenterRayTracker.TargetObjectName);
				GazePointerColor(CenterRayTracker.FocusLockPercentage, 1f);
			}
			else
			{
				FocusObjectInGUI("");
				GazePointerColor(0.5f, 0f);
			}

			GazePointerPositionAndScale(CenterRayTracker.TargetObjectHitPoint,
			                            CenterRayTracker.TargetObjectDistance);
		}
		else
		{
			FocusObjectInGUI("");
			GazePointerColor(CenterRayTracker.FocusLockPercentage, 0f);
		}

		if (CenterRayTracker.TargetObject != null &&
		    CenterRayTracker.TargetObjectFocusLocked)
		{
			if (!this.MoveMode)
			{
				// If there is a hit and object ray hit is set to trigger movement, trigger going forward
				if (CenterRayTracker.TargetObject.CompareTag ("UIButton3D"))
				{
					switch (CenterRayTracker.TargetObjectName)
					{
						case "Continue":
							break;
						default:
							break;
					}
				}
			}
			else
			{
				if (m_Agent == null)
				{
					m_Agent = gameObject.GetComponent<NavMeshAgent>();
				}
				// If there is a hit and object ray hit is set to trigger movement, trigger going forward
				if (CenterRayTracker.TargetObject.CompareTag ("UIButton3D"))
				{
					switch (CenterRayTracker.TargetObjectName)
					{
					case "Continue":
						this.ScriptTarget = CenterRayTracker.TargetObject.GetComponent<VMENextInfoPoint>().target;
						DebugInGUI ("NextStop: \n" + this.ScriptTarget);
						m_Agent.destination = this.ScriptTarget.position;
						break;
					default:
						break;
					}
				}
				// If there is a hit and object ray hit is set to trigger script, trigger going towards target
				if (CenterRayTracker.TargetObject.CompareTag ("TriggerScripted"))
				{
					this.ScriptTarget = CenterRayTracker.TargetObject.transform;
					DebugInGUI ("NextStop: \n" + this.ScriptTarget);
					m_Agent.destination = this.ScriptTarget.position;
				}
			}
		}
	}

	private void CheckNavMeshAgent()
	{
		if (MoveMode)
		{
			if(gameObject.GetComponent<NavMeshAgent>() == null)
			{
				Debug.LogWarning("VMEPlayerController: No NavMeshAgent attached.");
				this.MoveMode = false;
			}
			else
			{
				m_Agent = gameObject.GetComponent<NavMeshAgent>();
				m_Agent.baseOffset = 0.0f;
				m_Agent.autoTraverseOffMeshLink = true;
				m_Agent.updateRotation = false;
				m_Agent.height = 1.8f;
			}
		}
	}

	/// <summary>
	/// Moves the CC forward.
	/// </summary>
	private void MoveForward()
	{
		//TODO: Well, yes. Let's implement this for any practical free forward movement
		throw new NotImplementedException();
	}

	/// <summary>
	/// Updates the orientation.
	/// </summary>
	void LateUpdate()
	{
		if (HaltUpdateMovement)
			return;

		#if UNITY_ANDROID && !UNITY_EDITOR
		if (m_PlayerController.GetComponent<Cardboard>().UpdateState())
		{
			m_PlayerController.transform.rotation = m_PlayerController.GetComponent<Cardboard>().HeadRotation;
		}
		#endif
	}

	
	/// <summary>
	/// Starts the scripted flight.
	/// </summary>
	/// <param name="target">Target.</param>
	void ScriptedMovement(Transform target)
	{
		if (HaltUpdateMovement)
			return;
	}

	/// <summary>
	/// Debug message GUI
	/// </summary>
	/// <param name="msg">Message.</param>
	private void DebugInGUI(string msg)
	{
		m_DebugText.text = msg;
	}

	/// <summary>
	/// FO text in GUI
	/// </summary>
	/// <param name="msg">Message.</param>
	private void FocusObjectInGUI(string msg)
	{
		m_FocusObjectText.text = msg;
	}

	/// <summary>
	/// MM text in GUI
	/// </summary>
	/// <param name="msg">If set to <c>true</c> message.</param>
	private void MoveModeInGUI(bool msg)
	{
		m_MoveModeText.text = "MoveMode " + msg.ToString();
	}

	/// <summary>
	/// Sets the color of the pointer.
	/// </summary>
	/// <param name="color">Color.</param>
	/// <param name="alpha">Alpha.</param>
	private void GazePointerColor(float color, float alpha)
	{
		m_FocusGazePointer.UpdateColor(color, alpha);
	}

	/// <summary>
	/// Sets the pointer position and scale.
	/// </summary>
	/// <param name="pos">Position.</param>
	/// <param name="scale">Scale.</param>
	private void GazePointerPositionAndScale(Vector3 pos, float scale)
	{
		m_FocusGazePointer.UpdatePositionAndScale(pos, scale);
	}
}
