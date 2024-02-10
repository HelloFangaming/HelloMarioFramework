using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SimpleAnimation))]
public class SimpleAnimationSetSpeed : MonoBehaviour
{

    public float speed;

    //Use this to change the speed for the Simple Animation component
    void Start()
    {
        GetComponent<SimpleAnimation>().GetState("Default").speed = speed;
        Destroy(this);
    }

}
