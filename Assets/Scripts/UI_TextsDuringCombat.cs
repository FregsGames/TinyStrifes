using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_TextsDuringCombat : MonoBehaviour
{
    //Panels that contains the information
    public GameObject attackInfo, turnInfo;
    //Attack info components
    public TextMeshProUGUI attackName, attackCost, attackTargetCount, attackDesc;
    //Turn info components
    public TextMeshProUGUI turnText;

    private bool showingAttackInfo;

    public void HidePanels()
    {
        attackInfo.SetActive(false);
        turnInfo.SetActive(false);
    }


    public void ShowAttackPanel()
    {
        attackInfo.SetActive(true);
        turnInfo.SetActive(false);
        HideAttackTooltip();
    }
    public void ShowTurnInfoPanel()
    {
        attackInfo.SetActive(false);
        turnInfo.SetActive(true);
    }
    public void ShowAttackTooltip(Attack attack)
    {
        if (showingAttackInfo)
            return;
        ShowAttackPanel();
        attackName.text = attack.attackName;
        attackCost.text = "Cost: " + attack.baseCost;
        if (attack.maxTargets > 0)
            attackTargetCount.text = "Max targets: " + attack.maxTargets;
        else
            attackTargetCount.text = "";
        attackDesc.text = attack.attackDesc;
    }
    public void HideAttackTooltip()
    {
        attackName.text = "";
        attackCost.text = "";
        attackTargetCount.text = "";
        attackDesc.text = "";
    }
    public void ShowTurnInfo(string info)
    {

        ShowTurnInfoPanel();
        turnText.text = info;
    }

    public IEnumerator ShowAttackUsedBy(Fighter fighter, Attack attack)
    {
        ShowTurnInfo(fighter.fighterName + " uses " + attack.attackName);
        showingAttackInfo = true;
        yield return new WaitForSeconds(2);
        showingAttackInfo = false;
    }

    //Singleton
    public static UI_TextsDuringCombat instance;
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
