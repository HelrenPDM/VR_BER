using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(VMEObjectAction))]
public class VMEObjectActionEditor : Editor {

	public VMEObjectAction _target;

	void OnEnable()
	{
		_target = (VMEObjectAction)target;
	}
	
	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		_target.HasLockMethod = EditorGUILayout.Toggle("Has Lock Method", _target.HasLockMethod);
		if (_target.HasLockMethod)
		{
			SerializedProperty lockmthds = serializedObject.FindProperty("m_LockMethodName");

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(lockmthds, true);
			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
			}
		}

		_target.HasEnterMethod = EditorGUILayout.Toggle("Has Enter Method", _target.HasEnterMethod);
		if (_target.HasEnterMethod)
		{
			SerializedProperty entermthds = serializedObject.FindProperty("m_EnterMethodName");
			
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(entermthds, true);
			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
			}
		}

		_target.HasExitMethod = EditorGUILayout.Toggle("Has Exit Method", _target.HasExitMethod);
		if (_target.HasExitMethod)
		{
			SerializedProperty exitmthds = serializedObject.FindProperty("m_ExitMethodName");
			
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(exitmthds, true);
			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
			}
		}
		
		if (GUI.changed)
			EditorUtility.SetDirty(target);
	}
}