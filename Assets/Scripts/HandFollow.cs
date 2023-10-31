using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandFollow : MonoBehaviour
{
    [SerializeField] private GameObject hand;

    [SerializeField] private GameObject controller;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        hand.transform.localPosition = controller.transform.localPosition;
    }
}
