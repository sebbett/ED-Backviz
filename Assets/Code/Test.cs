using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Start()
    {
        TextAsset ta = Resources.Load<TextAsset>("faction_list");
        string text = ta.text;
        string[] lines = text.Split('\n');
        db.InsertFactions(lines);
    }
}
