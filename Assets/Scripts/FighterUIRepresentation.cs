using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FighterUIRepresentation : MonoBehaviour
{
    //UI References
    public Image fighterImage, lifeBar;
    public TextMeshProUGUI lifeText;
    private Animator animator;
    public Fighter fighter;
    bool currentlySelected, currentlyTargeted;
    public Sprite greenBar, redBar, yellowBar;

    public void SetFighter(Fighter fighter)
    {
        this.fighter = fighter;
        fighterImage.sprite = fighter.image;
        fighter.OnTurnStarted.AddListener(FightersTurn);
        fighter.OnTurnEnded.AddListener(FightersTurnEnd);
        fighter.OnAttackReceived.AddListener(PlayDamageAnimation);
        fighter.OnAttackReceivedEnded.AddListener(UpdateBarColor);
        fighter.OnSkillDone.AddListener(PlaySelfAnimation);
        fighter.OnDeath.AddListener(FighterDead);
        fighter.OnTargeted.AddListener(FighterHasBeenTargeted);
        fighter.OnUntargeted.AddListener(FighterHasBeenUnargeted);

        //Flip components fighters is on the right
        if (Combat._combat.TeamContains(fighter, false))
        {
            Debug.Log("Inverting " + fighter.fighterName);
            lifeBar.rectTransform.localScale = new Vector3(-1,1,1);
        }
            
    }

    public void Hover()
    {
        FightersHoveringController.instance.HoverFighter(fighter);
    }

    public void Unhover()
    {
        FightersHoveringController.instance.Unhover();
    }

    void FighterHasBeenTargeted()
    {
        currentlyTargeted = true;
        StartCoroutine(SelectedAnimation());
    }

    void FighterHasBeenUnargeted()
    {
        currentlyTargeted = false;
    }

    void UpdateBarColor()
    {
        if (fighter == null)
            return;
        Debug.Log("Updatin color " + (float)fighter.HPS() / fighter.BaseHPS());
        if ((float)fighter.HPS() / fighter.BaseHPS() > 0.66f)
        {
            lifeBar.sprite = greenBar;
            return;
        }

        if ((float)fighter.HPS() / fighter.BaseHPS() > 0.33f)
        {
            lifeBar.sprite = yellowBar;
            return;
        }

        lifeBar.sprite = redBar;
        return;
    }

    void Update()
    {
        if (fighter == null)
            return;
        lifeBar.fillAmount =  ((float)fighter.HPS() / fighter.BaseHPS());
        lifeText.text = fighter.HPS() + "/" + fighter.BaseHPS();
    }

    public void FightersTurn()
    {
        currentlySelected = true;
        StartCoroutine(IdleAnimation());
    }

    void FighterDead()
    {
        animator.SetTrigger("Death");
        fighterImage.enabled = false;
        lifeBar.enabled = false;
        lifeText.enabled = false;
    }

    public void FightersTurnEnd()
    {
        currentlySelected = false;
    }

    void PlayDamageAnimation()
    {
        animator.SetTrigger(fighter.lastAttackReceived.targetAnimationTrigger);
    }

    void PlaySelfAnimation()
    {
        animator.SetTrigger(fighter.lastSkillDone.casterAnimationTrigger);
    }

    IEnumerator IdleAnimation()
    {
        var originalPos = GetComponent<RectTransform>().anchoredPosition;
        while (currentlySelected)
        {
            GetComponent<RectTransform>().anchoredPosition = new Vector2(
                originalPos.x, originalPos.y + Mathf.Sin(Time.time * 10) * 10);
            yield return 0;
        }
        GetComponent<RectTransform>().anchoredPosition = originalPos;
    }

    IEnumerator SelectedAnimation()
    {
        var originalPos = GetComponent<RectTransform>().anchoredPosition;
        while (currentlyTargeted)
        {
            GetComponent<RectTransform>().anchoredPosition = new Vector2(
                originalPos.x + Mathf.Sin(Time.time * 10) * 10, originalPos.y);
            yield return 0;
        }
        GetComponent<RectTransform>().anchoredPosition = originalPos;
    }

    private void OnValidate()
    {
        var index = transform.GetSiblingIndex();
        switch (index)
        {
            case 0:
                GetComponent<RectTransform>().anchorMin = new Vector2(0.5f,1);
                GetComponent<RectTransform>().anchorMax = new Vector2(0.5f,1);
                GetComponent<RectTransform>().pivot = new Vector2(0.5f,1);
                GetComponent<RectTransform>().anchoredPosition = new Vector3(250, -100, 0);
                break;
            case 1:
                GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f);
                GetComponent<RectTransform>().anchorMax = new Vector2(0, 0.5f);
                GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
                GetComponent<RectTransform>().anchoredPosition = new Vector3(50, 0, 0);
                break;
            default:
                GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0);
                GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0);
                GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
                GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 20, 0);
                break;
        }

    }
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        var index = transform.GetSiblingIndex();
        switch (index)
        {
            case 0:
                GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1);
                GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1);
                GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1);
                GetComponent<RectTransform>().anchoredPosition = new Vector3(250, -100, 0);
                break;
            case 1:
                GetComponent<RectTransform>().anchorMin = new Vector2(0, 0.5f);
                GetComponent<RectTransform>().anchorMax = new Vector2(0, 0.5f);
                GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
                GetComponent<RectTransform>().anchoredPosition = new Vector3(50, 0, 0);
                break;
            default:
                GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0);
                GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0);
                GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
                GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 20, 0);
                break;
        }
    }
}
