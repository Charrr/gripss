using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Neuron;
using System;
using System.IO;
using System.Linq;

public class GesturePlayback : MonoBehaviour
{
    [Tooltip("Avatar to apply gestures to.")]
    public GameObject Avatar;

    private NeuronTransformsInstance _instance;

    public Transform[] BoneTransforms;

    private List<float[,]> playbackSequence; // Sequence of transform values recorded in one frame

    public string Delimiter = ";";

    public float SecondsPerFrame = 0.1f;

    [Tooltip("Name of the txt/csv file to read data from.")]
    public string FileName = "";

    private string _filePath;

    private void OnValidate()
    {
        SecondsPerFrame = Mathf.Max(SecondsPerFrame, 0.01f);
        if (Avatar)
        {
            _instance = Avatar.GetComponent<NeuronTransformsInstance>();
            BoneTransforms = _instance.transforms;
        }

        _filePath = Application.dataPath + FileName;
    }
    // Start is called before the first frame update
    void Start()
    {
        _filePath = Application.dataPath + FileName;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartPlayback()
    {
        string data = ReadDataFromFile(_filePath);
        if (data != "")
        {
            playbackSequence = ParseDataToSequence(data);

            Debug.Log($"Playback sequence loaded. Frame count: {playbackSequence.Count}. Number of bones: {BoneTransforms.Length}.");
            Debug.Log($"Playback started. Playing one frame every {SecondsPerFrame} second(s).");

            StartCoroutine(ApplyPlaybackSequence());

            Debug.Log("Playback finished.");
        }
        else
        {
            Debug.LogError("Data is empty! Cannot start the playback.");
        }
    }

    public void StopPlayback()
    {

    }

    private IEnumerator ApplyPlaybackSequence()
    {
        for (int i = 0; i < playbackSequence.Count; i++)
        {
            Debug.Log($"Frame {i} playing.");
            ApplyTransformValues(playbackSequence[i]);
            yield return new WaitForSeconds(SecondsPerFrame);
        }
    }

    /// <summary>
    /// Apply stored transforms to each bone.
    /// </summary>
    /// <param name="transformValues">Stored structured transform values.</param>
    private void ApplyTransformValues(float[,] transformValues)
    {
        for (int i = 0; i < BoneTransforms.Length; i++)
        {
            if (BoneTransforms[i])
            {
                BoneTransforms[i].localPosition = new Vector3(transformValues[i, 0], transformValues[i, 1], transformValues[i, 2]);
                BoneTransforms[i].localRotation = new Quaternion(transformValues[i, 3], transformValues[i, 4], transformValues[i, 5], transformValues[i, 6]);
            }
        }
    }

    /// <summary>
    /// Shape a row of transform values into 2D so it's more intuitive to access.
    /// </summary>
    /// <param name="values">Transform values recorded in one frame. Typically of length 59*7 = 413.</param>
    /// <returns>Restructured transform values. Typically of dimension [59, 7].</returns>
    private float[,] ReshapeTransformValues(float[] values)
    {
        float[,] transformValues = new float[BoneTransforms.Length, 7];
        for (int i = 0; i < BoneTransforms.Length; i++)
        {
            transformValues[i, 0] = values[i * 7];
            transformValues[i, 1] = values[i * 7 + 1];
            transformValues[i, 2] = values[i * 7 + 2];
            transformValues[i, 3] = values[i * 7 + 3];
            transformValues[i, 4] = values[i * 7 + 4];
            transformValues[i, 5] = values[i * 7 + 5];
            transformValues[i, 6] = values[i * 7 + 6];
        }
        return transformValues;
    }

    private List<float[,]> ParseDataToSequence(string data)
    {
        if (data == "")
        {
            Debug.LogError("Data is empty! Cannot parse the data to sequence.");
        }

        List<float[,]> playbackSequence = new List<float[,]>();

        string[] rows = data.Split('\n'); // split data row by row, with each row corresponding to a timestamp 
        for (int i = 1; i < rows.Length; i++) // start with i = 1 as row[0] is the header
        {
            // skip the loop if this row is empty, typically the last row
            if (string.IsNullOrWhiteSpace(rows[i])) break;

            // split one row of data
            string[] values_str = rows[i].Split(char.Parse(Delimiter));

            // remove empty elements, typically at the end of each row
            values_str = values_str.Where(str => !string.IsNullOrWhiteSpace(str)).ToArray();

            // convert each value to float
            float[] values = Array.ConvertAll(values_str, str => float.Parse(str, System.Globalization.CultureInfo.InvariantCulture));

            playbackSequence.Add(ReshapeTransformValues(values));
        }
        return playbackSequence;
    }

    string ReadDataFromFile(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"Could not read data from file path {path}!");
            return "";
        }
        else
        {
            StreamReader sr = new StreamReader(path);
            return sr.ReadToEnd();
        }
    }
}
