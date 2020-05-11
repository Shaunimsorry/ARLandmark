#if  UNITY_ANDROID || UNITY_EDITOR

//-----------------------------------------------------------------------
// <copyright file="HelloARController.cs" company="Google">
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading;

using SenseAR;
using SenseARInternal;

public class HelloMarkerController : MonoBehaviour
{
    public enum ReferenceImageType
    {
        SingleImage,
        Patt,
        Database,
    }

    /// <summary>
    /// Standard AR device prefab
    /// </summary>
    public GameObject SenseARBasePrefab;

    /// <summary>
    /// A model to place when a raycast from a user touch hits a plane.
    /// </summary>
    public GameObject AndyAndroidPrefab;

    /// <summary>
    //  axis prefab
    /// </summary>
    public GameObject AxisPrefab;

    public ReferenceImageType type;
    public Texture2D[] textures;
    public string config;
    public string[] patts;
    public string path;

    /// <summary>
    /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
    /// </summary>
    private bool m_IsQuitting = false;


    // prefab instance
    private GameObject SenseARInstance;
    private GameObject axisInstance;
    private SenseARSLAMController slamController;
	private SenseARUpdateTexture updateTexture;
    private AndroidPermissionUtil permissionUtil;
    private ARCoreSession session;
    private ReferenceImageDatabase database;

    // start and stop SLAM UI
    public Text InfoDisplay;
    public Button shotButton;

    private bool bCompleteReadingImageDatabase = false;
    private byte[] ImageDatabaseBuffer;
    private int ImageDatabaseBufferLength;

    private bool bCompleteReadingConfig = false;
    private byte[] ConfigBuffer;
    private int ConfigBufferLength;

    private bool bCompleteReadingPatts = false;
    private byte[][] PattsBuffer;
    private int[] PattsBufferLength;

    public Dictionary<int, GameObject> PlacedGameobject = new Dictionary<int, GameObject>();

    void Start()
    {
        SenseARInstance = null;
        axisInstance = null;
        slamController = null;
        permissionUtil = GameObject.Find("Instantiate").GetComponent<AndroidPermissionUtil>();

        ApiArAvailability arAvailability = ApiArAvailability.AR_AVAILABILITY_SUPPORTED_NOT_INSTALLED;
        NativeSession.CheckApkAvailability(ref arAvailability);
        if (arAvailability == ApiArAvailability.AR_AVAILABILITY_SUPPORTED_NOT_INSTALLED)
        {
            ApiArInstallStatus status = ApiArInstallStatus.AR_INSTALL_STATUS_INSTALL_REQUESTED;
            NativeSession.RequestInstall(1,ref status);
        }

        if (shotButton)
            shotButton.gameObject.SetActive(false);


        NativeSession.CheckAuthorized(getAppId());

        if (type == ReferenceImageType.Patt)
        {
            StartCoroutine(ReadConfig());
            StartCoroutine(ReadPatts());
        }

        if (type == ReferenceImageType.Database)
        {
            StartCoroutine(ReadImageDatabase());
        }
    }

    private IEnumerator ReadConfig()
    {
        string sPath = Application.streamingAssetsPath + config;
        WWW www = new WWW(sPath);
        yield return www;
        if (www.error != null)
        {
            Debug.LogError(www.error);
        }

        bCompleteReadingConfig = true;
        ConfigBuffer = www.bytes;
        ConfigBufferLength = www.bytesDownloaded;
    }

    private IEnumerator ReadImageDatabase()
    {
        string sPath = Application.streamingAssetsPath + path;
        Debug.Log("imagedatabase path: " + sPath);
        WWW www = new WWW(sPath);
        yield return www;
        if(www.error != null)
        {
            Debug.LogError(www.error);
        }

        bCompleteReadingImageDatabase = true;
        ImageDatabaseBuffer = www.bytes;
        ImageDatabaseBufferLength = www.bytesDownloaded;
        //Config.DatabaseBuffer = www.bytes;
        //Config.DatabaseBufferSize = www.bytesDownloaded;
        Debug.Log("imagedb buffer size " + www.bytesDownloaded);
    }

    private IEnumerator ReadPatts()
    {
        PattsBuffer = new byte[patts.Length][];
        PattsBufferLength = new int[patts.Length];
        for(int i=0; i < patts.Length; ++i)
        {
            string sPath = Application.streamingAssetsPath + patts[i];
            WWW www = new WWW(sPath);
            yield return www;
            if (www.error != null)
            {
                Debug.LogError(www.error);
            }

            PattsBuffer[i] = www.bytes;
            PattsBufferLength[i] = www.bytesDownloaded;

        }
    }

    private void AsynCheckCapability() {
        ApiArStatus ret = NativeSession.CheckAuthorized(getAppId());
        Debug.Log("Log: HelloARController Capability ret = " + ret);
    }

    private string getAppId() {
        return "10010001107";
    }

