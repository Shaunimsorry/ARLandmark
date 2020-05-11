#if  UNITY_ANDROID || UNITY_EDITOR 
//-----------------------------------------------------------------------
// <copyright file="HitTestApi.cs" company="Google">
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
//remove ArHitResult_getDistance
//rename GoogleARCore to SenseAR to avoid conflict
//rename GoogleARCoreInternal to SenseARInternal to avoid conflict
//
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
    public class HitTestApi
    {
        private NativeSession m_NativeSession;

        public HitTestApi(NativeSession nativeSession)
        {
            m_NativeSession = nativeSession;
        }

        public void SetHitTestMode(IntPtr frameHandle, ApiHitTestMode mode)
        {
            ExternApi.arFramePointQueryMode(m_NativeSession.SessionHandle, frameHandle, mode);
        }

        public bool Raycast(IntPtr frameHandle, float x, float y, TrackableHitFlags filter,
            List<TrackableHit> outHitList, bool isOnlyQueryingNearestHit)
        {
            outHitList.Clear();

            IntPtr hitResultListHandle = IntPtr.Zero;
            ExternApi.arQueryResultListCreate(m_NativeSession.SessionHandle, ref hitResultListHandle);
            ExternApi.arFramePointQuery(m_NativeSession.SessionHandle, frameHandle, x, y, hitResultListHandle);

            int hitListSize = 0;
            ExternApi.arQueryResultListGetSize(m_NativeSession.SessionHandle, hitResultListHandle, ref hitListSize);

            for (int i = 0; i < hitListSize; i++)
            {
                TrackableHit trackableHit = new TrackableHit();
                if (HitResultListGetItemAt(hitResultListHandle, i, ref trackableHit))
				{
					outHitList.Add(trackableHit);
                }
            }

            ExternApi.arQueryResultListDestroy(hitResultListHandle);
            return outHitList.Count != 0;
        }

        public bool Raycast(IntPtr frameHandle, float[] ray_origin, float[] ray_direction, TrackableHitFlags filter,
            List<TrackableHit> outHitList, bool isOnlyQueryingNearestHit)
        {
            outHitList.Clear();

            IntPtr hitResultListHandle = IntPtr.Zero;
            ExternApi.arQueryResultListCreate(m_NativeSession.SessionHandle, ref hitResultListHandle);
            ExternApi.arFrameRayQuery(m_NativeSession.SessionHandle, frameHandle, ray_origin, ray_direction, hitResultListHandle);

            int hitListSize = 0;
            ExternApi.arQueryResultListGetSize(m_NativeSession.SessionHandle, hitResultListHandle, ref hitListSize);

            for (int i = 0; i < hitListSize; i++)
            {
                TrackableHit trackableHit = new TrackableHit();
                if (HitResultListGetItemAt(hitResultListHandle, i, ref trackableHit))
                {
                    outHitList.Add(trackableHit);
                }
            }

            ExternApi.arQueryResultListDestroy(hitResultListHandle);
            return outHitList.Count != 0;
        }

        private bool HitResultListGetItemAt(IntPtr hitResultListHandle, int index, ref TrackableHit outTrackableHit)
        {
            // Query the hit result.
            IntPtr hitResultHandle = IntPtr.Zero;
            ExternApi.arQueryResultCreate(m_NativeSession.SessionHandle, ref hitResultHandle);
            ExternApi.arQueryResultListGetItem(m_NativeSession.SessionHandle, hitResultListHandle, index, hitResultHandle);
            if (hitResultHandle == IntPtr.Zero)
            {
                ExternApi.arQueryResultDestroy(hitResultHandle);
                return false;
            }

            // Query the pose from hit result.
            IntPtr poseHandle = m_NativeSession.PoseApi.Create();
            ExternApi.arQueryResultGetHitPose(m_NativeSession.SessionHandle, hitResultHandle, poseHandle);
            Pose hitPose = m_NativeSession.PoseApi.ExtractPoseValue(poseHandle);

            // Query the distance from hit result.
            float hitDistance = 0.0f;

            // Query the trackable from hit result.
            IntPtr trackableHandle = IntPtr.Zero;
            ExternApi.arQueryResultAcquireNode(m_NativeSession.SessionHandle, hitResultHandle, ref trackableHandle);
            Trackable trackable = m_NativeSession.TrackableFactory(trackableHandle);
            //m_NativeSession.TrackableApi.Release(trackableHandle);


            // TODO fixed me, need to release hitResultHandle
            //ExternApi.ArHitResult_destroy(hitResultHandle);

            // Calculate trackable hit flags.
            TrackableHitFlags flag = TrackableHitFlags.None;
            if (trackable == null)
            {
                Debug.Log("Could not create trackable from hit result.");
                m_NativeSession.PoseApi.Destroy(poseHandle);
                return false;
            }
            else if (trackable is TrackedPlane)
			{
				flag |= TrackableHitFlags.PlaneWithinInfinity;

            }
            else if (trackable is TrackedPoint)
            {
                flag |= TrackableHitFlags.PointCloud;
            }
            else
            {
                m_NativeSession.PoseApi.Destroy(poseHandle);
                return false;
            }

            outTrackableHit = new TrackableHit(hitPose, hitDistance, flag, trackable);
            m_NativeSession.PoseApi.Destroy(poseHandle);
            return true;
        }

        private struct ExternApi
        {
            // set Hit test mode function.
            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arFramePointQueryMode(IntPtr session, IntPtr frame, ApiHitTestMode mode);

            // Hit test function.
            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arFramePointQuery(IntPtr session,
                IntPtr frame, float pixel_x, float pixel_y, IntPtr hit_result_list);

            // Hit test function.
            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arFrameRayQuery(IntPtr session,
                IntPtr frame, float[] ray_origin_3, float[] ray_direction_3, IntPtr hit_result_list);

            // Hit list functions.
            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arQueryResultListCreate(IntPtr session, ref IntPtr out_hit_result_list);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arQueryResultListDestroy(IntPtr hit_result_list);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arQueryResultListGetSize(IntPtr session, IntPtr hit_result_list, ref int out_size);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arQueryResultListGetItem(IntPtr session, IntPtr hit_result_list,
                int index, IntPtr out_hit_result);

            // Hit Result funcitons.
            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arQueryResultCreate(IntPtr session, ref IntPtr out_hit_result);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arQueryResultDestroy(IntPtr hit_result);


            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arQueryResultGetHitPose(IntPtr session, IntPtr hit_result, IntPtr out_pose);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arQueryResultAcquireNode(IntPtr session, IntPtr hit_result,
                ref IntPtr out_trackable);
        }
    }
}

 #endif