using UnityEngine;

public class LastRaycastScript : MonoBehaviour
{
    private Camera _camera;

    public static GameObject underMouse;
    public static GameObject underMouseLeftClick;
    public static GameObject underMouseRightClick;

    void Awake()
    {
        _camera = Camera.main;

        if (Object.FindObjectsOfType<LastRaycastScript>().Length > 1)
            throw new System.Exception("One script per scene");
    }

    void Update()
    {
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            underMouse = hit.collider.gameObject;

            if (Input.GetMouseButton(0))
            {
                underMouseLeftClick = hit.collider.gameObject;
            }
            else
            {
                underMouseLeftClick = null;
            }
            if (Input.GetMouseButton(1))
            {
                underMouseRightClick = hit.collider.gameObject;
            }
            else
            {
                underMouseRightClick = null;
            }
        }
        else
        {
            underMouse = null;
            underMouseLeftClick = null;
            underMouseRightClick = null;
        }
    }
}
