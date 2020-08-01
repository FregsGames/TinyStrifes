using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterIA : MonoBehaviour
{
    public int tries; //Tries to get a random attack that is not the recovery one

    public Attack GetRandomUsableAttack(Fighter fighter)
    {
        var tries = 30;
        for(int i = 0; i < tries; i++)
        {
            var possibleAttack = fighter.attacks[Random.Range(0, fighter.attacks.Length)];
            if (possibleAttack.isRecover)
                continue;
            if (possibleAttack.baseCost <= fighter.Energy())
                return possibleAttack;
        }
        foreach(Attack attack in fighter.attacks)
        {
            if (attack.isRecover)
                return attack;
        }
        return null;
    }

    public int GetRandomUsableAttackIndex(Fighter fighter)
    {
        var tries = 30;
        for (int i = 0; i < tries; i++)
        {
            var possibleIndex = Random.Range(0, fighter.attacks.Length);
            var possibleAttack = fighter.attacks[possibleIndex];
            if (possibleAttack.isRecover)
                continue;
            if (possibleAttack.baseCost <= fighter.Energy())
                return possibleIndex;
        }

        for (int i = 0; i < fighter.attacks.Length; i++)
        {
            if (fighter.attacks[i].isRecover)
                return i;
        }
        return -1;
    }

    //Singleton
    public static FighterIA _fighterIA;
    void Awake()
    {
        if (_fighterIA == null)
        {
            _fighterIA = this;
        }
        else
        {
            Destroy(this);
        }
    }
}
