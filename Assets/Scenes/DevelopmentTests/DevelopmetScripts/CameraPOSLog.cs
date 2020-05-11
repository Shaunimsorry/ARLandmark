using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraPOSLog : MonoBehaviour
{
    public Camera tgtCamera;
    public Text logTxt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        logTxt.text = tgtCamera.transform.position.ToString();   
    }
}
