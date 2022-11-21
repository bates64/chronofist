using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostFinalJab : MonoBehaviour
{
    public AK.Wwise.Event MyEvent;
    // Use this for initialization.
    public void PlayFinalJab()
    {
        MyEvent.Post(gameObject);
    }
}

