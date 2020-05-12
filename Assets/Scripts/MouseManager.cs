using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public Camera MainCamera;
    public int mapSize;
    public bool rightMouseClickHold;
    public const float speed = 1f;

    // mouse movement
    public Vector2 MousePosition;
    public Vector3 CameraPosition;

    //public float maxX;
    //public float minX;
    //public float maxZ;
    //public float minZ;




    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
           moveCameraXZ();
      }



    // Camera Movement on the XZ-plane while holding right Mouse Button

    public void moveCameraXZ()
    {
            if (Input.GetMouseButtonDown(1))
            {
                rightMouseClickHold = true;
                MousePosition = Input.mousePosition;
                CameraPosition = MainCamera.transform.position;
            }

            if (Input.GetMouseButtonUp(1))
            {
                rightMouseClickHold = false;
            }

            if (rightMouseClickHold)
            {
                MoveCamera();
            }
        }

         void MoveCamera()
        {
            var xOffset = (MousePosition.x - Input.mousePosition.x) * Time.deltaTime * speed;
            var yOffset = (MousePosition.y - Input.mousePosition.y) * Time.deltaTime * speed;

            // change to xCamPos + yOffset and zCamPos + xOffset to disable auto movement
            var newXPos = CameraPosition.x += yOffset;
            var newZPos = CameraPosition.z += xOffset;

            newXPos = Mathf.Clamp(newXPos, 0, mapSize);
            newZPos = Mathf.Clamp(newZPos, 0, mapSize);

            MainCamera.transform.SetPositionAndRotation(
                new Vector3(newXPos, CameraPosition.y, newZPos), MainCamera.transform.rotation);
        }

    }


    //{
    //    var x = Input.GetAxisRaw("Mouse X") * Time.deltaTime * speed;
    //    var z = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * speed;
    //
    //    if (Input.GetAxis("Mouse X") > 0 || Input.GetAxis("Mouse X") < 0)
    // {


    // MainCamera.transform.position += new Vector3(x,
    //                                   0.0f, z) ;

    // Vector3 newPos = MainCamera.transform.position;
    // var newPosMitBoundaries = new Vector3(Mathf.Clamp(newPos.x, newPos.x, newPos.x), newPos.y, Mathf.Clamp(newPos.z, newPos.z, newPos.z));

    //MainCamera.transform.position = newPosMitBoundaries;
    //  }




