#if  UNITY_ANDROID || UNITY_EDITOR 


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
    public class HandGestureApi
    {
        private NativeSession m_NativeSession;

        public HandGestureApi(NativeSession nativeSession)
        {
            m_NativeSession = nativeSession;
        }

        ~HandGestureApi()
        {
        }

        public HandGestureType GetHandGestureType(IntPtr handGestureHandle)
        {
            ApiHandGestureType apiHandGestureType = ApiHandGestureType.ARHAND_GESTURE_TYPE_UNKNOWN;
            ExternApi.arHandGestureGetHandGestureType(m_NativeSession.SessionHandle, handGestureHandle, ref apiHandGestureType);

            return apiHandGestureType.ToHandGestureType();
        }

        public HandSide GetHandSide(IntPtr handGestureHandle)
        {
            ApiHandSide apiHandSide = ApiHandSide.ARHAND_SIDE_HAND_UNKNOWN;
            ExternApi.arHandGestureGetHandSide(m_NativeSession.SessionHandle, handGestureHandle, ref apiHandSide);

            return apiHandSide.ToHandSide();
        }

        public HandTowards GetHandTowards(IntPtr handGestureHandle)
        {
            ApiHandTowards apiHandTowards = ApiHandTowards.ARHAND_TOWARDS_UNKNOWN;
            ExternApi.arHandGestureGetHandTowards(m_NativeSession.SessionHandle, handGestureHandle, ref apiHandTowards);

            return apiHandTowards.ToHandTowards();
        }

        public float GetGestureTypeConfidence(IntPtr handGestureHandle)
        {
            float gestureTypeConfidence = -1f;
            ExternApi.arHandGestureGetGestureTypeConfidence(m_NativeSession.SessionHandle, handGestureHandle, ref gestureTypeConfidence);

            return gestureTypeConfidence;
        }

        public int GetLandMark2DCount(IntPtr handGestureHandle)
        {
            int landMark2DCount = -1;
            ExternApi.arHandGestureGetLandMark2DCount(m_NativeSession.SessionHandle, handGestureHandle, ref landMark2DCount);

            return landMark2DCount;
        }

        public List<Vector2> GetLandMark2DList(IntPtr handGestureHandle)
        {
            int landMark2DCount = -1;
            ExternApi.arHandGestureGetLandMark2DCount(m_NativeSession.SessionHandle, handGestureHandle, ref landMark2DCount);
            
            float[] landMark2DArray = new float[landMark2DCount*2];
            ExternApi.arHandGestureGetLandMark2DArray(m_NativeSession.SessionHandle, handGestureHandle, landMark2DArray);

            List<Vector2> landMark2DList = new List<Vector2>();

            for(int i=0; i<landMark2DCount; i++)
            {
                landMark2DList.Add(new Vector2(landMark2DArray[i*2], landMark2DArray[i*2+1]));
            }

            return landMark2DList;
        }

        private struct ExternApi
        {
            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arHandGestureGetHandGestureType(IntPtr sessionHandle, IntPtr handGestureHandle,
            ref ApiHandGestureType outHandGestureType);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arHandGestureGetHandSide(IntPtr sessionHandle, IntPtr handGestureHandle,
            ref ApiHandSide outHandSide);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arHandGestureGetHandTowards(IntPtr sessionHandle, IntPtr handGestureHandle,
            ref ApiHandTowards outHandTowards);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arHandGestureGetGestureTypeConfidence(IntPtr sessionHandle, IntPtr handGestureHandle,
            ref float outGestureTypeConfidence);


            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arHandGestureGetLandMark2DCount(IntPtr sessionHandle, IntPtr handGestureHandle,
            ref int outLandMark2DCount);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arHandGestureGetLandMark2DArray(IntPtr sessionHandle, IntPtr handGestureHandle,
            float[] outLandMark2DArray);

        }
    }
}

 #endif