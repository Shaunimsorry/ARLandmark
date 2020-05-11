#if  UNITY_ANDROID || UNITY_EDITOR 
//-----------------------------------------------------------------------
// <copyright file="Frame.cs" company="Google">
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
// modification lsit
// 1.remove class CameraMetadata
// 2.remove class CameraImage
//rename GoogleARCore to SenseAR to avoid conflict
//rename GoogleARCoreInternal to SenseARInternal to avoid conflict
//
// </copyright>
//-----------------------------------------------------------------------

namespace SenseAR
{
    using System;
    using System.Collections.Generic;
    using SenseARInternal;
    using UnityEngine;

    /// <summary>
    /// Provides a snapshot of the state of ARCore at a specific timestamp associated with the current frame.  Frame
    /// holds information about ARCore's state including tracking status, the pose of the camera relative to the world,
    /// estimated lighting parameters, and information on updates to objects (like Planes or Point Clouds) that ARCore
    /// is tracking.
    /// </summary>
    public class Frame
    {
        //// @cond EXCLUDE_FROM_DOXYGEN

        /// <summary>
        /// Gets the manager for the static frame.
        /// </summary>
        private static FrameManager s_FrameManager;

        //// @endcond

        /// <summary>
        /// Gets the tracking state of the ARCore device for the frame.  If the state is not <c>Tracking</c>,
        /// the values in the frame may be very inaccurate and generally should not be used.
        /// Tracking loss is often caused when the camera does not have enough visual features to track (e.g. a white
        /// wall) or the device is being moved very rapidly.
        /// </summary>
        public static TrackingState TrackingState
        {
            get
            {
                if (s_FrameManager == null)
                {
                    return TrackingState.Stopped;
                }

                return s_FrameManager.GetCameraTrackingState();
            }
        }

        /// <summary>
        /// Gets the pose of the ARCore device for the frame in Unity world coordinates.
        /// </summary>
        public static Pose Pose
        {
            get
            {
                if (s_FrameManager == null)
                {
                    return Pose.identity;
                }

                return s_FrameManager.GetPose();
            }
        }

        /// <summary>
        /// Gets the current light estimate for this frame.
        /// </summary>
        public static LightEstimate LightEstimate
        {
            get
            {
                if (s_FrameManager == null)
                {
                    return new LightEstimate(LightEstimateState.NotValid, 0.0f, Color.white);
                }

                return s_FrameManager.GetLightEstimate();
            }
        }


        //// @cond EXCLUDE_FROM_DOXYGEN

        /// <summary>
        /// Initializes the static Frame.
        /// </summary>
        /// <param name="frameManager">The manager for the static frame.</param>
        public static void Initialize(FrameManager frameManager)
        {
            Frame.s_FrameManager = frameManager;
        }

        /// <summary>
        /// Cleans up the static frame.
        /// </summary>
        public static void Destroy()
        {
            s_FrameManager = null;
        }

        //// @endcond

        /// <summary>
        /// Container for state related to the ARCore point cloud for the Frame.
        /// </summary>
        public static class PointCloud
        {
            /// <summary>
            /// Gets a value indicating whether new point cloud data became available in the current frame.
            /// </summary>
            /// <returns><c>true</c> if new point cloud data became available in the current frame, otherwise
            /// <c>false</c>.</returns>
            public static bool IsUpdatedThisFrame
            {
                get
                {
                    if (Frame.s_FrameManager == null)
                    {
                        return false;
                    }

                    return Frame.s_FrameManager.PointCloudManager.GetIsUpdatedThisFrame();
                }
            }

            /// <summary>
            /// Gets the count of point cloud points in the frame.
            /// </summary>
            public static int PointCount
            {
                get
                {
                    if (Frame.s_FrameManager == null)
                    {
                        return 0;
                    }

                    return Frame.s_FrameManager.PointCloudManager.GetPointCount();
                }
            }

            /// <summary>
            /// Gets a point from the point cloud collection at an index.
            /// </summary>
            /// <param name="index">The index of the point cloud point to get.</param>
            /// <returns>The point from the point cloud at <c>index</c>.</returns>
            public static Vector3 GetPoint(int index)
            {
                if (Frame.s_FrameManager == null)
                {
                    return Vector3.zero;
                }

                return Frame.s_FrameManager.PointCloudManager.GetPoint(index);
            }

            /// <summary>
            /// Copies the point cloud for a frame into a supplied list reference.
            /// </summary>
            /// <param name="points">A list that will be filled with point cloud points by this method call.</param>
            public static void CopyPoints(List<Vector4> points)
            {
                if (Frame.s_FrameManager == null)
                {
                    points.Clear();
                    return;
                }

                Frame.s_FrameManager.PointCloudManager.CopyPoints(points);
            }
        }
    }
}

 #endif