using System;
using System.Collections;
using System.Collections.Generic;
using Brainiac;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("update");
    }

    private void OnGUI()
    {
        Debug.Log("ongui");
    }
}