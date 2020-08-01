using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FightersHoveringController : MonoBehaviour
{
    public GameObject hoveringPanel;
    public TextMeshProUGUI nameText, strText, defText, speedText;

    public void HoverFighter(Fighter fighter)
    {
        nameText.text = fighter.fighterName;
        strText.text = "Strength: " + fighter.Strength();
        defText.text = "Defense: " + fighter.Defense();
        speedText.text = "Speed: " + fighter.Speed();

        hoveringPanel.SetActive(true);
    }

    public void HoverBaseFighter(Fighter fighter)
    {
        nameText.text = fighter.fighterName;
        strText.text = "Strength: " + fighter.baseStrength;
        defText.text = "Defense: " + fighter.BaseDefense();
        speedText.text = "Speed: " + fighter.baseSpeed;

        hoveringPanel.SetActive(true);
    }

    public void Unhover()
    {
        hoveringPanel.SetActive(false);
    }

    public static FightersHoveringController instance;
    //Singleton
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
}
