using eds;
using eds.ui;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public UIState uiState;

    [Header("Search Bar")]
    public Button searchButton;
    public TMP_InputField searchField;
    public TMP_Text noFactionFound;

    private void Awake()
    {
        GameManager.Events.updateFactions += updateFactions;
        searchField.onSubmit.AddListener((query) => PerformSearch(query));
        searchField.onSelect.AddListener((value) => updateUiState(UIState.search));
        searchField.onDeselect.AddListener((value) => updateUiState(UIState.map));
    }

    private void updateFactions(Faction[] factions)
    {
        if (factions.Length < 1) StartCoroutine("ShowNoFactionFound");
    }

    public void PerformSearch(string query)
    {
        string[] request = new string[] { query };
        _ = Requests.GetFactionByName(request);
    }

    public void updateUiState(UIState uiState)
    {
        this.uiState = uiState;

        if(this.uiState == UIState.search || this.uiState == UIState.about)
        {
            GameManager.Events.disableMovement?.Invoke();
        }
        else
        {
            GameManager.Events.enableMovement?.Invoke();
        }
    }

    public void OpenKofi()
    {
        Application.OpenURL("https://ko-fi.com/sebinspace");
    }

    public void OpenGithub()
    {
        Application.OpenURL("https://github.com/sebbett/ED-Backviz");
    }

    public IEnumerator ShowNoFactionFound()
    {
        noFactionFound.enabled = true;
        yield return new WaitForSeconds(3);
        noFactionFound.enabled = false;
    }
}

//Example request calls
//_ = Requests.GetFactionByName("faction");
//_ = Requests.GetSystemByName("system");

namespace eds.ui
{
    [global::System.Serializable]
    public enum UIState
    {
        search,
        map,
        about
    }
}