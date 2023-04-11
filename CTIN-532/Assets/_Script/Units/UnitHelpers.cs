using System.Collections.Generic;
using UnityEngine;

namespace Assets._Script.Units
{
    public class UnitHelpers : MonoBehaviour
    {
        public static UnitController GetNearestHostileUnit(UnitController sourceUnit)
        {
            var allUnits = FindObjectsOfType<UnitController>();
            List<UnitController> unitsOfPlayer = new();
            foreach (var unit in allUnits)
            {
                if (unit.Owner != sourceUnit.Owner)
                {
                    if (unit != sourceUnit)
                    {
                        unitsOfPlayer.Add(unit);
                    }
                }
            }
            UnitController result = null;
            float distance = float.MaxValue;
            foreach (var unit in unitsOfPlayer)
            {
                float tempDistance = Vector3.Distance(sourceUnit.gameObject.transform.position, unit.transform.position);
                if (tempDistance <= distance)
                {
                    result = unit;
                    distance = tempDistance;
                }
            }
            return result;
        }
    }
}
