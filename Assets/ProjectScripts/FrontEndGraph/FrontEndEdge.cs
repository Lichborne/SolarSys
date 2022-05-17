using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

// for overall idea behing fornd end graphs, see FrontEndNode.cs. The edge also contains information about 
// the nodes it is connected to so that information needed for operations such as deletion can be directly 
// accessed (even if in two steps) instead of indirectly, and each graph traversal is shorter (because only
// local connections are known). This is aligned with how interactions are based on targeting and is faster.
public class FrontEndEdge : MonoBehaviour
{
    // to represent the database edge which it represents; public because access is needed by many operations; 
    // safety has to be ensured by careful use, regardless of protection level
    public Backend.GraphEdge _databaseEdge { get; set; } = null;          

    // separate text for edges; cannot be a part of the prefabs due to universal scaling being uncircumventable from inside game object
    public GameObject _textObject { get; set; } = null;                   

    // parent object; other classes need frequent access, so getters and setters seemed needlessly indirect; 
    // safety has to be ensured by careful use, regardless of protection level
    public GameObject _parent { get; set; } = null;

    // child object; other classes need frequent access, so getters and setters seemed needlessly indirect;
    // safety has to be ensured by careful use, regardless of protection level
    public GameObject _child { get; set; } = null;
    
    // public because certain unity functionality used for keeping rotations aligned does not work otherwise, and 
    // because system needs access multiple times a second, for every object, therefore it is not prudent to go indirectly.
    public float _rotation { get; set; } = 0;

    // to record whether the edge is curved or not 
    private bool _isCurved = false;

    // to represent displacement for text object from edge when the edge is curved, same as above
    private float _scale = 2;         

    // this is needed so that when edge text is close to the camera, it faces it, otherwise not; private here because it should not be changed
    private Camera _cameraToLookAt;     

    // the displacement scale fro self reference edges
    private const float SELFSCALE = 4f; 

    // a quarter turn 
    private const float QUARTERTURN = 90f;          

    // max distance of camera from obejct at which text is rotated to face it
    private const float MAXDISTANCE = 30f;       
    

    void Start() 
    {
        // we set the camera
        _cameraToLookAt = Camera.main;
    }
    void Update()
    {   
        if (_databaseEdge == null || _textObject == null) {
            return;
        }
        //if it's a self reference edge, we just update its position in a more simple manner, get the text object in a nice place, and return
        if (_parent == _child) 
        {
            // text positioning is done base don the rotation of the edges; since due to scaling the two objects
            // have to be decoupled, we need to implement this manually using the sine and cosine components of 
            // relevant spatial displacements
            gameObject.transform.position = new Vector3(_parent.transform.position.x, _parent.transform.position.y, _parent.transform.position.z); 
            _textObject.transform.position = new Vector3(gameObject.transform.position.x + (float)(SELFSCALE*Math.Sin(ConvertToRadians(_rotation))), 
                    gameObject.transform.position.y + (float)(SELFSCALE*Math.Cos(ConvertToRadians(_rotation))), gameObject.transform.position.z);
        } 
        else 
        {
            gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            
            // rotate towards child
            gameObject.transform.LookAt(_child.transform);

            gameObject.transform.Rotate(0.0f, 0.0f, _rotation);
            
            //for scaling
            Vector3 ls = gameObject.transform.localScale;

            // get scaling measure
            ls.z = Vector3.Distance(_parent.transform.position, _child.transform.position);
            
            // some additional scaling if the edge is curved
            if (_isCurved) {

                ls.y = Vector3.Distance(_parent.transform.position, _child.transform.position)/3;

                ls.x = Vector3.Distance(_parent.transform.position, _child.transform.position)/8;

            }

            _scale = ls.z/6;

            //scale
            gameObject.transform.localScale = ls;

            //move into position
            gameObject.transform.position = new Vector3(
                (_child.transform.position.x+_parent.transform.position.x)/2,
                (_child.transform.position.y+_parent.transform.position.y)/2,
                (_child.transform.position.z+_parent.transform.position.z)/2);
            
            if (!_isCurved) 
            {
                _textObject.transform.position = new Vector3((_child.transform.position.x+_parent.transform.position.x)/2, gameObject.transform.position.y + 1, (_child.transform.position.z+_parent.transform.position.z)/2);
            
            } 
            else if (_rotation == 0) 
            {
                // once again, for here and below, the text positions are calculated based on trig components of, in this case
                // a scaled distance metric. What this conveniently results in is that depending on how the edges are arranged spatially
                // the texts that lie flat fill overlap minimally (you can see this by rotatinng a double-connected pair of nodes,
                // one around the other, 360 degress). There were alternative approaches, but this seemed the best mix of 
                // still decent appearance, and readability
                _textObject.transform.position = new Vector3(gameObject.transform.position.x, 
                        gameObject.transform.position.y + (float)(_scale*Math.Cos(ConvertToRadians(_rotation))), gameObject.transform.position.z + (float)(_scale*Math.Sin(ConvertToRadians(_rotation))));
            }
            else if (_rotation > 0) 
            {
                _textObject.transform.position = new Vector3(gameObject.transform.position.x + (float)(_scale*Math.Sin(ConvertToRadians(_rotation))), 
                    gameObject.transform.position.y + (float)(_scale*Math.Cos(ConvertToRadians(_rotation))), gameObject.transform.position.z);
                
            } 
            else 
            {
                _textObject.transform.position = new Vector3(gameObject.transform.position.x - (float)(_scale*Math.Sin(ConvertToRadians(_rotation))), 
                    gameObject.transform.position.y + (float)(_scale*Math.Cos(ConvertToRadians(_rotation))), gameObject.transform.position.z);
            }
        }
        // if within range, return
        if (Vector3.Distance(gameObject.transform.position, _cameraToLookAt.transform.position) < MAXDISTANCE) {
            // if we are close, enable
            if(!_textObject.activeSelf) {
                _textObject.SetActive(true);
            }

            // transform to always look at the camera
            _textObject.transform.rotation = Quaternion.LookRotation(-_cameraToLookAt.transform.forward, _cameraToLookAt.transform.up);
            _textObject.transform.Rotate(180, 90, 180);
            
            return;
        }

        _textObject.SetActive(false);
        //_textObject.transform.LookAt(_parent.transform);
        //_textObject.transform.rotation = new Quaternion (gameObject.transform.rotation.x, gameObject.transform.rotation.y, gameObject.transform.rotation.z, gameObject.transform.rotation.w);

  }
    // constructors are ill advised, so we use this instead
    public void InstantiateEdge(bool isCurvedEdge, Backend.GraphEdge graphEdge, GameObject text, GameObject p, GameObject c, float rotation = 0f) { 

        _isCurved = isCurvedEdge;
        _databaseEdge = graphEdge;
        _textObject = text;
        _parent = p;
        _child = c;
        _rotation = rotation;

        gameObject.transform.Rotate(0.0f, 0.0f, rotation);

    }

    public double ConvertToRadians(double angle)
    {
        return (Math.PI / 180) * angle;
    }
}
