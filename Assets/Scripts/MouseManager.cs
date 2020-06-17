using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseManager : MonoBehaviour
{
    private UnityEngine.Vector3 prevMousePosition;
    private Boolean previousClickWasRight;
    public int xAndZScrollFactor = 3;
    public int Scrollfactor = 3;
    public int maxHeight = 50;
    public int minHeight = 20;
    public Boolean foundMaxCoordinates = false;
    float largestXInScene = 0;
    float largestZInScene = 0;
    float smallestXInScene = 1000000000;
    float smallestZInScene = 1000000000;
    public float largestXOffset = 0;
    public float largestZOffset = 0;
    public float smallestXOffset = 0;
    public float smallestZOffset = 0;

    float x_step = 17.321f;
    float y_step = 5f;
    float line_offset = 8.661f;

    // Start is called before the first frame update
    void Start()
    {
        prevMousePosition = Input.mousePosition;
        // this is just a random position that is over the tiles
        UnityEngine.Vector3 cameraStartPosition = new UnityEngine.Vector3(80, 50, 40);
        Camera.main.transform.position = cameraStartPosition;
        previousClickWasRight = false;
    }

    // Update is called once per frame
    void Update()
    {
        // this happens in the first update method and not in start to prevent the case where a game object is not instantiated
        if (!foundMaxCoordinates)
        {
            findMaxDimensions();
            foundMaxCoordinates = true;
        }

        // check if the mouse wheel turned
        if (Input.mouseScrollDelta.y != 0)
        {
            UnityEngine.Vector3 yChange = new UnityEngine.Vector3(0, Input.mouseScrollDelta.y, 0);
            Camera.main.transform.position += -yChange * Scrollfactor;

            // limit max height
            if (Camera.main.transform.position.y > maxHeight)
            {
                Camera.main.transform.position = new UnityEngine.Vector3(Camera.main.transform.position.x, maxHeight, Camera.main.transform.position.z);
            }
            // limit min height
            if (Camera.main.transform.position.y < minHeight)
            {
                Camera.main.transform.position = new UnityEngine.Vector3(Camera.main.transform.position.x, minHeight, Camera.main.transform.position.z);
            }
        }

        // we need to constantly reevaluate the last mouse position
        UnityEngine.Vector3 currentMousePosition = Input.mousePosition;
        if (Input.GetMouseButton(1)) // right mouse click
        {
            // use the movement measurement only if the measurement happened while we had right clicked previously
            if (previousClickWasRight)
            {
                UnityEngine.Vector3 diffMousePosition = currentMousePosition - prevMousePosition;
                // we only want the change to happen in x and z position, so we put y to zero
                // y would be scrolling in/out which we will handle via mousewheel
                UnityEngine.Vector3 noYDiffMousePosition = new UnityEngine.Vector3(diffMousePosition.x, 0, diffMousePosition.y);
                // we invert it since it feels more natural for it to be drag and drop if it's done while the mouse button is pressed
                UnityEngine.Vector3 invertedDiff = -noYDiffMousePosition / xAndZScrollFactor;


                // move the camera by just as many pixels as the mouse moved since the last update
                Camera.main.transform.position += invertedDiff;

                // check min x/max x
                if (Camera.main.transform.position.x < smallestXInScene - smallestXOffset)
                {
                    UnityEngine.Vector3 camPos = new UnityEngine.Vector3(smallestXInScene - smallestXOffset, Camera.main.transform.position.y, Camera.main.transform.position.z);
                    Camera.main.transform.position = camPos;
                }
                else if (Camera.main.transform.position.x > largestXInScene - largestXOffset)
                {
                    UnityEngine.Vector3 camPos = new UnityEngine.Vector3(largestXInScene - largestXOffset, Camera.main.transform.position.y, Camera.main.transform.position.z);
                    Camera.main.transform.position = camPos;
                }

                // check min y/max y
                if (Camera.main.transform.position.z < smallestZInScene - smallestZOffset)
                {
                    UnityEngine.Vector3 camPos = new UnityEngine.Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, smallestZInScene - smallestZOffset);
                    Camera.main.transform.position = camPos;
                }
                else if (Camera.main.transform.position.z > largestZInScene - largestZOffset)
                {
                    UnityEngine.Vector3 camPos = new UnityEngine.Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, largestZInScene - largestZOffset);
                    Camera.main.transform.position = camPos;
                }
            }
            previousClickWasRight = true;

        }
        else
        {
            previousClickWasRight = false;
        }
        // after we used the position, we save the last position. This allows us to compare it to the next one to see the delta.
        prevMousePosition = currentMousePosition;

        // print name of left clicked tile
        if (Input.GetMouseButtonDown(0))
        {
            // generate ray from mouse position
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // check if object was hit
            if (Physics.Raycast(mouseRay, out hit, maxHeight + 200, 1000))
            {
                int x = hit.collider.gameObject.GetComponent<Tile>()._coordinateWidth;
                int y = hit.collider.gameObject.GetComponent<Tile>()._coordinateHeight;
                GameObject.Find("GameManager").GetComponent<GameManager>().TileClicked(x, y);
            }
        }


    }

    private void findMaxDimensions()
    {

        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (go.transform.position.x > largestXInScene)
            {
                largestXInScene = go.transform.position.x;
            }
            if (go.transform.position.z > largestZInScene)
            {
                largestZInScene = go.transform.position.z;
            }

            if (go.transform.position.x < smallestXInScene)
            {
                smallestXInScene = go.transform.position.x;
            }
            if (go.transform.position.z < smallestZInScene)
            {
                smallestZInScene = go.transform.position.z;
            }
        }

    }
}
