using System.Collections.Generic;
using UnityEngine;
using static MapNodeController;

namespace Assets._Script.Units
{
    public class UnitHelpers : MonoBehaviour
    {
        public static UnitController GetNearestHostileUnit(UnitController sourceUnit)
        {
            return GetNearestHostileUnit(sourceUnit.transform, sourceUnit.Owner);
        }

        public static UnitController GetNearestHostileUnit(Transform sourceTransform, Player sourcePlayer)
        {
            var allUnits = FindObjectsOfType<UnitController>();
            if (allUnits != null)
            {
                List<UnitController> unitsOfPlayer = new();
                foreach (var unit in allUnits)
                {
                    if (unit.Owner != sourcePlayer)
                    {
                        if (unit.transform != sourceTransform)
                        {
                            unitsOfPlayer.Add(unit);
                        }
                    }
                }
                UnitController result = null;
                float distance = float.MaxValue;
                foreach (var unit in unitsOfPlayer)
                {
                    float tempDistance = Vector3.Distance(sourceTransform.position, unit.transform.position);
                    if (tempDistance <= distance)
                    {
                        result = unit;
                        distance = tempDistance;
                    }
                }
                return result;
            }
            else
            {
                return null;
            }
        }


        public static MapNodeController GetRandomNodeByPlayerOrNeutral(Player owner)
        {
            var mapNodes = FindObjectsOfType<MapNodeController>();
            if (mapNodes != null)
            {
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

                return possibleNodes[Random.Range(0, possibleNodes.Count)];
            }
            else
            {
                return null;
            }
        }

        public static MapNodeController GetClosestNodeByPlayerOrNeutral(Vector3 fromPosition, Player owner)
        {
            var mapNodes = FindObjectsOfType<MapNodeController>();
            if (mapNodes != null)
            {
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
            else
            {
                return null;
            }
        }

        public static MapNodeController GetClosestNode(Vector3 pos)
        {
            var mapNodes = FindObjectsOfType<MapNodeController>();
            if (mapNodes != null)
            {
                if (mapNodes.Length == 0) return null;
                MapNodeController result = null;
                float dist = float.MaxValue;
                for (int i = 0; i < mapNodes.Length; i++)
                {
                    float cur = Vector3.Distance(mapNodes[i].transform.position, pos);
                    if (cur < dist)
                    {
                        dist = cur;
                        result = mapNodes[i];
                    }
                }
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
