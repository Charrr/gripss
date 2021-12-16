using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Neuron;
using System.IO;

public class GestureRecorder : MonoBehaviour
{
    [Tooltip("Avatar to track gestures from.")]
    public GameObject Avatar;

    private NeuronTransformsInstance _instance;

    public Transform[] BoneTransforms;

    public string Delimiter = ";"; // in case comma is used as decimal separator

    public float SampleInterval = 0.1f;

    public int ParticipantID = 0;

    [Tooltip("Name of the txt/csv file to write data to.")]
    public string SaveToPath = "";

    private string _filePath;

    private bool _is_recording = false;

    private void OnValidate()
    {
        SampleInterval = Mathf.Max(SampleInterval, 0.01f);
        if (Avatar)
        {
            _instance = Avatar.GetComponent<NeuronTransformsInstance>();
            BoneTransforms = _instance.transforms;
        }
        SaveToPath = "/myData/Recording_pID" + ParticipantID + "_" + System.DateTime.Now.ToString("HHmm") + ".csv";
        _filePath = Application.dataPath + SaveToPath;
    }

    // Start is called before the first frame update
    void Start()
    {
        _filePath = Application.dataPath + SaveToPath;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnApplicationQuit()
    {
        if (_is_recording) StopRecording();
    }

    public void StartRecording()
    {
        _is_recording = true;
        Debug.Log("Started recording.");
        InvokeRepeating("WriteDataToFile", 0f, SampleInterval);
    }

    public void StopRecording()
    {
        _is_recording = false;
        CancelInvoke("WriteDataToFile");
        Debug.Log("Stopped recording.");
    }

    string ConvertTransformsToString(Transform[] transforms)
    {
        string formattedTransforms = "";

        for (int i = 0; i < transforms.Length; i++)
        {
            formattedTransforms += transforms[i].localPosition.x + Delimiter;
            formattedTransforms += transforms[i].localPosition.y + Delimiter;
            formattedTransforms += transforms[i].localPosition.z + Delimiter;
            formattedTransforms += transforms[i].localRotation.x + Delimiter;
            formattedTransforms += transforms[i].localRotation.y + Delimiter;
            formattedTransforms += transforms[i].localRotation.z + Delimiter;
            formattedTransforms += transforms[i].localRotation.w + Delimiter;
        }

        return formattedTransforms;
    }

    void WriteDataToFile()
    {
        if (!File.Exists(_filePath))
        {
            using (StreamWriter sw = File.CreateText(_filePath))
            {
                string header = "";
                for (int i = 0; i < BoneTransforms.Length; i++)
                {
                    header += $"bone{i}_px" + Delimiter + $"bone{i}_py" + Delimiter + $"bone{i}_pz" + Delimiter;
                    header += $"bone{i}_rx" + Delimiter + $"bone{i}_ry" + Delimiter + $"bone{i}_rz" + Delimiter + $"bone{i}_rw";
                }
                sw.WriteLine(header);
            }
            Debug.Log($"New file created at {_filePath}.");
        }

        using (StreamWriter sw = File.AppendText(_filePath))
        {
            sw.WriteLine(ConvertTransformsToString(BoneTransforms));
        }
    }
}
