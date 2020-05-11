#if  UNITY_ANDROID || UNITY_EDITOR 


namespace SenseAR
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using SenseARInternal;
    using UnityEngine;

    /// <summary>
    /// hand gestures in the real world detected and tracked
    /// </summary>
    public class TrackedHandGesture : Trackable
    {
        public HandGestureType HandGestureType
        {
            get{
                if (m_TrackableNativeHandle == null)
                {
                    return HandGestureType.ARHAND_GESTURE_TYPE_UNKNOWN;
                }

                return m_NativeSession.HandGestureApi.GetHandGestureType(m_TrackableNativeHandle);
            }
        }

        public HandSide HandeSide
        {
            get{
                if (m_TrackableNativeHandle == null)
                {
                    return HandSide.ARHAND_SIDE_HAND_UNKNOWN;
                }

                return m_NativeSession.HandGestureApi.GetHandSide(m_TrackableNativeHandle);
            }
        }

        public HandTowards HandTowards
        {
            get{
                if (m_TrackableNativeHandle == null)
                {
                    return HandTowards.ARHAND_TOWARDS_UNKNOWN;
                }

                return m_NativeSession.HandGestureApi.GetHandTowards(m_TrackableNativeHandle);
            }
        }

        public float GestureTypeConfidence
        {
            get{
                if (m_TrackableNativeHandle == null)
                {
                    return -1f;
                }

                return m_NativeSession.HandGestureApi.GetGestureTypeConfidence(m_TrackableNativeHandle);
            }
        }

        public int LandMark2DCount
        {
            get{
                if (m_TrackableNativeHandle == null)
                {
                    return -1;
                }

                return m_NativeSession.HandGestureApi.GetLandMark2DCount(m_TrackableNativeHandle);
            }
        }

        public List<Vector2> LandMark2D
        {
            get{
                if (m_TrackableNativeHandle == null)
                {
                    return null;
                }

                return m_NativeSession.HandGestureApi.GetLandMark2DList(m_TrackableNativeHandle);
            }
        }


        //// @cond EXCLUDE_FROM_DOXYGEN
        /// <summary>
        /// Construct TrackedHandGesture from a native handle.
        /// </summary>
        /// <param name="nativeHandle">A handle to the native ARCore API Trackable.</param>
        /// <param name="nativeSession">The ARCore native api.</param>
        public TrackedHandGesture(IntPtr nativeHandle, NativeSession nativeSession)
            : base(nativeHandle, nativeSession)
        {
            m_TrackableNativeHandle = nativeHandle;
            m_NativeSession = nativeSession;
        }

    }
}
 #endif