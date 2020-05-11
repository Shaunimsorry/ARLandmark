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
    /// A image in the real world detected and tracked by ARCore.
    /// </summary>
    public class TrackedImage : Trackable
    {
        //// @cond EXCLUDE_FROM_DOXYGEN

        /// <summary>
        /// Construct TrackedImage from a native handle.
        /// </summary>
        /// <param name="nativeHandle">A handle to the native ARCore API Trackable.</param>
        /// <param name="nativeSession">The ARCore native api.</param>
        public TrackedImage(IntPtr nativeHandle, NativeSession nativeSession)
            : base(nativeHandle, nativeSession)
        {
            m_TrackableNativeHandle = nativeHandle;
            m_NativeSession = nativeSession;
        }

        //// @endcond

        /// <summary>
        /// Gets the center pose of the image.
        /// </summary>
        public Pose CenterPose
        {
            get
            {
                return m_NativeSession.ImageApi.GetCenterPose(m_TrackableNativeHandle);
            }
        }

        /// <summary>
        /// Gets the rotation of the plane.
        /// </summary>
        public int Index
        {
            get
            {
                return m_NativeSession.ImageApi.GetIndex(m_TrackableNativeHandle);
            }
        }

        /// <summary>
        /// Gets the extent of the image in the X dimension, centered on the image position.
        /// </summary>
        public float ExtentX
        {
            get
            {
                return m_NativeSession.ImageApi.GetExtentX(m_TrackableNativeHandle);
            }
        }

        /// <summary>
        /// Gets the extent of the image in the Z dimension, centered on the image position.
        /// </summary>
        public float ExtentZ
        {
            get
            {
                return m_NativeSession.ImageApi.GetExtentZ(m_TrackableNativeHandle);
            }
        }

        /// <summary>
        /// Gets the extent of the image in the X dimension, centered on the image position.
        /// </summary>
        public byte[] name
        {
            get
            {
                return m_NativeSession.ImageApi.GetName(m_TrackableNativeHandle);
            }
        }
    }
}
 #endif