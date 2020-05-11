#if  UNITY_ANDROID || UNITY_EDITOR 
//-----------------------------------------------------------------------
// <copyright file="StandardARUpdateTexture.cs" >
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
//rename GoogleARCore to StandardAR to avoid conflict
//rename GoogleARCoreInternal to StandardARInternal to avoid conflict
//
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Collections;
using UnityEngine;
using SenseARInternal;
using SenseAR;
using UnityEngine.Rendering;

public class SenseARUpdateTextureOES : MonoBehaviour
{
    private NativeSession m_NativeSession;

    private int FrameWidth = 1920;
    private int FrameHeight = 1080;
    private int ScreenWidth = 1080;
    private int ScreenHeight = 2160;
    private float AspectRatio = 0;

    private SenseARCameraPose standardARCameraPose;

    private CommandBuffer m_VideoCommandBuffer;

    private Matrix4x4 _displayTransform;

    public Material BackgroundMaterial;
    private Texture2D m_BackgroundTexture;

    private byte[] _videoTexture_Y_bytes;
    private byte[] _videoTexture_uv_RGB_bytes;


    void Awake()
    {


    }

    // Use this for initialization
    void Start()
    {
        m_NativeSession = Session.GetNativeSession();
        standardARCameraPose = GetComponent<SenseARCameraPose>();

        m_NativeSession.getTextureSize(ref FrameWidth, ref FrameHeight);
        AspectRatio = (float)FrameWidth / FrameHeight;
        Debug.Log("frame width: " + FrameWidth + " frame height: " + FrameHeight + " aspect: " + AspectRatio);

        _videoTexture_Y_bytes = new byte[FrameWidth * FrameHeight];
        _videoTexture_uv_RGB_bytes = new byte[FrameWidth * FrameHeight / 4 * 3];

    }

    private void OnDestroy()
    {
        _videoTexture_Y_bytes = null;
        _videoTexture_uv_RGB_bytes = null;
    }


    private int FrameIndex = 1;
    IEnumerator UploadPNG(byte[] pic)
    {
        yield return new WaitForEndOfFrame();

        string filename = "image" + FrameIndex++ + ".png";
        string[] path_save = new string[] { "/mnt/sdcard/DCIM/Camera" };
        if (Application.platform == RuntimePlatform.Android)
        {
            if (!Directory.Exists(path_save[0]))
            {
                Directory.CreateDirectory(path_save[0]);
            }
            path_save[0] = path_save[0] + "/" + filename;
            File.WriteAllBytes(path_save[0], pic);
        }

        ScanFile(path_save);
    }

    void ScanFile(string[] path)
    {
        using (AndroidJavaClass PlayerActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject playerActivity = PlayerActivity.GetStatic<AndroidJavaObject>("currentActivity");
            using (AndroidJavaObject Conn = new AndroidJavaObject("android.media.MediaScannerConnection", playerActivity, null))
            {
                Conn.CallStatic("scanFile", playerActivity, path, null, null);
            }
        }
    }

    public void Shoot()
    {
        //StartCoroutine (UploadPNG (m_BackgroundTexture.EncodeToPNG()));
    }

    bool IsVideoUpdated()
    {
        if (m_NativeSession == null)
            return false;

        return m_NativeSession.isVideoUpdated();
    }

    void UpdateSession()
    {
        if (m_NativeSession == null)
            return;

        IntPtr FrameHandle = m_NativeSession.GetFramePtr();
        if (FrameHandle != IntPtr.Zero)
            Session.SessionManager.Update(FrameHandle);

        m_NativeSession.Update();
    }

    void UpdateAndroidTexture()
    {
        if (BackgroundMaterial == null)
        {
            // A background rending material has not been assigned.
            return;
        }

        if (m_BackgroundTexture == null)
        {
            int cameraTextureId = 0;
            m_NativeSession.GetCameraTextureID(ref cameraTextureId);
            m_BackgroundTexture = Texture2D.CreateExternalTexture(FrameWidth, FrameHeight, TextureFormat.RGBA32, false,
               false, new IntPtr(cameraTextureId));
            m_BackgroundTexture.wrapMode = TextureWrapMode.Clamp;
            m_BackgroundTexture.filterMode = FilterMode.Bilinear;
            m_BackgroundTexture.hideFlags = HideFlags.HideAndDontSave;

            m_VideoCommandBuffer = new CommandBuffer();
            m_VideoCommandBuffer.Blit(null, BuiltinRenderTextureType.CurrentActive, BackgroundMaterial);
            GetComponent<Camera>().AddCommandBuffer(CameraEvent.BeforeForwardOpaque, m_VideoCommandBuffer);
        }
        else
        {
            if (Screen.orientation == ScreenOrientation.LandscapeLeft)
            {
                int cos = 1;
                int sin = 0;
                int offsetX = 0;
                int offsetY = 0;

                float ScaleX;
                float ScaleY;
                float STX;
                float STY;


                int ViewPortWidth = Screen.width;
                int ViewPortHeight = (int)(Screen.width / AspectRatio);

                STY = 0.5f - Screen.height / 2.0f / ViewPortHeight;
                ScaleY = (1.0f - STY) * Screen.height / ((Screen.height + ViewPortHeight) / 2.0f);
                STX = 0;
                ScaleX = 1;

                _displayTransform[0, 0] = ScaleX * cos;
                _displayTransform[1, 0] = ScaleX * sin;
                _displayTransform[0, 1] = -sin * ScaleX;
                _displayTransform[1, 1] = ScaleY * cos;
                _displayTransform[0, 2] = offsetX + STX * cos - sin * STY;
                _displayTransform[1, 2] = offsetY + sin * STX + cos * STY;

                BackgroundMaterial.SetMatrix("_UnityDisplayTransform", _displayTransform);
            }

        }

        BackgroundMaterial.SetTexture("_MainTex", m_BackgroundTexture);
    }

    void UpdatePose()
    {
        if (m_NativeSession == null)
            return;

        IntPtr poseHandle = m_NativeSession.PoseApi.Create();

        m_NativeSession.updateTexture_y_memcpy_uv_assign_RGB_pose(_videoTexture_Y_bytes, _videoTexture_uv_RGB_bytes, poseHandle);

        Pose resultPose = m_NativeSession.PoseApi.ExtractPoseValue(poseHandle);

        standardARCameraPose.SetPose(resultPose);

        m_NativeSession.PoseApi.Destroy(poseHandle);
    }

    void Update()
    {
        UpdateSession();
        UpdateAndroidTexture();
        UpdatePose();
    }
}

#endif
