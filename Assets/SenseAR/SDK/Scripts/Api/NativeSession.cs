#if  UNITY_ANDROID || UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="NativeSession.cs" company="Google">
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
    using System.Runtime.InteropServices;
    using SenseAR;
    using UnityEngine;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
    Justification = "Internal")]
    public class NativeSession
    {
        private IntPtr m_SessionHandle = IntPtr.Zero;

        private int m_Texture_id = 0;

        private NativeSession()
        {
        }

        ~NativeSession()
        {
            Destroy();
        }

        public IntPtr SessionHandle
        {
            get
            {
                return m_SessionHandle;
            }
        }



        public static NativeSession CreateSession()
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");

            NativeSession nativeSession = new NativeSession();
            ExternApi.arApplicationCreate( context.GetRawObject(), ref nativeSession.m_SessionHandle);

            if (nativeSession.m_SessionHandle == IntPtr.Zero)
            {
                Debug.LogError("ARCore failed to create a session.");
                return null;
            }

            nativeSession.AnchorApi = new AnchorApi(nativeSession);
            nativeSession.CameraApi = new CameraApi(nativeSession);
            nativeSession.FrameApi = new FrameApi(nativeSession);
            nativeSession.HitTestApi = new HitTestApi(nativeSession);
            nativeSession.LightEstimateApi = new LightEstimateApi(nativeSession);
            nativeSession.PlaneApi = new PlaneApi(nativeSession);
            nativeSession.PointApi = new PointApi(nativeSession);
            nativeSession.PointCloudApi = new PointCloudApi(nativeSession);
            nativeSession.PoseApi = new PoseApi(nativeSession);
            nativeSession.SessionApi = new SessionApi(nativeSession);
            nativeSession.SessionConfigApi = new SessionConfigApi(nativeSession);
            nativeSession.TrackableApi = new TrackableApi(nativeSession);
            nativeSession.TrackableListApi = new TrackableListApi(nativeSession);
            nativeSession.HandGestureApi = new HandGestureApi(nativeSession);
            nativeSession.ImageApi = new ImageApi(nativeSession);
            nativeSession.ReferenceImageDatabaseApi = new ReferenceImageDatabaseApi(nativeSession);

            return nativeSession;
        }

        public void Destroy()
        {
            if (m_SessionHandle != IntPtr.Zero)
            {
                ExternApi.arApplicationPause();
				ExternApi.arApplicationDestroy(ref m_SessionHandle);
				m_SessionHandle = IntPtr.Zero;
            }
        }

        public static ApiArStatus CheckAuthorized(string appId)
        {
            byte[] tmpStr2 = new byte[appId.Length + 1];
            tmpStr2[appId.Length] = 0;
            byte[] tmpStr3 = System.Text.Encoding.Default.GetBytes(appId);
            Array.Copy(tmpStr3, tmpStr2, appId.Length);


            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
            return ExternApi.arApplicationCheckAuthorized(context.GetRawObject(), tmpStr2);
        }

        public static ApiArStatus CheckAlgorithm(ApiAlgorithmType type, ApiAlgorithmMode mode)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");

            return ExternApi.arApplicationCheckAlgorithm(context.GetRawObject(), type, mode);
        }

        public void StopSLAM()
        {
            ExternApi.arApplicationStop();
        }

        public void StartSLAM()
        {
            ExternApi.arApplicationStartUp();
        }

        public bool Resume(ARCoreSessionConfig sessionConfig)
        {
            bool result = ExternApi.arApplicationResume();
            if (sessionConfig.SLAMAlgorithmMode == ApiAlgorithmMode.Enabled)
            {
                ExternApi.Util_CreateTextureOES(ref m_Texture_id);
                SessionApi.SetCameraTextureName(m_Texture_id);
            }
            return result;
        }

        public bool Pause()
        {
            return ExternApi.arApplicationPause();
        }

        public AnchorApi AnchorApi { get; private set; }

        public CameraApi CameraApi { get; private set; }

        public FrameApi FrameApi { get; private set; }

        public HitTestApi HitTestApi { get; private set; }

        public LightEstimateApi LightEstimateApi { get; private set; }

        public PlaneApi PlaneApi { get; private set; }

        public PointApi PointApi { get; private set; }

        public PointCloudApi PointCloudApi { get; private set; }

        public HandGestureApi HandGestureApi {get; private set;}

        public PoseApi PoseApi { get; private set; }

        public SessionApi SessionApi { get; private set; }

        public SessionConfigApi SessionConfigApi { get; private set; }

        public TrackableApi TrackableApi { get; private set; }

        public TrackableListApi TrackableListApi { get; private set; }

        public ImageApi ImageApi { get; private set; }

        public ReferenceImageDatabaseApi ReferenceImageDatabaseApi { get; private set; }

        public IntPtr GetFramePtr()
        {
            IntPtr FrameHandle = IntPtr.Zero;
            ExternApi.arApplicationGetFrame(ref FrameHandle);
            return FrameHandle;
        }

        public bool Update()
        {
            return ExternApi.arApplicationUpdate();
        }

        public bool isVideoUpdated()
        {
            return FrameApi.isImageAvaliable(GetFramePtr());
        }

		public void updateTexture_y_memcpy_uv_assign_RGB_pose(byte[] grayPtr, byte[] uvPtr, IntPtr PoseHandle)
		{
			ExternApi.arApplicationGetFrame_Y8_UVRGB_AndPose(m_SessionHandle, grayPtr, uvPtr, PoseHandle);
		}

		public void getTextureSize(ref int width, ref int height)
		{
            FrameApi.GetImageResolution(GetFramePtr(), ref width, ref height);
		}

        public void getVerticalFov(ref float oVerticalFov)
        {
            SessionApi.getVerticalFov(ref oVerticalFov);
        }

        /// <summary>
        /// Factory method for creating and reusing TrackedPlane references from native handles.
        /// </summary>
        /// <param name="nativeHandle">A native handle to a plane that has been acquired.  RELEASE WILL BE HANDLED BY
        /// THIS METHOD.</param>
        /// <returns>A reference to a tracked plane. </returns>
        public Trackable TrackableFactory(IntPtr nativeHandle)
        {
            if (nativeHandle == IntPtr.Zero)
            {
                return null;
            }

            Trackable result;

            ApiTrackableType trackableType = TrackableApi.GetType(nativeHandle);
            if (trackableType == ApiTrackableType.Plane)
            {
                result = new TrackedPlane(nativeHandle, this);
            }
            else if(trackableType == ApiTrackableType.HandGesture)
            {
                result = new TrackedHandGesture(nativeHandle, this);
            }
            else if (trackableType == ApiTrackableType.Image)
            {
                result = new TrackedImage(nativeHandle, this);
            }
            else
            {
                UnityEngine.Debug.LogFormat("Cant find {0}", trackableType);
                throw new NotImplementedException("TrackableFactory:: No contructor for requested trackable type.");
            }

            return result;
        }

        public static void CheckApkAvailability(ref ApiArAvailability arAvailability)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            ExternApi.arApplicationApkCheckAvailability(activity.GetRawObject(), ref arAvailability);
        }

        public static ApiArStatus RequestInstall(int userRequestInstall, ref ApiArInstallStatus installStatus)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            return ExternApi.arApplicationApkRequestInstall(activity.GetRawObject(), userRequestInstall, ref installStatus);
        }

        public void GetCameraTextureID(ref int cameraTextureId)
        {
            cameraTextureId = m_Texture_id;
        }

        private struct ExternApi
        {
            [DllImport(ApiConstants.SenseARShimApi)]
            public static extern ApiArStatus arApplicationCheckAuthorized(IntPtr appContext, byte[] appId);

            [DllImport(ApiConstants.SenseARShimApi)]
            public static extern ApiArStatus arApplicationCheckAlgorithm(IntPtr appContextd, ApiAlgorithmType type, ApiAlgorithmMode mode);

            [DllImport(ApiConstants.SenseARShimApi)]
            public static extern void arApplicationCreate( IntPtr appContext, ref IntPtr sessionHandle);

            [DllImport(ApiConstants.SenseARShimApi)]
			public static extern void arApplicationDestroy(ref IntPtr sessionHandle);

            [DllImport(ApiConstants.SenseARShimApi)]
            public static extern void arApplicationGetFrame(ref IntPtr FrameHandle);

            [DllImport(ApiConstants.SenseARShimApi)]
            public static extern bool arApplicationResume();

            [DllImport(ApiConstants.SenseARShimApi)]
            public static extern bool arApplicationPause();

            [DllImport(ApiConstants.SenseARShimApi)]
            public static extern bool arApplicationUpdate();

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arApplicationStartUp();

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arApplicationStop();

			[DllImport(ApiConstants.SenseARShimApi)]
			public static extern void arApplicationGetFrame_Y8_UVRGB_AndPose(IntPtr sessionHandle, byte[] grayPtr, byte[] uvPtr, IntPtr out_pose);

            [DllImport(ApiConstants.SenseARShimApi)]
            public static extern void arApplicationApkCheckAvailability(IntPtr activity, ref ApiArAvailability out_availability);

            [DllImport(ApiConstants.SenseARShimApi)]
            public static extern ApiArStatus arApplicationApkRequestInstall(IntPtr activity, int user_requested_install, ref ApiArInstallStatus out_install_status);

            [DllImport(ApiConstants.SenseARUtilApi)]
            public static extern void Util_CreateTextureOES(ref int textureId);

        }
    }
}

 #endif