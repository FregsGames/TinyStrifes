using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButtonInGame : MonoBehaviour
{
    public void OnClick()
    {
        GameManager.instance.GoToMenu();
    }

    public void OnClickNextCombat()
    {
        GameManager.instance.NextCombat();
    }
}
