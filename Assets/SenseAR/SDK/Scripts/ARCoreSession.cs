#if  UNITY_ANDROID || UNITY_EDITOR 
//-----------------------------------------------------------------------
// <copyright file="ARCoreSession.cs" company="Google">
//
// Copyright 2017 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
//modification list
//1.modify ConnectOnAwake default value
//2.add packageName
//3.modify Awake
//4.add OpenCamera
//5.modify _ResumeSession
//6.add resume_Coroutine
//7.remove Connect
//8.add OnApplicationFocus
//9.add OnApplicationPause
//10.add OnApplicationQuit
//11.add Connect_NoPermission
//rename GoogleARCore to SenseAR to avoid conflict
//rename GoogleARCoreInternal to SenseARInternal to avoid conflict
//
// </copyright>
//-----------------------------------------------------------------------

namespace SenseAR
{
    using System;
    using System.Runtime.InteropServices;
    using SenseARInternal;
    using UnityEngine;
//    using UnityEngine.XR;
	using System.Collections;

    /// <summary>
    /// A component that manages the ARCore Session in a Unity scene.
    /// </summary>
    public class ARCoreSession : MonoBehaviour
    {
        /// <summary>
        /// A scriptable object specifying the ARCore session configuration.
        /// </summary>
        [Tooltip("A scriptable object specifying the ARCore session configuration.")]
        public ARCoreSessionConfig SessionConfig;

        /// <summary>
        /// Toggles whether the tango service should be automatically connected upon Awake.
        /// </summary>
        [Tooltip("Toggles whether the tango service should be automatically connected upon Awake.")]
        public bool ConnectOnAwake = true;

        private SessionManager m_SessionManager;

        /// <summary>
        /// Unity Awake.
        /// </summary>
        public void Awake()
        {
            if (Application.isEditor)
            {
                enabled = false;
                return;
            }

            if (FindObjectsOfType<ARCoreSession>().Length > 1)
            {
                ARDebug.LogError("Multiple SessionComponents present in the game scene.  Destroying the gameobject " +
                 "of the newest one.");
                Destroy(gameObject);
                return;
            }
				
			m_SessionManager = SessionManager.CreateSession();
            Session.Initialize(m_SessionManager);

            if (Session.ConnectionState != SessionConnectionState.Uninitialized)
            {
                ARDebug.LogError("Could not create an ARCore session.  The current Unity Editor may not support this " +
                    "version of ARCore.");
                return;
            }

            if (ConnectOnAwake)
            {
//				Connect ();
				Connect_NoPermission(SessionConfig);
            }
			
			//OpenCamera();
        }
		
		private IEnumerator OpenCamera()
		{
			yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
			if (!Application.HasUserAuthorization(UserAuthorization.WebCam)) yield break;
			WebCamDevice[] devices = WebCamTexture.devices;
		}

        /// <summary>
        /// Unity OnDestroy.
        /// </summary>
        public void OnDestroy()
        {
            Frame.Destroy();
            Session.Destroy();
        }

        /// <summary>
        /// Unity Update.
        /// </summary>
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
            if(pause == true)
            {
                ARDebug.LogInfo("OnApplicationPause false, call session pause");
				if (m_SessionManager != null)
                m_SessionManager.Pause();
            }
            else
            {
                ARDebug.LogInfo("OnApplicationPause true, call session resume");
				if (m_SessionManager != null)
                m_SessionManager.Resume(SessionConfig);
            }
        }

        public void OnApplicationQuit()
        {
			#if UNITY_EDITOR
			#else
            m_SessionManager.Pause();
			#endif
        }

        public void SetReferenceImageDatabase(ReferenceImageDatabase database)
        {
            m_SessionManager.SetReferenceImageDatabase(SessionConfig, database);
        }

        public ReferenceImageDatabase GetReferenceImageDatabase()
        {
            return m_SessionManager.GetReferenceImageDatabase();
        }

        public void Connect()
        {
            Connect_NoPermission(SessionConfig);
        }

        public void Connect_NoPermission(ARCoreSessionConfig sessionConfig)
		{
			if (m_SessionManager == null)
			{
				ARDebug.LogError("Cannot connect because ARCoreSession failed to initialize.");
			}

			if (sessionConfig == null)
			{
				ARDebug.LogError("Unable to connect ARSession session due to missing ARSessionConfig.");
				m_SessionManager.ConnectionState = SessionConnectionState.MissingConfiguration;
			}

			// We have already connected at least once.
			if (Session.ConnectionState != SessionConnectionState.Uninitialized)
			{
				ARDebug.LogError("Multiple attempts to connect to the ARSession.  Note that the ARSession connection " +
					"spans the lifetime of the application and cannot be reconfigured.  This will change in future " +
					"versions of ARCore.");
			}

			// Create an asynchronous task for the potential permissions flow and service connection.
			Action<SessionConnectionState> onTaskComplete;
			var returnTask = new AsyncTask<SessionConnectionState>(out onTaskComplete);
			returnTask.ThenAction((connectionState) =>
				{
					m_SessionManager.ConnectionState = connectionState;
				});

			_ResumeSession(sessionConfig, onTaskComplete);
		}

        /// <summary>
        /// Connects to the ARCore service.
        /// </summary>
        /// <param name="sessionConfig">The session configuration to connect with.</param>
        /// <param name="onComplete">A callback for when the result of the connection attempt is known.</param>
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
			
			// ArSession_resume needs to be called in the UI thread due to b/69682628.
//			AsyncTask.PerformActionInUIThread(() =>
//			                                  {				
				if (!m_SessionManager.Resume(sessionConfig))
				{
					onComplete(SessionConnectionState.ConnectToServiceFailed);
				}
				else
				{
					onComplete(SessionConnectionState.Connected);
				}
//			});
//			StartCoroutine (resume_Coroutine(onComplete));
        }

    }
}

 #endif