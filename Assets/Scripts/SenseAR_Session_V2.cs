using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Avoiding ARCore and Sticking to the SenseAR Libraries
using SenseAR;
using SenseARInternal;

public class SenseAR_Session_V2 : MonoBehaviour
{
    //This is a lightweight probably buggy vesion of the SenseAR ARCore SessionStarter.
    
    public Text debugTxt;
    public ARCoreSessionConfig ARSeshConfig;
    public SessionManager ARSession;

    //Unity Awake
    public void Awake()
    {
        ARSession = SessionManager.CreateSession();
        Session.Initialize(ARSession);

        Connect_NoPermisssion(ARSeshConfig);
    }

    private IEnumerator OpenCamera()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if(!Application.HasUserAuthorization(UserAuthorization.WebCam)) yield break;
        WebCamDevice[] devices = WebCamTexture.devices;
    }

    public void OnDestroy()
    {
        Frame.Destroy();
        ARSeshConfig.Destroy();
    }

    public void Update()
    {
        AsyncTask.OnUpdate();
        debugTxt.text = "Connetion State: "+ARSession.ConnectionState.ToString();
    }

    public void Connect()
    {
        Connect_NoPermisssion(ARSeshConfig);
    }
    //More Unknownery
    public void Connect_NoPermisssion(ARCoreSessionConfig sessionConfig)
    {
        Action<SessionConnectionState> onTaskComplete;
        var returnTask = new AsyncTask<SessionConnectionState>(out onTaskComplete);
        returnTask.ThenAction((connectionState) =>
        {
            ARSession.ConnectionState = connectionState;
        });

        _ResumeSession(sessionConfig, onTaskComplete);
    }

    public void _ResumeSession(ARCoreSessionConfig sessionConfig, Action<SessionConnectionState> onComplete)
    {
        Frame.Initialize(ARSession.FrameManager);
        onComplete(SessionConnectionState.Connected);
    }   
}
