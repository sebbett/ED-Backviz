using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

public static class edbgs_new
{

    /*public static SystemData.System GetSystemByName(string name)
    {
        SystemData.System value = null;
        API_GetSystemByName(name).ContinueWith(continuationAction: task =>
        {
            //URL should return JSON-formatted text, convert that into a usable object
            //value = SystemData.ConvertFromRequestJson(task.Result);
        });

        return value;
    }

    private static async Task<string> API_GetSystemByName(string name)
    {
        string url = $"https://elitebgs.app/api/ebgs/v5/systems?name={name.Replace(" ", "%20")}";
        return await API_GET(url);
    }

    private static async Task<string> API_GET(string url)
    {
        Debug.Log($"EDBGS REQUEST: {url}");
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        return await reader.ReadToEndAsync();
    }*/
}
