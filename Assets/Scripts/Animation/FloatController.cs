/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FloatBehavior : MonoBehaviour
{
    float originalY;
	float timeOffSet; //make editable so they aren't all floating at the same time

    public float floatStrength = 1; // You can change this in the Unity Editor to 
                                    // change the range of y positions that are possible.

    void Start()
    {
        this.originalY = this.transform.position.y;
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x,
            originalY + ((float)Math.Sin(Time.time) * floatStrength),
            transform.position.z);
    }
}*/