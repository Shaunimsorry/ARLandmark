﻿#if  UNITY_ANDROID || UNITY_EDITOR 
//-----------------------------------------------------------------------
// <copyright file="PointApi.cs" company="Google">
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
//remove ArPoint_getOrientationMode
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
    public class PointApi
    {
        private NativeSession m_NativeSession;

        public PointApi(NativeSession nativeSession)
        {
            m_NativeSession = nativeSession;
        }

        public Pose GetPose(IntPtr pointHandle)
        {
            // TODO 
            var poseHandle = m_NativeSession.PoseApi.Create();
            //ExternApi.ArPoint_getPose(m_NativeSession.SessionHandle, planeHandle, poseHandle);
			Pose resultPose = m_NativeSession.PoseApi.ExtractPoseValue(poseHandle);
            m_NativeSession.PoseApi.Destroy(poseHandle);
            return resultPose;
        }

        //private struct ExternApi
        //{
        //    [DllImport(ApiConstants.SenseARNativeApi)]
        //    public static extern void ArPoint_getPose(IntPtr session, IntPtr point, IntPtr out_pose);
        //}
    }
}

 #endif