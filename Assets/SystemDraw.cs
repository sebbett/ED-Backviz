using UnityEngine;
using eds;
using eds.api;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections;
using System.Threading.Tasks;

public class SystemDraw : MonoBehaviour
{
    public float scrollSensitivity = 0.1f;
    public string json;
    private Camera mainCamera;
    public string[] requests;

    private void Awake()
    {
        GameManager.Events.updateSystems += updateSystems;
    }

    private void updateSystems(eds.System[] systems)
    {
        foreach(eds.System sys in systems)
        {
            Debug.Log(sys.name);
        }
    }

    private void Start()
    {
        StartCoroutine("MakeRequest");
    }

    private IEnumerator MakeRequest()
    {
        yield return new WaitForSeconds(5);
        _ = Requests.GetSystemByName(requests);
    }
}
