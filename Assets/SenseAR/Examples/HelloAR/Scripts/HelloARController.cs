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

public class HelloARController : MonoBehaviour
{
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

    // start and stop SLAM UI
    public Text InfoDisplay;
    public Button shotButton;

    // for touch insection
    private Touch[] prevTouches = null;
    private Touch[] thisTouches = null;
    private bool bPlaceModel;
    private GameObject selectedObj_;

    void Start()
    {
        SenseARInstance = null;
        axisInstance = null;
        slamController = null;
        permissionUtil = GameObject.Find("Instantiate").GetComponent<AndroidPermissionUtil>();

        prevTouches = new Touch[2];
        selectedObj_ = null;
        bPlaceModel = true;

        ApiArAvailability arAvailability = ApiArAvailability.AR_AVAILABILITY_SUPPORTED_NOT_INSTALLED;
        NativeSession.CheckApkAvailability(ref arAvailability);
        if (arAvailability != ApiArAvailability.AR_AVAILABILITY_SUPPORTED_INSTALLED)
        {
            ApiArInstallStatus status = ApiArInstallStatus.AR_INSTALL_STATUS_INSTALL_REQUESTED;
            NativeSession.RequestInstall(1,ref status);
        }

        if (shotButton)
            shotButton.gameObject.SetActive(false);


        ApiArStatus ret =  NativeSession.CheckAuthorized(getAppId());
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
        if (shotButton)
            shotButton.gameObject.SetActive(true);

        if(SenseARInstance==null)
        {
            SenseARInstance = (GameObject)Instantiate(SenseARBasePrefab, transform.position, transform.rotation);
            slamController = GameObject.Find("SlamController").GetComponent<SenseARSLAMController>();
			updateTexture = GameObject.Find("ARCamera").GetComponent<SenseARUpdateTexture>();

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

        _RemoveAnchorGameObject();

        Debug.LogError("end DestroySenseAR");
    }


    private void _RemoveAnchorGameObject()
    {
        // for single object
        if (m_AnchorObj != null)
        {
            bPlaceModel = true;
            m_ModelObj = null;

            Destroy(m_AnchorObj);
            Debug.LogError("after m_Anchor.Destroy");
        }
        m_AnchorObj = null;

        // for multiobject
        GameObject[] sceneObjs = GameObject.FindGameObjectsWithTag("ARObj");
        foreach (GameObject obj in sceneObjs)
        {
            GameObject AnchorParent = obj.transform.parent.gameObject;
            Destroy(obj);
            Destroy(AnchorParent);
            Debug.LogError("AnchorParent.Destroy");
        }

        Debug.LogError("after _RemoveAnchorGameObject");
    }

    public void OnApplicationPause(bool pause)
    {
        if (pause == true)
        {
            ARDebug.LogInfo("OnApplicationPause false, HelloARController _RemoveAnchorGameObject");
            _RemoveAnchorGameObject();
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

#if UNITY_EDITOR

#elif UNITY_ANDROID
        if (InfoDisplay != null && slamController != null)
        {
            string str = slamController.GetSLAMDebugStr();
            InfoDisplay.text = str;
        }   
#endif
            //HandleTouch(AndyAndroidPrefab);
            HandleTouch_Single(AndyAndroidPrefab);

        _QuitOnConnectionErrors();
    }


    GameObject m_AnchorObj = null;
    GameObject m_ModelObj = null;
    bool m_bOverGameObj = true;
    public void HandleTouch_Single(GameObject prefab)
    {
        Session.SetHitTestMode(ApiHitTestMode.PolygonPersistence);

        if (Input.touchCount == 0)
            return;

        if (Input.touches[0].phase == TouchPhase.Began)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId) == false)
                m_bOverGameObj = false;
            else
                m_bOverGameObj = true;
        }

        if (slamController == null || Frame.TrackingState != TrackingState.Tracking)
            return;

