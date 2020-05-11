#if  UNITY_ANDROID || UNITY_EDITOR 
//-----------------------------------------------------------------------
// <copyright file="FrameManager.cs" company="Google">
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
    using SenseAR;
    using UnityEngine;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
     Justification = "Internal")]
    public class FrameManager
    {
        private NativeSession m_NativeSession;

        private IntPtr m_FrameHandle = IntPtr.Zero;

        private TrackableManager m_TrackableManager;

        private List<TrackableHit> m_TrackableHitList = new List<TrackableHit>();

        public FrameManager(NativeSession nativeSession)
        {
            m_NativeSession = nativeSession;
            m_TrackableManager = new TrackableManager(nativeSession);
            PointCloudManager = new PointCloudManager(nativeSession);
        }

        public PointCloudManager PointCloudManager { get; private set; }

        public Pose GetPose()
        {
            var cameraHandle = m_NativeSession.FrameApi.AcquireCamera(m_FrameHandle);
            Pose result = m_NativeSession.CameraApi.GetPose(cameraHandle);
            m_NativeSession.CameraApi.Release(cameraHandle);
            return result;
        }

        public LightEstimate GetLightEstimate()
        {
            return m_NativeSession.FrameApi.GetLightEstimate(m_FrameHandle);
        }

        public void TransformDisplayUvCoords(ref ApiDisplayUvCoords uvQuad)
        {
            m_NativeSession.FrameApi.TransformDisplayUvCoords(m_FrameHandle, ref uvQuad);
        }

        public Matrix4x4 GetCameraProjectionMatrix(float nearClipping, float farClipping)
        {
            var cameraHandle = m_NativeSession.FrameApi.AcquireCamera(m_FrameHandle);
            var result = m_NativeSession.CameraApi.GetProjectionMatrix(cameraHandle, nearClipping, farClipping);
            m_NativeSession.CameraApi.Release(cameraHandle);
            return result;
        }

        public TrackingState GetCameraTrackingState()
        {
            var cameraHandle = m_NativeSession.FrameApi.AcquireCamera(m_FrameHandle);
            TrackingState result = m_NativeSession.CameraApi.GetTrackingState(cameraHandle);
            m_NativeSession.CameraApi.Release(cameraHandle);
            return result;
        }

        public void SetHitTestMode(ApiHitTestMode mode)
        {
            m_NativeSession.HitTestApi.SetHitTestMode(m_FrameHandle, mode);
        }

        public bool Raycast(float x, float y, TrackableHitFlags filter, out TrackableHit hitResult)
        {
            hitResult = new TrackableHit();

            // Note that the Unity's screen coordinate (0, 0) starts from bottom left.
            bool ret = m_NativeSession.HitTestApi.Raycast(m_FrameHandle, x, Screen.height - y, filter, m_TrackableHitList,
                true);
            if (ret && m_TrackableHitList.Count != 0)
            {
                hitResult = m_TrackableHitList[0];
            }

            return ret;
        }

        public bool Raycast(float[] ray_origin, float[] ray_direction, TrackableHitFlags filter, out TrackableHit hitResult)
        {
            hitResult = new TrackableHit();

            // Note that the Unity's screen coordinate (0, 0) starts from bottom left.
            bool ret = m_NativeSession.HitTestApi.Raycast(m_FrameHandle, ray_origin, ray_direction, filter, m_TrackableHitList,
                true);
            if (ret && m_TrackableHitList.Count != 0)
            {
                hitResult = m_TrackableHitList[0];
            }

            return ret;
        }

        public bool RaycastAll(float x, float y, TrackableHitFlags filter, List<TrackableHit> hitResults)
        {
            // Note that the Unity's screen coordinate (0, 0) starts from bottom left.
            return m_NativeSession.HitTestApi.Raycast(m_FrameHandle, x, Screen.height - y, filter, hitResults, true);
        }

        public void UpdateFrame(IntPtr frameHandle)
        {
            m_FrameHandle = frameHandle;
            PointCloudManager.UpdateFrame(m_FrameHandle);
        }
    }
}

 #endif