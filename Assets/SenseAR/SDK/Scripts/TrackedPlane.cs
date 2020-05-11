#if  UNITY_ANDROID || UNITY_EDITOR 
//-----------------------------------------------------------------------
// <copyright file="TrackedPlane.cs" company="Google">
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

namespace SenseAR
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using SenseARInternal;
    using UnityEngine;

    /// <summary>
    /// A planar surface in the real world detected and tracked by ARCore.
    /// </summary>
    public class TrackedPlane : Trackable
    {
        //// @cond EXCLUDE_FROM_DOXYGEN

        /// <summary>
        /// Construct TrackedPlane from a native handle.
        /// </summary>
        /// <param name="nativeHandle">A handle to the native ARCore API Trackable.</param>
        /// <param name="nativeSession">The ARCore native api.</param>
        public TrackedPlane(IntPtr nativeHandle, NativeSession nativeSession)
            : base(nativeHandle, nativeSession)
        {
            m_TrackableNativeHandle = nativeHandle;
            m_NativeSession = nativeSession;
        }

        //// @endcond

        /// <summary>
        /// Gets a reference to the plane subsuming this plane, if any.  If not null, only the subsuming plane should be
        /// considered valid for rendering.
        /// </summary>
        public TrackedPlane SubsumedBy
        {
            get
            {
               return m_NativeSession.PlaneApi.GetSubsumedBy(m_TrackableNativeHandle);
            }
        }

        /// <summary>
        /// Gets the center position of the plane.
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return m_NativeSession.PlaneApi.GetCenterPose(m_TrackableNativeHandle).position;
            }
        }

        /// <summary>
        /// Gets the rotation of the plane.
        /// </summary>
        public Quaternion Rotation
        {
            get
            {
                return m_NativeSession.PlaneApi.GetCenterPose(m_TrackableNativeHandle).rotation;
            }
        }

        /// <summary>
        /// Gets the extent of the plane in the X Y Z dimension, centered on the plane position.
        /// </summary>
        public Vector3 Extent
        {
            get
            {
                return m_NativeSession.PlaneApi.GetExtent(m_TrackableNativeHandle);
            }
        }

        /// <summary>
        /// Gets a list of points (in clockwise order) in Unity world space representing a boundary polygon for
        /// the plane.
        /// </summary>
        /// <param name="boundaryPolygonPoints">A list of <b>Vector3</b> to be filled by the method call.</param>
        public void GetBoundaryPolygon(List<Vector3> boundaryPolygonPoints)
        {
            m_NativeSession.PlaneApi.GetPolygon(m_TrackableNativeHandle, boundaryPolygonPoints);
        }
    }
}
 #endif