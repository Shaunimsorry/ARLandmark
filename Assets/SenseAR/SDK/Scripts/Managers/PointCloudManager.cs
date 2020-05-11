#if  UNITY_ANDROID || UNITY_EDITOR 
//-----------------------------------------------------------------------
// <copyright file="PointCloudManager.cs" company="Google">
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
    public class PointCloudManager
    {
        private NativeSession m_NativeSession;

        private IntPtr m_PointCloudHandle = IntPtr.Zero;

        private long m_LastUpdateTimeStamp = -1;

        public PointCloudManager(NativeSession nativeSession)
        {
            m_NativeSession = nativeSession;
        }

        public void UpdateFrame(IntPtr frameHandle)
        {
            if (m_PointCloudHandle != IntPtr.Zero)
            {
                // After first frame, release previous frame's point cloud.
                m_NativeSession.PointCloudApi.Release(m_PointCloudHandle);
            }

            m_PointCloudHandle = m_NativeSession.FrameApi.AcquirePointCloud(frameHandle);
        }

        public bool GetIsUpdatedThisFrame()
        {
            return true;
        }

        public int GetPointCount()
        {
            return m_NativeSession.PointCloudApi.GetNumberOfPoints(m_PointCloudHandle);
        }

        public Vector4 GetPoint(int index)
        {
            return m_NativeSession.PointCloudApi.GetPoint(m_PointCloudHandle, index);
        }

        public void CopyPoints(List<Vector4> points)
        {
            m_NativeSession.PointCloudApi.CopyPoints(m_PointCloudHandle, points);
        }
    }
}

 #endif