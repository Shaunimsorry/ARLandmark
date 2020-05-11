#if  UNITY_ANDROID || UNITY_EDITOR 
//-----------------------------------------------------------------------
// <copyright file="PointCloudApi.cs" company="Google">
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
    public class PointCloudApi
    {
        private NativeSession m_NativeSession;

        private float[] m_CachedVector = new float[4];

        public PointCloudApi(NativeSession nativeSession)
        {
            m_NativeSession = nativeSession;
        }

        public int GetNumberOfPoints(IntPtr pointCloudHandle)
        {
            int pointCount = 0;
            ExternApi.arPointCloudGetNumberOfPoints(m_NativeSession.SessionHandle, pointCloudHandle, ref pointCount);

            return pointCount;
        }

        public Vector4 GetPoint(IntPtr pointCloudHandle, int index)
        {
            IntPtr pointCloudNativeHandle = IntPtr.Zero;
            ExternApi.arPointCloudGetData(m_NativeSession.SessionHandle, pointCloudHandle, ref pointCloudNativeHandle);
            IntPtr pointHandle = new IntPtr(pointCloudNativeHandle.ToInt64() +
                                            (Marshal.SizeOf(typeof(Vector4)) * index));
            Marshal.Copy(pointHandle, m_CachedVector, 0, 4);

            // Negate z axis because points are returned in OpenGl space.
            return new Vector4(m_CachedVector[0], m_CachedVector[1], -m_CachedVector[2], m_CachedVector[3]);
        }

        public void CopyPoints(IntPtr pointCloudHandle, List<Vector4> points)
        {
            points.Clear();

            IntPtr pointCloudNativeHandle = IntPtr.Zero;
            int pointCloudSize = GetNumberOfPoints(pointCloudHandle);

            ExternApi.arPointCloudGetData(m_NativeSession.SessionHandle, pointCloudHandle, ref pointCloudNativeHandle);

            MarshalingHelper.AddUnmanagedStructArrayToList<Vector4>(pointCloudNativeHandle,
                    pointCloudSize, points);

            for (int i = 0; i < pointCloudSize; ++i)
            {
                // Negate z axis because points are returned in OpenGl space.
                points[i] = new Vector4(points[i].x, points[i].y,
                        -points[i].z, points[i].w);
            }
        }

        public void Release(IntPtr pointCloudHandle)
        {
            ExternApi.arPointCloudRelease(pointCloudHandle);
        }

        private struct ExternApi
        {
            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arPointCloudGetNumberOfPoints(IntPtr session, IntPtr pointCloudHandle,
                ref int pointCount);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arPointCloudGetData(IntPtr session, IntPtr pointCloudHandle,
                ref IntPtr pointCloudData);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arPointCloudRelease(IntPtr pointCloudHandle);
        }
    }
}

 #endif