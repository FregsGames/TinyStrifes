using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FighterSelectTeams : MonoBehaviour
{
    //This class represents the different chooseable fighters on the selecting screen

    public GameObject fighterGO; //Prefab of the fighter
    private Fighter fighter;

    private void Start()
    {
        fighter = fighterGO.GetComponent<Fighter>();
        GetComponent<Image>().sprite = fighter.image;
    }

    public void OnClick()
    {
        TeamMaker.instance.AddFighter(fighter, fighterGO);
    }

    public void OnHover()
    {
        FightersHoveringController.instance.HoverBaseFighter(fighter);
    }

    public void OnUnhover()
    {
        FightersHoveringController.instance.Unhover();
    }
}
