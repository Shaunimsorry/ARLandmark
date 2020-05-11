#if  UNITY_ANDROID || UNITY_EDITOR 
//-----------------------------------------------------------------------
// <copyright file="SessionApi.cs" company="Google">
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
//modify variable name
//remove ArSession_reportEngineType
//add GetSLAMInfo
//add GetDisplaySize
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
    public class SessionApi
    {
        private NativeSession m_NativeSession;
        private IntPtr m_MapHandle = IntPtr.Zero;

        public SessionApi(NativeSession nativeSession)
        {
            m_NativeSession = nativeSession;
            ExternApi.arWorldAcquireMap(m_NativeSession.SessionHandle, ref m_MapHandle);
        }

        public ApiArStatus CheckSupported(ARCoreSessionConfig config)
        {
            IntPtr configHandle;
            if (config == null)
            {
                configHandle = IntPtr.Zero;
                return ApiArStatus.ErrorUnsupportedConfiguration;
            }
            else
            {
                configHandle = m_NativeSession.SessionConfigApi.Create();
                m_NativeSession.SessionConfigApi.UpdateApiConfigWithArCoreSessionConfig(configHandle, config);
            }

            //ApiArStatus ret = ExternApi.ArSession_checkSupported(m_NativeSession.SessionHandle, configHandle);
            m_NativeSession.SessionConfigApi.Destroy(configHandle);
            return ApiArStatus.Success;
        }

        public bool SetConfiguration(ARCoreSessionConfig sessionConfig)
        {
            IntPtr configHandle = m_NativeSession.SessionConfigApi.Create();
            m_NativeSession.SessionConfigApi.UpdateApiConfigWithArCoreSessionConfig(configHandle, sessionConfig);

            bool ret = ExternApi.arWorldConfigure(m_NativeSession.SessionHandle, configHandle) == 0;
            m_NativeSession.SessionConfigApi.Destroy(configHandle);

            return ret;
        }

        public void SetCameraTextureName(int texture_id)
        {
            ExternApi.arWorldSetCameraTextureName(m_NativeSession.SessionHandle, texture_id);
        }

        public int GetTrackableCount(ApiTrackableType type)
        {
            IntPtr listHandle = m_NativeSession.TrackableListApi.Create();
            ExternApi.arMapGetAllNodes(m_NativeSession.SessionHandle, m_MapHandle,type, listHandle);
            int count = m_NativeSession.TrackableListApi.GetCount(listHandle);
            m_NativeSession.TrackableListApi.Destroy(listHandle);

            return count;
        }

        public void GetAllTrackables(List<Trackable> trackables, ApiTrackableType type)
        {
            IntPtr listHandle = m_NativeSession.TrackableListApi.Create();
            ExternApi.arMapGetAllNodes(m_NativeSession.SessionHandle, m_MapHandle, type, listHandle);

            trackables.Clear();
            int count = m_NativeSession.TrackableListApi.GetCount(listHandle);
            for (int i = 0; i < count; i++)
            {
                IntPtr trackableHandle = m_NativeSession.TrackableListApi.AcquireItem(listHandle, i);
                trackables.Add(m_NativeSession.TrackableFactory(trackableHandle));
            }

            m_NativeSession.TrackableListApi.Destroy(listHandle);
        }

        public Anchor CreateAnchor(Pose pose)
        {
            IntPtr poseHandle = m_NativeSession.PoseApi.Create(pose);
            IntPtr anchorHandle = IntPtr.Zero;
            ExternApi.arMapAcquireNewAnchor(m_NativeSession.SessionHandle, m_MapHandle, poseHandle, ref anchorHandle);
            var anchorResult = Anchor.AnchorFactory(anchorHandle, m_NativeSession);
            m_NativeSession.PoseApi.Destroy(poseHandle);

            return anchorResult;
        }

        public void SetDisplayGeometry(ScreenOrientation orientation, int width, int height)
        {
            const int androidRotation0 = 0;
            const int androidRotation90 = 1;
            const int androidRotation180 = 2;
            const int androidRotation270 = 3;

            int androidOrientation = 0;
            switch (orientation)
            {
                case ScreenOrientation.LandscapeLeft:
                    androidOrientation = androidRotation90;
                    break;
                case ScreenOrientation.LandscapeRight:
                    androidOrientation = androidRotation270;
                    break;
                case ScreenOrientation.Portrait:
                    androidOrientation = androidRotation0;
                    break;
                case ScreenOrientation.PortraitUpsideDown:
                    androidOrientation = androidRotation180;
                    break;
            }

            ExternApi.arWorldSetDisplayGeometry(m_NativeSession.SessionHandle, androidOrientation, width, height);
        }

        public void getAllPlanesVertexCount(ref int vertex_count)
        {
            ExternApi.arMapGetAllPlanesVertexCount(m_NativeSession.SessionHandle, m_MapHandle, ref vertex_count);
        }

        public void getAllPlanesVertexArray(float[] vertices)
        {
            ExternApi.arMapGetAllPlanesVertexArray(m_NativeSession.SessionHandle, m_MapHandle, vertices);
        }

        public void getAllPlanesIndexCount(ref int mesh_index_count)
        {
            ExternApi.arMapGetAllPlanesIndexCount(m_NativeSession.SessionHandle, m_MapHandle, ref mesh_index_count);
        }

        public void getAllPlanesIndexArray(ushort[] meshIndices)
        {
            ExternApi.arMapGetAllPlanesIndexArray(m_NativeSession.SessionHandle, m_MapHandle, meshIndices);
        }

        public void GetSLAMInfo(byte[] SLAMInfo)
		{
            int length = 0;
            ExternApi.arWorldGetStringValue (m_NativeSession.SessionHandle, ApiParameterEnum.SLAM_Info, SLAMInfo, ref length, 2048);
		}

        public void getVerticalFov(ref float fov)
        {
            float[] value = new float[1];
            ExternApi.arWorldGetFloatValue(m_NativeSession.SessionHandle, ApiParameterEnum.Video_Vertical_Fov, value, 1);
            fov = value[0];
        }

        public ApiArStatus HostAndAcquireNewCloudAnchor(IntPtr anchor, out IntPtr outputAnchor)
		{
			outputAnchor = IntPtr.Zero;
            ApiArStatus status;
			status = ExternApi.arWorldHostAnchor(m_NativeSession.SessionHandle, anchor, ref outputAnchor);
            Debug.Log("justsoso" + status);
            return status;
		}

		public ApiArStatus ResolveAndAcquireNewCloudAnchor(String anchorId, out IntPtr outputAnchor)
		{
			outputAnchor = IntPtr.Zero;
			return ExternApi.arWorldResolveAnchor (m_NativeSession.SessionHandle, anchorId, ref outputAnchor);
		}

		public void SetKeyAndSecret(String key, String secret)
		{
			ExternApi.arWorldSetKeyAndSecret (m_NativeSession.SessionHandle, key, secret);
		}

        public float GetMapQuality()
        {
            float[] quality = new float[1];
            ExternApi.arWorldGetFloatValue(m_NativeSession.SessionHandle, ApiParameterEnum.SLAM_Map_Quality, quality, 1);
            return quality[0];
        }

        public ApiAlgorithmState GetMapState()
        {
            int[] state = new int[1];
            ExternApi.arWorldGetIntValue(m_NativeSession.SessionHandle, ApiParameterEnum.SLAM_Detail_State, state, 1);
            return (ApiAlgorithmState)state[0];
        }

        private struct ExternApi
        {
            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern ApiArStatus arWorldAcquireMap(IntPtr sessionHandle, ref IntPtr mapHandle);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arWorldDestroy(IntPtr sessionHandle);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern ApiArStatus arWorldConfigure(IntPtr sessionHandle, IntPtr config);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arWorldSetDisplayGeometry(IntPtr sessionHandle, int rotation, int width,
                int height);

			[DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arWorldGetIntValue(IntPtr sessionHandle, ApiParameterEnum type, int[] value, int size);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arWorldSetIntValue(IntPtr sessionHandle, ApiParameterEnum type, int[] value, int size);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arWorldGetFloatValue(IntPtr sessionHandle, ApiParameterEnum type, float[] value, int size);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arWorldSetFloatValue(IntPtr sessionHandle, ApiParameterEnum type, float[] value, int size);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arWorldSetStringValue(IntPtr sessionHandle, ApiParameterEnum type, byte[] value, int size);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arWorldGetStringValue(IntPtr sessionHandle, ApiParameterEnum type, byte[] value, ref int length, int size);

			[DllImport(ApiConstants.SenseARNativeApi)]
			public static extern ApiArStatus arWorldHostAnchor(IntPtr sessionHandle, IntPtr anchor, ref IntPtr outputAnchor);

			[DllImport(ApiConstants.SenseARNativeApi)]
			public static extern ApiArStatus arWorldResolveAnchor(IntPtr sessionHandle, String anchorId, ref IntPtr outputAnchor);

			[DllImport(ApiConstants.SenseARNativeApi)]
			public static extern void arWorldSetKeyAndSecret(IntPtr SessionHandle, String key, String secret);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arWorldSetCameraTextureName(IntPtr SessionHandle, int texture_id);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern ApiArStatus arMapAcquireNewAnchor(IntPtr sessionHandle, IntPtr mapHandle, IntPtr poseHandle, ref IntPtr anchorHandle);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arMapGetAllNodes(IntPtr sessionHandle, IntPtr mapHandle, ApiTrackableType filterType, IntPtr trackableList);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arMapGetAllPlanesVertexCount(IntPtr sessionHandle, IntPtr mapHandle, ref int count);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arMapGetAllPlanesIndexCount(IntPtr sessionHandle, IntPtr mapHandle, ref int count);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arMapGetAllPlanesVertexArray(IntPtr sessionHandle, IntPtr mapHandle, float[] vertices);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arMapGetAllPlanesIndexArray(IntPtr sessionHandle, IntPtr mapHandle, ushort[] indices);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arMapRelease(IntPtr mapHandle);

        }
    }
}
 #endif