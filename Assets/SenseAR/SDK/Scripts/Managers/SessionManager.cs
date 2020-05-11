#if  UNITY_ANDROID || UNITY_EDITOR 
//-----------------------------------------------------------------------
// <copyright file="SessionManager.cs" company="Google">
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
//rename GoogleARCore to SenseAR to avoid conflict
//rename GoogleARCoreInternal to SenseARInternal to avoid conflict
//
// </copyright>
//-----------------------------------------------------------------------

namespace SenseARInternal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using SenseAR;
    using UnityEngine;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
     Justification = "Internal")]
    public class SessionManager
    {
        private NativeSession m_NativeSession;
		public AnchorApi AnchorApi { get; set; }

        private SessionManager()
        {
        }

        ~SessionManager()
        {
            Destroy();
        }

        public SessionConnectionState ConnectionState { get; set; }

        public FrameManager FrameManager { get; private set; }

		public static SessionManager CreateSession()
        {
            var sessionManager = new SessionManager();
			sessionManager.m_NativeSession = NativeSession.CreateSession();
			sessionManager.AnchorApi = new AnchorApi(sessionManager.m_NativeSession);
            if (sessionManager.m_NativeSession != null)
            {
                sessionManager.FrameManager = new FrameManager(sessionManager.m_NativeSession);
                sessionManager.ConnectionState = SessionConnectionState.Uninitialized;
            }
            else
            {
                // Eventually we will provide more detail here: ARCore not installed, device not
                // supported, ARCore version not supported, etc.; however the API to support these
                // details does not exist yet.
                //
                // For now, just bundle all the possible errors into a generic connection failed.
                sessionManager.ConnectionState = SessionConnectionState.ConnectToServiceFailed;
            }

            return sessionManager;
        }

        public void Destroy()
        {
            if (m_NativeSession != null)
            {
                m_NativeSession.Destroy();
                m_NativeSession = null;
            }
        }

        public bool CheckSupported(ARCoreSessionConfig config)
        {
            return m_NativeSession.SessionApi.CheckSupported(config) == ApiArStatus.Success;
        }

        public bool SetConfiguration(ARCoreSessionConfig config)
        {
            return m_NativeSession.SessionApi.SetConfiguration(config);
        }

        public void SetReferenceImageDatabase(ARCoreSessionConfig config, ReferenceImageDatabase database)
        {
            m_NativeSession.SessionConfigApi.SetReferenceImageDatabase(IntPtr.Zero, database.m_ReferenceImageDatabaseNativeHandle);
        }

        public ReferenceImageDatabase GetReferenceImageDatabase()
        {
            return new ReferenceImageDatabase(m_NativeSession, m_NativeSession.SessionConfigApi.GetReferenceImageDatabase(IntPtr.Zero));
        }

        public bool Update(IntPtr frameHandle)
        {
            m_NativeSession.SessionApi.SetDisplayGeometry(Screen.orientation, Screen.width, Screen.height);
            FrameManager.UpdateFrame(frameHandle);

            return true;
        }

        public bool Resume(ARCoreSessionConfig sessionConfig)
        {
            return m_NativeSession.Resume(sessionConfig);
        }

        public bool Pause()
        {
            return m_NativeSession.Pause();
        }

        public Anchor CreateWorldAnchor(Pose pose)
        {
            return m_NativeSession.SessionApi.CreateAnchor(pose);
        }

        public NativeSession GetNativeSession()
        {
            return m_NativeSession;
        }

		public bool HostAndAcquireNewCloudAnchor(IntPtr anchor, out IntPtr outputAnchor)
		{
			return m_NativeSession.SessionApi.HostAndAcquireNewCloudAnchor(anchor, out outputAnchor) == ApiArStatus.Success;
		}

		public bool ResolveAndAcquireNewCloudAnchor(String anchorId, out IntPtr outputAnchor)
		{
			return m_NativeSession.SessionApi.ResolveAndAcquireNewCloudAnchor(anchorId, out outputAnchor) == ApiArStatus.Success;
		}

		public void SetKeyAndSecret(string key, string secret)
		{
			m_NativeSession.SessionApi.SetKeyAndSecret (key, secret);
		}

        public float GetMapQuality()
        {
            return m_NativeSession.SessionApi.GetMapQuality();
        }

        public ApiAlgorithmState GetMapState()
        {
            return m_NativeSession.SessionApi.GetMapState();
        }


    }
}

 #endif