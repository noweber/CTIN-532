using UnityEngine;
using UnityEngine.UI;

public class MapNodeController : MonoBehaviour
{
    public AudioClip GainNodeSound;
    public AudioClip LoseNodeSound;
    public AudioClip SelectSound;

    public Player Owner;

    public Material[] OwnerMaterialsMap;

    public GameObject Select_Sphere;

    [HideInInspector]
    public bool isSelected = false;

    public MinimapHQIcon minimap_icon;

    private SelectedObjects playerSelection;

    private bool isCurrentlyCollidingWithTrigger;

    private float timeBetweenCollisionChecks = 0.5f;

    private float timeUntilNextCollisionCheck;


    public enum Player
    {
        Neutral = 0,
        Human = 1,
        AI = 2
    };

    public void Start()
    {
        playerSelection = FindObjectOfType<SelectedObjects>();
        SelectedObjects[] controllers = FindObjectsOfType<SelectedObjects>();
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
            minimap_icon.setOwner(player);
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
            if (Physics.Raycast(ray, out raycastHit))
            {
                if (raycastHit.transform.gameObject == gameObject)
                {
                    ToggleSelect();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (isCurrentlyCollidingWithTrigger)
        {
            timeUntilNextCollisionCheck -= Time.deltaTime;
        }
    }

    public virtual void OnTriggerStay(Collider other)
    {
        isCurrentlyCollidingWithTrigger = true;
        if (timeUntilNextCollisionCheck <= 0)
        {
            ConvertToTeamColor(other);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        ConvertToTeamColor(other);
    }

    public void OnTriggerExit(Collider other)
    {
        isCurrentlyCollidingWithTrigger = false;
        timeUntilNextCollisionCheck = timeBetweenCollisionChecks;
    }

    private void ConvertToTeamColor(Collider other)
    {
        // Convert the node to a team on collision with a unit:
        BaseUnitLogic unitController = other.GetComponent<BaseUnitLogic>();
        if (unitController != null)
        {
            if (unitController.Owner == Player.Human && Owner != Player.Human)
            {
                SetOwner(Player.Human);
                AudioManager.Instance.PlaySFX(GainNodeSound, 1.0f);
            }
            else if (unitController.Owner == Player.AI && Owner != Player.AI)
            {
                AudioManager.Instance.PlaySFX(LoseNodeSound, 1.0f);
                SetOwner(Player.AI);
            }
        }
    }

    public void Deselect()
    {
        Select_Sphere.SetActive(false);
        isSelected = false;
    }

    public void ToggleSelect()
    {
        if (Owner == Player.Human)
        {
            AudioManager.Instance.PlaySFX(SelectSound, 1.0f);
            if (isSelected)
            {
                playerSelection.SetSelectedMapNode(null);
                Deselect();
            }
            else
            {
                playerSelection.SetSelectedMapNode(this);
                Select_Sphere.SetActive(true);
                isSelected = true;
            }
        }
    }
}
