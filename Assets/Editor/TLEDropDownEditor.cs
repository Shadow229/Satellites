using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(TLEFile))]
public class TLEDropDownEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TLEFile script = (TLEFile)target;

        GUIContent arrayList = new GUIContent("TLEFiles");
        script.listIndex = EditorGUILayout.Popup(arrayList, script.listIndex, script.TLEFiles.ToArray());
    }
}
