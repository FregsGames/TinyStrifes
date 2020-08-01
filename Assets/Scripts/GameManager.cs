using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject[] teamAFighters, teamBFighters;
    private GameObject[] instancesA, instancesB;
    private void OnEnable()
    {
        SceneManager.sceneLoaded += StartCombat;
    }

    public void StartCombat(Scene sc, LoadSceneMode lsm)
    {
        if (!sc.name.Equals("Game"))
            return;

        Combat._combat.teamA = new Fighter[teamAFighters.Length];
        instancesA = new GameObject[teamAFighters.Length];
        for (int i = 0; i < teamAFighters.Length; i++)
        {
            instancesA[i] = Instantiate(teamAFighters[i]);
            Combat._combat.teamA[i] = instancesA[i].GetComponent<Fighter>();
        }

        instancesB = new GameObject[teamBFighters.Length];
        Combat._combat.teamB = new Fighter[teamBFighters.Length];
        for (int i = 0; i < teamBFighters.Length; i++)
        {
            instancesB[i] = Instantiate(teamBFighters[i]);
            Combat._combat.teamB[i] = instancesB[i].GetComponent<Fighter>();
        }

        Combat._combat.StartCombat();
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void NextCombat()
    {
        teamBFighters = Tournament.instance.NextCombat();
        SceneManager.LoadScene("Game");
    }

    //Singleton
    public static GameManager instance;
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
