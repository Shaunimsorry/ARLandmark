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
    public class FrameApi
    {
        private NativeSession m_NativeSession;

        public FrameApi(NativeSession nativeSession)
        {
            m_NativeSession = nativeSession;
        }

        public IntPtr AcquireCamera(IntPtr frameHandle)
        {
            if (frameHandle == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            IntPtr cameraHandle = IntPtr.Zero;
            ExternApi.arFrameAcquireCamera(m_NativeSession.SessionHandle, frameHandle,
                ref cameraHandle);
            return cameraHandle;
        }

        public IntPtr AcquirePointCloud(IntPtr frameHandle)
        {
            IntPtr pointCloudHandle = IntPtr.Zero;
            ExternApi.arFrameAcquirePointCloud(m_NativeSession.SessionHandle, frameHandle,
                ref pointCloudHandle);
            return pointCloudHandle;
        }

        public IntPtr AcquireDenseMesh(IntPtr frameHandle)
        {
            IntPtr denseMeshHandle = IntPtr.Zero;
            ExternApi.arFrameAcquireDenseMesh(m_NativeSession.SessionHandle, frameHandle,
                ref denseMeshHandle);
            return denseMeshHandle;
        }

        public LightEstimate GetLightEstimate(IntPtr frameHandle)
        {
            IntPtr lightEstimateHandle = m_NativeSession.LightEstimateApi.Create();
            ExternApi.arFrameGetLightEstimate(m_NativeSession.SessionHandle, frameHandle,
                lightEstimateHandle);

            LightEstimateState state = m_NativeSession.LightEstimateApi.GetState(lightEstimateHandle);
            float pixelIntensity = m_NativeSession.LightEstimateApi.GetPixelIntensity(lightEstimateHandle);
            Color colorCorrection = m_NativeSession.LightEstimateApi.GetColorCorrection(lightEstimateHandle);

            m_NativeSession.LightEstimateApi.Destroy(lightEstimateHandle);

            return new LightEstimate(state, pixelIntensity, colorCorrection);
        }

        public void TransformDisplayUvCoords(IntPtr frameHandle, ref ApiDisplayUvCoords uv)
        {
            ApiDisplayUvCoords uvOut = new ApiDisplayUvCoords();
            ExternApi.arFrameTransformDisplayUvCoords(m_NativeSession.SessionHandle, frameHandle,
                ApiDisplayUvCoords.NumFloats, ref uv, ref uvOut);

            uv = uvOut;
        }

        public bool isImageAvaliable(IntPtr frameHandle)
        {
            return ExternApi.arFrameIsImageDataUpdated(m_NativeSession.SessionHandle, frameHandle);
        }

        public void GetImageResolution(IntPtr frameHandle, ref int width, ref int height)
        {
            ExternApi.arFrameGetImageResolution(m_NativeSession.SessionHandle, frameHandle, ref width, ref height);
        }

        public bool isDepthImageAvaliable(IntPtr frameHandle)
        {
            return ExternApi.arFrameIsDepthImageAvaliable(m_NativeSession.SessionHandle, frameHandle);
        }

        public void GetDepthImageResolution(IntPtr frameHandle, ref int width, ref int height)
        {
            ExternApi.arFrameGetDepthImageResolution(m_NativeSession.SessionHandle,frameHandle, ref width, ref height);
        }

        public void GetDepthImageFormat(IntPtr frameHandle, ref ARPixelFormat format)
        {
            ExternApi.arFrameGetDepthImageFormat(m_NativeSession.SessionHandle, frameHandle, ref format);
        }

        public void GetDepthImageData(IntPtr frameHandle, byte[] data)
        {
            ApiArStatus status = ExternApi.arFrameGetDepthImageData(m_NativeSession.SessionHandle, frameHandle, data);
        }

        public void GetDepthImageData16(IntPtr frameHandle, UInt16[] data)
        {
            ExternApi.ArUnity_GetDepthImageDataUInt16(m_NativeSession.SessionHandle, frameHandle, data);
        }

        private struct ExternApi
        {
            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern int arFrameAcquireCamera(IntPtr sessionHandle, IntPtr frameHandle,
                ref IntPtr cameraHandle);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern int arFrameAcquirePointCloud(IntPtr sessionHandle, IntPtr frameHandle,
                ref IntPtr pointCloudHandle);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern int arFrameAcquireDenseMesh(IntPtr sessionHandle, IntPtr frameHandle,
                ref IntPtr denseMeshHandle);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arFrameTransformDisplayUvCoords(IntPtr session, IntPtr frame,
                int numElements, ref ApiDisplayUvCoords uvsIn, ref ApiDisplayUvCoords uvsOut);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arFrameGetLightEstimate(IntPtr sessionHandle, IntPtr frameHandle,
                IntPtr lightEstimateHandle);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern ApiArStatus arFrameGetImageResolution(IntPtr sessionHandle, IntPtr frameHandle,
                ref int width, ref int height);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern bool arFrameIsImageDataUpdated(IntPtr sessionHandle, IntPtr frameHandle);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern ApiArStatus arFrameGetDepthImageResolution(IntPtr sessionHandle, IntPtr frameHandle,
                ref int width, ref int height);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern bool arFrameIsDepthImageAvaliable(IntPtr sessionHandle, IntPtr frameHandle);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arFrameGetDepthImageFormat(IntPtr sessionHandle, IntPtr frameHandle,
                ref ARPixelFormat format);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern ApiArStatus arFrameGetDepthImageData(IntPtr sessionHandle, IntPtr frameHandle,
                 byte[] depthBuffer);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern ApiArStatus ArUnity_GetDepthImageDataUInt16(IntPtr sessionHandle, IntPtr frameHandle,
                UInt16[] depthBuffer);

        }
    }
}

 #endif