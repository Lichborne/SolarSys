using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColour : MonoBehaviour
{   
    [SerializeField]
    private GameObject node;
    private Renderer nodeRenderer;
    private Color nodeColor;
    private float randomChannelOne, randomChannelTwo, randomChannelThree;

    // Start is called before the first frame update
    void Start()
    {
        nodeRenderer = node.GetComponent<Renderer>();
        gameObject.GetComponent<Button>().onClick.AddListener(ChangeNodeColour);
    }

    public void ChangeNodeColour()
    {
        randomChannelOne = Random.Range(0f, 1f);
        randomChannelTwo = Random.Range(0f, 1f);
        randomChannelThree = Random.Range(0f, 1f);

        nodeColor = new Color(randomChannelOne, randomChannelTwo, randomChannelThree, 1f);

        nodeRenderer.material.SetColor("_BaseColor", nodeColor);
    }
}
