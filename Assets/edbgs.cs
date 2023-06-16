using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using static System.Net.WebRequestMethods;

public static class edbgs
{
    public static string ConvertSpaces(string input)
    {
        return input.Replace(" ", "%20");
    }

    public static string GetFactionByName(string name)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"https://elitebgs.app/api/ebgs/v5/factions?name={ConvertSpaces(name)}");
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        return reader.ReadToEnd();
    }
}
