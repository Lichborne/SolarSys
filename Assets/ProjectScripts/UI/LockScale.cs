using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockScale : MonoBehaviour
{
    private Vector3 originalScale;
    private Vector3 parentOriginalScale;

    private void Start()
    {
        // afaik RectTransform inherits from Transform 
        // so this should also work for UI objects.

        originalScale = transform.localScale;

        parentOriginalScale = gameObject.transform.localScale;
    }

    private void LateUpdate()
    {
        var currentParentScale = gameObject.transform.localScale;

        // Get the relative difference to the original scale
        var diffX = currentParentScale.x / parentOriginalScale.x;
        var diffY = currentParentScale.y / parentOriginalScale.y;
        var diffZ = currentParentScale.z / parentOriginalScale.z;

        // Apply the inverted differences to the original scale
        transform.localScale = new Vector3 (originalScale.x* 1/diffX, originalScale.y*1/diffY, originalScale.z*1/diffZ);
    }
}