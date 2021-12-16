using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GestureRecorder))]
public class GestureRecorderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GestureRecorder gr = (GestureRecorder)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Start Recording"))
        {
            gr.StartRecording();
        }

        if (GUILayout.Button("Stop Recording"))
        {
            gr.StopRecording();
        }

        GUILayout.EndHorizontal();

        base.OnInspectorGUI();
    }
}
