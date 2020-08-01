using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

//This class links the Unity UI with the combat logic 

public class CombatUIController : MonoBehaviour
{
    public GameObject teamAPanel, teamBPanel; // These panels contains the fighters of both teams
    public GameObject fighterRepresentationPrefab;
    public Sprite[] randomBackgrounds;
    public Image background, victoryDefeatText;
    public Sprite victoryImage, defeatImage;
    public GameObject goToMenuButton, nextCombatButton;
    public Sprite diamond, bar;
    private GameObject[] teamAFightersSlots, teamBFightersSlots; //Fighters slots

    //Variables related to the fighter attacking this turn
    public GameObject skillsPanel;
    private Button[] currentFighterSkills;
    public TextMeshProUGUI currentFighterName, currentFighterEnergyText;
    public Image currentFighterEnergy;
    private Fighter currentFighter;

    //Combat UI logic variables
    private bool selectingTargets, isPlayerTurn;
    private List<Fighter> selectedTargets;

    #region Setup
    //Setup methods
    private void Start()
    {
        if(randomBackgrounds.Length > 0)
            background.sprite = randomBackgrounds[UnityEngine.Random.Range(0,randomBackgrounds.Length)];
    }
    void StoreUIReferences()
    {
        currentFighterSkills = skillsPanel.GetComponentsInChildren<Button>();
    }
    public void SetSelectionBindings()
    {
        var i = 0;
        for (i = 0; i < teamAFightersSlots.Length; i++)
        {
            if (teamAFightersSlots[i].GetComponent<Button>().isActiveAndEnabled)
            {
                int x = i;
                teamAFightersSlots[x].GetComponent<Button>()
                    .onClick.AddListener(delegate() { SelectTarget(x); });
            }
        }
        var j = i;
        for (i = 0; i < teamBFightersSlots.Length; i++)
        {
            if (teamBFightersSlots[i].GetComponent<Button>().isActiveAndEnabled)
            {
                int y = j + i;
                int z = i;
                teamBFightersSlots[z].GetComponent<Button>()
                    .onClick.AddListener(delegate () { SelectTarget(y); });
            }
        }
    }
    public void SetTeams(Fighter[] teamA, Fighter[] teamB)
    {
        selectedTargets = new List<Fighter>();
        teamAFightersSlots = new GameObject[teamA.Length];
        for (int i = 0; i < teamA.Length; i++)
        {
            //Create fighter representation and assing fighter to it
            var fighterRepresentation = Instantiate(fighterRepresentationPrefab, teamAPanel.transform);
            fighterRepresentation.GetComponent<FighterUIRepresentation>().SetFighter(teamA[i]);
            teamAFightersSlots[i] = fighterRepresentation;
        }


        teamBFightersSlots = new GameObject[teamB.Length];
        for (int i = 0; i < teamB.Length; i++)
        {
            //Create fighter representation and assing fighter to it
            var fighterRepresentation = Instantiate(fighterRepresentationPrefab, teamBPanel.transform);
            fighterRepresentation.GetComponent<FighterUIRepresentation>().SetFighter(teamB[i]);
            teamBFightersSlots[i] = fighterRepresentation;
        }
        StoreUIReferences();
        SetSelectionBindings();
    } 
    #endregion
    public void SetTurn(Fighter fighter, bool isPlayerTurn) //Called each turn
    {
        currentFighter = fighter;
        this.isPlayerTurn = isPlayerTurn;
        //Set buttons
        for (int i = 0; i < currentFighterSkills.Length; i++)
        {
            if(fighter.attacks.Length -1 >= i)
            {
                currentFighterSkills[i].GetComponentInChildren<TextMeshProUGUI>().text
                                = fighter.attacks[i].attackName;
                currentFighterSkills[i].gameObject.SetActive(true);

                if(fighter.attacks[i].baseCost > fighter.Energy() ||! isPlayerTurn)
                    currentFighterSkills[i].interactable = false;
                else
                    currentFighterSkills[i].interactable = true;

                //Show special event icon if attack has it
                if(fighter.attacks[i].multiplyEvent != null)
                {
                    currentFighterSkills[i].GetComponentsInChildren<Image>()[1].enabled = true;
                    currentFighterSkills[i].GetComponentsInChildren<Image>()[1].sprite =
                        fighter.attacks[i].multiplyEventIcon;
                }
                else
                {
                    currentFighterSkills[i].GetComponentsInChildren<Image>()[1].enabled = false;
                }
            }
            else
            {
                currentFighterSkills[i].gameObject.SetActive(false);
            }
        }
        //Set energy and name
        currentFighterEnergy.fillAmount = (float)fighter.Energy()/fighter.baseEnergy;
        currentFighterEnergyText.text = fighter.Energy() + "/" + fighter.baseEnergy; 
        currentFighterName.text = fighter.FighterName();

        selectingTargets = false;
    }
    public void OnSkillPressed(int index)
    {
        if (selectingTargets)
            return;

        if (index > currentFighter.attacks.Length - 1)
        {
            Debug.LogError("Skill index out of bounds");
            return;
        }
        selectedTargets.Clear();
        selectingTargets = true;
        StartCoroutine(SelectingTargets(index, true));
    }
    public void OnSkillHovered(int index)
    {
        if (index > currentFighter.attacks.Length - 1 || selectingTargets)
            return;

        UI_TextsDuringCombat.instance.ShowAttackTooltip(currentFighter.attacks[index]);
    }
    public void OnSkillUnhovered()
    {
        if (selectingTargets)
            return;
        UI_TextsDuringCombat.instance.HideAttackTooltip();
    }
    public void CombatEnd(bool playerWin)
    {
        if (Tournament.instance != null && Tournament.instance.onTournament)
        {
            var tournamentEnded = Tournament.instance.IsLastCombat();
            if (!tournamentEnded && playerWin)
            {
                UI_TextsDuringCombat.instance.HidePanels();
                StartCoroutine(ShowNextCombatButton());
                return;
            }
        }

        if (playerWin)
            victoryDefeatText.sprite = victoryImage;
        else
            victoryDefeatText.sprite = defeatImage;

        UI_TextsDuringCombat.instance.HidePanels();
        StartCoroutine(ShowVictoryDefeat());
    }
    IEnumerator ShowNextCombatButton()
    {

        var rectButton = nextCombatButton.GetComponent<RectTransform>();
        while (rectButton.anchoredPosition.y < -400)
        {
            rectButton.anchoredPosition = new Vector2(rectButton.anchoredPosition.x,
                rectButton.anchoredPosition.y + Time.deltaTime * 400f);

            yield return 0;
        }

        rectButton.anchoredPosition = new Vector2(rectButton.anchoredPosition.x, -400);
    }
    IEnumerator ShowVictoryDefeat()
    {
        var rect = victoryDefeatText.GetComponent<RectTransform>();
        skillsPanel.SetActive(false);

        while (rect.anchoredPosition.y > -200)
        {
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x,
                rect.anchoredPosition.y - Time.deltaTime * 200f);

            yield return 0;
        }

