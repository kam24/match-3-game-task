using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TilesStack))]
public class TilesStackEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var button = (IApplyButton)target;
        if (GUILayout.Button("Apply"))
            button.Apply();
    }
}

