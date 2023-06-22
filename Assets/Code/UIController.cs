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
    public Image spinner;

    [Header("Info Panel")]
    public Canvas infoPanel;
    public TMP_Text systemName;
    public Button copyButton;
    public TMP_Text factionsList;

    [Header("About")]
    public Canvas aboutCanvas;
    public Button aboutButton;
    public Button githubButton;
    public Button kofiButton;
    public Button closeButton;

    [Header("Status")]
    public TMP_Text status;


    private eds.System selectedSystem;

    private void Awake()
    {
        Game.Events.updateFactions += updateFactions;
        Game.Events.updateGameStatus += updateGameStatus;
        Game.Events.sysButtonClicked += sysButtonClicked;
        searchField.onSubmit.AddListener((query) => PerformSearch(query));
        searchField.onSelect.AddListener((value) => updateUiState(UIState.search));
        searchField.onDeselect.AddListener((value) => updateUiState(UIState.map));

        aboutButton.onClick.AddListener(() => updateUiState(UIState.about));
        githubButton.onClick.AddListener(() => OpenGithub());
        kofiButton.onClick.AddListener(() => OpenKofi());
        closeButton.onClick.AddListener(() => updateUiState(UIState.map));
        copyButton.onClick.AddListener(() => CopySystemNameToClipboard());
    }

    private void CopySystemNameToClipboard()
    {
        if(selectedSystem.name != "" && selectedSystem.name != null)
            GUIUtility.systemCopyBuffer = selectedSystem.name;
    }

    private void sysButtonClicked(eds.System system)
    {
        infoPanel.enabled = true;
        systemName.text = system.name;

        selectedSystem = system;

        string factions = "";
        foreach(eds.System.Faction sf in system.factions)
        {
            if(sf.faction_id == system.controlling_minor_faction_id)
            {
                factions += $"<color=orange>{sf.name}\n[CONTROLLING]</color>\n\n";
            }
            else
            {
                factions += $"{sf.name}\n\n";
            }
        }
        factionsList.text = factions;
    }

    private void updateGameStatus(string status)
    {
        this.status.text = status;
        spinner.enabled = status.ToLower() != "done.";
    }

    private void updateFactions(Faction[] factions)
    {
        if (factions.Length < 1) StartCoroutine("ShowNoFactionFound");
    }

    public void PerformSearch(string query)
    {
        string[] request = new string[] { query };
        _ = Requests.GetFactionByName(request);
        Game.Events.updateGameStatus("Performing Search...");
    }

    public void updateUiState(UIState uiState)
    {
        this.uiState = uiState;

        switch (this.uiState)
        {
            case UIState.search:
                Game.Events.disableMovement();
                break;
            case UIState.about:
                Game.Events.disableMovement();
                aboutCanvas.enabled = true;
                break;
            case UIState.map:
                Game.Events.enableMovement();
                aboutCanvas.enabled = false;
                break;
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
        Game.Events.updateGameStatus("Done.");
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