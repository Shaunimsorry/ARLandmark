#if  UNITY_ANDROID || UNITY_EDITOR 
//-----------------------------------------------------------------------
// <copyright file="PlaneApi.cs" company="Google">
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
//remove ArPlane_isPoseInExtents
//remove ArPlane_acquireSubsumedBy
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
    public class PlaneApi
    {
        private const int k_MaxPolygonSize = 1024;
        private NativeSession m_NativeSession;
        private float[] m_TmpPoints;
        private GCHandle m_TmpPointsHandle;

        public PlaneApi(NativeSession nativeSession)
        {
            m_NativeSession = nativeSession;
            m_TmpPoints = new float[k_MaxPolygonSize * 2];
            m_TmpPointsHandle = GCHandle.Alloc(m_TmpPoints, GCHandleType.Pinned);
        }

        ~PlaneApi()
        {
            m_TmpPointsHandle.Free();
        }

        public Pose GetCenterPose(IntPtr planeHandle)
        {
            var poseHandle = m_NativeSession.PoseApi.Create();
            ExternApi.arPlaneNodeGetCenterPose(m_NativeSession.SessionHandle, planeHandle, poseHandle);
            Pose resultPose = m_NativeSession.PoseApi.ExtractPoseValue(poseHandle);
            m_NativeSession.PoseApi.Destroy(poseHandle);
            return resultPose;
        }

        public Vector3 GetExtent(IntPtr planeHandle)
        {
            float[] extent = new float[3];
            ExternApi.arPlaneNodeGetExtent(m_NativeSession.SessionHandle, planeHandle, extent);
            return new Vector3(extent[0], extent[1], extent[2]);
        }

        public void GetPolygon(IntPtr planeHandle, List<Vector3> points)
        {
            points.Clear();
            int pointCount = 0;
            ExternApi.arPlaneNodeGetPolygon3DSize(m_NativeSession.SessionHandle, planeHandle, ref pointCount);
            if (pointCount < 1)
            {
                return;
            }
            else if (pointCount > k_MaxPolygonSize)
            {
				return;
			}

            ExternApi.arPlaneNodeGetPolygon3D(m_NativeSession.SessionHandle, planeHandle, m_TmpPointsHandle.AddrOfPinnedObject());

            var planeCenter = GetCenterPose(planeHandle);
            var unityWorldTPlane = Matrix4x4.TRS(planeCenter.position, planeCenter.rotation, Vector3.one);
            for (int i = pointCount - 3; i >= 0; i -= 3)
            {
				var point = unityWorldTPlane.MultiplyPoint3x4(new Vector3(m_TmpPoints[i], m_TmpPoints[i + 1], -m_TmpPoints[i + 2]));
                points.Add(point);
            }
        }

        public TrackedPlane GetSubsumedBy(IntPtr planeHandle)
        {
            IntPtr subsumerHandle = IntPtr.Zero;
			return null;
        }

        public bool IsPoseInPolygon(IntPtr planeHandle, Pose pose)
        {
            // The int is used as a boolean value as the C API expects a int32_t value to represent a boolean.
            int isPoseInPolygon = 0;
            var poseHandle = m_NativeSession.PoseApi.Create(pose);
            ExternApi.arPlaneNodeIsPoseInPolygon(m_NativeSession.SessionHandle, planeHandle, poseHandle, ref isPoseInPolygon);
            m_NativeSession.PoseApi.Destroy(poseHandle);
            return isPoseInPolygon != 0;
        }

        public bool IsPoseInPolygon(IntPtr planeHandle, IntPtr poseHandle)
        {
            // The int is used as a boolean value as the C API expects a int32_t value to represent a boolean.
            int isPoseInPolygon = 0;
            ExternApi.arPlaneNodeIsPoseInPolygon(m_NativeSession.SessionHandle, planeHandle, poseHandle, ref isPoseInPolygon);
            return isPoseInPolygon != 0;
        }

        private struct ExternApi
        {
            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arPlaneNodeGetCenterPose(IntPtr sessionHandle, IntPtr planeHandle,
                IntPtr poseHandle);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arPlaneNodeGetExtent(IntPtr sessionHandle, IntPtr planeHandle,
                float[] extent);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arPlaneNodeGetPolygon3DSize(IntPtr sessionHandle, IntPtr planeHandle,
                ref int polygonSize);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arPlaneNodeGetPolygon3D(IntPtr sessionHandle, IntPtr planeHandle,
                IntPtr polygonXZ);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arPlaneNodeIsPoseInPolygon(IntPtr sessionHandle, IntPtr planeHandle,
                IntPtr poseHandle, ref int isPoseInPolygon);
        }
    }
}

 #endif