using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Tile currentTile;
    public Tile start;

    public GameObject ArrowLeft;
    public GameObject ArrowRight;
    public GameObject ArrowUp;
    public GameObject ArrowDown;

    [HideInInspector]
    public int death = 0;
    [HideInInspector]
    public int woodCount = 0;
    [HideInInspector]
    public bool success = false;
    [HideInInspector]
    public bool hasTreature = false;

    private bool isStabled = false;
    private bool isInit = false;

    private Coroutine lastEnter;
    private Coroutine lastReplay;

    public int loadingCount = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(!isInit && loadingCount == 100) {
            lastEnter = StartCoroutine(enter(currentTile));
            isInit= true;
        }
        if (success)
        {
            restart();
        }
    }

    #region Coroutines
    IEnumerator enter(Tile t)
    {
        stopAllSound();
        yield return new WaitForSeconds(t.waitTime);
        // light center tile
        currentTile.select();
        yield return new WaitForSeconds(2);

        // left
        if (currentTile.left != null)
        {
            ArrowLeft.SetActive(true);
            currentTile.left.select();
            yield return StartCoroutine(playSound(currentTile.left));
        }

        // up
        if (currentTile.up != null)
        {
            ArrowUp.SetActive(true);
            currentTile.up.select();
            yield return StartCoroutine(playSound(currentTile.up));
        }


        // right
        if (currentTile.right != null)
        {
            ArrowRight.SetActive(true);
            currentTile.right.select();
            yield return StartCoroutine(playSound(currentTile.right));
        }

        // down
        if (currentTile.down != null)
        {
            ArrowDown.SetActive(true);
            currentTile.down.select();
            yield return StartCoroutine(playSound(currentTile.down));
        }
        isStabled = true;
        lastEnter= null;
    }

    IEnumerator playAll()
    {
        // left
        if (currentTile.left != null)
        {
            ArrowLeft.GetComponent<MeshRenderer>().material.color = Color.red;
            yield return StartCoroutine(playSound(currentTile.left));
            ArrowLeft.GetComponent<MeshRenderer>().material.color = Color.white;
        }

        // up
        if (currentTile.up != null)
        {
            ArrowUp.GetComponent<MeshRenderer>().material.color = Color.red;
            yield return StartCoroutine(playSound(currentTile.up));
            ArrowUp.GetComponent<MeshRenderer>().material.color = Color.white;
        }


        // right
        if (currentTile.right != null)
        {
            ArrowRight.GetComponent<MeshRenderer>().material.color = Color.red;
            yield return StartCoroutine(playSound(currentTile.right));
            ArrowRight.GetComponent<MeshRenderer>().material.color = Color.white;
        }

        // down
        if (currentTile.down != null)
        {
            ArrowDown.GetComponent<MeshRenderer>().material.color = Color.red;
            yield return StartCoroutine(playSound(currentTile.down));
            ArrowDown.GetComponent<MeshRenderer>().material.color = Color.white;
        }

        lastReplay = null;
    }

    IEnumerator playSound(Tile tile)
    {
        tile.play();
        yield return new WaitForSeconds(Mathf.Min(5f,tile.getLength()));
        tile.stop();
        yield return new WaitForSeconds(2);
    }
    #endregion

    #region Input Callback

    void OnExit()
    {
        Application.Quit();
    }

    void OnReplay()
    {
        if (isStabled && lastReplay == null)
        {
            StopAllCoroutines();
            stopAllSound();
            lastReplay = StartCoroutine(playAll());
        }
    }

    void OnUp()
    {
         movement(currentTile.up, transform.position + new Vector3(0, 0, 10));
    }

    void OnDown()
    {
        movement(currentTile.down, transform.position + new Vector3(0, 0, -10));
    }

    void OnLeft()
    {
        movement(currentTile.left, transform.position + new Vector3(-10, 0, 0));
    }

    void OnRight()
    {
        movement(currentTile.right, transform.position + new Vector3(10, 0, 0));            

    }
    #endregion

    #region helper funcs
    private void movement(Tile tile, Vector3 target)
    {
        if (tile != null)
        {
            int result = tile.entered(this);
            
            // blocked
            if (result == 1)
                return;
            // dead
            if (result == 2)
            {
                move(start, Vector3.zero);
                return;
            }
            // move
            else
            {              
                move(tile, target);
            }
        }
    }

    private void move(Tile tile, Vector3 target)
    {
        if (lastEnter != null)
        {
            StopCoroutine(lastEnter);
            lastEnter= null;
        }
        if (lastReplay != null)
        {
            StopCoroutine(lastReplay);
            lastReplay= null;
        }
        disableAllArrow();
        currentTile.highLightOff();
        currentTile = tile;
        transform.position = target;
        isStabled = false;
        if(lastEnter == null)
            lastEnter = StartCoroutine(enter(tile));
    }

    private void stopAllSound()
    {
        if(currentTile.up != null)
            currentTile.up.stop();
        if (currentTile.left != null)
            currentTile.left.stop();
        if (currentTile.right != null)
            currentTile.right.stop();
        if (currentTile.down != null)
            currentTile.down.stop();
    }

    private void disableAllArrow()
    {
        ArrowDown.SetActive(false);
        ArrowLeft.SetActive(false);
        ArrowRight.SetActive(false);
        ArrowUp.SetActive(false);
    }

    private void restart()
    {
        death = 0;
        woodCount = 0;
        success = false;
        hasTreature = false;

        isStabled = false;
        move(start, Vector3.zero);
    }
    #endregion
}
