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

using System;
using System.Collections.Generic;
using SenseAR;
using SenseARInternal;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SenseAR_Demo
{
    public class HelloARCloud_AnchorController : MonoBehaviour
    {
        public enum EPlaceType
        {
            Place_None = 0,
            Place_Anc = 1,
            Place_Mode = 2
        }
        /// <summary>
        /// curr Place Obj Type;
        /// </summary>
        private EPlaceType currPlaceType = EPlaceType.Place_None;

        public bool bEnableUseClounAnc;
        public HelloARCloud_CloundRoomManager CloundRoom;
        /// <summary>
        /// Standard AR device prefab
        /// </summary>
        public GameObject SenseARBasePrefab;


        /// <summary>
        /// A model to place when a raycast from a user touch hits a plane.
        /// </summary>
        private GameObject AndyAndroidPrefab;


        /// <summary>
        /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
        /// </summary>
        private bool m_IsQuitting = false;


        // prefab instance
        private GameObject SenseARInstance;
        private GameObject axisInstance;


        private SenseARCameraPose slamCameraPose;
        public SenseARCameraPose GetCameraPose()
        {
            return slamCameraPose;
        }


        private SenseARSLAMController slamController;
        public SenseARSLAMController GetSLAMController()
        {
            return slamController;
        }

        private SenseARUpdateTexture updateTexture;
        public SenseARUpdateTexture GetARUpdateTexture()
        {
            return updateTexture;
        }

        private AndroidPermissionUtil permissionUtil;
        public AndroidPermissionUtil GetPermissionUtil()
        {
            return permissionUtil;
        }

        public static HelloARCloud_AnchorController instance { get; private set; }


        bool m_bOverGameObj = true;
        // for touch insection
        private Touch[] prevTouches = null;
        private Touch[] thisTouches = null;
        private bool bPlaceModel;
        private GameObject selectedObj_;

      

        private void Awake()
        {
            instance = this;
        }


        void Start()
        {
            SenseARInstance = null;
            axisInstance = null;
            CloundRoom = GetComponent<HelloARCloud_CloundRoomManager>();
            slamController = null;
            permissionUtil = GameObject.Find("Instantiate").GetComponent<AndroidPermissionUtil>();


            prevTouches = new Touch[2];
            selectedObj_ = null;
            bPlaceModel = true;

            ApiArAvailability arAvailability = ApiArAvailability.AR_AVAILABILITY_SUPPORTED_NOT_INSTALLED;
            NativeSession.CheckApkAvailability(ref arAvailability);
            if (arAvailability == ApiArAvailability.AR_AVAILABILITY_SUPPORTED_NOT_INSTALLED)
            {
                ApiArInstallStatus status = ApiArInstallStatus.AR_INSTALL_STATUS_INSTALL_REQUESTED;
                NativeSession.RequestInstall(1, ref status);
            }


            NativeSession.CheckAuthorized(getAppId());
           
        }

     
        private void AsynCheckCapability()
        {
            ApiArStatus ret = NativeSession.CheckAuthorized(getAppId());
            Debug.Log("Log: HelloARController Capability ret = " + ret);
        }

        private string getAppId()
        {
            return "10010001107";
        }

        private void CreateSenseAR()
        {
            if (SenseARInstance == null)
            {
                SenseARInstance = (GameObject)Instantiate(SenseARBasePrefab, transform.position, transform.rotation);
                slamController = SenseARInstance.GetComponentInChildren<SenseARSLAMController>();
                updateTexture = SenseARInstance.GetComponentInChildren<SenseARUpdateTexture>();
                slamCameraPose = SenseARInstance.GetComponentInChildren<SenseARCameraPose>();
               
                // display size return screen size when portrait mode
                //int ScreenWidth = 0, ScreenHeight = 0;
                //Session.GetNativeSession().SessionApi.GetDisplaySize(ref ScreenWidth, ref ScreenHeight);

                if (Screen.orientation == ScreenOrientation.LandscapeLeft ||
                   Screen.orientation == ScreenOrientation.LandscapeRight)
                {
                    //Screen.SetResolution(ScreenHeight, ScreenWidth, true);
                    Screen.SetResolution(Screen.width, Screen.height, true);
                }
                else
                {
                    //Screen.SetResolution(ScreenWidth, ScreenHeight, true);
                    Screen.SetResolution(Screen.height, Screen.width, true);
                }
            }
        }

        private void DestroySenseAR()
        {

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


        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            //#if UNITY_EDITOR

            //#elif UNITY_ANDROID
            //        if (InfoDisplay != null && slamController != null)
            //        {
            //            string str = slamController.GetSLAMDebugStr();
            //            InfoDisplay.text = str;
            //        }   
            //#endif
            HandleTouch_Single();
            _QuitOnConnectionErrors();
        }

        /// <summary>
        /// change The Entity Prefab with for Spawn ;
        /// </summary>
        /// <param name="pEntityPrefab"></param>
        public void ChangeSpawnEntity(GameObject pEntityPrefab)
        {
            if (pEntityPrefab != null)
            {
                //bPlaceModel = true;
                //AndyAndroidPrefab = pEntityPrefab;
                //currPlaceType = EPlaceType.Place_Mode;


                //this is demo test without server sync
                //SpawnDemoMode(pEntityPrefab);
            }
        }


  


        public void HandleTouch_Single()
        {
            if(currPlaceType == EPlaceType.Place_None)
            {
                return;
            }
            Session.SetHitTestMode(ApiHitTestMode.PolygonPersistence);

            if (Input.touchCount == 0)
                return;

            if (Input.touches[0].phase == TouchPhase.Began)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId) == false)
                {
                    m_bOverGameObj = false;
                }
                else
                {
                    m_bOverGameObj = true;
                    TouchOnSceneObj(EventSystem.current.currentSelectedGameObject);
                }

            }

            if (slamController == null || Frame.TrackingState != TrackingState.Tracking)
                return;

            ARDebug.LogInfo("bPlaceModel" + bPlaceModel);
            ARDebug.LogInfo("HandleTouch_Single:IsPointerOverGameObject:" + m_bOverGameObj);

            TrackableHit hit;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinBounds | TrackableHitFlags.PlaneWithinPolygon;
            if (m_bOverGameObj == false && Session.Raycast(Input.touches[0].position.x, Input.touches[0].position.y, raycastFilter, out hit))
            {

                //DZ_DebugConsole.Log("Raycast:" + Input.touches[0].position.x + "," + Input.touches[0].position.y);
                ARDebug.LogInfo("Raycast:" + Input.touches[0].position.x + "," + Input.touches[0].position.y);

                if(currPlaceType == EPlaceType.Place_Anc)
                {
                    if (bPlaceModel)
                    {
                        TouchHandleCreatCloudAncObj( hit);
                        bPlaceModel = false;

                    }
                }
                else
                {
                    if (bPlaceModel)
                    {
                        TouchHandleCreateMode(hit.Pose.position, hit.Pose.rotation);
                    }
                    else if (selectedObj_ != null)
                    {
                        selectedObj_.transform.position = hit.Pose.position;
                    }
                }
              
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

        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "SenseTime_Control_Icon.png");
        }

        private void TouchHandleCreatCloudAncObj(TrackableHit pHIt)
        {
            
            if (CloundRoom.IsRoommer())
            {
                Anchor anchor = pHIt.Trackable.CreateAnchor(pHIt.Pose);
                CloundRoom.RegisterRoomCloudAnc(anchor,CreateCloudSuccess,CreateCloudFalure);
   
            }
        }

        private void TouchHandleCreateMode(Vector3 pPosition,Quaternion pRot)
        {
            //DZ_DebugConsole.Log("TouchHandleCreateMode");
            if (AndyAndroidPrefab != null)
            {
                CloundRoom.SpwanMode(AndyAndroidPrefab, pPosition, pRot);
            }
            AndyAndroidPrefab = null;
        }






        public void StartAR()
        {
            CreateSenseAR();
        }

        public void StopAR()
        {
            ResetAll();
            DestroySenseAR();
        }

        public void ResetAll()
        {
            prevTouches = new Touch[2];
            selectedObj_ = null;
            bPlaceModel = true;
            currPlaceType = EPlaceType.Place_None;
            AndyAndroidPrefab = null;
            CloundRoom.ResetALL();
            cloudSuccEvent = null;
            cloudFailureEvent = null;

            LoadCloudSuccEvent = null;
            LoadCloudFailureEvent = null;
            
        }

        /// <summary>
        /// Create Room
        /// </summary>
        /// <param name="pSuccessCall"></param>
        public void CreateRoom(HelloARCloud_CloundRoomManager.CloudRoomEvent pSuccessCall)
        {
            CloundRoom.createRoom(pSuccessCall);
        }

        /// <summary>
        /// Begin Create Cloud Anc
        /// </summary>
        /// <param name="placeCloudAncComCall"></param>
        /// <param name="placeCloudAncFailureCall"></param>
        public void BeginPlaceRoomCloudAnc(HelloARCloud_CloundRoomManager.RoomCloudAncEvent placeCloudAncComCall,
            HelloARCloud_CloundRoomManager.RoomCloudAncEvent placeCloudAncFailureCall)
        {
            cloudSuccEvent = placeCloudAncComCall;
            cloudFailureEvent = placeCloudAncFailureCall;
            currPlaceType = EPlaceType.Place_Anc;
            bPlaceModel = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pSuccessCall"></param>
        /// <param name="RoomID"></param>
        public void EnterRoom(HelloARCloud_CloundRoomManager.CloudRoomEvent pSuccessCall,int RoomID)
        {
            CloundRoom.EnterRoom(pSuccessCall,RoomID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pLoadCloudAncComCall">Load And Spawn CloudAnc Success CallBack</param>
        /// <param name="pLoadCloudAncFailureCall">Load And Spawn CloudAnc Failure CallBack</param>
        public void BeginLoadRoomCloudAnc(HelloARCloud_CloundRoomManager.RoomCloudAncEvent pLoadCloudAncComCall,
            HelloARCloud_CloundRoomManager.RoomCloudAncEvent pLoadCloudAncFailureCall)
        {
            LoadCloudSuccEvent = pLoadCloudAncComCall;
            LoadCloudFailureEvent = pLoadCloudAncFailureCall;

            CloundRoom.LoadRoomCloudAnc(LoadCloudSuccess, LoadCloudFalure);
        }

     
        private HelloARCloud_CloundRoomManager.RoomCloudAncEvent cloudSuccEvent;
        private HelloARCloud_CloundRoomManager.RoomCloudAncEvent cloudFailureEvent;
      
        private void CreateCloudFalure(int id, string error)
        {
            if (cloudFailureEvent != null)
                cloudFailureEvent.Invoke(id, error);
            bPlaceModel = true;
        }

        private void CreateCloudSuccess(int id, string error)
        {
            if(cloudSuccEvent != null)
                cloudSuccEvent.Invoke(id, error);
            currPlaceType = EPlaceType.Place_Mode;
            bPlaceModel = true;
        }

        private HelloARCloud_CloundRoomManager.RoomCloudAncEvent LoadCloudSuccEvent;
        private HelloARCloud_CloundRoomManager.RoomCloudAncEvent LoadCloudFailureEvent;
     

        private void LoadCloudFalure(int id, string error)
        {
            if (LoadCloudFailureEvent != null)
                LoadCloudFailureEvent.Invoke(id, error);
        }

        private void LoadCloudSuccess(int id, string error)
        {
            if(LoadCloudSuccEvent != null)
                LoadCloudSuccEvent.Invoke(id, error);
        }
    }
}
#endif
