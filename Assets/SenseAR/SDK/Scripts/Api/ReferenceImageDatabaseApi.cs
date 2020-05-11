#if  UNITY_ANDROID || UNITY_EDITOR 
//-----------------------------------------------------------------------
// <copyright file="FrameApi.cs" company="Google">
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
//modify AcquireCamera
//remove AcquireCameraImageBytes
//modify AcquirePointCloud
//modify GetLightEstimate
//remove AcquireImageMetadata
//modify TransformDisplayUvCoords
//modify GetUpdatedTrackables
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
    public class ReferenceImageDatabaseApi
    {
        private NativeSession m_NativeSession;

        public ReferenceImageDatabaseApi(NativeSession nativeSession)
        {
            m_NativeSession = nativeSession;
        }

        public IntPtr CreateReferenceImageDatebase()
        {
            IntPtr imageDatabaseHandle = IntPtr.Zero;
            ExternApi.arReferenceImageDatabaseCreate(m_NativeSession.SessionHandle, ref imageDatabaseHandle);
            return imageDatabaseHandle;
        }

        public void Deserialize(IntPtr imageDatabaseHandle, byte[] buffer, Int64 size)
        {
            ExternApi.arReferenceImageDatabaseDeserialize(m_NativeSession.SessionHandle, imageDatabaseHandle, buffer, size);
        }

        public void Serialize(IntPtr imageDatabaseHandle, ref byte[] buffer)
        {
            Int64 size = 0;
            ExternApi.arReferenceImageDatabaseSerialize(m_NativeSession.SessionHandle, imageDatabaseHandle, ref buffer, ref size);
        }

        public void LoadConfig(IntPtr imageDatabaseHandle, byte[] buffer, Int64 size)
        {
            ExternApi.arReferenceImageDatabaseLoadConfigure(m_NativeSession.SessionHandle, imageDatabaseHandle, buffer);
        }

        public void AddPatt(IntPtr imageDatabaseHandle, byte[] buffer, Int64 size)
        {
            ExternApi.arReferenceImageDatabaseAddPatt(m_NativeSession.SessionHandle, imageDatabaseHandle, buffer, size);
        }

        public void AddImage(IntPtr imageDatabaseHandle, string image_name, Texture2D image)
        {
            Debug.Log("image name:" + image_name);
            Debug.Log("image width:" + image.width);
            Debug.Log("image height:" + image.height);
            byte[] grayscaleBytes = _ConvertTextureToGrayscaleBytes(image);
            int out_index = 0;
            ExternApi.arReferenceImageDatabaseAddImage(m_NativeSession.SessionHandle, imageDatabaseHandle, image_name.ToCharArray(), grayscaleBytes, image.width, image.height, image.width, ref out_index);
        }

        public void AddImageWithPhysicalSize(IntPtr imageDatabaseHandle, string image_name, Texture2D image, float width_in_meters)
        {
            Debug.Log("image name:" + image_name);
            Debug.Log("image width:" + image.width);
            Debug.Log("image height:" + image.height);
            byte[] grayscaleBytes = _ConvertTextureToGrayscaleBytes(image);
            int out_index = 0;
            ExternApi.arReferenceImageDatabaseAddImage(m_NativeSession.SessionHandle, imageDatabaseHandle, image_name.ToCharArray(), grayscaleBytes, image.width, image.height, image.width, ref out_index);
        }

        private byte[] _ConvertTextureToGrayscaleBytes(Texture2D image)
        {
            byte[] grayscaleBytes = null;

            if (image.format == TextureFormat.RGB24 || image.format == TextureFormat.RGBA32)
            {
                Color[] pixels = image.GetPixels();
                grayscaleBytes = new byte[pixels.Length];
                for (int i = 0; i < image.height; i++)
                {
                    for (int j = 0; j < image.width; j++)
                    {
                        grayscaleBytes[(i * image.width) + j] =
                            (byte)((
                            (0.213 * pixels[((image.height - 1 - i) * image.width) + j].r) +
                            (0.715 * pixels[((image.height - 1 - i) * image.width) + j].g) +
                            (0.072 * pixels[((image.height - 1 - i) * image.width) + j].b)) * 255);
                    }
                }
            }
            else
            {
                Debug.LogError("Unsupported texture format " + image.format);
            }

            return grayscaleBytes;
        }

        public int GetImageNum(IntPtr imageDatabaseHandle)
        {
            int num = 0;
            ExternApi.arReferenceImageDatabaseGetNumImages(m_NativeSession.SessionHandle, imageDatabaseHandle, ref num);
            return num;
        }

        public void Destroy(IntPtr imageDatabaseHandle)
        {
            ExternApi.arReferenceImageDatabaseDestroy(imageDatabaseHandle);
        }

        private struct ExternApi
        {
            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arReferenceImageDatabaseCreate(IntPtr sessionHandle, ref IntPtr imageDatabaseHandle);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern ApiArStatus arReferenceImageDatabaseDeserialize(IntPtr sessionHandle, IntPtr imageDatabaseHandle, byte[] buffer, Int64 size);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arReferenceImageDatabaseSerialize(IntPtr sessionHandle, IntPtr imageDatabaseHandle,
                ref byte[] out_image_database_raw_bytes, ref Int64 size);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arReferenceImageDatabaseLoadConfigure(IntPtr sessionHandle, IntPtr imageDatabaseHandle, byte[] buffer);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern ApiArStatus arReferenceImageDatabaseAddPatt(IntPtr sessionHandle, IntPtr imageDatabaseHandle, byte[] buffer, Int64 size);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern ApiArStatus arReferenceImageDatabaseAddImage(IntPtr sessionHandle, IntPtr imageDatabaseHandle, char[] image_name, byte[] image_grayscale_pixels, 
                int width, int height, int stride, ref int out_index);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern ApiArStatus arReferenceImageDatabaseAddImageWithPhysicalSize(IntPtr sessionHandle, IntPtr imageDatabaseHandle, char[] image_name, byte[] image_grayscale_pixels,
                int width, int height, int stride, float width_in_meters, ref int out_index);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arReferenceImageDatabaseGetNumImages(IntPtr sessionHandle, IntPtr imageDatabaseHandle, ref int out_num_images);

            [DllImport(ApiConstants.SenseARNativeApi)]
            public static extern void arReferenceImageDatabaseDestroy(IntPtr imageDatabaseHandle);

        }
    }
}

#endif
