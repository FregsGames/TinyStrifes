using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    public Fighter[] teamA, teamB;
    public static Combat _combat;
    private List<Fighter> fightersOrder; //Sort by speed

    public void StartCombat()
    {
        CombatUIController._combatUIController.SetTeams(teamA, teamB);
        fightersOrder = new List<Fighter>();
        StoreFightersInSpeedList();

        fightersOrder.Sort((p1, p2) => -((Fighter)p1).baseSpeed.CompareTo(((Fighter)p2).baseSpeed));
        StartCoroutine(NextTurn(fightersOrder[0]));
    }

    void StoreFightersInSpeedList()
    {
        foreach (Fighter fA in teamA)
        {
            if(!fA.Defeated())
                fightersOrder.Add(fA);
        }
        foreach (Fighter fB in teamB)
        {
            if (!fB.Defeated())
                fightersOrder.Add(fB);
        }
    }

    IEnumerator NextTurn(Fighter fighter)
    {
        yield return 0;
        StopAllCoroutines();
        UI_TextsDuringCombat.instance.ShowTurnInfo(fighter.fighterName + " turn");
        fightersOrder.Remove(fighter);
        fighter.FighterTurnStart();

        if (fightersOrder.Count == 0)
            StoreFightersInSpeedList();

        fightersOrder.Sort((p1, p2) => -((Fighter)p1).Speed().CompareTo(((Fighter)p2).Speed()));

        if (fighter.Defeated())
        {
            StartCoroutine(NextTurn(fightersOrder[0]));
            yield break;
        }

        if (TeamContains(fighter, true)) //Player's turn
        {
            Debug.Log("Starting player turn");
            CombatUIController._combatUIController.SetTurn(fighter, true);
        }
        else //IA turn
        {
            Debug.Log("Starting IA turn");
            CombatUIController._combatUIController.SetTurn(fighter, false);
            CombatUIController._combatUIController.OnSkillSelectedByIA(
                (FighterIA._fighterIA.GetRandomUsableAttackIndex(fighter)));
        }

        yield return 0;
        fightersOrder.Sort((p1, p2) => ((Fighter)p1).Speed().CompareTo(((Fighter)p2).Speed()));
    }

    public IEnumerator ResolveTurn(Fighter caster, Attack attack, Fighter[] targets)
    {
        caster.ConsumeEnergy(attack.baseCost);
        caster.lastSkillDone = attack;
        caster.OnSkillDone.Invoke();
        SFXController.instance.PlayEffect(attack.selfSoundIndex);

        float multiplier = 1.00f;
        if(attack.multiplyEvent != null && TeamContains(caster,true))
        {
            var multiplyEvent = Instantiate(attack.multiplyEvent).GetComponent<CombatEvent>();
            yield return StartCoroutine(multiplyEvent.MoveToPosition());
            yield return StartCoroutine(multiplyEvent.TriggerEvent());
            multiplier = multiplyEvent.multiplier;
            yield return StartCoroutine(multiplyEvent.QuitAndDestroy());
        }

        yield return StartCoroutine(UI_TextsDuringCombat.instance.ShowAttackUsedBy(caster, attack));

        var tempAttack = attack.InstanceAttack(caster, targets);

        foreach (Fighter target in tempAttack.Target())
        {
            yield return StartCoroutine(ResolveAttackTarget(tempAttack, target, multiplier, caster.Strength()));
        }
        caster.FighterTurnEnd();
        CombatUIController._combatUIController.TurnEnded();
        StartCoroutine(NextTurn(fightersOrder[0]));
    }

    private IEnumerator ResolveAttackTarget(Attack attackInstance, Fighter target, float multiplier, int str)
    {
        if (attackInstance.isOffensive)
        {
            SFXController.instance.PlayEffect(attackInstance.attackSoundIndex);
            yield return target.TakeDamage(attackInstance, multiplier, str);
            if (target.Defeated() && (CheckADefeat()||CheckBDefeat()))
                EndCombat();
        }
        if (attackInstance.isDefensive)
        {
            yield return target.Heal(attackInstance, multiplier);
        }

        if (attackInstance.strenghtChange != 0)
            target.ModifyStr(attackInstance.strenghtChange);

        if (attackInstance.defenseChange != 0)
            target.ModifyDef(attackInstance.defenseChange);

        if (attackInstance.speedChange != 0)
            target.ModifySpeed(attackInstance.speedChange);

        yield return 0;
    }

    private void EndCombat()
    {
        if (CheckBDefeat())
        {
            StopAllCoroutines();
            CombatUIController._combatUIController.CombatEnd(true);

        }
        if (CheckADefeat())
        {
            StopAllCoroutines();
            CombatUIController._combatUIController.CombatEnd(false);
        }
    }

    private bool CheckADefeat()
    {
        foreach(Fighter teamAFighter in teamA)
        {
            if (!teamAFighter.Defeated())
            {
                return false;
            }
        }
        return true;
    }
    private bool CheckBDefeat()
    {
        foreach (Fighter teamBFighter in teamB)
        {
            if (!teamBFighter.Defeated())
            {
                return false;
            }
        }
        return true;
    }

    //Singleton
    void Awake()
    {
        if (_combat == null)
        {
            _combat = this;
        }
        else
        {
            Destroy(this);
        }
    }

    //Utilities
    public bool TeamContains(Fighter fighter, bool isTeamA)
    {
        if (isTeamA)
        {
            foreach(Fighter f in teamA)
            {
                if (f == fighter)
                    return true;
            }
            return false;
        }
        else
        {
            foreach (Fighter f in teamB)
            {
                if (f == fighter)
                    return true;
            }
            return false;
        }
    }
}