        ARDebug.LogInfo("bPlaceModel" + bPlaceModel);
        ARDebug.LogInfo("HandleTouch_Single:IsPointerOverGameObject:" + m_bOverGameObj);

        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;
        if (m_bOverGameObj == false && Session.Raycast(Input.touches[0].position.x, Input.touches[0].position.y, raycastFilter, out hit))
        {
            ARDebug.LogInfo("Raycast:" + Input.touches[0].position.x + "," + Input.touches[0].position.y);

            if (bPlaceModel)
            {
                m_ModelObj = MonoBehaviour.Instantiate(prefab, hit.Pose.position, hit.Pose.rotation);

                // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                // world evolves.
                //Anchor anchor = hit.Trackable.CreateAnchor(hit.Pose);
                //m_AnchorObj = anchor.gameObject;
                m_AnchorObj = new GameObject("anchor");

                // Make Andy model a child of the anchor.
                m_ModelObj.transform.parent = m_AnchorObj.transform;
                m_ModelObj.tag = "ARObj";
                m_ModelObj.layer = 10;

                bPlaceModel = false;
            }

            m_ModelObj.transform.position = hit.Pose.position;
        }
    }

    // handle touch insection
    private void TouchOnSceneObj(GameObject gameObj)
    {
        bPlaceModel = false;
        if (selectedObj_ == null)
        {
            selectedObj_ = gameObj;
        }

        if (selectedObj_ != gameObj)
        {
            selectedObj_ = gameObj;
        }
    }

    public void HandleTouch(GameObject prefab)
    {
        if (slamController == null || Frame.TrackingState != TrackingState.Tracking)
            return;

        Camera targetCamera = slamController.m_Camera;
        thisTouches = Input.touches;
        switch (Input.touchCount)
        {
            case 1:
                if (thisTouches[0].phase == TouchPhase.Began)
                {
                    if (EventSystem.current.IsPointerOverGameObject(thisTouches[0].fingerId) == false)
                    {
                        bPlaceModel = true;

                        RaycastHit objhit;
                        Ray ray;
                        ray = targetCamera.ScreenPointToRay(thisTouches[0].position);
                        if (Physics.Raycast(ray, out objhit, Mathf.Infinity, 1 << 10))
                        {
                            GameObject gameObj = objhit.collider.gameObject;
                            if (gameObj != null && gameObj.CompareTag("ARObj"))
                            {
                                TouchOnSceneObj(gameObj);
                            }

                            bPlaceModel = false;
                        }
                        else
                        {
                            selectedObj_ = null;
                        }
                    }
                }
                else if (thisTouches[0].phase == TouchPhase.Moved)
                {
                    if (selectedObj_ != null)
                    {
                        // Raycast against the location the player touched to search for planes.
                        TrackableHit hit;
                        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;

                        if (Session.Raycast(thisTouches[0].position.x, thisTouches[0].position.y, raycastFilter, out hit))
                        {
                            selectedObj_.transform.position = hit.Pose.position;
                        }
                    }
                }
                else if (thisTouches[0].phase == TouchPhase.Ended)
                {
                    if (thisTouches[0].position == prevTouches[0].position && bPlaceModel)
                    {
                        // Raycast against the location the player touched to search for planes.
                        TrackableHit hit;
                        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;

                        if (Session.Raycast(thisTouches[0].position.x, thisTouches[0].position.y, raycastFilter, out hit))
                        {
                            GameObject andyObject = MonoBehaviour.Instantiate(prefab, hit.Pose.position, hit.Pose.rotation);

                            // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                            // world evolves.
                            Anchor anchor = hit.Trackable.CreateAnchor(hit.Pose);

                            // Make Andy model a child of the anchor.
                            andyObject.transform.parent = anchor.transform;
                            andyObject.tag = "ARObj";
                            andyObject.layer = 10;
                            selectedObj_ = andyObject;
                        }
                    }
                    bPlaceModel = false;
                }
                break;
            default:
                break;
        }

        prevTouches = thisTouches;
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
