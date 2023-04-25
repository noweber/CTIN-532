using Assets._Script;
using UnityEngine;

public class MapNodeController : MonoBehaviour
{
    public Player Owner;

    public Material[] OwnerMaterialsMap;

    public MeshRenderer OwnerRenderer;

    public GameObject Select_Sphere;

    [HideInInspector]
    public bool isSelected = false;

    public MinimapHQIcon minimap_icon;

    private SelectedObjects playerSelection;

    private float timeBetweenCollisionChecks = 1.0f;

    private float timeUntilNextCollisionCheck;

    public enum Player
    {
        Neutral = 0,
        Human = 1,
        AI = 2
    };

    private float secondsUntilNextNodeSound;

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
        //Debug.Log("Node converted to: " + Owner.ToString());
        if (OwnerMaterialsMap != null)
        {
            OwnerRenderer.material = OwnerMaterialsMap[(int)Owner];
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

    private void LateUpdate()
    {
        if (DependencyService.Instance.GameFsm().CurrentState != Assets._Script.Game.GameState.District || DependencyService.Instance.DistrictFsm().CurrentState != DistrictState.Play)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (timeUntilNextCollisionCheck > 0)
        {
            timeUntilNextCollisionCheck -= Time.deltaTime;
        }
        if (!CanPlayNodeSound())
        {
            UpdateNodeSoundTimer(Time.deltaTime);
        }
    }
    public void OnTriggerStay(Collider other)
    {
        CheckIfShouldConvertColor(other);
    }

    public void OnTriggerEnter(Collider other)
    {
        CheckIfShouldConvertColor(other);
    }

    public void OnTriggerExit(Collider other)
    {
        CheckIfShouldConvertColor(other);
    }

    private void CheckIfShouldConvertColor(Collider other)
    {
        if (timeUntilNextCollisionCheck <= 0)
        {
            timeUntilNextCollisionCheck = timeBetweenCollisionChecks;
            ConvertToTeamColor(other);
        }
    }

    private void ConvertToTeamColor(Collider other)
    {
        // Convert the node to a team on collision with a unit:
        UnitController unitController = other.GetComponent<UnitController>();
        if (unitController != null)
        {
            if (unitController.Owner == Player.Human && Owner != Player.Human)
            {
                if (CanPlayNodeSound())
                {
                    ResetNodeSoundTimer();
                    AudioManager.Instance.PlayWithRandomizedPitch(AudioManager.Instance.GainNodeSound);
                }
                SetOwner(Player.Human);
                PlayerResourcesManager.Instance.GetPlayerResourcesController(Player.Human).AddNode();
                PlayerResourcesManager.Instance.GetPlayerResourcesController(Player.AI).RemoveNode();
            }
            else if (unitController.Owner == Player.AI && Owner != Player.AI)
            {
                if (CanPlayNodeSound())
                {
                    ResetNodeSoundTimer();
                    AudioManager.Instance.PlayWithRandomizedPitch(AudioManager.Instance.LoseNodeSound);
                }
                SetOwner(Player.AI);
                PlayerResourcesManager.Instance.GetPlayerResourcesController(Player.Human).RemoveNode();
                PlayerResourcesManager.Instance.GetPlayerResourcesController(Player.AI).AddNode();
                if (playerSelection.SelectedMapNode == this)
                {
                    playerSelection.SetSelectedMapNode(null);
                }
            }
            unitController.SelectNewPathAndTarget();
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
            if (isSelected)
            {
                playerSelection.SetSelectedMapNode(null);
                Deselect();
            }
            else
            {
                if (CanPlayNodeSound())
                {
                    ResetNodeSoundTimer();
                    AudioManager.Instance.PlayWithRandomizedPitch(AudioManager.Instance.SelectSound);
                }
                playerSelection.SetSelectedMapNode(this);
                Select_Sphere.SetActive(true);
                isSelected = true;
            }
        }
    }

    private void ResetNodeSoundTimer()
    {
        secondsUntilNextNodeSound = timeBetweenCollisionChecks;
    }

    private void UpdateNodeSoundTimer(float seconds)
    {
        secondsUntilNextNodeSound -= seconds;
    }

    private bool CanPlayNodeSound()
    {
        return secondsUntilNextNodeSound <= 0;
    }
}
