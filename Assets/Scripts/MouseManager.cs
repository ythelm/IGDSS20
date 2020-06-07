/* using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{

    public Camera _camera; //The camera object

    private Vector3 _lastPanPosition; //The position of the mouse when the dragging begins

    private float PanSpeed = 200f; //Movement speed multiplier of the camera translation
    private float ZoomSpeedMouse = 50f; //Multiplier for zoom factor

    public float[] BoundsX = new float[2]; //Camera bounds on the X axis
    public float[] BoundsZ = new float[2]; //Camera bounds on the ZX axis
    public float[] ZoomBounds = new float[] { 10f, 85f }; //Zoom bounds on the Y axis
    public GameManager gameManager;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GetTile();
        MousePanning();
    }

    void GetTile()
    {   // left button
        if (Input.GetMouseButtonDown(0))
        {
            // print the name of the hitted object
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            int layerMask = LayerMask.GetMask("Tiles");
            if (Physics.Raycast(ray, out hit, 1000f, layerMask))
            {
                Tile t = hit.collider.gameObject.GetComponent<Tile>() as Tile;
                Debug.Log("Tile type: " + t._type);
                gameManager.TileClicked(t._coordinateHeight, t._coordinateWidth);
            }
        }
    }

    void MousePanning()
    {
        // On mouse down, capture it's position.
        // Otherwise, if the mouse is still down, pan the camera.
        if (Input.GetMouseButtonDown(1))
        {
            _lastPanPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(1))
        {
            PanCamera(Input.mousePosition);
        }

        // Check for scrolling to zoom the camera
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        ZoomCamera(scroll, ZoomSpeedMouse);
    }

    void PanCamera(Vector3 newPanPosition)
    {
        // Determine how much to move the camera
        Vector3 offset = _camera.ScreenToViewportPoint(_lastPanPosition - newPanPosition);
        Vector3 move = new Vector3(-offset.y * PanSpeed, 0, offset.x * PanSpeed);

        // Perform the movement
        _camera.transform.Translate(move, Space.World);

        // Ensure the camera remains within bounds.
        Vector3 pos = _camera.transform.position;
        pos.x = Mathf.Clamp(_camera.transform.position.x, BoundsX[0], BoundsX[1]);
        pos.z = Mathf.Clamp(_camera.transform.position.z, BoundsZ[0], BoundsZ[1]);
        _camera.transform.position = pos;

        // Cache the position
        _lastPanPosition = newPanPosition;
    }

    void ZoomCamera(float offset, float speed)
    {
        if (offset == 0)
        {
            return;
        }

        _camera.fieldOfView = Mathf.Clamp(_camera.fieldOfView - (offset * speed), ZoomBounds[0], ZoomBounds[1]);
    }

    public void InitializeBounds(float Xleft, float Xright, float Ztop, float Zbottom)
    {

        BoundsX = new float[2] { Xleft, Xright };
        BoundsZ = new float[2] { Ztop, Zbottom };

        _camera.transform.position = new Vector3((Xleft + Xright) / 2, _camera.transform.position.y, (Ztop + Zbottom) / 2);
    }
} */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public float scrollSpeed = 100f;
    private readonly float maxY = 85f;
    private readonly float minY = 25f;
    public float panSpeed = 7f;
    public Bounds bounds;
    public Camera cam;
    private Vector3 prev_pos;
    private Vector3 smoothCamPos;
    public GameManager gameManager;

    #region MonoBehaviour
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        smoothCamPos = cam.transform.position; // Initial move
        InitializeBounds();
    }

    // Update is called once per frame
    void Update()
    {
        PanCam();
        SelectTile();
        ZoomCam();
    }

    private void LateUpdate()
    {
        Vector3 desiredPos = this.smoothCamPos;
        Vector3 smoothedPos = Vector3.Lerp(cam.transform.position, desiredPos, 0.125f);
        cam.transform.position = smoothedPos;
    }
    #endregion

    #region Methods
    // 
    void PanCam()
    {
        // right button
        if (Input.GetMouseButtonDown(1))
            prev_pos = cam.ScreenToViewportPoint(Input.mousePosition);
        if (Input.GetMouseButton(1))
        {
            // move the mouse pans the camera on X-Z plane
            Vector3 direction = prev_pos - cam.ScreenToViewportPoint(Input.mousePosition);
            Vector3 pos = cam.transform.position;
            pos.x += direction.y * panSpeed;
            pos.z -= direction.x * panSpeed;
            // boundaries
            pos.x = Mathf.Clamp(pos.x, bounds.min.x, bounds.max.x);
            pos.z = Mathf.Clamp(pos.z, bounds.min.z, bounds.max.z);

            cam.transform.position = pos;
            this.smoothCamPos = pos;
        }
    }
    // print the selected tile type 
    void SelectTile()
    {   // left button
        if (Input.GetMouseButtonDown(0))
        {
            // print the name of the hitted object
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            int layerMask = LayerMask.GetMask("Tiles");
            if (Physics.Raycast(ray, out hit, 1000f, layerMask))
            {
                Tile t = hit.collider.gameObject.GetComponent<Tile>() as Tile;
                Debug.Log("Tile type: " + t._type);
                gameManager.TileClicked(t._coordinateHeight, t._coordinateWidth);
            }
        }
    }
    // Zoom camera in and out by mouse scrolling
    void ZoomCam()
    {
        float scrollAxis = Input.GetAxis("Mouse ScrollWheel");
        Vector3 camPos = cam.transform.position;

        if (scrollAxis != 0f)
        {
            Vector3 desiredPos = cam.transform.position + (cam.transform.forward * scrollSpeed) * scrollAxis;
            if ((camPos.y >= maxY - 5f && scrollAxis < 0) || (camPos.y <= minY + 5f && scrollAxis > 0))
            {
                desiredPos = camPos;
            }
            desiredPos.y = Mathf.Clamp(desiredPos.y, minY, maxY);
            desiredPos.x = Mathf.Clamp(desiredPos.x, bounds.min.x, bounds.max.x);
            desiredPos.z = Mathf.Clamp(desiredPos.z, bounds.min.z, bounds.max.z);
            this.smoothCamPos = desiredPos;
        }
    }

    void InitializeBounds()
    {
        // Determine scene bounds
        var rnds = FindObjectsOfType<Renderer>();
        if (rnds.Length == 0)
            return; // nothing to see here, go on

        var b = rnds[0].bounds;
        for (int i = 1; i < rnds.Length; i++)
            b.Encapsulate(rnds[i].bounds);
        this.bounds = b;
    }
    #endregion
}
