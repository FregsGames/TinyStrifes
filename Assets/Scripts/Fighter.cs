using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour, IComparer<Fighter>, IComparer
{
    public string fighterName;
    public Attack[] attacks;
    public Sprite image;

    //Base stats
    public int baseStrength, baseDefense, baseSpeed, baseHps, baseEnergy;

    //Combat stats
    private int strength, defense, speed, hps, energy;
    private bool defeated;

    public Attack lastAttackReceived;
    public Attack lastSkillDone;

    public UnityEngine.Events.UnityEvent OnTurnStarted;
    public UnityEngine.Events.UnityEvent OnTurnEnded;
    public UnityEngine.Events.UnityEvent OnAttackReceived;
    public UnityEngine.Events.UnityEvent OnAttackReceivedEnded;
    public UnityEngine.Events.UnityEvent OnS;
    public UnityEngine.Events.UnityEvent OnSkillDone;
    public UnityEngine.Events.UnityEvent OnDeath;
    public UnityEngine.Events.UnityEvent OnTargeted;
    public UnityEngine.Events.UnityEvent OnUntargeted;

    private void Start()
    {
        hps = baseHps;
        defense = baseDefense;
        speed = baseSpeed;
        energy = baseEnergy;
        strength = baseStrength;
    }
    public void FighterTurnStart()
    {
        OnTurnStarted.Invoke();
    }
    public void FighterTurnEnd()
    {
        OnTurnEnded.Invoke();
    }
    public IEnumerator TakeDamage(Attack attack, float multiplier, int attackStr)
    {
        lastAttackReceived = attack;
        OnAttackReceived.Invoke();

        float newHPS = hps - Mathf.Clamp(attack.baseDamage * multiplier - defense + attackStr, 0, 10000);
        if (newHPS < 0)
            newHPS = 0;

        //Decrease health slowly (bar representation)
        float hpsFloat = hps;

        while (hpsFloat > newHPS)
        {
            hpsFloat -= Time.deltaTime * 200f;
            hps = (int) hpsFloat;
            yield return 0;
        }
        hps = (int)newHPS;

        if (hps <= 0)
        {
            UI_TextsDuringCombat.instance.ShowTurnInfo(fighterName + " defeated!");
            yield return new WaitForSeconds(2);
            OnDeath.Invoke();
            defeated = true;
            hps = 0;
        }
        OnAttackReceivedEnded.Invoke();
        OnUntargeted.Invoke();
        yield return 0;
    }
    public IEnumerator Heal(Attack attack, float multiplier)
    {
        lastSkillDone = attack;
        OnSkillDone.Invoke();

        if (attack.isRecover)
        {
            energy = baseEnergy;
        }

        float newHPS = hps + attack.baseHealing * multiplier;
        if (newHPS > baseHps)
            newHPS = baseHps;

        //Increase health slowly (bar representation)
        float hpsFloat = (float)hps;

        while (hpsFloat < newHPS)
        {
            Debug.Log(hps + " " + newHPS);

            hpsFloat += Time.deltaTime * 10f;
            hps = (int)hpsFloat;
            yield return 0;
        }
        hps = (int)newHPS;
        OnAttackReceivedEnded.Invoke();
    }

    public void ConsumeEnergy(int amount)
    {
        energy -= amount;
    }
    public void TargetFighter()
    {
        OnTargeted.Invoke();
    }
    public void UnargetFighter()
    {
        OnUntargeted.Invoke();
    }
    public void ModifyStr(int points)
    {
        strength = Mathf.Clamp(strength + points, 0, 1000);
    }

    public void ModifyDef(int points)
    {
        defense = Mathf.Clamp(defense + points, 0, 1000);
    }

    public void ModifySpeed(int points)
    {
        speed = Mathf.Clamp(speed + points, 0, 1000);
    }

    #region Getters
    public string FighterName()
    {
        return fighterName;
    }

    public bool Defeated()
    {
        return defeated;
    }

    public int BaseEnergy()
    {
        return baseEnergy;
    }

    public int Energy()
    {
        return energy;
    }

    public int BaseHPS()
    {
        return baseHps;
    }

    public int BaseStrength()
    {
        return baseStrength;
    }

    public int BaseDefense()
    {
        return baseDefense;
    }

    public int BaseSpeed()
    {
        return baseSpeed;
    }
    public int HPS()
    {
        return hps;
    }

    public int Strength()
    {
        return strength;
    }

    public int Defense()
    {
        return defense;
    }

    public int Speed()
    {
        return speed;
    }
    #endregion

    public int Compare(Fighter x, Fighter y)
    {
        return x.Speed().CompareTo(y.Speed());
    }

    public int Compare(object x, object y)
    {
        return ((Fighter)x).Speed().CompareTo(((Fighter)y).Speed());
    }
}
