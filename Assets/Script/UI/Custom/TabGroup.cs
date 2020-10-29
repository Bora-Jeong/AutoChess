using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public int defaultOnIndex;

    public Color[] tabIdleColor;
    public Color[] tabHoverColor;
    public Color[] tabActiveColor;

    public List<GameObject> objectsToSwap;

    public TabButton[] tabButtons;
    private TabButton selectedTab;

    private void Start()
    {
        OnTabSelected(tabButtons[defaultOnIndex]);
    }

    public void OnTabEnter(TabButton button)
    {
        UIManager.instance.PlayButtonHoverSfx();
        ResetTabs();
        if(selectedTab == null || button != selectedTab)
        {
            for(int i = 0; i < button.graphics.Length; i++)
            {
                button.graphics[i].color = tabHoverColor[i];
            }
        }
    }

    public void OnTabExit(TabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton button)
    {
        UIManager.instance.PlayButtonClickSfx();
        if(selectedTab != null)
        {
            selectedTab.DeSelect();
        }
        selectedTab = button;
        selectedTab.Select();
        ResetTabs();

        for(int i = 0; i < button.graphics.Length; i++)
        {
            button.graphics[i].color = tabActiveColor[i];
        }

        int index = 0;
        for(int i = 0; i < tabButtons.Length; i++)
        {
            if (tabButtons[i] == button)
            {
                index = i;
                break;
            }
        }

        for(int i = 0; i < objectsToSwap.Count; i++)
        {
            objectsToSwap[i].SetActive(i == index);
        }
    }

    public void ResetTabs()
    {
        foreach(TabButton button in tabButtons)
        {
            if (selectedTab != null && button == selectedTab) continue;
            for (int i = 0; i < button.graphics.Length; i++)
            {
                button.graphics[i].color = tabIdleColor[i];
            }
        }
    }
}
