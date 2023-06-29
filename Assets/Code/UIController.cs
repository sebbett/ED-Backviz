using eds;
using eds.ui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public UIState uiState;

    [Header("Search Bar")]
    public Button searchButton;
    public TMP_InputField searchField;
    public GameObject factionIsTracked;
    public Image spinner;

    [Header("SearchSuggestions")]
    public GameObject searchSuggestionPrefab;
    public GameObject searchSuggestionBox;
    private List<GameObject> currentSearchSuggestions = new List<GameObject>();

    [Header("FactionDetails")]
    public Canvas factionDetailsCanvas;
    public TMP_Text factionDetailsName;
    public TMP_Text factionDetailsInfo;
    public Button factionDetailsClose;
    public Button factionDetailsTrack;

    [Header("Info Panel")]
    public Canvas infoPanel;
    public TMP_Text systemName;
    public Button copyButton;
    public Button expandButton;
    public Transform factionsList;
    public GameObject factionListButtonPrefab;
    public Color greenState, yellowState, blueState, redState, greyState;
    private List<GameObject> factionListButtons = new List<GameObject>();

    [Header("Expanded Info Panel")]
    public GameObject conflictWidgetContainer;
    public GameObject conflictWidgetPrefab;
    private List<GameObject> conflictWidgets = new List<GameObject>();

    [Header("About")]
    public Canvas aboutCanvas;
    public Button aboutButton;
    public Button githubButton;
    public Button kofiButton;
    public Button closeButton;

    [Header("Status")]
    public TMP_Text status;

    private eds.System selectedSystem;
    private string currentSearchValue;
    private string lastSearchValue;

    private void Awake()
    {
        Game.Events.updateFactions += updateFactions;
        Game.Events.updateGameStatus += updateGameStatus;
        Game.Events.sysButtonClicked += sysButtonClicked;

        //searchField.onSubmit.AddListener((query) => PerformSearch(query));
        searchField.onSelect.AddListener((value) => updateUiState(UIState.search));
        searchField.onDeselect.AddListener((value) => updateUiState(UIState.map));
        searchField.onValueChanged.AddListener((value) => updateCurrentSearchValue(value));

        aboutButton.onClick.AddListener(() => updateUiState(UIState.about));
        githubButton.onClick.AddListener(() => OpenGithub());
        kofiButton.onClick.AddListener(() => OpenKofi());
        closeButton.onClick.AddListener(() => updateUiState(UIState.map));
        copyButton.onClick.AddListener(() => CopySystemNameToClipboard());
        expandButton.onClick.AddListener(() => ToggleInfoPanelState());
    }

    private void ToggleInfoPanelState()
    {
        if (uiState == UIState.system_details) updateUiState(UIState.map);
        else updateUiState(UIState.system_details);
    }

    private void Update()
    {
        if (uiState == UIState.search)
        {
            if (currentSearchValue != lastSearchValue)
            {
                lastSearchValue = currentSearchValue;
                updateSearchSuggestions(currentSearchValue);
            }
        }

        infoPanel.GetComponent<Animator>().SetBool("isOpen", uiState == UIState.system_details);
    }

    private void updateCurrentSearchValue(string value)
    {
        currentSearchValue = value;
    }

    private void updateSearchSuggestions(string currentSearchValue)
    {
        _ = Requests.SearchFactionByName(currentSearchValue, (factions) => setSearchSuggestionsUiObjects(factions));
    }

    private void setSearchSuggestionsUiObjects(Faction[] factions)
    {
        foreach (GameObject go in currentSearchSuggestions)
        {
            Destroy(go);
        }

        List<GameObject> newSearchSuggestions = new List<GameObject>();
        foreach(Faction f in factions)
        {
            GameObject newSuggestion = Instantiate(searchSuggestionPrefab, searchSuggestionBox.transform.position, Quaternion.identity);
            newSuggestion.transform.SetParent(searchSuggestionBox.transform);
            newSuggestion.GetComponent<Button>().onClick.AddListener(() => PerformSearch(f.name));
            newSuggestion.GetComponentInChildren<TMP_Text>().text = f.name;
            newSearchSuggestions.Add(newSuggestion);
        }

        currentSearchSuggestions = newSearchSuggestions;
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

        if(factionListButtons.Count > 0)
        {
            foreach(GameObject go in factionListButtons)
            {
                Destroy(go);
            }
        }

        foreach(eds.System.Faction f in system.factions)
        {
            GameObject newListItem = Instantiate(factionListButtonPrefab);
            if (f.faction_id == system.controlling_minor_faction_id)
            {
                newListItem.GetComponentInChildren<TMP_Text>().text = $"<color=orange>{f.name}</color>";
            }
            else
            {
                newListItem.GetComponentInChildren<TMP_Text>().text = f.name;
            }

            string state = Game.Manager.FactionStateInSystem(f.faction_id, system.id);
            newListItem.transform.Find("$state").transform.Find("$label").GetComponent<TMP_Text>().text = state;
            newListItem.transform.Find("$state").GetComponent<Image>().color = GetStateColor(state);
            newListItem.transform.Find("$influence").GetComponent<TMP_Text>().text = Game.Manager.FactionInfluenceInSystem(f.faction_id, system.id);
            newListItem.GetComponent<Button>().onClick.AddListener(() => PerformSearch(f.name));
            newListItem.transform.SetParent(factionsList);
            factionListButtons.Add(newListItem);
        }

        if(conflictWidgets.Count > 0)
        {
            foreach(GameObject cw in conflictWidgets) Destroy(cw);
        }

        foreach(eds.System.Conflict conflict in system.conflicts)
        {
            GameObject newConflictWidget = Instantiate(conflictWidgetPrefab, Vector3.zero, Quaternion.identity);
            newConflictWidget.transform.Find("$score").GetComponent<TMP_Text>().text = $"{conflict.faction1.days_won}{conflict.faction2.days_won}";
            newConflictWidget.transform.Find("$side_a_name").GetComponent<TMP_Text>().text = conflict.faction1.name;
            newConflictWidget.transform.Find("$side_b_name").GetComponent<TMP_Text>().text = conflict.faction2.name;
            newConflictWidget.transform.Find("$side_a_stake").GetComponent<TMP_Text>().text = conflict.faction1.stake;
            newConflictWidget.transform.Find("$side_b_stake").GetComponent<TMP_Text>().text = conflict.faction2.stake;
            newConflictWidget.transform.Find("$type").GetComponent<TMP_Text>().text = conflict.type;
            newConflictWidget.transform.Find("$status").GetComponent<TMP_Text>().text = conflict.status;

            newConflictWidget.transform.SetParent(conflictWidgetContainer.transform);
            conflictWidgets.Add(newConflictWidget);
        }
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
        if (uiState != UIState.system_details)
        {
            if (!Game.Manager.FactionIsTracked(query))
            {
                updateUiState(UIState.map);
                if (currentSearchSuggestions.Count > 0)
                {
                    foreach (GameObject go in currentSearchSuggestions)
                    {
                        Destroy(go);
                    }
                }
                string[] request = new string[] { query };
                _ = Requests.GetFactionByName(request, (factions) => ShowFactionDetails(factions));
                Game.Events.updateGameStatus("Performing Search...");
            }
            else
            {
                StartCoroutine("ShowFactionIsTracked");
            }
        }
    }

    private void ShowFactionDetails(Faction[] factions)
    {
        if (factions.Length > 0)
        {
            Faction faction = factions[0];
            factionDetailsName.text = faction.name;
            factionDetailsInfo.text = ($"Home System: {faction.faction_presence[0].system_name}\nAllegiance: {faction.allegiance}\nGovernment: {faction.government}\nSystems: {faction.faction_presence.Count}\n");
            factionDetailsClose.onClick.AddListener(() => updateUiState(UIState.map));
            factionDetailsTrack.onClick.AddListener(() => FactionDetailsBeginTracking(faction));
            updateUiState(UIState.faction_details);
        }
    }

    public void FactionDetailsBeginTracking(Faction faction)
    {
        Game.Manager.AddFaction(faction);
        updateUiState(UIState.map);
    }

    public void updateUiState(UIState uiState)
    {
        this.uiState = uiState;

        switch (this.uiState)
        {
            case UIState.search:
                Game.Events.disableMovement();
                break;
            case UIState.faction_details:
                Game.Events.disableMovement();
                factionDetailsCanvas.enabled = true;
                break;
            case UIState.system_details:
                Game.Events.disableMovement();
                break;
            case UIState.about:
                Game.Events.disableMovement();
                aboutCanvas.enabled = true;
                break;
            case UIState.map:
                Game.Events.enableMovement();
                aboutCanvas.enabled = false;
                factionDetailsCanvas.enabled = false;
                Game.Events.updateGameStatus("Done.");
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

    public IEnumerator ShowFactionIsTracked()
    {
        factionIsTracked.SetActive(true);
        yield return new WaitForSeconds(3);
        factionIsTracked.SetActive(false);
        Game.Events.updateGameStatus("Done.");
    }

    public Color GetStateColor(string state)
    {
        state = state.ToLower();

        string[] greenStates = new string[] {"incursion","infested"};
        string[] yellowStates = new string[] {"blight","drought","outbreak","infrastructurefailure","naturaldisaster","revolution","coldwar","tradewar","pirateattack","terroristattack","retreat","unhappy","bust","civilunrest"};
        string[] blueStates = new string[] {"publicholiday","technologicalleap","historicevent","colonisation","expansion","happy","elated","boom","investment","civilliberty"};
        string[] redStates = new string[] {"war","civilwar","elections","despondent","famine","lockdown"};
        string[] greyStates = new string[] {"discontented","none"};

        Color value = new Color();

        if (greenStates.Contains(state)) value = greenState;
        else if (yellowStates.Contains(state)) value = yellowState;
        else if (blueStates.Contains(state)) value = blueState;
        else if (redStates.Contains(state)) value = redState;
        else value = greyState;

        return value;
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
        faction_details,
        system_details,
        about
    }
}