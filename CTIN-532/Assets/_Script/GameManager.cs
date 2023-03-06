using System.Collections.Generic;
using UnityEngine;
using static MapNodeController;

public class GameManager : MonoBehaviour
{
    public List<MapNodeController> Selected_Nodes;
    public MapNodeController[] MapNodes;

    public List<BaseUnitController> Player_Units;
    public List<BaseUnitController> Enemy_Units;

    public bool refreshGoal = true;
    public bool enermyRefreshGoal = true;

    private void Awake()
    {
        Selected_Nodes = new List<MapNodeController>();
    }

    private void Start()
    {
        FindMapNodes();
    }

    public void FindMapNodes()
    {
        if (MapNodes.Length == 0)
        {
            MapNodes = FindObjectsOfType<MapNodeController>();
        }
    }

    public MapNodeController GetRandomNodeByPlayerOrNeutral(Player owner)
    {
        FindMapNodes();
        List<MapNodeController> nodes = new();
        foreach (var node in MapNodes)
        {
            if (node.Owner == owner || node.Owner == Player.Neutral)
            {
                nodes.Add(node);
            }
        }
        if (nodes.Count == 0)
        {
            return null;
        }

        return nodes[Random.Range(0, nodes.Count)];
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
        FindMapNodes();
        if (MapNodes.Length == 0) return null;
        MapNodeController res = null;
        float dist = float.MaxValue;
        for (int i = 0; i < MapNodes.Length; i++)
        {
            if ((MapNodes[i].Owner == owner) == isAlly)
            {
                float cur = Vector3.Distance(MapNodes[i].transform.position, pos);
                if (cur < dist)
                {
                    dist = cur;
                    res = MapNodes[i];
                }
            }
        }
        return res;
    }

    public MapNodeController closestNode(Vector3 pos)
    {
        FindMapNodes();
        if (MapNodes.Length == 0) return null;
        MapNodeController res = null;
        float dist = float.MaxValue;
        for (int i = 0; i < MapNodes.Length; i++)
        {
            float cur = Vector3.Distance(MapNodes[i].transform.position, pos);
            if (cur < dist)
            {
                dist = cur;
                res = MapNodes[i];
            }
        }
        return res;
    }

    public BaseUnitController closestEnermy(Vector3 pos, MapNodeController.Player owner, bool repeat)
    {
        if (owner == MapNodeController.Player.Human)
        {
            if (Enemy_Units.Count == 0) return null;
            BaseUnitController res = null;
            float dist = float.MaxValue;
            for (int i = 0; i < Enemy_Units.Count; i++)
            {
                if (Enemy_Units[i].hunterTargeted && !repeat) continue;
                float cur = Vector3.Distance(Enemy_Units[i].transform.position, pos);
                if (cur < dist)
                {
                    dist = cur;
                    res = Enemy_Units[i];
                }
            }
            return res;
        }
        else
        {
            if (Player_Units.Count == 0) return null;
            BaseUnitController res = null;
            float dist = float.MaxValue;
            for (int i = 0; i < Player_Units.Count; i++)
            {
                if (Player_Units[i].hunterTargeted && !repeat) continue;
                float cur = Vector3.Distance(Player_Units[i].transform.position, pos);
                if (cur < dist)
                {
                    dist = cur;
                    res = Player_Units[i];
                }
            }
            return res;
        }

    }
}
