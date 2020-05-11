#if  UNITY_ANDROID || UNITY_EDITOR 
//-----------------------------------------------------------------------
// <copyright file="SenseARUpdateTexture.cs" >
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

using System;
using System.IO;
using System.Collections;
using UnityEngine;
using SenseARInternal;
using SenseAR;
using UnityEngine.Rendering;

public class SenseARUpdateTexture : MonoBehaviour {
    private NativeSession m_NativeSession;
	
	private int FrameWidth = 1920;
	private int FrameHeight = 1080;
	private int ScreenWidth = 1080;
	private int ScreenHeight = 2160;
    private float AspectRatio = 0;

    private SenseARCameraPose SenseARCameraPose;

#if UNITY_EDITOR
	private Color32[] coloraerrt;
	private Texture2D vvsd;
    public Material rgbMaterial;

	private int updateTestIndex = 0;
	private int updatePixelsNumPerFrame = 100000;
	private float updateColorStep = 0.1f;
	private int updateTestChannel = 0;
	private Color updateTestColor32;
#endif

    // yuv material
	private Texture2D _videoTextureY;
	private byte[] _videoTexture_Y_bytes;
	private Texture2D _videoTextureUV;
	private byte[] _videoTexture_uv_RGB_bytes;
	public Material yuvMaterial;

    // command buffer
	private bool bCommandBufferInitialized;
    private CommandBuffer m_VideoCommandBuffer;
    private Material _Material;

    private Matrix4x4 _displayTransform;
    public bool LinearColorSpace = false;

	public bool bBackgroundNoCrop = false;          

	void Awake() {


	}

	// Use this for initialization
	void Start () {

        m_NativeSession = Session.GetNativeSession();
        SenseARCameraPose = GetComponent<SenseARCameraPose> ();

#if UNITY_EDITOR
		genUnityTexturesForEditor();
		_Material = rgbMaterial;

#elif UNITY_ANDROID
        m_NativeSession.getTextureSize (ref FrameWidth, ref FrameHeight);
        AspectRatio = (float)FrameWidth / FrameHeight;
        Debug.Log("frame width: " + FrameWidth + " frame height: " + FrameHeight + " aspect: " + AspectRatio);

        //m_NativeSession.SessionApi.GetDisplaySize (ref ScreenWidth, ref ScreenHeight);
        //Debug.Log("screen width: " + ScreenWidth + " screen height: " + ScreenHeight);

        //Screen.SetResolution (Screen.width, Screen.height, true);
		_Material = yuvMaterial;
		
		bCommandBufferInitialized = false;
		genUnityTextures ();
#endif
    }

    private void OnDestroy()
    {
        _videoTextureY = null;
        _videoTexture_Y_bytes = null;

        _videoTextureUV = null;
        _videoTexture_uv_RGB_bytes = null;
    }

	void InitializeCommandBuffer()
	{
		m_VideoCommandBuffer = new CommandBuffer();
        int ViewPortWidth = Screen.width;
        int ViewPortHeight = (int)(Screen.width / AspectRatio);

        Vector2 MaintexST;
        MaintexST.y = 0.5f - Screen.height / 2.0f / ViewPortHeight;
        MaintexST.x = (1.0f - MaintexST.y) * Screen.height / ((Screen.height + ViewPortHeight) / 2.0f);
        _Material.SetVector("_MainTextST", MaintexST);

        m_VideoCommandBuffer.Blit(null, BuiltinRenderTextureType.CurrentActive, _Material);
        GetComponent<Camera>().AddCommandBuffer(CameraEvent.BeforeForwardOpaque, m_VideoCommandBuffer);
		bCommandBufferInitialized = true;
	}

	// init no crop camera background
	void InitializeCommandBufferNoCrop()
	{	
		m_VideoCommandBuffer = new CommandBuffer();

        m_NativeSession.getTextureSize (ref FrameWidth, ref FrameHeight);
        AspectRatio = (float)FrameWidth / FrameHeight;

		Debug.Log("frame width: " + FrameWidth + " frame height: " + FrameHeight + " aspect: " + AspectRatio);

		float viewPortW = Screen.height*AspectRatio, viewPortH = Screen.height;
		float x = 0;
		float y = 0;

		Debug.Log("screen width: " + Screen.width + " screen height: " + Screen.height + " aspect: " + AspectRatio);
		Debug.Log("viewport width: " + viewPortW + " viewport height: " + viewPortH + " aspect: " + AspectRatio);

		Vector2 MaintexST;
        MaintexST.y = 0.0f;
        MaintexST.x = 1.0f;
        _Material.SetVector("_MainTextST", MaintexST);

		m_VideoCommandBuffer.Blit(null, BuiltinRenderTextureType.CurrentActive, _Material);
		GetComponent<Camera>().pixelRect = new Rect(x, y, viewPortW, viewPortH);
		GetComponent<Camera>().aspect = (float)viewPortW/(float)viewPortH;
        GetComponent<Camera>().AddCommandBuffer(CameraEvent.BeforeForwardOpaque, m_VideoCommandBuffer);
		bCommandBufferInitialized = true;

	}

