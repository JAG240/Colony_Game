using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class FollowMouse : MonoBehaviour
{
    [SerializeField] Grid grid;
    private Tilemap ground;

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        transform.position = grid.WorldToCell(new Vector3(Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).x), Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition).y), 0f));
    }
}
