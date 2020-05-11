#if  UNITY_ANDROID || UNITY_EDITOR 
//-----------------------------------------------------------------------
// <copyright file="Anchor.cs" company="Google">
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
    /// Attaches a GameObject to an ARCore {@link Trackable}.  The transform of the GameObject will be updated to
    /// maintain the semantics of the attachment relationship, which varies between sub-types of Trackable.
    /// </summary>
    public class Anchor : MonoBehaviour
    {
		internal IntPtr m_AnchorNativeHandle { get; private set; }

        private NativeSession m_NativeSession;

        private TrackingState m_LastFrameTrackingState = TrackingState.Stopped;

        /// <summary>
        /// Gets the tracking state of the anchor.
        /// </summary>
        public TrackingState TrackingState
        {
            get
            {
                return m_NativeSession.AnchorApi.GetTrackingState(m_AnchorNativeHandle);
            }
        }

        //// @cond EXCLUDE_FROM_DOXYGEN

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Internal")]
        public static Anchor AnchorFactory(IntPtr anchorNativeHandle, NativeSession nativeSession)
        {
            if (anchorNativeHandle == IntPtr.Zero)
            {
                return null;
            }

            Anchor anchor = (new GameObject()).AddComponent<Anchor>();
            anchor.gameObject.name = "Anchor";
            anchor.m_AnchorNativeHandle = anchorNativeHandle;
            anchor.m_NativeSession = nativeSession;
            anchor.Update();

            return anchor;
        }

        //// @endcond

        private void Update()
        {
            if (m_AnchorNativeHandle == IntPtr.Zero)
            {
                Debug.LogError("Anchor components instantiated outside of ARCore are not supported. " +
                    "Please use a 'Create' method within ARCore to instantiate anchors.");
                return;
            }

            var pose = m_NativeSession.AnchorApi.GetPose(m_AnchorNativeHandle);
            transform.position = pose.position;
            transform.rotation = pose.rotation;

            TrackingState currentFrameTrackingState = TrackingState;
            if (m_LastFrameTrackingState != currentFrameTrackingState)
            {
                bool isAnchorTracking = currentFrameTrackingState == TrackingState.Tracking;
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(isAnchorTracking);
                }

                m_LastFrameTrackingState = currentFrameTrackingState;
            }
        }

        private void OnDestroy()
        {
            if (m_AnchorNativeHandle == IntPtr.Zero)
            {
                return;
            }

            m_NativeSession.AnchorApi.Release(m_AnchorNativeHandle);
        }
    }
}

 #endif