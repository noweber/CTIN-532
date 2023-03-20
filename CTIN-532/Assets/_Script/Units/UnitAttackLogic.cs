using UnityEngine;
using static MapNodeController;

public class UnitAttackLogic : BaseUnitLogic
{
    protected override void SelectTarget()
    {
        var gameManager = FindObjectOfType<GameManager>();
        // TODO: Refactor this to use a reference to the level and not the game manager.
        if (base.Owner == Player.Human)
        {
            base.Target = gameManager.GetClosestNodeByPlayerOrNeutral(this.transform.position, Player.AI).transform;
        }
        else
        {
            base.Target = gameManager.GetClosestNodeByPlayerOrNeutral(this.transform.position, Player.Human).transform;
        }
        TargetCoordinates = new Vector2Int((int)Target.transform.position.x, (int)Target.transform.position.z);
    }
}
