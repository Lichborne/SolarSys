//Original Code From Tobias J. at https://forum.unity.com/threads/implement-a-drag-and-drop-script-with-c.130515/

using System.Collections;
using UnityEngine;
 
class Drag : MonoBehaviour
{
    private Color mouseOverColor = Color.blue;
    private Color originalColor = Color.grey;
    private bool dragging = false;
    private float distance;
 
   
    void OnMouseEnter()
    {
        GetComponent<Renderer>().material.color = mouseOverColor;
    }
 
    void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = originalColor;
    }
 
    void OnMouseDown()
    {
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        dragging = true;
    }
 
    void OnMouseUp()
    {
        dragging = false;
    }
 
    void Update()
    {
        if (dragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);
            transform.position = rayPoint;
        }
    }
}
 