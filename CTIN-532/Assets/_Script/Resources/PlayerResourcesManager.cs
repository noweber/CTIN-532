using UnityEngine;
using static MapNodeController;

public class PlayerResourcesManager : Singleton<PlayerResourcesManager>
{
    private PlayerResourcesController humanResourcesController;

    private PlayerResourcesController aiResourcesController;

    private void Start()
    {
        FetchResourceControllers();
    }

    public void ResetData()
    {
        humanResourcesController.ResetData();
        aiResourcesController.ResetData();
    }

    public PlayerResourcesController GetPlayerResourcesController(Player player)
    {
        if (player == Player.Human)
        {
            return humanResourcesController;
        }
        else if (player == Player.AI)
        {
            return aiResourcesController;
        }
        else
        {
            return null;
        }
    }

    private void FetchResourceControllers()
    {
        var playerResourceControllers = FindObjectsOfType<PlayerResourcesController>();
        foreach (var controller in playerResourceControllers)
        {
            if (controller.PlayerToControlResourceFor == Player.Human)
            {
                if (humanResourcesController != null)
                {
                    Debug.LogError("There are multiple resource controllers in the scene for the human player!");
                }
                humanResourcesController = controller;
            }
            if (controller.PlayerToControlResourceFor == Player.AI)
            {
                if (aiResourcesController != null)
                {
                    Debug.LogError("There are multiple resource controllers in the scene for the AI player!");
                }
                aiResourcesController = controller;
            }
        }
    }
}
