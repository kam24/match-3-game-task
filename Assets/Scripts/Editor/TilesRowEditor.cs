using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TilesRow))]
public class TilesRowEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var button = (IAddTileButton)target;
        if (GUILayout.Button("Add tile"))
            button.AddTile();
    }
}

