using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GesturePlayback))]
public class GesturePlaybackEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GesturePlayback gp = (GesturePlayback)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Start Playback"))
        {
            gp.StartPlayback();
        }

        if (GUILayout.Button("Stop Playback"))
        {
            gp.StopPlayback();
        }

        GUILayout.EndHorizontal();

        base.OnInspectorGUI();
    }
}
