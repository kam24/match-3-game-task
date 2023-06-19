using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridLayers))]
public class GridLayersEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var button = (ILayerButton)target;
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add layer"))
            button.AddLayer();
        if (GUILayout.Button("Remove layer"))
            button.RemoveLayer();
        GUILayout.EndHorizontal();
    }
}

