using System;
using System.Runtime.InteropServices;
using SenseARInternal;
using UnityEngine;
//    using UnityEngine.XR;
using System.Collections;
using SenseAR;

public class SenseARSession : MonoBehaviour
{
    public ARCoreSessionConfig SessionConfig;
    public bool ConnectOnAwake = true;
    private SessionManager m_SessionManager;

    public void Awake()
    {

        m_SessionManager = SessionManager.CreateSession();
        Session.Initialize(m_SessionManager);

        if (ConnectOnAwake)
        {
            Connect_NoPermission(SessionConfig);
        }   
    }

    private IEnumerator OpenCamera()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (!Application.HasUserAuthorization(UserAuthorization.WebCam)) yield break;
        WebCamDevice[] devices = WebCamTexture.devices;
    }
    public void OnDestroy()
    {
        Frame.Destroy();
        Session.Destroy();
    }

    public void Update()
    {
        if (m_SessionManager == null)
        {
            return;
        }
        AsyncTask.OnUpdate();
    }

    public void OnApplicationFocus(bool focus)
    {
    
    }
    public void OnApplicationPause(bool pause)
    {
    }

    public void OnApplicationQuit()
    {
        #if UNITY_EDITOR
        #else
        m_SessionManager.Pause();
        #endif
    }

    public void Connect()
    {
        Connect_NoPermission(SessionConfig);
    }

    public void Connect_NoPermission(ARCoreSessionConfig sessionConfig)
    {
        Action<SessionConnectionState> onTaskComplete;
        var returnTask = new AsyncTask<SessionConnectionState>(out onTaskComplete);
        returnTask.ThenAction((connectionState) =>
            {
                m_SessionManager.ConnectionState = connectionState;
            });

        _ResumeSession(sessionConfig, onTaskComplete);
    }

    private void _ResumeSession(ARCoreSessionConfig sessionConfig, Action<SessionConnectionState> onComplete)
    {
        if (!m_SessionManager.CheckSupported(sessionConfig))
        {
            ARDebug.LogError("The requested ARCore session configuration is not supported.");
            onComplete(SessionConnectionState.InvalidConfiguration);
            return;
        }

        if (!m_SessionManager.SetConfiguration(sessionConfig))
        {
            ARDebug.LogError("ARCore connection failed because the current configuration is not supported.");
            onComplete(SessionConnectionState.InvalidConfiguration);
            return;
        }

        Frame.Initialize(m_SessionManager.FrameManager);
                    
            if (!m_SessionManager.Resume(sessionConfig))
            {
                onComplete(SessionConnectionState.ConnectToServiceFailed);
            }
            else
            {
                onComplete(SessionConnectionState.Connected);
            }

    }
}

