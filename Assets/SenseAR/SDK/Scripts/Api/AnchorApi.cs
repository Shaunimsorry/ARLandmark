#if  UNITY_ANDROID || UNITY_EDITOR 
//-----------------------------------------------------------------------
// <copyright file="AnchorApi.cs" company="Google">
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
//remove ArAnchor_detach
//modify variable name
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
    public class AnchorApi
    {
		private NativeSession m_NativeSession;

        public AnchorApi(NativeSession nativeSession)
        {
            m_NativeSession = nativeSession;
        }

        public Pose GetPose(IntPtr anchorHandle)
        {
            var poseHandle = m_NativeSession.PoseApi.Create();
            ExternApi.arAnchorGetPose(m_NativeSession.SessionHandle, anchorHandle, poseHandle);
            Pose resultPose = m_NativeSession.PoseApi.ExtractPoseValue(poseHandle);
            m_NativeSession.PoseApi.Destroy(poseHandle);
            return resultPose;
        }

        public TrackingState GetTrackingState(IntPtr anchorHandle)
        {
            ApiTrackingState trackingState = ApiTrackingState.Stopped;
            ExternApi.arAnchorGetTrackingState(m_NativeSession.SessionHandle, anchorHandle,
                ref trackingState);
            return trackingState.ToTrackingState();
        }

        public void Release(IntPtr anchorHandle)
        {
            ExternApi.arAnchorRelease(anchorHandle);
        }

		public string acquireCloudAnchorId(IntPtr anchorHandle)
		{
			byte[] anchorId = new byte[64];
			ExternApi.arAnchorGetCloudAnchorId (m_NativeSession.SessionHandle, anchorHandle, anchorId, 64);
			var result = System.Text.Encoding.Default.GetString(anchorId);
			return result;
		}

		public ApiCloudAnchorState getCloudAnchorState(IntPtr anchorHandle)
		{
			ApiCloudAnchorState state = ApiCloudAnchorState.None;
			ExternApi.arAnchorGetCloudState (m_NativeSession.SessionHandle, anchorHandle, ref state);
			return state;
		}

        public IntPtr CreateList()
        {
            IntPtr listHandle = IntPtr.Zero;
            ExternApi.arAnchorListCreate(m_NativeSession.SessionHandle, ref listHandle);
            return listHandle;
        }

        public int GetListSize(IntPtr anchorListHandle)
        {
            int size = 0;
            ExternApi.arAnchorListGetSize(m_NativeSession.SessionHandle, anchorListHandle, ref size);
            return size;
        }

        public IntPtr AcquireListItem(IntPtr anchorListHandle, int index)
        {
            IntPtr anchorHandle = IntPtr.Zero;
            ExternApi.arAnchorListAcquireItem(m_NativeSession.SessionHandle, anchorListHandle, index,
                ref anchorHandle);
            return anchorHandle;
        }

        public void DestroyList(IntPtr anchorListHandle)
        {
            ExternApi.arAnchorListDestroy(anchorListHandle);
        }

        private struct ExternApi
        {
            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arAnchorGetPose(IntPtr sessionHandle, IntPtr anchorHandle, IntPtr poseHandle);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arAnchorGetTrackingState(IntPtr sessionHandle, IntPtr anchorHandle,
                ref ApiTrackingState trackingState);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arAnchorRelease(IntPtr anchorHandle);

			[DllImport(ApiConstants.SenseARNativeApi)]
			public static extern void arAnchorGetCloudAnchorId(IntPtr SessionHandle, IntPtr anchorHandle, byte[] anchorId, int size);

			[DllImport(ApiConstants.SenseARNativeApi)]
			public static extern void arAnchorGetCloudState(IntPtr SessionHandle, IntPtr anchorHandle, ref ApiCloudAnchorState outputCloudState);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arAnchorListCreate(IntPtr sessionHandle, ref IntPtr outputAnchorListHandle);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arAnchorListDestroy(IntPtr anchorListHandle);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arAnchorListGetSize(IntPtr sessionHandle, IntPtr anchorListHandle, ref int outputSize);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arAnchorListAcquireItem(IntPtr sessionHandle, IntPtr anchorListHandle,  int index,
                ref IntPtr outputAnchorHandle);
        }
    }
}

 #endif