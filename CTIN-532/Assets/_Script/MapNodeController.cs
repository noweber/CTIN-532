using UnityEngine;
using UnityEngine.UI;

public class MapNodeController : MonoBehaviour
{
    public Player Owner;

    public Material[] OwnerMaterialsMap;

    public enum Player
    {
        Neutral = 0,
        Human = 1,
        AI = 2
    };

    private PlayerSelectionController playerSelection;

    public void Start()
    {
        PlayerSelectionController[] controllers = FindObjectsOfType<PlayerSelectionController>();
        foreach (var controller in controllers)
        {
            if (controller.Owner == MapNodeController.Player.Human)
            {
                playerSelection = controller;
            }
        }
    }

    public void SetOwner(Player player)
    {
        Owner = player;
        Debug.Log("Node converted to: " + Owner.ToString());
        if (OwnerMaterialsMap != null)
        {
            GetComponent<MeshRenderer>().material = OwnerMaterialsMap[(int)Owner];
        }
        else
        {
            Debug.LogError("Set of materials for each player is null when setting the owner of a map node.");
        }
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, 256.0f))
            {
                if (raycastHit.transform != null)
                {
                    if (transform.gameObject == raycastHit.transform.gameObject && playerSelection != null)
                    {
                        if (Owner == Player.Human)
                        {
                            Debug.LogError("A node was selected by the player via clicking.");
                            playerSelection.SelectedMapNode = this;
                        }
                    }
                }
            }
        }
    }

    public void SpawnUnit(GameObject unitPrefab, Sprite unitSprite, Transform parent)
    {
        GameObject spawnedUnit = Instantiate(unitPrefab, transform);
        spawnedUnit.transform.parent = parent;
        UnitController unitController = spawnedUnit.GetComponent<UnitController>();
        unitController.Owner = Owner;
        unitController.SetSprite(unitSprite);
    }

    public void OnTriggerEnter(Collider other)
    {
        // Convert the node to a team on collision with a unit:
        UnitController unitController = other.GetComponent<UnitController>();
        if (unitController != null)
        {
            if (unitController.Owner == Player.Human && Owner != Player.Human)
            {
                SetOwner(Player.Human);
            }
            else if (unitController.Owner == Player.AI && Owner != Player.AI)
            {
                SetOwner(Player.AI);
            }
        }
    }
}
