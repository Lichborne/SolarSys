using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class placeObject : MonoBehaviour
{   
    public GameObject ghost;
    public GameObject placed;
    public float distance = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
            if(Input.GetMouseButtonDown(0)) {
                
                RaycastHit hit; 
                Instantiate(ghost, transform.position + transform.forward* distance, transform.rotation);
                   

                if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity)) {
                    ghost.transform.position = hit.point;

                    //StartCoroutine(WaitforClick());
                    if(Input.GetMouseButtonDown(0)) {
                        Instantiate(placed, ghost.transform.position, ghost.transform.rotation);
                    }
                }
                Destroy(ghost);
            }
        }
    }

