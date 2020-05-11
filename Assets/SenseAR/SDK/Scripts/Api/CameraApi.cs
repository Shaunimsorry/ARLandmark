#if  UNITY_ANDROID || UNITY_EDITOR 
//-----------------------------------------------------------------------
// <copyright file="CameraApi.cs" company="Google">
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
//modify function name
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
    public class CameraApi
    {
        private NativeSession m_NativeSession;

        public CameraApi(NativeSession nativeSession)
        {
            m_NativeSession = nativeSession;
        }

        public TrackingState GetTrackingState(IntPtr cameraHandle)
        {
            ApiTrackingState apiTrackingState = ApiTrackingState.Stopped;
            ExternApi.arCameraGetTrackingState(m_NativeSession.SessionHandle,
                cameraHandle, ref apiTrackingState);
            return apiTrackingState.ToTrackingState();
        }

        public Pose GetPose(IntPtr cameraHandle)
        {
            if (cameraHandle == IntPtr.Zero)
            {
                return Pose.identity;
            }

            IntPtr poseHandle = m_NativeSession.PoseApi.Create();
            ExternApi.arCameraGetPose(m_NativeSession.SessionHandle, cameraHandle, poseHandle);
            Pose resultPose = m_NativeSession.PoseApi.ExtractPoseValue(poseHandle);
            m_NativeSession.PoseApi.Destroy(poseHandle);
            return resultPose;
        }

        public Matrix4x4 GetProjectionMatrix(IntPtr cameraHandle, float near, float far)
        {
            Matrix4x4 matrix = Matrix4x4.identity;
            ExternApi.arCameraGetProjectionMatrix(m_NativeSession.SessionHandle, cameraHandle,
                near, far, ref matrix);
            return matrix;
        }

        public void Release(IntPtr cameraHandle)
        {
            ExternApi.arCameraRelease(cameraHandle);
        }

        private struct ExternApi
        {
            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arCameraGetTrackingState(IntPtr sessionHandle, IntPtr cameraHandle,
                ref ApiTrackingState outTrackingState);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arCameraGetPose(IntPtr sessionHandle, IntPtr cameraHandle, IntPtr outPose);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arCameraGetProjectionMatrix(IntPtr sessionHandle, IntPtr cameraHandle,
                float near, float far, ref Matrix4x4 outMatrix);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arCameraRelease(IntPtr cameraHandle);
        }
    }
}

 #endif