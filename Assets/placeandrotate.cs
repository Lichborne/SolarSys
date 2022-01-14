using System;
using UnityEngine;

// code adapted from Weimann, Jason, "Let your players place objects or turrets in Unity (RTS / Base Building games)," May 8, 2018, https://unity3d.college/2018/05/08/let-players-place-objects-turrets-unity-rts-base-building-games/ 

public class placeandrotate: MonoBehaviour
{
    [SerializeField]
    private GameObject[] placeableObjectPrefabs;

    [SerializeField]
    private GameObject[] visualAidPrefabs;

    private GameObject currentPlaceableObject;


    private float mouseWheelRotation;

    private int spawnDistance = 8;

    private int currentPrefabIndex = -1;

    private void Update()
    {
        HandleNewObjectHotkey();

        if (currentPlaceableObject != null)
        {
            MoveCurrentObjectToMouse();
            RotateFromMouseWheel();
            ReleaseIfClicked();
        }
    }

    private void HandleNewObjectHotkey()
    {
        for (int i = 0; i < visualAidPrefabs.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + 1 + i))
            {
                if (PressedKeyOfCurrentPrefab(i))
                {
                    Destroy(currentPlaceableObject);
                    currentPrefabIndex = -1;
                }
                else
                {
                    if (currentPlaceableObject != null)
                    {
                        Destroy(currentPlaceableObject);
                    }

                    currentPlaceableObject = Instantiate(visualAidPrefabs[i]);
                    currentPrefabIndex = i;
                }

                break;
            }
        }
    }

    private bool PressedKeyOfCurrentPrefab(int i)
    {
        return currentPlaceableObject != null && currentPrefabIndex == i;
    }

    private void MoveCurrentObjectToMouse()
    {
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo)) {
            currentPlaceableObject.transform.position = hitInfo.point;
            //currentPlaceableObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
        } else if (currentPrefabIndex == 0) {

            currentPlaceableObject.transform.position =  Camera.main.transform.position;

            currentPlaceableObject.transform.forward =  Camera.main.transform.forward;
            
            currentPlaceableObject.transform.rotation =  Camera.main.transform.rotation;

            currentPlaceableObject.transform.position += currentPlaceableObject.transform.forward * spawnDistance;
        }
    }

    private void RotateFromMouseWheel()
    {
        Debug.Log(Input.mouseScrollDelta);
        mouseWheelRotation += Input.mouseScrollDelta.y;
        //currentPlaceableObject.transform.Rotate(Vector3.up, mouseWheelRotation * 10f);
    }

    private void ReleaseIfClicked()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(placeableObjectPrefabs[currentPrefabIndex], currentPlaceableObject.transform.position, currentPlaceableObject.transform.rotation);
            Destroy(currentPlaceableObject);
            currentPlaceableObject = null;
        }
    }
}