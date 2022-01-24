// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class camerafol : MonoBehaviour
// {                    
//     GameObject planet;
//     public Camera cameraToLookAt;
//     int zoom = 20;
//     int normal = 60; // Base zoom 
//     float smooth = 5; // Smooth the transition to the object

//     private bool isZoomed = false; // Have we zoomed into object yet?



//  void Start() 
//  {
//      //transform.Rotate( 180,0,0 );
//      cameraToLookAt = Camera.main;

//  }
 
//  void Update() 
//  {
//          if(Input.GetMouseButtonDown(0)) // If left click
//          {
//             Ray ray = cameraToLookAt.ScreenPointToRay(Input.mousePosition);
//             RaycastHit hit;
//             // Casts the ray and get the first game object hit
//             if (Physics.Raycast(ray, out hit, 100f) & (hit.transform != null))
//             {
//                 if (hit.transform.gameObject.name == "Sphere")
//                 {
//                     zoomToObject(hit.transform.gameObject);
//                 }
//                 // transform.forward = new Vector3(Camera.main.transform.forward.x, transform.forward.y, Camera.main.transform.forward.z);
//             } 
//         }

//         //  if (isZoomed)
//         //  {
//         //     zoomToObject(planet);
//         //     cameraToLookAt.fieldOfView = Mathf.Lerp(cameraToLookAt.fieldOfView, zoom, Time.deltaTime*smooth);
//         //  }

//         //  else 
//         //  {
//         //     zoomToObject(planet);
//         //     cameraToLookAt.fieldOfView = Mathf.Lerp(cameraToLookAt.fieldOfView, normal, Time.deltaTime*smooth);
//         //  }

//  }   

//     public void zoomToObject(GameObject planet)
//     {
//         Camera.main.transform.position = planet.transform.position;

//     }
//  }