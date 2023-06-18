using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using eds.api;
using Newtonsoft.Json.Linq;
using UnityEngine;

[System.Serializable]
public struct RequestParams
{
    public string key;
    public string value;

    public RequestParams(string key, string value)
    {
        this.key = key;
        this.value = value.Replace(" ", "%20");
    }
}

public static class Requests
{
    public static async Task GetSystemByName(string[] names)
    {
        List<RequestParams> requests = new List<RequestParams>();
        foreach(string s in names)
        {
            requests.Add(new RequestParams("name", s));
        }

        var results = await Task.Run(() => api.GetSystemAsync(requests.ToArray()));

        GameManager.Events.updateSystems(results);
    }
    public static async Task GetSystemByID(string[] ids)
    {
        List<RequestParams> requests = new List<RequestParams>();
        foreach (string s in ids)
        {
            requests.Add(new RequestParams("id", s));
        }

        var results = await Task.Run(() => api.GetSystemAsync(requests.ToArray()));

        GameManager.Events.updateSystems(results);
    }
    public static async Task GetFactionByName(string[] names)
    {
        List<RequestParams> requests = new List<RequestParams>();
        foreach (string s in names)
        {
            requests.Add(new RequestParams("name", s));
        }

        var results = await Task.Run(() => api.GetSystemAsync(requests.ToArray()));

        GameManager.Events.updateSystems(results);
    }
    public static async Task GetFactionByID(string[] ids)
    {
        List<RequestParams> requests = new List<RequestParams>();
        foreach (string s in ids)
        {
            requests.Add(new RequestParams("id", s));
        }

        var results = await Task.Run(() => api.GetSystemAsync(requests.ToArray()));

        GameManager.Events.updateSystems(results);
    }
}

namespace eds.api
{
    public static class api
    {
        public const string ebgs_url = "https://elitebgs.app/api/ebgs/v5/";

        public static async Task<eds.System[]> GetSystemAsync(params RequestParams[] requestParams)
        {
            return await API_GetSystem(requestParams);
        }
        public static async Task<eds.Faction[]> GetFactionAsync(params RequestParams[] requestParams)
        {
            return await API_GetFaction(requestParams);
        }

        public static async Task<eds.System[]> API_GetSystem(params RequestParams[] requestParams)
        {
            if (requestParams.Length > 0)
            {
                List<eds.System> systems = new List<eds.System>();

                //elitebgs will only allow us to query 9 systems at a time, so split the requests into pages
                int i = 0;
                var query = from s in requestParams let num = i++ group s by num / 9 into g select g.ToArray();
                RequestParams[][] requestPages = query.ToArray();

                //loop through each page
                for (int p = 0; p < requestPages.Length; p++)
                {
                    StringBuilder url = new StringBuilder(ebgs_url);
                    url.Append("systems");
                    //loop through each request on the page
                    for (int r = 0; r < requestPages[p].Length; r++)
                    {
                        if (r == 0)
                        {
                            url.Append($"?{requestPages[p][r].key}={requestPages[p][r].value.Replace(" ", "%20")}");
                        }
                        else
                        {
                            url.Append($"&{requestPages[p][r].key}={requestPages[p][r].value.Replace(" ", "%20")}");
                        }
                    }

                    //Make the request
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url.ToString());
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string json = await reader.ReadToEndAsync();

                    //Turn the 'docs' node in the json into individual json strings
                    var jsonObject = JObject.Parse(json);
                    JArray docsArray = (JArray)jsonObject["docs"];
                    foreach (JToken docToken in docsArray)
                    {
                        // Convert each element to a JSON string
                        string docJsonString = docToken.ToString();

                        // Do something with the JSON string
                        eds.System sys = Conversions.SystemFromJson(docJsonString);
                        systems.Add(sys);
                    }
                }

                return systems.ToArray();
            }
            else
            {
                Debug.LogError("api.API_GetSystemByName() - No RequestParam provided");
                return new eds.System[0];
            }
        }
        public static async Task<eds.Faction[]> API_GetFaction(params RequestParams[] requestParams)
        {
            if (requestParams.Length > 0)
            {
                List<eds.Faction> systems = new List<eds.Faction>();

                //elitebgs will only allow us to query 9 systems at a time, so split the requests into pages
                int i = 0;
                var query = from s in requestParams let num = i++ group s by num / 9 into g select g.ToArray();
                RequestParams[][] requestPages = query.ToArray();

                //loop through each page
                for (int p = 0; p < requestPages.Length; p++)
                {
                    StringBuilder url = new StringBuilder(ebgs_url);
                    url.Append("factions");
                    //loop through each request on the page
                    for (int r = 0; r < requestPages[p].Length; r++)
                    {
                        if (r == 0)
                        {
                            url.Append($"?{requestPages[p][r].key}={requestPages[p][r].value.Replace(" ", "%20")}");
                        }
                        else
                        {
                            url.Append($"&{requestPages[p][r].key}={requestPages[p][r].value.Replace(" ", "%20")}");
                        }
                    }

                    //Make the request
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url.ToString());
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string json = await reader.ReadToEndAsync();

                    //Turn the 'docs' node in the json into individual json strings
                    var jsonObject = JObject.Parse(json);
                    JArray docsArray = (JArray)jsonObject["docs"];
                    foreach (JToken docToken in docsArray)
                    {
                        // Convert each element to a JSON string
                        string docJsonString = docToken.ToString();

                        // Do something with the JSON string
                        eds.Faction fac = Conversions.FactionFromJson(docJsonString);
                        systems.Add(fac);
                    }
                }

                return systems.ToArray();
            }
            else
            {
                Debug.LogError("api.API_GetSystemByName() - No RequestParam provided");
                return new eds.Faction[0];
            }
        }
    }
}