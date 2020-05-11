#if  UNITY_ANDROID || UNITY_EDITOR 
//-----------------------------------------------------------------------
// <copyright file="FrameApi.cs" company="Google">
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
//modify AcquireCamera
//remove AcquireCameraImageBytes
//modify AcquirePointCloud
//modify GetLightEstimate
//remove AcquireImageMetadata
//modify TransformDisplayUvCoords
//modify GetUpdatedTrackables
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
    public class ImageApi
    {
        private NativeSession m_NativeSession;

        public ImageApi(NativeSession nativeSession)
        {
            m_NativeSession = nativeSession;
        }

        public Pose GetCenterPose(IntPtr imageHandle)
        {
            var poseHandle = m_NativeSession.PoseApi.Create();
            ExternApi.arImageNodeGetCenterPose(m_NativeSession.SessionHandle, imageHandle, poseHandle);
            Pose resultPose = m_NativeSession.PoseApi.ExtractPoseValue(poseHandle);
            m_NativeSession.PoseApi.Destroy(poseHandle);
            return resultPose;
        }

        public float GetExtentX(IntPtr imageHandle)
        {
            float extent_x = 0.0f;
            ExternApi.arImageNodeGetExtentX(m_NativeSession.SessionHandle, imageHandle, ref extent_x);
            return extent_x;
        }

        public float GetExtentZ(IntPtr imageHandle)
        {
            float extent_z = 0.0f;
            ExternApi.arImageNodeGetExtentZ(m_NativeSession.SessionHandle, imageHandle, ref extent_z);
            return extent_z;
        }

        public int GetIndex(IntPtr imageHandle)
        {
            int index = 0;
            ExternApi.arImageNodeGetIndex(m_NativeSession.SessionHandle, imageHandle, ref index);
            return index;
        }

        public byte[] GetName(IntPtr imageHandle)
        {
            byte[] name = null;
            ExternApi.arImageNodeGetName(m_NativeSession.SessionHandle, imageHandle, ref name);
            return name;
        }

        private struct ExternApi
        {
            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arImageNodeGetCenterPose(IntPtr sessionHandle, IntPtr imageHandle,
                IntPtr poseHandle);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arImageNodeGetExtentX(IntPtr sessionHandle, IntPtr imageHandle,
                ref float out_extent_x);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arImageNodeGetExtentZ(IntPtr sessionHandle, IntPtr imageHandle,
                ref float out_extent_z);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arImageNodeGetIndex(IntPtr sessionHandle, IntPtr imageHandle,
                ref int out_index);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arImageNodeGetName(IntPtr sessionHandle, IntPtr imageHandle,
               ref byte[] name);

        }
    }
}

#endif