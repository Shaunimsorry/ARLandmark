#if  UNITY_ANDROID || UNITY_EDITOR 
//-----------------------------------------------------------------------
// <copyright file="TrackableApi.cs" company="Google">
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
//remove ArTrackable_getAnchors
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
    public class TrackableApi
    {
        private NativeSession m_NativeSession;

        public TrackableApi(NativeSession nativeSession)
        {
            m_NativeSession = nativeSession;
        }

        public ApiTrackableType GetType(IntPtr trackableHandle)
        {
            ApiTrackableType type = ApiTrackableType.Plane;
            ExternApi.arNodeGetType(m_NativeSession.SessionHandle, trackableHandle, ref type);
            return type;
        }

        public TrackingState GetTrackingState(IntPtr trackableHandle)
        {
            ApiTrackingState apiTrackingState = ApiTrackingState.Stopped;
            ExternApi.arNodeGetTrackingState(m_NativeSession.SessionHandle, trackableHandle,
                ref apiTrackingState);
            return apiTrackingState.ToTrackingState();
        }

        public bool AcquireNewAnchor(IntPtr trackableHandle, Pose pose, out IntPtr anchorHandle)
        {
            IntPtr poseHandle = m_NativeSession.PoseApi.Create(pose);
            anchorHandle = IntPtr.Zero;
            int status = ExternApi.arNodeAcquireNewAnchor(m_NativeSession.SessionHandle, trackableHandle, poseHandle,
                ref anchorHandle);
            m_NativeSession.PoseApi.Destroy(poseHandle);
            return status == 0;
        }

        public void Release(IntPtr trackableHandle)
        {
            ExternApi.arNodeRelease(trackableHandle);
        }

        private struct ExternApi
        {
            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arNodeGetType(IntPtr sessionHandle, IntPtr trackableHandle,
                ref ApiTrackableType trackableType);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arNodeGetTrackingState(IntPtr sessionHandle,
                IntPtr trackableHandle, ref ApiTrackingState trackingState);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern int arNodeAcquireNewAnchor(IntPtr sessionHandle, IntPtr trackableHandle,
                IntPtr poseHandle, ref IntPtr anchorHandle);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arNodeRelease(IntPtr trackableHandle);
        }
    }
}

 #endif