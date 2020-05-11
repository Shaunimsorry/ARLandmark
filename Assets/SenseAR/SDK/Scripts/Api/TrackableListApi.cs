#if  UNITY_ANDROID || UNITY_EDITOR 
//-----------------------------------------------------------------------
// <copyright file="TrackableListApi.cs" company="Google">
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
    public class TrackableListApi
    {
        private NativeSession m_NativeSession;

        public TrackableListApi(NativeSession nativeSession)
        {
            m_NativeSession = nativeSession;
        }

        public IntPtr Create()
        {
            IntPtr handle = IntPtr.Zero;
            ExternApi.arNodeListCreate(m_NativeSession.SessionHandle, ref handle);
            return handle;
        }

        public void Destroy(IntPtr listHandle)
        {
            ExternApi.arNodeListDestroy(listHandle);
        }

        public int GetCount(IntPtr listHandle)
        {
            int count = 0;
            ExternApi.arNodeListGetSize(m_NativeSession.SessionHandle, listHandle, ref count);
            return count;
        }

        public IntPtr AcquireItem(IntPtr listHandle, int index)
        {
            IntPtr trackableHandle = IntPtr.Zero;
            ExternApi.arNodeListAcquireItem(m_NativeSession.SessionHandle, listHandle, index,
                ref trackableHandle);
            return trackableHandle;
        }

        private struct ExternApi
        {
            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arNodeListCreate(IntPtr sessionHandle, ref IntPtr trackableListHandle);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arNodeListDestroy(IntPtr trackableListHandle);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arNodeListGetSize(IntPtr sessionHandle, IntPtr trackableListHandle,
                ref int outSize);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arNodeListAcquireItem(IntPtr sessionHandle, IntPtr trackableListHandle,
                int index, ref IntPtr outTrackable);
        }
    }
}

 #endif