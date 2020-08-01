using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CombatEvent : MonoBehaviour
{
    //This is the final position of the panel on the screen
    private Vector2 targetPosition = new Vector2(0, -170f);
    private Vector2 hidePosition = new Vector2(0, -1200);
    private Vector2 bonusTextHidePos = new Vector2(0, 50f);
    private Vector2 bonusTextShowPos = new Vector2(-800f, 50f);
    public float multiplier = 1.00f;
    public TextMeshProUGUI eventTextRef, bonusTextRef;
    public string eventText;

    private void Awake()
    {
        gameObject.transform.SetParent(FindObjectOfType<Canvas>().gameObject.transform, false);
        GetComponent<RectTransform>().anchoredPosition = hidePosition;
        eventTextRef.text = eventText;
        bonusTextRef.GetComponent<RectTransform>().anchoredPosition = bonusTextHidePos;
    }

    public IEnumerator ShowBonusText()
    {
        bonusTextRef.text = "Bonus X " + multiplier.ToString("F1") + "!!!";
        while(bonusTextRef.GetComponent<RectTransform>().anchoredPosition.x > bonusTextShowPos.x)
        {
            bonusTextRef.GetComponent<RectTransform>().anchoredPosition = 
                new Vector2(bonusTextRef.GetComponent<RectTransform>().anchoredPosition.x - Time.deltaTime * 3550f,
                bonusTextRef.GetComponent<RectTransform>().anchoredPosition.y);
            yield return 0;
        }
        bonusTextRef.GetComponent<RectTransform>().anchoredPosition = bonusTextShowPos;
    }


    //This corroutine is called by another class in order to show the event panel
    public IEnumerator MoveToPosition()
    {
        RectTransform rect = GetComponent<RectTransform>();
        Debug.Log(rect.anchoredPosition.y + " " + targetPosition.y);

        while (rect.anchoredPosition.y < targetPosition.y)
        {
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, 
                rect.anchoredPosition.y + Time.deltaTime * 2050);
            yield return 0;
        }

        rect.anchoredPosition = targetPosition;
        yield return 0;
    }

    private IEnumerator FadeOutText()
    {
        Color color = eventTextRef.color;
        while(color.a > 0)
        {
            color = new Color(color.r, color.g, color.b, color.a - Time.deltaTime * 10f);
            eventTextRef.color = color;
            yield return 0;
        }

        eventTextRef.color = new Color(1,1,1,0);
    }

    public IEnumerator QuitAndDestroy()
    {
        RectTransform rect = GetComponent<RectTransform>();
        Debug.Log(rect.anchoredPosition.y + " " + targetPosition.y);

        while (rect.anchoredPosition.y > hidePosition.y)
        {
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x,
                rect.anchoredPosition.y - Time.deltaTime * 3000);
            yield return 0;
        }

        Destroy(gameObject);

    }

    public virtual IEnumerator TriggerEvent() {
        yield return 0; 
    }
}
