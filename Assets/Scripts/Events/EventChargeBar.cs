using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventChargeBar : CombatEvent
{
    bool valueSelected, selectionProcess;
    public Image bar;
    public float speed = 9.0f;

    void Start()
    {
    }

    IEnumerator Test()
    {
        yield return StartCoroutine(MoveToPosition());
        yield return StartCoroutine(TriggerEvent());
        yield return StartCoroutine(QuitAndDestroy());
    }

    public override IEnumerator TriggerEvent()
    {
        selectionProcess = true;
        StartCoroutine(FillAndUnfill());
        StartCoroutine(CatchClick());
        while (!valueSelected)
            yield return 0;
        yield return StartCoroutine(ShowBonusText());
        yield return new WaitForSeconds(1f);
    }
    IEnumerator CatchClick()
    {
        while (!Input.GetMouseButton(0))
        {
            yield return 0;
        }
       selectionProcess = false;
       multiplier = 1f + bar.fillAmount;
       valueSelected = true;
    }
    IEnumerator FillAndUnfill()
    {
        while (selectionProcess)
        {
            while (bar.fillAmount > 0 && selectionProcess)
            {
                bar.fillAmount -= Time.deltaTime * speed;
                yield return 0;
            }
            speed = Random.Range(7.0f, 9.0f);

            while (bar.fillAmount < 1 && selectionProcess)
            {
                bar.fillAmount += Time.deltaTime * speed;
                yield return 0;
            }
            speed = Random.Range(6.0f, 9.0f);

            yield return 0;
        }

    }
}
