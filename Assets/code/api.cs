using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using eds;
using Newtonsoft.Json.Linq;
using UnityEngine;
using eds.api;
using UnityEngine.Rendering.VirtualTexturing;

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
        foreach (string s in names)
        {
            requests.Add(new RequestParams("name", s));
        }

        var results = await Task.Run(() => api.GetSystemAsync(requests.ToArray()));

        Game.Events.updateSystems?.Invoke(results);
    }
    public static async Task GetSystemByID(string[] ids)
    {
        List<RequestParams> requests = new List<RequestParams>();
        foreach (string s in ids)
        {
            requests.Add(new RequestParams("id", s));
        }

        var results = await Task.Run(() => api.GetSystemAsync(requests.ToArray()));

        Game.Events.updateSystems?.Invoke(results);
    }
    public static async Task GetSystemByID(string[] ids, Action<eds.System[]> callback)
    {
        List<RequestParams> requests = new List<RequestParams>();
        foreach (string s in ids)
        {
            requests.Add(new RequestParams("id", s));
        }

        var results = await Task.Run(() => api.GetSystemAsync(requests.ToArray()));

        callback(results);
    }
    public static async Task GetFactionByName(string[] names)
    {
        List<RequestParams> requests = new List<RequestParams>();
        foreach (string s in names)
        {
            requests.Add(new RequestParams("name", s));
        }

        var results = await Task.Run(() => api.GetFactionAsync(requests.ToArray()));

        Game.Events.updateFactions?.Invoke(results);
    } 
    public static async Task GetFactionByName(string[] names, Action<Faction[]> callback)
    {
        List<RequestParams> requests = new List<RequestParams>();
        foreach (string s in names)
        {
            requests.Add(new RequestParams("name", s));
        }

        var results = await Task.Run(() => api.GetFactionAsync(requests.ToArray()));

        callback(results);
    }
    public static async Task SearchFactionByName(string name, Action<Faction[]> callback)
    {
        List<RequestParams> requests = new List<RequestParams>();
        requests.Add(new RequestParams("beginsWith", name));

        var results = await Task.Run(() => api.GetFactionAsync(requests.ToArray()));

        callback(results);
    }
    public static async Task GetFactionByID(string[] ids)
    {
        List<RequestParams> requests = new List<RequestParams>();
        foreach (string s in ids)
        {
            requests.Add(new RequestParams("id", s));
        }

        var results = await Task.Run(() => api.GetFactionAsync(requests.ToArray()));

        Game.Events.updateFactions?.Invoke(results);
    }
}

#region blackbox eds.api
namespace eds.api
{
    public static class api
    {
        #pragma warning disable CS1998
        public static async Task<eds.System[]> GetSystemAsync(params RequestParams[] requestParams)
        {
            //Turn the 'docs' node in the json into individual json strings
            string[] response = API_Get("systems", requestParams);

            List<eds.System> systems = new List<eds.System>();
            foreach (string r in response)
            {
                var jsonObject = JObject.Parse(r);
                JArray docsArray = (JArray)jsonObject["docs"];
                foreach (JToken doc in docsArray)
                {
                    // Convert each element to a JSON string
                    string docJsonString = doc.ToString();

                    //Convert the string to an object and add it to the list
                    eds.System newSys = Conversions.SystemFromJson(docJsonString);

                    bool found = false;
                    foreach(eds.System sys in systems)
                    {
                        if(sys.id == newSys.id)
                        {
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        systems.Add(newSys);
                    }
                }
            }

            return systems.ToArray();
        }
        public static async Task<eds.Faction[]> GetFactionAsync(params RequestParams[] requestParams)
        {
            //Turn the 'docs' node in the json into individual json strings
            string[] response = API_Get("factions", requestParams);
            
            List<eds.Faction> factions = new List<eds.Faction>();
            foreach (string r in response)
            {
                var jsonObject = JObject.Parse(r);
                JArray docsArray = (JArray)jsonObject["docs"];
                foreach (JToken doc in docsArray)
                {
                    // Convert each element to a JSON string
                    string docJsonString = doc.ToString();

                    //Convert the string to an object and add it to the list
                    factions.Add(Conversions.FactionFromJson(docJsonString));
                }
            }

            return factions.ToArray();
        }
        #pragma warning restore CS1998

        public const string ebgs_url = "https://elitebgs.app/api/ebgs/v5/";
        //Agnostic API Getter
        public static string[] API_Get(string endpoint, params RequestParams[] requestParams)
        {
            if (requestParams.Length > 0)
            {
                //elitebgs will only allow us to query 9 systems at a time, so split the requests into pages
                int i = 0;
                var query = from s in requestParams let num = i++ group s by num / 10 into g select g.ToArray();
                RequestParams[][] requestPages = query.ToArray();

                //loop through each page
                List<string> responses = new List<string>();
                for (int p = 0; p < requestPages.Length; p++)
                {
                    StringBuilder url = new StringBuilder(ebgs_url);
                    url.Append(endpoint);
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
                    responses.Add(reader.ReadToEnd());
                }

                return responses.ToArray();
            }
            else
            {
                Debug.LogError("api.API_GetFaction() - No RequestParam provided");
                return null;
            }
        }
    }
}
#endregion