#if  UNITY_ANDROID || UNITY_EDITOR 
//-----------------------------------------------------------------------
// <copyright file="PoseApi.cs" company="Google">
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
    public class PoseApi
    {
        private NativeSession m_NativeSession;

        public PoseApi(NativeSession nativeSession)
        {
            m_NativeSession = nativeSession;
        }

        public IntPtr Create()
        {
            return Create(Pose.identity);
        }

        public IntPtr Create(Pose pose)
        {
            ApiPoseData rawPose = new ApiPoseData(pose);

            IntPtr poseHandle = IntPtr.Zero;
            ExternApi.arPoseCreate(ref rawPose, ref poseHandle);
            return poseHandle;
        }

        public void Destroy(IntPtr nativePose)
        {
            ExternApi.arPoseDestroy(nativePose);
        }

        public Pose ExtractPoseValue(IntPtr poseHandle)
        {
            ApiPoseData poseValue = new ApiPoseData(Pose.identity);
            ExternApi.arPoseGetPoseRaw(poseHandle, ref poseValue);
            return poseValue.ToUnityPose();
        }

        private struct ExternApi
        {
            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arPoseCreate(ref ApiPoseData rawPose, ref IntPtr poseHandle);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arPoseDestroy(IntPtr poseHandle);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arPoseGetPoseRaw(IntPtr poseHandle,
                ref ApiPoseData rawPose);
        }
    }
}

 #endif