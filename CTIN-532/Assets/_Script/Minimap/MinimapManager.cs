using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapManager : Singleton<MinimapManager>
{
    public float unit_scale;
    public float HQ_scale;
    public float unit_height;
    public float HQ_height;

    private GameManager game_manager;
    private CameraControl main_camera_control;

    public RectTransform miniMap_Rect;
    public Camera miniMap_Camera;
    public float map_height;

    private Vector2 localCood;
    private MapNodeController cur_Node;

    private void Start()
    {
        game_manager = FindObjectOfType<GameManager>();
        main_camera_control = FindObjectOfType<CameraControl>();
    }

    private void Update()
    {
        findMapNode();
        if (Input.GetMouseButtonDown(0) && game_manager.gameState>0 && game_manager.gameState <3 )
        {
            if(cur_Node != null)
            {
                main_camera_control.setFocus(cur_Node.transform.position);
                cur_Node.ToggleSelect();
            }
        }
    }

    private void findMapNode()
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(miniMap_Rect, Input.mousePosition, null, out localCood))
        {
            Rect imageRectSize = miniMap_Rect.rect;

            localCood.x = (localCood.x - imageRectSize.x) / imageRectSize.width;
            localCood.y = (localCood.y - imageRectSize.y) / imageRectSize.height;

            if (localCood.x >= 0 && localCood.x <= 1 && localCood.y >= 0 && localCood.y <= 1)
            {
                Vector3 mouse_worldPosition = miniMap_Camera.ScreenToWorldPoint(new Vector2(localCood.x * miniMap_Camera.pixelWidth,
                    localCood.y * miniMap_Camera.pixelHeight));
                mouse_worldPosition.y = map_height;

                if (cur_Node != null) { cur_Node.minimap_icon.highlighted(false); }
                cur_Node = game_manager.closestNode(mouse_worldPosition);
                cur_Node.minimap_icon.highlighted(true);
            }
            else
            {
                if (cur_Node != null)
                {
                    cur_Node.minimap_icon.highlighted(false);
                    cur_Node = null;
                }
            }
        }

    }
}
