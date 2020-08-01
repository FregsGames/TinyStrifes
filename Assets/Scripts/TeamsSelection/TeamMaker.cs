using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamMaker : MonoBehaviour
{
    //Button references
    public TeamFighterRepresentation[] teamA, teamB;
    public TeamFighterRepresentation selectedSlot;

    public GameObject[] allFighters; //Fighters Prefab
    public GameObject fighterSelectTeamsPrefab;
    public GameObject fightersPanel;

    private bool isTournament;

    private void Start()
    {
        if (FindObjectOfType<Tournament>() != null)
            isTournament = true;

        for (int i = 0; i < allFighters.Length; i++)
        {
            var fPrefab = Instantiate(fighterSelectTeamsPrefab, fightersPanel.transform).
                GetComponent<FighterSelectTeams>().fighterGO = allFighters[i];
            ;
        }
        SelectSlot(teamA[0]);
    }

    public void StartGame()
    {
        if (isTournament) {
            StartTournament();
            return;
        }


        List<GameObject> teamAPrefabs = new List<GameObject>();
        for (int i = 0; i < teamA.Length; i++)
        {
            if (teamA[i].fighter != null)
                teamAPrefabs.Add(teamA[i].fPrefab);
        }

        if (teamAPrefabs.Count == 0) //Check that there is at least 1 fighter on team A
            return;


        List<GameObject> teamBPrefabs = new List<GameObject>();
        for (int i = 0; i < teamB.Length; i++)
        {
            if (teamB[i].fighter != null)
                teamBPrefabs.Add(teamB[i].fPrefab);
        }

        if (teamBPrefabs.Count == 0) //Check that there is at least 1 fighter on team B
            return;

        GameManager.instance.teamAFighters = teamAPrefabs.ToArray();
        GameManager.instance.teamBFighters = teamBPrefabs.ToArray();

        MenuController.instance.StartGame();
    }

    void StartTournament()
    {
        List<GameObject> teamAPrefabs = new List<GameObject>();
        for (int i = 0; i < teamA.Length; i++)
        {
            if (teamA[i].fighter != null)
                teamAPrefabs.Add(teamA[i].fPrefab);
        }

        if (teamAPrefabs.Count == 0) //Check that there is at least 1 fighter on team A
            return;

        Tournament.instance.InitializeTournament();

        GameManager.instance.teamAFighters = teamAPrefabs.ToArray();
        GameManager.instance.teamBFighters = Tournament.instance.NextCombat();

        MenuController.instance.StartGame();
    }

    public void AddFighter(Fighter fighter, GameObject fighterGO)
    {
        if (selectedSlot != null)
        {
            if (FighterIsAlreadyAdded(fighter))
                return;
            selectedSlot.AssignFighter(fighter);
            selectedSlot.fPrefab = fighterGO;
            SelectNextEmptySlot();
        }
    }
    public void SelectNextEmptySlot()
    {
        for (int i = 0; i < teamA.Length; i++)
        {
            if (teamA[i].fighter == null)
            {
                SelectSlot(teamA[i]);
                return;
            }
        }

        if (isTournament)
        {
            selectedSlot = null;
            return;
        }

        for (int i = 0; i < teamB.Length; i++)
        {
            if (teamB[i].fighter == null)
            {
                SelectSlot(teamB[i]);
                return;
            }
        }

        selectedSlot = null;
    }
    public void SelectSlot(TeamFighterRepresentation tf)
    {
        if(selectedSlot != null)
            selectedSlot.Highlight(false);
        selectedSlot = tf;
        tf.Highlight(true);
    }

    bool FighterIsAlreadyAdded(Fighter f)
    {
        for (int i = 0; i < teamA.Length; i++)
        {
            if(f == teamA[i].fighter)
                return true;
        }

        if (isTournament)
            return false;
        for (int i = 0; i < teamB.Length; i++)
        {
            if (f == teamB[i].fighter)
                return true;
        }

        return false;
    }

    public static TeamMaker instance;
    //Singleton
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
}
