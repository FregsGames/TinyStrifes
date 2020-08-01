using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tournament : MonoBehaviour
{
    public CombatFights[] enemyFights;
    private int index;
    public GameObject[] team;
    public bool onTournament;

    public void InitializeTournament()
    {
        onTournament = true;
        index = 0;
    }

    public GameObject[] NextCombat()
    {
        return enemyFights[index].enemies;
    }

    public bool IsLastCombat()
    {
        if (enemyFights.Length - 1 == index)
            return true;
        index++;
        return false;
    }

    [System.Serializable]
    public class CombatFights
    {
        public GameObject[] enemies;
    }

    //Singleton
    public static Tournament instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }
}
