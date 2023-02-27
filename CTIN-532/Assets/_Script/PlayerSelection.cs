using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelection : Singleton<PlayerSelection>
{
    public enum UnitLogic
    {
        Attack,
        Defend,
        Intercept,
        Hunt,
        Random
    }

    public UnitLogic SelectedLogic;

    public void SelectUnitLogic(UnitLogic selection)
    {
        SelectedLogic = selection;
    }
}
