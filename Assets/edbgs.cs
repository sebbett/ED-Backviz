using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using static System.Net.WebRequestMethods;

public static class edbgs
{
    public static async void GetFactionByName(string name)
    {
        string url = $"https://elitebgs.app/api/ebgs/v5/factions?name={name.Replace(" ", "%20")}";
        string[] urls = new string[] {url};
        string[] result = await DownloadDataAsync(urls);

        foreach(string r in result)
        {

        }
    }

    public static async void GetFactionByID(string id)
    {
        string url = $"https://elitebgs.app/api/ebgs/v5/factions?id={id.Replace(" ", "%20")}";
        string[] urls = new string[] { url };
        string[] result = await DownloadDataAsync(urls);

        foreach (string r in result)
        {

        }
    }

    public static async void GetSystemByName(string name)
    {
        string url = $"https://elitebgs.app/api/ebgs/v5/systems?name={name.Replace(" ", "%20")}";
        string[] urls = new string[] { url };
        string[] result = await DownloadDataAsync(urls);

        foreach (string r in result)
        {

        }
    }

    public static async void GetSystemByID(string id)
    {
        string url = $"https://elitebgs.app/api/ebgs/v5/systems?id={id.Replace(" ", "%20")}";
        string[] urls = new string[] { url };
        string[] result = await DownloadDataAsync(urls);

        foreach (string r in result)
        {
            //EventManager.addSystem?.Invoke(SystemData.ConvertFromRequestJson(r));
        }
    }

    private static async Task<string[]> DownloadDataAsync(string[] urls)
    {
        List<Task<string>> tasks = new List<Task<string>>();

        foreach(string url in urls)
        {
            tasks.Add(Task.Run(() => MakeRequest(url)));
        }

        var results = await Task.WhenAll(tasks);

        List<string> value = new List<string>();
        foreach(var item in results)
        {
            value.Add(item);
        }

        return value.ToArray();
    }

    private static string MakeRequest(string url)
    {
        Debug.Log($"EDBGS REQUEST: {url}");
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        return reader.ReadToEnd();
    }
}
