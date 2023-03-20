using UnityEngine;
using static MapNodeController;

public class UnitDefendLogic : BaseUnitLogic
{
    public float radius = 8f;

    protected override void SelectTarget()
    {
        var gameManager = FindObjectOfType<GameManager>();
        if (base.Owner == Player.Human)
        {
            base.Target = gameManager.GetClosestUnitByPlayer(this.transform.position, Player.AI).transform;
        }
        else
        {
            base.Target = gameManager.GetClosestUnitByPlayer(this.transform.position, Player.Human).transform;
        }

        if (Vector3.Distance(this.transform.position, base.Target.transform.position) > radius)
        {
            base.Target = null;
        }

        if (base.Target != null)
        {
            TargetCoordinates = new Vector2Int((int)Target.transform.position.x, (int)Target.transform.position.z);
        }
    }
}
