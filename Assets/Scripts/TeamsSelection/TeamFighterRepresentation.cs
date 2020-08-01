using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamFighterRepresentation : MonoBehaviour
{
    //This class represents the slots on the selecting fighters screen

    private TeamMaker tMaker;
    public Fighter fighter;
    private Image image;
    public Sprite emptysprite, highlighterdSprite;

    public GameObject fPrefab;

    private void Start()
    {
        image = GetComponent<Image>();
        emptysprite = image.sprite;
        tMaker = TeamMaker.instance;
    }

    public void AssignFighter(Fighter f)
    {
        fighter = f;
        image.sprite = fighter.image;
        Highlight(false);
    }

    public void OnClick()
    {
        if(fighter == null)
        {
            tMaker.SelectSlot(this);
        }
        else
        {
            fPrefab = null;
            image.sprite = emptysprite;
            fighter = null;
            tMaker.SelectSlot(this);
        }
    }

    public void Highlight(bool highlighting)
    {
        if (highlighting && fighter == null)
            image.sprite = highlighterdSprite;

        if (!highlighting && fighter == null)
            image.sprite = emptysprite;
    }

}