    public void CreateSenseAR()
    {
        if (type == ReferenceImageType.Database && bCompleteReadingImageDatabase == false)
        {
            Debug.Log("CreateSenseAR Failed");
            return;
        }

        if (type == ReferenceImageType.Patt && bCompleteReadingConfig == false)
        {
            Debug.Log("CreateSenseAR Failed");
            return;
        }

        if (shotButton)
            shotButton.gameObject.SetActive(true);

        if(SenseARInstance==null)
        {
            SenseARInstance = (GameObject)Instantiate(SenseARBasePrefab, transform.position, transform.rotation);
            slamController = GameObject.Find("SlamController").GetComponent<SenseARSLAMController>();
			updateTexture = GameObject.Find("ARCamera").GetComponent<SenseARUpdateTexture>();
            session = SenseARInstance.GetComponent<ARCoreSession>();
            database = new ReferenceImageDatabase(Session.GetNativeSession());
            if (type == ReferenceImageType.SingleImage)
            {
                for(int i=0; i<textures.Length; ++i)
                {
                    database.AddImage(textures[i].name, textures[i]);
                }
            }
            else if(type == ReferenceImageType.Patt)
            {
                database.LoadConfig(ConfigBuffer, ConfigBufferLength);
                for (int i = 0; i < patts.Length; ++i)
                {
                    database.AddPatt(PattsBuffer[i], PattsBufferLength[i]);
                }
            }
            else
            {
                database.Deserialize(ImageDatabaseBuffer, ImageDatabaseBufferLength);
            }
            session.SetReferenceImageDatabase(database);

            session.Connect();

            // display size return screen size when portrait mode
            //int ScreenWidth = 0, ScreenHeight = 0;
            //Session.GetNativeSession().SessionApi.GetDisplaySize(ref ScreenWidth, ref ScreenHeight);
        }

        if (AxisPrefab)
        {
            axisInstance = (GameObject)Instantiate(AxisPrefab, new Vector3(0, 0, 0), transform.rotation);
        }
    }

    public void DestroySenseAR()
    {


        if (shotButton)
            shotButton.gameObject.SetActive(false);

        if (SenseARInstance)
        {
            slamController = null;
            database.Destroy();
            database = null;
            Destroy(SenseARInstance);
            SenseARInstance = null;

            Debug.LogError("after SenseAR.Destroy");
        }

        if (axisInstance)
        {
            Destroy(axisInstance);
            axisInstance = null;

            Debug.LogError("after axis.Destroy");
        }

        Debug.LogError("end DestroySenseAR");
    }

    public void OnApplicationPause(bool pause)
    {
        if (pause == true)
        {
            ARDebug.LogInfo("OnApplicationPause false, HelloARController _RemoveAnchorGameObject");
        }
    }

	public void Shoot()
	{
		updateTexture.Shoot ();
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (type == ReferenceImageType.Database && bCompleteReadingImageDatabase == false)
            return;

        if (slamController == null)
            return;

        if (session != null && session.SessionConfig.SLAMAlgorithmMode == ApiAlgorithmMode.Disabled && session.SessionConfig.ImageTrackingAlgorithmMode == ApiAlgorithmMode.Enabled)
            DrawAugmentedImage(AndyAndroidPrefab);

        if (session != null && session.SessionConfig.SLAMAlgorithmMode == ApiAlgorithmMode.Enabled && session.SessionConfig.ImageTrackingAlgorithmMode == ApiAlgorithmMode.Enabled)
            DrawAugmentedImageWithSLAM(AndyAndroidPrefab);

        _QuitOnConnectionErrors();
    }

    private void DrawAugmentedImage(GameObject prefab)
    {
        Dictionary<int, Pose>.KeyCollection keyCol = slamController.MarkerMap.Keys;
        foreach (int key in PlacedGameobject.Keys)
        {
            if (!slamController.MarkerMap.ContainsKey(key))
            {
                PlacedGameobject[key].SetActive(false);
            }
        }

        foreach (int key in keyCol)
        {
            if (PlacedGameobject.ContainsKey(key))
            {
                PlacedGameobject[key].transform.position = slamController.MarkerMap[key].position;
                PlacedGameobject[key].transform.rotation = slamController.MarkerMap[key].rotation;
                PlacedGameobject[key].SetActive(true);
            }
            else
            {
                GameObject obj = Instantiate(prefab, slamController.MarkerMap[key].position, slamController.MarkerMap[key].rotation);
                obj.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                PlacedGameobject.Add(key, obj);
            }
        }
    }

    private void DrawAugmentedImageWithSLAM(GameObject prefab)
    {
        Dictionary<int, Pose>.KeyCollection keyCol = slamController.MarkerMap.Keys;
        foreach (int key in keyCol)
        {
            if (PlacedGameobject.ContainsKey(key))
            {
                PlacedGameobject[key].transform.position = slamController.MarkerMap[key].position;
                PlacedGameobject[key].transform.rotation = slamController.MarkerMap[key].rotation;
            }
            else
            {
                GameObject obj = Instantiate(prefab, slamController.MarkerMap[key].position, slamController.MarkerMap[key].rotation);
                PlacedGameobject.Add(key, obj);
            }
        }
    }


    /// <summary>
    /// Quit the application if there was a connection error for the ARCore session.
    /// </summary>
    private void _QuitOnConnectionErrors()
    {
        if (m_IsQuitting)
        {
            return;
        }

        // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
        if (Session.ConnectionState == SessionConnectionState.UserRejectedNeededPermission)
        {
            _ShowAndroidToastMessage("Camera permission is needed to run this application.");
            m_IsQuitting = true;
            Invoke("DoQuit", 0.5f);
        }
        else if (Session.ConnectionState == SessionConnectionState.ConnectToServiceFailed)
        {
            _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
            m_IsQuitting = true;
            Invoke("DoQuit", 0.5f);
        }
    }

    /// <summary>
    /// Actually quit the application.
    /// </summary>
    private void DoQuit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Show an Android toast message.
    /// </summary>
    /// <param name="message">Message string to show in the toast.</param>
    private void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                    message, 0);
                toastObject.Call("show");
            }));
        }
    }
}
#endif
