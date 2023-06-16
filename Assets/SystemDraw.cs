using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemDraw : MonoBehaviour
{
    public float scrollSensitivity = 0.1f;
    public SystemData[] systemData;
    public GUIStyle style;
    public float offsetDamp = 0.01f;
    private Camera mainCamera;
    private float scale = 1.0f;

    private Vector3 currentOffset = Vector3.zero;
    private Vector3 wantedOffset = Vector3.zero;
    private SystemData selectedSystem;

    private void Start()
    {
        mainCamera = Camera.main;
        selectedSystem = systemData[0];
    }

    private void FixedUpdate()
    {
        wantedOffset = selectedSystem.coordinates;
        currentOffset = Vector3.Lerp(currentOffset, wantedOffset, offsetDamp);
    }

    private void OnGUI()
    {
        foreach (SystemData sd in systemData)
        {
            Vector2 wtsp = mainCamera.WorldToScreenPoint((sd.coordinates - currentOffset) * scale);
            Vector2 coords = new Vector2(wtsp.x, Screen.height - wtsp.y);
            string icon = "+";
            if (sd.id == selectedSystem.id) icon = "< + >";
            if(GUI.Button(new Rect(coords.x - 10, coords.y - 10, 20, 20), icon, style))
            {
                selectedSystem = sd;
            }
        }

        scale += Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity * scale;
        scale = Mathf.Max(scale, 0.001f);
        

        GUILayout.BeginArea(new Rect(5, 40, 300, 400));
        GUILayout.BeginVertical();
        GUILayout.Label($"Selected System: {selectedSystem.name}");
        GUILayout.Label($"ID: {selectedSystem.id}");
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}

[System.Serializable]
public struct SystemData
{
    public string id;
    public string name;
    public Vector3 coordinates;
}
