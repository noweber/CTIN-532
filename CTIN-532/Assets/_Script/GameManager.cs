using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<MapNodeController> Selected_Nodes;
    public MapNodeController[] All_Nodes;

    public List<BaseUnitController> Player_Units;
    public List<BaseUnitController> Enemy_Units;

    public bool refreshGoal = true;
    public bool enermyRefreshGoal = true;

    private void Awake()
    {
        Selected_Nodes = new List<MapNodeController>();
    }

    public void loadNodes()
    {
        if (All_Nodes.Length == 0)
        {
            All_Nodes = FindObjectsOfType<MapNodeController>();
        }
    }

    public MapNodeController getRandomSelectedNode()
    {
        if (Selected_Nodes.Count == 0) return null;
        return Selected_Nodes[Random.Range(0, Selected_Nodes.Count)];
    }

    public MapNodeController closestSelected(Vector3 pos, MapNodeController.Player owner, bool isAlly)
    {
        if (Selected_Nodes.Count == 0) return null;
        MapNodeController res = null;
        float dist = float.MaxValue;
        for (int i = 0; i < Selected_Nodes.Count; i++)
        {
            if ((Selected_Nodes[i].Owner == owner) == isAlly)
            {
                float cur = Vector3.Distance(Selected_Nodes[i].transform.position, pos);
                if (cur < dist)
                {
                    dist = cur;
                    res = Selected_Nodes[i];
                }
            }
        }
        return res;
    }

    public MapNodeController closestNode(Vector3 pos, MapNodeController.Player owner, bool isAlly)
    {
        loadNodes();
        if (All_Nodes.Length == 0) return null;
        MapNodeController res = null;
        float dist = float.MaxValue;
        for (int i = 0; i < All_Nodes.Length; i++)
        {
            if ((All_Nodes[i].Owner == owner) == isAlly)
            {
                float cur = Vector3.Distance(All_Nodes[i].transform.position, pos);
                if (cur < dist)
                {
                    dist = cur;
                    res = All_Nodes[i];
                }
            }
        }
        return res;
    }

    public BaseUnitController closestEnermy(Vector3 pos)
    {
        if (Enemy_Units.Count == 0) return null;
        BaseUnitController res = null;
        float dist = float.MaxValue;
        for (int i = 0; i < Enemy_Units.Count; i++)
        {
            float cur = Vector3.Distance(Enemy_Units[i].transform.position, pos);
            if (cur < dist)
            {
                dist = cur;
                res = Enemy_Units[i];
            }
        }
        return res;
    }
}