	void genUnityTextures()
	{
		_videoTextureY = new Texture2D (FrameWidth, FrameHeight, TextureFormat.Alpha8, false, LinearColorSpace);
		_videoTexture_Y_bytes = new byte[FrameWidth * FrameHeight];

		_videoTextureUV = new Texture2D(FrameWidth/2, FrameHeight/2, TextureFormat.RGB24, false, LinearColorSpace);
		_videoTexture_uv_RGB_bytes = new byte[FrameWidth*FrameHeight/4 * 3];
	}

#if UNITY_EDITOR
	void genUnityTexturesForEditor()
	{
		vvsd = new Texture2D(FrameWidth, FrameHeight, TextureFormat.ARGB32, false);
		coloraerrt = new Color32[FrameHeight * FrameWidth];

		for (int i = 0; i < FrameHeight * FrameWidth; i++)
		{
			coloraerrt[i] = Color.blue;
		}

		vvsd.SetPixels32(coloraerrt);
		vvsd.Apply();
	}
	
	private void updateUpdateTestColor32()
	{
		float maxVal = 0.9f;
		float minVal = 0;
		if (updateTestChannel == 0) {
			updateTestColor32.r = (updateTestColor32.r >= maxVal - updateColorStep) ? minVal : (updateTestColor32.r + updateColorStep);
		}
		
		if (updateTestChannel == 1) {
			updateTestColor32.g = (updateTestColor32.g >= maxVal - updateColorStep) ? minVal : (updateTestColor32.g + updateColorStep);
		}
		
		if (updateTestChannel == 2) {
			updateTestColor32.b = (updateTestColor32.b >= maxVal - updateColorStep) ? minVal : (updateTestColor32.b + updateColorStep);
		}
		
		
		updateTestChannel = (updateTestChannel == 2) ? 0 : updateTestChannel + 1;
	}
	
	private void updateTest()
	{
		int updateTestIndexPre = updateTestIndex;
		int pixelEnd = FrameWidth * FrameHeight;
		while (
			updateTestIndex < pixelEnd &&
			updateTestIndex < updateTestIndexPre + updatePixelsNumPerFrame) {
			coloraerrt [updateTestIndex++] = updateTestColor32;
		}
		
		if (updateTestIndex >= pixelEnd) {
			updateTestIndex = 0;
			updateUpdateTestColor32 ();
		}
		
		vvsd.SetPixels32(coloraerrt);
		vvsd.Apply();
		_Material.mainTexture = vvsd;
	}
#endif	

	void updateAndroidTexture_YUV()
	{
		if (m_NativeSession == null)
			return;
		
		IntPtr poseHandle = m_NativeSession.PoseApi.Create ();

		m_NativeSession.updateTexture_y_memcpy_uv_assign_RGB_pose (_videoTexture_Y_bytes, _videoTexture_uv_RGB_bytes, poseHandle);
        
		Pose resultPose = m_NativeSession.PoseApi.ExtractPoseValue (poseHandle);

        SenseARCameraPose.SetPose (resultPose);
		
		m_NativeSession.PoseApi.Destroy (poseHandle);

		_videoTextureY.LoadRawTextureData (_videoTexture_Y_bytes);
		_videoTextureY.Apply ();

		_videoTextureUV.LoadRawTextureData (_videoTexture_uv_RGB_bytes);
		_videoTextureUV.Apply ();

		//Test for texture_y and texture_uv
		//StartCoroutine (UploadPNG (_videoTextureY.EncodeToPNG()));
		//StartCoroutine (UploadPNG (_videoTextureUV.EncodeToPNG()));

		_Material.SetTexture ("_uvTex", _videoTextureUV);
		_Material.mainTexture = _videoTextureY;
        if (LinearColorSpace)
            _Material.SetFloat("_gamma", 2.02f);

    }

    private int FrameIndex = 1;
	IEnumerator UploadPNG (byte[] pic) {
		yield return new WaitForEndOfFrame();  

		string filename = "image" + FrameIndex++ + ".png";
		string[] path_save = new string[]{"/mnt/sdcard/DCIM/Camera"};
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
		StartCoroutine (UploadPNG (_videoTextureY.EncodeToPNG()));
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
        if(FrameHandle!=IntPtr.Zero)
            Session.SessionManager.Update(FrameHandle);

        m_NativeSession.Update();
    }

	void OnPreRender()
	{
		if (!bCommandBufferInitialized) {
			if(bBackgroundNoCrop)
			{
				InitializeCommandBufferNoCrop();
			}
			else
			{
				InitializeCommandBuffer ();
			}
		}

		#if UNITY_EDITOR

		#elif UNITY_ANDROID
        
        Vector3 Angles;
        Angles.x = 0.0f;
        Angles.y = 0.0f;
        Angles.z = 0.0f;


        int androidOrientation = 0;
        switch (Screen.orientation)
        {
            case ScreenOrientation.LandscapeLeft:
                //androidOrientation = androidRotation90;
                break;
            case ScreenOrientation.LandscapeRight:
                //androidOrientation = androidRotation270;
                Angles.x = 0.0f;
                Angles.y = 0.0f;
                Angles.z = 180.0f;
                break;
            case ScreenOrientation.Portrait:
                //androidOrientation = androidRotation0;
                Angles.x = 0.0f;
                Angles.y = 0.0f;
                Angles.z = -90.0f;
                break;
            case ScreenOrientation.PortraitUpsideDown:
                //androidOrientation = androidRotation180;
                break;
        }


        Quaternion quad=Quaternion.Euler(Angles);
        _displayTransform = Matrix4x4.Rotate(quad);

        _Material.SetMatrix("_DisplayTransform", _displayTransform);

		#endif
	}

    void Update()
    {
#if UNITY_EDITOR
		updateTest();
#elif UNITY_ANDROID

        if(IsVideoUpdated() == true)
		{
            UpdateSession();
            updateAndroidTexture_YUV();
        }

#endif
    }
}

 #endif