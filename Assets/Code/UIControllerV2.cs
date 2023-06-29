using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using eds;
using System;
using TMPro;


public class UIControllerV2 : MonoBehaviour
{
    [Serializable]
    public enum UIState
    {
        main,
        search,
        faction_details,
        faction_management,
        system_details,
        about
    }

    public UIState state;

    [Header("Search Bar")]
    public TMP_InputField searchInput;
    public Image searchProgressComplete;
    public Image searchProgressSpinner;
    public GameObject searchSuggestionsContainer;
    public GameObject searchSuggestionPrefab;
    private List<GameObject> searchSuggestionObjects;

    [Header("About")]
    public Canvas aboutCanvas;
    public Button aboutOpenButton;
    public Button aboutGithubButton;
    public Button aboutKofiButton;
    public Button aboutCloseButton;
}
