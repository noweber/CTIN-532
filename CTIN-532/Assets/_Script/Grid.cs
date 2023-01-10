using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{

    public int row = 10, col = 10;
    public GameObject tile;
    
    private float tileSize = 10;

    // Start is called before the first frame update
    void Start()
    {
        generate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void generate()
    {
        float x = transform.position.x,z = transform.position.z;
        float offset = tileSize * tile.transform.localScale.x;
        for(int i = 0; i < row; i++)
        {
            for(int j = 0;j<col; j++)
            {
                Instantiate(tile,new Vector3(x,transform.position.y,z) , Quaternion.identity, transform);
                z += tileSize;
            }
            z = 0;
            x += tileSize;
        }
    }
}
