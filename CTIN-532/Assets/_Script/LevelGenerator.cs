using UnityEngine;

/// <summary>
/// This class creates the game's entities procedurally which sets up a playable level within the game.
/// </summary>
public class LevelGenerator : MonoBehaviour
{
    /// <summary>
    /// This is a reference to a map generator which will create the underlying data structure for map data.
    /// The level will place game entities based on the map data.
    /// </summary>
    private MapGenerator mapGenerator;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        // Search the scene for a reference to the map generator:
        mapGenerator = FindObjectOfType<MapGenerator>();

        // Turn off the map generator's response to input so that the level generator can clear its entities before a new map is generated and add them back in afterwards.
        if(mapGenerator != null)
        {
            mapGenerator.RespondsToInputSystem = false;
        } else
        {
            Debug.LogWarning("The level generator was unable to find a reference to the map generator.");
        }
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        if (mapGenerator != null)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                mapGenerator.RegenerateRoomMap();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                mapGenerator.RegenerateCaveMap();
            }
        }
    }
}
