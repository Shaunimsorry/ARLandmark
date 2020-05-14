using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleDebug : MonoBehaviour
{
    public Text debugText;
    public GameObject TransformReporter;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        debugText.text = TransformReporter.transform.position.ToString();        
    }
}
