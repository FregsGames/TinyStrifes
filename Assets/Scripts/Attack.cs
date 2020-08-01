using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    //Attack properties
    public string attackName;
    public string attackDesc;
    public bool isOffensive;
    public bool isDefensive;
    public int strenghtChange; // Skills can modify fighters stats on combat
    public int defenseChange;
    public int speedChange;
    public bool isArea;
    public bool isSelfCast; //Skill has effect on user
    public bool canChooseTargets = true; //If area and !canChooseTargets it will affect to all fighters
    public bool affectsOnlyEnemies;
    public bool affectsOnlyAllies;
    public bool isRecover;
    public string targetAnimationTrigger = "Explode";
    public string casterAnimationTrigger = "Heal";
    public int attackSoundIndex = 3;
    public int selfSoundIndex = 0;

    public int baseCost;
    public int maxTargets; //If area and maxTargets = 0, it affects all fighters
    public int baseDamage;
    public int baseHealing;

    //Optional event triggered during combat (minigame)
    //It is a prefab gameobject with a CombatEvent script attached
    public GameObject multiplyEvent;
    public Sprite multiplyEventIcon;

    //Combat variables, used when an attack is instantiated 
    private Fighter[] target;
    private Fighter caster;
    private int damage;

    public Attack InstanceAttack(Fighter caster, Fighter[] target)
    {
        this.target = target;
        this.caster = caster;

        if(baseDamage > 0)
            damage = baseDamage + caster.Strength();

        return this;
    }

    public Fighter[] Target()
    {
        return target;
    }

    public Fighter Caster()
    {
        return caster;
    }
}
