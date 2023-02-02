using UnityEngine;
using UnityEngine.UI;

public class MapNodeController : MonoBehaviour
{
    public Player Owner;

    public Material[] OwnerMaterialsMap;

    public GameObject Select_Sphere;

    [HideInInspector]
    public bool isSelected = false;

    private GameManager gameManager;

    public enum Player
    {
        Neutral = 0,
        Human = 1,
        AI = 2
    };

    private PlayerSelectionController playerSelection;

    public void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
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
        /*if (Input.GetMouseButtonDown(0))
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
        }*/
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit))
            {
                if (raycastHit.transform.gameObject == gameObject)
                {
                    toggleSelect();
                }
            }
        }
    }

    public void SpawnUnit(GameObject unitPrefab, Sprite unitSprite, Transform parent)
    {
        GameObject spawnedUnit = Instantiate(unitPrefab, transform);
        spawnedUnit.transform.parent = parent;
        /*UnitController unitController = spawnedUnit.GetComponent<UnitController>();
        unitController.Owner = Owner;
        unitController.SetSprite(unitSprite);*/
    }

    public void OnTriggerEnter(Collider other)
    {
        // Convert the node to a team on collision with a unit:
        BaseUnitController unitController = other.GetComponent<BaseUnitController>();
        if (unitController != null)
        {
            if (unitController.Owner == Player.Human && Owner != Player.Human)
            {
                SetOwner(Player.Human);
                gameManager.enermyRefreshGoal = true;
                gameManager.refreshGoal = true;
            }
            else if (unitController.Owner == Player.AI && Owner != Player.AI)
            {
                SetOwner(Player.AI);
                gameManager.enermyRefreshGoal = true;
                gameManager.refreshGoal = true;
            }
        }

    }

    private void toggleSelect()
    {
        if (isSelected)
        {
            gameManager.Selected_Nodes.Remove(this);
            Select_Sphere.SetActive(false);
            isSelected = false;
        }
        else
        {
            gameManager.Selected_Nodes.Add(this);
            Select_Sphere.SetActive(true);
            isSelected = true;
        }
        gameManager.refreshGoal = true;
    }
}
