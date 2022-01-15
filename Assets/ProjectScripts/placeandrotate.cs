using System;
using UnityEngine;

// code adapted from Weimann, Jason, "Let your players place objects or turrets in Unity (RTS / Base Building games)," May 8, 2018, https://unity3d.college/2018/05/08/let-players-place-objects-turrets-unity-rts-base-building-games/ 

public class placeandrotate: MonoBehaviour
{
    [SerializeField]
    private GameObject[] _objectPrefabs;

    [SerializeField]
    private GameObject[] _visualAidPrefabs;

    private GameObject _currentObject;

    private float _mouseWheelRotation;

    private int _spawnDistance = 15;

    private int _currentPrefabIndex = -1;

    private void Update()
    {
        HandleNewObjectHotkey();

        if (_currentObject != null)
        {
            MoveCurrentObjectToMouse();
            RotateFromMouseWheel();
            ReleaseIfClicked();
        }
    }

    private void HandleNewObjectHotkey()
    {
        for (int i = 0; i < _visualAidPrefabs.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + 1 + i))
            {
                if (PressedKeyOfCurrentPrefab(i))
                {
                    Destroy(_currentObject);
                    _currentPrefabIndex = -1;
                }
                else
                {
                    if (_currentObject != null)
                    {
                        Destroy(_currentObject);
                    }

                    _currentObject = Instantiate(_visualAidPrefabs[i]);
                    _currentPrefabIndex = i;
                }

                break;
            }
        }
    }

    private bool PressedKeyOfCurrentPrefab(int i)
    {
        return _currentObject != null && _currentPrefabIndex == i;
    }

    private void MoveCurrentObjectToMouse()
    {
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo)) 
        {
            _currentObject.transform.position = hitInfo.point;
            //_currentObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);

        } else if (_currentPrefabIndex == 0) {

            _currentObject.transform.position =  Camera.main.transform.position;
            _currentObject.transform.forward =  Camera.main.transform.forward;
            _currentObject.transform.rotation =  Camera.main.transform.rotation;

            _currentObject.transform.position += _currentObject.transform.forward * _spawnDistance;
        }
    }

    private void RotateFromMouseWheel()
    {
        Debug.Log(Input.mouseScrollDelta);
        _mouseWheelRotation += Input.mouseScrollDelta.y;
        //_currentObject.transform.Rotate(Vector3.up, _mouseWheelRotation * 10f);
    }

    private void ReleaseIfClicked()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(_objectPrefabs[_currentPrefabIndex], _currentObject.transform.position, _currentObject.transform.rotation);

            Destroy(_currentObject);

            _currentObject = null;
        }
    }
}