#if  UNITY_ANDROID || UNITY_EDITOR 
//-----------------------------------------------------------------------
// <copyright file="SessionConfigApi.cs" company="Google">
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
//remove ArConfig_setUpdateMode
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
    public class SessionConfigApi
    {
        private NativeSession m_NativeSession;

        public SessionConfigApi(NativeSession nativeSession)
        {
            m_NativeSession = nativeSession;
        }

        public IntPtr Create()
        {
            IntPtr configHandle = IntPtr.Zero;
            ExternApi.arConfigCreate(ref configHandle);
            return configHandle;
        }

        public void Destroy(IntPtr configHandle)
        {
            ExternApi.arConfigDestroy(configHandle);
        }

        public void UpdateApiConfigWithArCoreSessionConfig(IntPtr configHandle, ARCoreSessionConfig arCoreSessionConfig)
        {
            ExternApi.arConfigSetTrackingRunMode(configHandle, ApiTrackingRunMode.Auto);

            ExternApi.arConfigSetWorldAlignmentMode(configHandle, ApiWorldAlignmentMode.Gravity_Heading);

            //SLAM
            ExternApi.arConfigSetAlgorithmMode(configHandle, ApiAlgorithmType.SLAM, arCoreSessionConfig.SLAMAlgorithmMode);
            if (arCoreSessionConfig.SLAMAlgorithmMode == ApiAlgorithmMode.Enabled)
            {
                ExternApi.arConfigSetAlgorithmStreamMode(configHandle, ApiAlgorithmType.SLAM, arCoreSessionConfig.SLAMStreamMode);
            }

            //HandGesture
            ExternApi.arConfigSetAlgorithmMode(configHandle, ApiAlgorithmType.Hand_Gesture, arCoreSessionConfig.HandGestureAlgorithmMode);
            if (arCoreSessionConfig.HandGestureAlgorithmMode == ApiAlgorithmMode.Enabled)
            {
                ExternApi.arConfigSetAlgorithmStreamMode(configHandle, ApiAlgorithmType.Hand_Gesture, arCoreSessionConfig.HandGestureStreamMode);
            }

            //ImageTracking
            ExternApi.arConfigSetAlgorithmMode(configHandle, ApiAlgorithmType.Image_Tracking, arCoreSessionConfig.ImageTrackingAlgorithmMode);
            if (arCoreSessionConfig.ImageTrackingAlgorithmMode == ApiAlgorithmMode.Enabled)
            {
                ExternApi.arConfigSetAlgorithmStreamMode(configHandle, ApiAlgorithmType.Image_Tracking, arCoreSessionConfig.ImageTrackingStreamMode);
            }

            //LightEstimate
            ExternApi.arConfigSetAlgorithmMode(configHandle, ApiAlgorithmType.Light_Estimation, arCoreSessionConfig.LightEstimationAlgorithmMode);
            if (arCoreSessionConfig.LightEstimationAlgorithmMode == ApiAlgorithmMode.Enabled)
            {
                ExternApi.arConfigSetIlluminationEstimateMode(configHandle, arCoreSessionConfig.LightEstimation);
                ExternApi.arConfigSetAlgorithmStreamMode(configHandle, ApiAlgorithmType.Light_Estimation, arCoreSessionConfig.LightEstimationStreamMode);
            }

            ExternApi.arConfigSetAlgorithmMode(configHandle, ApiAlgorithmType.Plane_Detection, arCoreSessionConfig.PlaneFindingAlgorithmMode);
            if (arCoreSessionConfig.PlaneFindingAlgorithmMode == ApiAlgorithmMode.Enabled)
            {
                ExternApi.arConfigSetPlaneDetectingMode(configHandle, arCoreSessionConfig.PlaneFinding);
                ExternApi.arConfigSetAlgorithmStreamMode(configHandle, ApiAlgorithmType.Plane_Detection, arCoreSessionConfig.PlaneFindingStreamMode);
            }

            ExternApi.arConfigSetAlgorithmMode(configHandle, ApiAlgorithmType.Cloud_Anchor, arCoreSessionConfig.CloudAnchorAlgorithmMode);
            if (arCoreSessionConfig.CloudAnchorAlgorithmMode == ApiAlgorithmMode.Enabled)
            {
                ExternApi.arConfigSetAlgorithmStreamMode(configHandle, ApiAlgorithmType.Cloud_Anchor, arCoreSessionConfig.CloudAnchorStreamMode);
            }

        }

        public void SetReferenceImageDatabase(IntPtr configHandle, IntPtr imageDatabase)
        {
            ExternApi.arConfigSetReferenceImageDatabase(m_NativeSession.SessionHandle, configHandle, imageDatabase);
        }

        public IntPtr GetReferenceImageDatabase(IntPtr configHandle)
        {
            IntPtr imageDatabase = IntPtr.Zero;
            ExternApi.arConfigGetReferenceImageDatabase(m_NativeSession.SessionHandle, configHandle, ref imageDatabase);
            return imageDatabase;
        }

        private struct ExternApi
        {
            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arConfigCreate(ref IntPtr out_config);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arConfigDestroy(IntPtr config);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arConfigSetIlluminationEstimateMode(IntPtr config,
                ApiLightEstimationMode light_estimation_mode);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arConfigSetPlaneDetectingMode(IntPtr config,
                ApiPlaneFindingMode plane_finding_mode);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arConfigSetTrackingRunMode(IntPtr config,
                ApiTrackingRunMode tracking_run_mode);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arConfigSetWorldAlignmentMode(IntPtr config,
                ApiWorldAlignmentMode world_alignment_mode);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arConfigSetAlgorithmStreamMode(IntPtr config, ApiAlgorithmType algorithmType,
                ApiStreamMode streamMode);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arConfigSetAlgorithmMode(IntPtr config,
                ApiAlgorithmType algorithmType, ApiAlgorithmMode algorithm_mode);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arConfigSetReferenceImageDatabase(IntPtr session, IntPtr config,
                                                                        IntPtr imageDatabase);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arConfigGetReferenceImageDatabase(IntPtr session, IntPtr config,
                                                                        ref IntPtr imageDatabase);


        }
    }
}

 #endif