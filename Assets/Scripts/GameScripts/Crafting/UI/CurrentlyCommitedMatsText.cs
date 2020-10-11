using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

public class CurrentlyCommitedMatsText : MonoBehaviour
{
    public CraftingNavigation gridNav;
    private TextMeshPro text;

    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
    }

    void Update()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("Current Items: \n");

        for (int i = 0; i < gridNav.committedItems.Count; i++)
        {
            sb.Append(string.Format("{0} +\n", gridNav.committedItems[i].name));
        }

        text.text = sb.ToString();
    }
}
