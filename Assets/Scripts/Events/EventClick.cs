using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventClick : CombatEvent
{
    public GameObject diamondPrefab;
    public float duration = 6f;
    public int prefabsToInstanciate = 20; //A skull per diamond will be instanciated
    private int skullsDestroyed, diamondsDestroyed;

    private void Start()
    {
    }

    IEnumerator Test()
    {
        yield return StartCoroutine(MoveToPosition());
        yield return StartCoroutine(TriggerEvent());
        yield return StartCoroutine(QuitAndDestroy());
    }

    public void SkullDestroyed()
    {
        skullsDestroyed++;
    }

    public void DiamondDestroyed()
    {
        diamondsDestroyed++;
    }

    public override IEnumerator TriggerEvent()
    {
        yield return StartCoroutine(InstanciateDiamondsAndSkulls());
        var tempMultiplier = Mathf.Clamp((diamondsDestroyed * 2 -  skullsDestroyed) / (prefabsToInstanciate * 0.5f),0,2);
        multiplier = tempMultiplier;
        yield return StartCoroutine(ShowBonusText());
        yield return new WaitForSeconds(1f);
    }

    IEnumerator InstanciateDiamondsAndSkulls()
    {
        var instanciatedPrefabs = 0;
        var delayBeetweenPrefabs = duration / prefabsToInstanciate;
        var lastInstanciatedWasSkull = false;

        while(instanciatedPrefabs < prefabsToInstanciate)
        {
            //Instiance prefab and randomize its properties
            Instantiate(diamondPrefab, transform).GetComponent<Diamond>().Randomize(lastInstanciatedWasSkull);
            lastInstanciatedWasSkull = !lastInstanciatedWasSkull;
            instanciatedPrefabs++;
            yield return new WaitForSeconds(delayBeetweenPrefabs);
        }
        yield return new WaitForSeconds(2);
    }
}
