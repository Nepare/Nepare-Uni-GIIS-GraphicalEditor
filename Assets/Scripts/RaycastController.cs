using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastController : MonoBehaviour
{
    private RaycastHit2D _hit;
    private Camera _cam;
    public static List<int> coords;

    void Awake()
    {
        _cam = GetComponent<Camera>();   
    }

    private void Update() {
        Vector2 CurMousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
        _hit = Physics2D.Raycast(CurMousePos, Vector2.zero);

        if (_hit.transform != null)
        {
            coords = _hit.transform.gameObject.GetComponent<PixelController>().GetCoords();
        }
        else
        {
            coords = null;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (coords != null)
            {
                // Debug.Log("Pixel selected: (" + coords[0].ToString() + ";" + coords[1].ToString() + ")");
                GameController.HandleSelectedPixel(_hit.transform.gameObject);
            }
            else
            {
                // Debug.Log("Nothing selected!");
            }
        }
    }
}
