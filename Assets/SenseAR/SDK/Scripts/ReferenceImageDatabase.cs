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

    public class ReferenceImageDatabase
    {
        public IntPtr m_ReferenceImageDatabaseNativeHandle = IntPtr.Zero;

        private NativeSession m_NativeSession;

        public ReferenceImageDatabase(NativeSession nativeSession)
        {
            m_NativeSession = nativeSession;
            m_ReferenceImageDatabaseNativeHandle = m_NativeSession.ReferenceImageDatabaseApi.CreateReferenceImageDatebase();
        }

        public ReferenceImageDatabase(NativeSession nativeSession, IntPtr imageDatabasePtr)
        {
            m_NativeSession = nativeSession;
            m_ReferenceImageDatabaseNativeHandle = imageDatabasePtr;
        }

        public void Deserialize(byte[] buffer, Int64 size)
        {
            m_NativeSession.ReferenceImageDatabaseApi.Deserialize(m_ReferenceImageDatabaseNativeHandle, buffer, size);
        }

        public void Serialize(ref byte[] buffer)
        {
            m_NativeSession.ReferenceImageDatabaseApi.Serialize(m_ReferenceImageDatabaseNativeHandle, ref buffer);
        }

        public void LoadConfig(byte[] buffer, Int64 size)
        {
            m_NativeSession.ReferenceImageDatabaseApi.LoadConfig(m_ReferenceImageDatabaseNativeHandle, buffer, size);
        }

        public void AddPatt(byte[] buffer, Int64 size)
        {
            m_NativeSession.ReferenceImageDatabaseApi.AddPatt(m_ReferenceImageDatabaseNativeHandle, buffer, size);
        }

        public void AddImage(string image_name, Texture2D image)
        {
            m_NativeSession.ReferenceImageDatabaseApi.AddImage(m_ReferenceImageDatabaseNativeHandle, image_name, image);
        }

        public void AddImageWithPhysicalSize(string image_name, Texture2D image, float width_in_meters)
        {
            m_NativeSession.ReferenceImageDatabaseApi.AddImageWithPhysicalSize(m_ReferenceImageDatabaseNativeHandle, image_name, image, width_in_meters);
        }

        public void Destroy(){
            m_NativeSession.ReferenceImageDatabaseApi.Destroy(m_ReferenceImageDatabaseNativeHandle);
        }

        public int Num
        {
            get
            {
                return m_NativeSession.ReferenceImageDatabaseApi.GetImageNum(m_ReferenceImageDatabaseNativeHandle);
            }
        }
    }
}
#endif