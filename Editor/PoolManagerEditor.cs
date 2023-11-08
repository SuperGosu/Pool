using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Gosu.Pool;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using System.Linq.Expressions;

[CustomEditor(typeof(PoolManager))]
public class PoolManagerEditor : Editor
{
	private PoolManager poolManager;

	SerializedProperty pools;
	GUISkin guiSkin;
	private void OnEnable()
	{
		poolManager = target as PoolManager;
		pools = serializedObject.FindProperty("pools");
		guiSkin = Resources.Load<GUISkin>("pool skin");
	}

	public override void OnInspectorGUI()
	{

		poolManager.isDontDestroyOnLoad = EditorGUILayout.Toggle("Dont Destroy Onload", poolManager.isDontDestroyOnLoad);
		DropAreaGUI();

		for (int i = 0; i < pools.arraySize; i++)
		{
			var element = pools.GetArrayElementAtIndex(i);
			int index = i;
			DrawPool(element,()=> deleteElement(index) );
		}
	}

	private void deleteElement(int index)
	{
		Undo.RecordObject(poolManager, "delete element");
		serializedObject.Update();
		pools.DeleteArrayElementAtIndex(index);
		serializedObject.ApplyModifiedProperties();
	}

	private void DrawPool(SerializedProperty gosuPool,System.Action deleteCallback)
	{

		GUILayout.BeginHorizontal();
		EditorGUI.BeginChangeCheck();

		serializedObject.Update();
		var nameFiel = gosuPool.FindPropertyRelative("name");
		var prefabFiel = gosuPool.FindPropertyRelative("prefab");

		if(prefabFiel.objectReferenceValue == null)
		{
			GUI.enabled = false;
		}
		else
		{
			GUI.enabled = true;
		}
		if(GUILayout.Button("Open", GUILayout.Width(90)))
		{
			AssetDatabase.OpenAsset(prefabFiel.objectReferenceValue);
		}

		GUI.enabled = true;
		nameFiel.stringValue = EditorGUILayout.TextField(nameFiel.stringValue, GUILayout.Width(100));

		prefabFiel.objectReferenceValue = EditorGUILayout.ObjectField(prefabFiel.objectReferenceValue,typeof(GameObject), true, GUILayout.Width(100));

		var maxSize = gosuPool.FindPropertyRelative("maxSize");
		var instanceNumber = gosuPool.FindPropertyRelative("instanceNumber");

		EditorGUILayout.LabelField("max size", GUILayout.Width(50));
		maxSize.intValue = EditorGUILayout.IntField(maxSize.intValue, GUILayout.Width(50));
		EditorGUILayout.LabelField("init number", GUILayout.Width(65));
		instanceNumber.intValue = Mathf.Min(instanceNumber.intValue, maxSize.intValue);
		instanceNumber.intValue = EditorGUILayout.IntField(instanceNumber.intValue, GUILayout.Width(50));

		if (GUILayout.Button("X", GUILayout.Width(20)))
		{
			deleteCallback?.Invoke();
		}

		if (EditorGUI.EndChangeCheck())
		{
			serializedObject.ApplyModifiedProperties();
		}

		GUILayout.EndHorizontal();
	}

	public void DropAreaGUI()
	{
		Event evt = Event.current;
		Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50, GUILayout.ExpandWidth(true));
		GUI.Box(drop_area, "Drag Object Add To Pool", guiSkin.box);

		switch (evt.type)
		{
			case EventType.DragUpdated:
			case EventType.DragPerform:
				if (!drop_area.Contains(evt.mousePosition))
					return;

				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

				if (evt.type == EventType.DragPerform)
				{
					DragAndDrop.AcceptDrag();

					foreach (Object dragged_object in DragAndDrop.objectReferences)
					{
						// Do On Drag Stuff here
						TryAddObject(dragged_object);
					}
				}
				break;
		}
	}

	private void TryAddObject(Object ob)
	{
		GameObject gameObject = ob as GameObject;
		if (gameObject == null)
			return;

		if (isAddedToPool(gameObject))
		{
			Debug.LogError("Existed pool for " + gameObject.name);
		}
		else
		{
			Debug.Log("Add new pool for " + gameObject.name);


			serializedObject.Update();
			pools.arraySize++;
			var lastElement = pools.GetArrayElementAtIndex(pools.arraySize - 1);
			lastElement.FindPropertyRelative("prefab").objectReferenceValue = ob;
			lastElement.FindPropertyRelative("name").stringValue = ob.name;
			lastElement.FindPropertyRelative("maxSize").intValue = 99;
			lastElement.FindPropertyRelative("instanceNumber").intValue = 0;
			serializedObject.ApplyModifiedProperties();
		}

	}

	private bool isAddedToPool(GameObject ob)
	{
		var serializedPools = serializedObject.FindProperty("pools");
		for (int i = 0; i < serializedPools.arraySize; i++)
		{
			var s = serializedPools.GetArrayElementAtIndex(i);
			var refPrefab = s.FindPropertyRelative("prefab");
			if (refPrefab == null)
				continue;

			var prefab = (GameObject)refPrefab.objectReferenceValue;
			if (prefab == ob)
			{
				return true;
			}
		}

		return false;
	}

}
