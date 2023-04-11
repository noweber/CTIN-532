using System;
using System.Collections.Generic;
using UnityEngine;
using static MapNodeController;

public class GameManager : MonoBehaviour
{
    public int DistrictNumber = -1;

    //public List<MapNodeController> Selected_Nodes;
    //public MapNodeController[] MapNodes;

    //public List<BaseUnitController> Player_Units;
    //public List<BaseUnitController> Enemy_Units;

    public bool refreshGoal = true;
    public bool enermyRefreshGoal = true;

    // 0 - startmenu; 1 - tutorial; 200 - main game loop; 300 - end scene
    public int gameState = 0;

    public bool minimap_enabled = false;
    public bool nodeSelect_enabled = false;
    public bool cardSelect_enabled = false;
    public bool logicSelect_enabled = false;
    public bool spawn_enabled = false;
    public bool cameraControl_enabled = false;
    public bool mapGeneerate_enable = false;

    public bool level_reset = false;
    public bool card_reset = false;
    public bool resource_reset = false;
    public bool resource_reset_AI = false;

    private void Awake()
    {
        //Selected_Nodes = new List<MapNodeController>();
    }

    private void Start()
    {
        ResetDistrict();
    }

    public void ResetDistrict()
    {
        DistrictNumber = 0;
    }

    #region MapNode Manager

    public UnitController GetClosestUnitByPlayer(Vector3 position, Player owner)
    {
        List<UnitController> unitsOfPlayer = new();
        var units = FindObjectsOfType<UnitController>();
        foreach (var unit in units)
        {
            if (unit.Owner == owner)
            {
                unitsOfPlayer.Add(unit);
            }
        }
        UnitController result = null;
        float distance = float.MaxValue;
        foreach (var unit in unitsOfPlayer)
        {
            float tempDistance = Vector3.Distance(position, unit.transform.position);
            if (tempDistance <= distance)
            {
                result = unit;
                distance = tempDistance;
            }
        }
        return result;
    }

    public MapNodeController GetRandomNodeByPlayerOrNeutral(Player owner)
    {
        var mapNodes = FindObjectsOfType<MapNodeController>();
        List<MapNodeController> possibleNodes = new();
        foreach (var node in mapNodes)
        {
            if (node.Owner == owner || node.Owner == Player.Neutral)
            {
                possibleNodes.Add(node);
            }
        }
        if (possibleNodes.Count == 0)
        {
            return null;
        }

        return possibleNodes[UnityEngine.Random.Range(0, possibleNodes.Count)];
    }

    public MapNodeController GetClosestNodeByPlayerOrNeutral(Vector3 fromPosition, Player owner)
    {
        var mapNodes = FindObjectsOfType<MapNodeController>();
        MapNodeController result = null;
        float distance = float.MaxValue;
        for (int i = 0; i < mapNodes.Length; i++)
        {
            if (mapNodes[i].Owner == owner || mapNodes[i].Owner == Player.Neutral)
            {
                float tempDistance = Vector3.Distance(mapNodes[i].transform.position, fromPosition);
                if (tempDistance < distance)
                {
                    distance = tempDistance;
                    result = mapNodes[i];
                }
            }
        }
        return result;
    }

    public MapNodeController closestNode(Vector3 pos)
    {
        var mapNodes = FindObjectsOfType<MapNodeController>();
        if (mapNodes.Length == 0) return null;
        MapNodeController res = null;
        float dist = float.MaxValue;
        for (int i = 0; i < mapNodes.Length; i++)
        {
            float cur = Vector3.Distance(mapNodes[i].transform.position, pos);
            if (cur < dist)
            {
                dist = cur;
                res = mapNodes[i];
            }
        }
        return res;
    }

    #endregion

    #region Inpute Manager
    private void Update()
    {
        updateControls();
    }

    private void updateControls()
    {
        if (gameState >= 200 && gameState < 300)
        {
            enableAll();
        }
        if (gameState == 0 || gameState >= 300)
        {
            disableAll();
        }
    }

    public void enableAll()
    {
        minimap_enabled = true;
        nodeSelect_enabled = true;
        cardSelect_enabled = true;
        logicSelect_enabled = true;
        spawn_enabled = true;
        cameraControl_enabled = true;
        mapGeneerate_enable = true;
    }

    public void disableAll()
    {
        minimap_enabled = false;
        nodeSelect_enabled = false;
        cardSelect_enabled = false;
        logicSelect_enabled = false;
        spawn_enabled = false;
        cameraControl_enabled = false;
        mapGeneerate_enable = false;
    }

    public void resetGame()
    {
        SelectedObjects[] unit = FindObjectsOfType<SelectedObjects>();
        foreach (SelectedObjects obj in unit)
        {
            obj.reset();
            Destroy(obj.unitParent);
        }
        level_reset = true;
        card_reset = true;
        resource_reset = true;
        resource_reset_AI = true;
    }
    #endregion
}
