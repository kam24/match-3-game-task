using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Tile))]
public class TilesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var button = (IApplyButton)target;
        if (GUILayout.Button("Apply"))
            button.Apply();
    }
}