        var rectButton = goToMenuButton.GetComponent<RectTransform>();
        while (rectButton.anchoredPosition.y < -200)
        {
            rectButton.anchoredPosition = new Vector2(rectButton.anchoredPosition.x,
                rectButton.anchoredPosition.y + Time.deltaTime * 400f);

            yield return 0;
        }

        rectButton.anchoredPosition = new Vector2(rectButton.anchoredPosition.x, -200);
    }

    public bool OnSkillSelectedByIA(int index)
    {
        Debug.Log("Skill nº " + index);
        if (selectingTargets)
            return false;

        if(index > currentFighter.attacks.Length - 1)
        {
            Debug.LogError("Skill index out of bounds");
            return false;
        }
        selectedTargets.Clear();
        selectingTargets = true;
        StartCoroutine(SelectingTargets(index, false));
        StartCoroutine(SelectFightersRandomly());

        return true;
    }
    
    //Selects random fighters until the attack is doable
    IEnumerator SelectFightersRandomly()
    {
        while (selectingTargets)
        {
            SelectTarget(UnityEngine.Random.Range(0,teamAFightersSlots.Length + teamBFightersSlots.Length));
            yield return 0;
        }
    }
    IEnumerator ShowSelectTargetText(int index, bool isTeamACharacter)
    {
        while (selectingTargets)
        {
            var selectTargetsCount = Mathf.Clamp(currentFighter.attacks[index].maxTargets,
                                0, GetAllAliveEnemies(true).Length);
            if (selectTargetsCount - selectedTargets.Count > 0)
                UI_TextsDuringCombat.instance.ShowTurnInfo("Select " +
                    (selectTargetsCount - selectedTargets.Count) + " targets");
            else
                UI_TextsDuringCombat.instance.ShowTurnInfo("Targets selected!");
            yield return 0;
        }
    }
    IEnumerator SelectingTargets(int index, bool isTeamACharacter)
    {
        Debug.Log("Selecting targets...");
        var selectedAttack = currentFighter.attacks[index];

        //Show info
        if (currentFighter.attacks[index].maxTargets > 0 && isTeamACharacter)
            StartCoroutine(ShowSelectTargetText(index, isTeamACharacter));

        //Target is choosable
        if (selectedAttack.canChooseTargets)
        {
            //A single target attack
            if (!selectedAttack.isArea && selectedAttack.isOffensive)
            {
                var target = GetFirstSelectedEnemy(isTeamACharacter);
                while (target == null)
                {
                    target = GetFirstSelectedEnemy(isTeamACharacter);
                    yield return 0;
                }
                
                StartCoroutine(Combat._combat.ResolveTurn(currentFighter, selectedAttack, new Fighter[] { target })) ;
                yield break;
            }

            //A single target skill (ally)
            if (!selectedAttack.isArea && selectedAttack.isDefensive)
            {
                var target = GetFirstSelectedAlly(isTeamACharacter);
                while (target == null)
                {
                    target = GetFirstSelectedAlly(isTeamACharacter);
                    yield return 0;
                }

                StartCoroutine(Combat._combat.ResolveTurn(currentFighter, selectedAttack, new Fighter[] { target }));
                yield break;
            }

            //Area skills
            if (selectedAttack.isArea)
            {
                //More than 1 enemy as targets
                if(selectedAttack.maxTargets > 0 && selectedAttack.affectsOnlyEnemies)
                {
                    var targets = GetXEnemies(selectedAttack.maxTargets, isTeamACharacter);
                    while (targets == null)
                    {
                        targets = GetXEnemies(selectedAttack.maxTargets, isTeamACharacter);
                        yield return 0;
                    }
                    StartCoroutine(Combat._combat.ResolveTurn(currentFighter, selectedAttack,
                        targets));
                    yield break;
                }

                //More than 1 ally as targets
                if (selectedAttack.maxTargets > 0 && selectedAttack.affectsOnlyAllies)
                {
                    var targets = GetXAllies(selectedAttack.maxTargets, isTeamACharacter);
                    while (targets == null)
                    {
                        targets = GetXAllies(selectedAttack.maxTargets, isTeamACharacter);
                        yield return 0;
                    }
                    StartCoroutine(Combat._combat.ResolveTurn(currentFighter, selectedAttack,
                        targets));
                    yield break;
                }
            }

            yield break;
        }

        //Target is not choosable

        //Selfcast
        if (!selectedAttack.isArea && selectedAttack.isSelfCast)
        {
            StartCoroutine(Combat._combat.ResolveTurn(currentFighter, selectedAttack, new Fighter[] { currentFighter }));
            yield break;
        }

        /// Affects all enemies
        if (selectedAttack.affectsOnlyEnemies && selectedAttack.maxTargets == 0)
        {
            StartCoroutine(Combat._combat.ResolveTurn(currentFighter, selectedAttack, GetAllAliveEnemies(isTeamACharacter)));
            yield break;
        }

        /// Affects all allies
        if (selectedAttack.affectsOnlyAllies && selectedAttack.maxTargets == 0)
        {
            StartCoroutine(Combat._combat.ResolveTurn(currentFighter, selectedAttack, GetAllAliveAllies(isTeamACharacter)));
            yield break;
        }

        /// Affects all fighters
        if (selectedAttack.maxTargets == 0)
        {
            StartCoroutine(Combat._combat.ResolveTurn(currentFighter, selectedAttack, GetAllAliveFighters()));
            yield break;
        }



        //An attack that does not let player choose a target //
        StartCoroutine(Combat._combat.ResolveTurn(currentFighter,selectedAttack,null));
        yield break;
    }
    public void TurnEnded()
    {
        selectingTargets = false;
        selectedTargets.Clear();
    }

    #region Selection
    // Selection management
    private void SelectTarget(int index)
    {
        if (selectingTargets)
        {
            if (index > teamAFightersSlots.Length - 1)
            {
                var fighter = Combat._combat.teamB[index - teamAFightersSlots.Length];
                //Target already selected
                if (selectedTargets.Contains(fighter))
                {
                    selectedTargets.Remove(fighter);
                    if(isPlayerTurn) fighter.UnargetFighter();
                    return;
                }

                //Select target
                selectedTargets.Add(fighter);
                if (isPlayerTurn) fighter.TargetFighter();
            }
            else
            {
                var fighter = Combat._combat.teamA[index];
                //Target already selected
                if (selectedTargets.Contains(fighter))
                {
                    selectedTargets.Remove(fighter);
                    if (isPlayerTurn) fighter.UnargetFighter();
                    return;
                }

                //Select target
                selectedTargets.Add(Combat._combat.teamA[index]);
                if (isPlayerTurn) fighter.TargetFighter();
            }
        }
    }

    private Fighter[] GetXEnemies(int count, bool teamACharacter)
    {
        List<Fighter> enemies = new List<Fighter>();
        var maxCount = count; //Used to check if there are some defeated enemies so count cannot be reached
        if (count > GetAllAliveEnemies(teamACharacter).Length)
            maxCount = GetAllAliveEnemies(teamACharacter).Length;

        for (int i = selectedTargets.Count - 1; i >= 0; i--)
        {
            if(Combat._combat.TeamContains(selectedTargets[i], !teamACharacter) &&
                !selectedTargets[i].Defeated())
            {
                enemies.Add(selectedTargets[i]);
            }
            else
            {
                selectedTargets.Remove(selectedTargets[i]);
            }
        }

        if(enemies.Count == maxCount)
            return enemies.ToArray();

        return null;
    }
    private Fighter[] GetXAllies(int count, bool isTeamACharacter)
    {
        List<Fighter> allies = new List<Fighter>();
        var maxCount = count; //Used to check if there are some defeated enemies so count cannot be reached
        if (count > GetAllAliveAllies(isTeamACharacter).Length)
            maxCount = GetAllAliveAllies(isTeamACharacter).Length;

        for (int i = selectedTargets.Count - 1; i >= 0; i--)
        {
            if (Combat._combat.TeamContains(selectedTargets[i], !isTeamACharacter) &&
                !selectedTargets[i].Defeated())
            {
                allies.Add(selectedTargets[i]);
            }
            else
            {
                selectedTargets.Remove(selectedTargets[i]);
            }
        }

        if (allies.Count == maxCount)
            return allies.ToArray();

        return null;
    }
    private Fighter[] GetAllAliveFighters()
    {
        List<Fighter> aliveFighers = new List<Fighter>();
        foreach(Fighter fighter in Combat._combat.teamA)
        {
            if (!fighter.Defeated())
                aliveFighers.Add(fighter);
        }
        foreach (Fighter fighter in Combat._combat.teamB)
        {
            if (!fighter.Defeated())
                aliveFighers.Add(fighter);
        }

        return aliveFighers.ToArray();
    }
    private Fighter[] GetAllAliveEnemies(bool isTeamACharacter)
    {
        List<Fighter> aliveEnemies = new List<Fighter>();
        if (isTeamACharacter)
        {
            foreach (Fighter fighter in Combat._combat.teamB)
            {
                if (!fighter.Defeated())
                    aliveEnemies.Add(fighter);
            }
            return aliveEnemies.ToArray();
        }
        else
        {
            foreach (Fighter fighter in Combat._combat.teamA)
            {
                if (!fighter.Defeated())
                    aliveEnemies.Add(fighter);
            }
            return aliveEnemies.ToArray();
        }

    }
    private Fighter[] GetAllAliveAllies(bool isTeamACharacter)
    {
        List<Fighter> aliveAllies = new List<Fighter>();
        if (isTeamACharacter)
        {
            foreach (Fighter fighter in Combat._combat.teamA)
            {
                if (!fighter.Defeated())
                    aliveAllies.Add(fighter);
            }

            return aliveAllies.ToArray();
        }
        else
        {
            foreach (Fighter fighter in Combat._combat.teamB)
            {
                if (!fighter.Defeated())
                    aliveAllies.Add(fighter);
            }

            return aliveAllies.ToArray();
        }

    }
    private Fighter GetFirstSelectedEnemy(bool isTeamACharacter)
    {
        if (isTeamACharacter)
        {
            foreach (Fighter fighter in selectedTargets)
            {
                foreach (Fighter enemy in Combat._combat.teamB)
                {
                    if (fighter == enemy && !fighter.Defeated())
                    {

                        return fighter;
                    }
                }
            }
            selectedTargets.Clear();
            return null;
        }
        else
        {
            foreach (Fighter fighter in selectedTargets)
            {
                foreach (Fighter enemy in Combat._combat.teamA)
                {
                    if (fighter == enemy && !fighter.Defeated())
                    {

                        return fighter;
                    }
                }
            }
            selectedTargets.Clear();
            return null;
        }

    }
    private Fighter GetFirstSelectedAlly(bool isTeamACharacter)
    {
        if (isTeamACharacter)
        {
            foreach (Fighter fighter in selectedTargets)
            {
                foreach (Fighter ally in Combat._combat.teamA)
                {
                    if (fighter == ally && !fighter.Defeated())
                    {
                        return fighter;
                    }
                }
            }
            selectedTargets.Clear();
            return null;
        }
        else
        {
            foreach (Fighter fighter in selectedTargets)
            {
                foreach (Fighter ally in Combat._combat.teamB)
                {
                    if (fighter == ally && !fighter.Defeated())
                    {
                        return fighter;
                    }
                }
            }
            selectedTargets.Clear();
            return null;
        }

    }
    #endregion


    public static CombatUIController _combatUIController;
    //Singleton
    void Awake()
    {
        if (_combatUIController == null)
        {
            _combatUIController = this;
        }
        else
        {
            Destroy(this);
        }
    }
}
