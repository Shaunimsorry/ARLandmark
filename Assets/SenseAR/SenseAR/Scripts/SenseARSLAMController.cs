﻿#if UNITY_ANDROID || UNITY_EDITOR
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


using System.Collections.Generic;
using UnityEngine;
using SenseAR;
using UnityEngine.UI;
using SenseARInternal;
using SenseAR.HelloAR;

public class SenseARSLAMController : MonoBehaviour
{

    private NativeSession m_NativeSession;

    /// <summary>
    /// The first-person camera being used to render the passthrough camera image (i.e. AR background).
    /// </summary>
    public Camera m_Camera;

    /// <summary>
    /// A prefab for tracking and visualizing detected planes.
    /// </summary>
    public GameObject m_TrackedPlanePrefab;
    private bool m_bEnableTrackedPlane = true;

	/// <summary>
	//  point cloud visulize prefab
	/// </summary>
	public GameObject m_PointCloudPrefab;

    /// <summary>
    /// A list to hold new planes ARCore began tracking in the current frame. This object is used across
    /// the application to avoid per-frame allocations.
    /// </summary>
    private List<TrackedPlane> m_NewPlanes = new List<TrackedPlane>();

    /// <summary>
    /// A list to hold all planes ARCore is tracking in the current frame. This object is used across
    /// the application to avoid per-frame allocations.
    /// </summary>
    private List<TrackedPlane> m_AllPlanes = new List<TrackedPlane>();

    /// <summary>
    /// A list to hold all hand gestures tracked in the current frame. This object is used across
    /// the application to avoid per-frame allocations.
    /// </summary>
    private List<TrackedHandGesture> m_AllHandGestures = new List<TrackedHandGesture>();

    /// <summary>
    /// plane mesh game object list for visulization
    /// </summary>
    private List<GameObject> PlaneLists = new List<GameObject>();


    /// <summary>
    // point cloud instance
    /// </summary>
    GameObject pointCloudInstance = null;

    /// <summary>
    // SLAM debug string
    /// </summary>
    private byte[] m_SLAMDebugInfo ;

    /// <summary>
    // previous plane count
    /// </summary>
    private int mOldPlanes = 0;

    private bool UseDepthTextureOcclusion = true;

    public Dictionary<int, Pose> MarkerMap = new Dictionary<int, Pose>();

    // Use this for initialization
    void Start()
    {
        m_NativeSession = Session.GetNativeSession();

        Vector3 tempPos = Vector3.zero;
        Quaternion tempQuad = Quaternion.identity;
        if(m_PointCloudPrefab)
        {
            pointCloudInstance = (GameObject)Instantiate(m_PointCloudPrefab, tempPos, tempQuad);
        }

        mOldPlanes = 0;
        m_SLAMDebugInfo = new byte[2048];
    }

    public string GetSLAMDebugStr()
    {
#if UNITY_ANDROID
        if (m_SLAMDebugInfo != null)
        {
            System.Array.Clear(m_SLAMDebugInfo, 0, 2048);
            m_NativeSession.SessionApi.GetSLAMInfo(m_SLAMDebugInfo);
            string str = System.Text.Encoding.Default.GetString(m_SLAMDebugInfo);
            return str;
        }
#endif
        return "";
    }

    private void OnDestroy()
    {
        if (pointCloudInstance)
        {
            Destroy(pointCloudInstance);
            pointCloudInstance = null;

            Debug.LogError("after pointCloud.Destroy");
        }

        mOldPlanes = 0;
        //_RemovePlaneGameObject();
    }

    /// <summary>
    /// The Unity Update() method.
    /// </summary>
    void Update()
    {
        float fov = new float();
        int FrameWidth = 0, FrameHeight = 0;
#if UNITY_ANDROID
        m_NativeSession.getTextureSize(ref FrameWidth, ref FrameHeight);
        m_NativeSession.getVerticalFov(ref fov);
#endif
        float cameraFocal = FrameHeight * 0.5f / Mathf.Tan(fov * 0.5f * Mathf.PI / 180.0f);
        float h = Screen.height * FrameWidth / Screen.width;
        float realFov = 2.0f * Mathf.Atan(0.5f * h / cameraFocal) * 180.0f / Mathf.PI;
        m_Camera.fieldOfView = realFov;

        if (GetComponentInParent<ARCoreSession>().SessionConfig.SLAMAlgorithmMode == ApiAlgorithmMode.Enabled && GetComponentInParent<ARCoreSession>().SessionConfig.PlaneFindingAlgorithmMode == ApiAlgorithmMode.Enabled)
        {
            if (m_TrackedPlanePrefab)
            {
                // Iterate over planes found in this frame and instantiate corresponding GameObjects to visualize them.
                GetPlanes(m_NewPlanes, m_AllPlanes);

                for (int i = 0; i < m_NewPlanes.Count; i++)
                {
                    // Instantiate a plane visualization prefab and set it to track the new plane. The transform is set to
                    // the origin with an identity rotation since the mesh for our prefab is updated in Unity World
                    // coordinates.
                    GameObject planeObject = Instantiate(m_TrackedPlanePrefab, Vector3.zero, Quaternion.identity, transform);
                    planeObject.GetComponent<TrackedPlaneVisualizer>().Initialize(m_NewPlanes[i]);
                    PlaneLists.Add(planeObject);
                }
            }
        }

        if(GetComponentInParent<ARCoreSession>().SessionConfig.HandGestureAlgorithmMode == ApiAlgorithmMode.Enabled)
        {
            //update hand gestures
            GetHandGestures(m_AllHandGestures);

            // for(int i = 0; i< m_AllHandGestures.Count; i++)
            // {
            //     Debug.Log("hand type "+m_AllHandGestures[i].HandGestureType);
            //     Debug.Log("hand side "+m_AllHandGestures[i].HandeSide);
            //     Debug.Log("hand towards "+m_AllHandGestures[i].HandTowards);
            //     Debug.Log("hand type confidence "+m_AllHandGestures[i].GestureTypeConfidence);
            //     Debug.Log("landmark 2d count "+m_AllHandGestures[i].LandMark2DCount);
            //     Debug.Log("landmark2d test "+m_AllHandGestures[i].LandMark2D[0]);
            // }
        }

        if (GetComponentInParent<ARCoreSession>().SessionConfig.ImageTrackingAlgorithmMode == ApiAlgorithmMode.Enabled && GetComponentInParent<ARCoreSession>().SessionConfig.ImageTrackingAlgorithmMode == ApiAlgorithmMode.Enabled)
        {
            GetTrackedImages();
        }

        if (GetComponentInParent<ARCoreSession>().SessionConfig.ImageTrackingAlgorithmMode == ApiAlgorithmMode.Enabled && GetComponentInParent<ARCoreSession>().SessionConfig.ImageTrackingAlgorithmMode == ApiAlgorithmMode.Enabled)

        {
            GetTrackedImagesWithSLAM();
        }
    }

    private void GetTrackedImages()
    {
        List<Trackable> trackedImages = new List<Trackable>();
        m_NativeSession.SessionApi.GetAllTrackables(trackedImages, ApiTrackableType.Image);
        MarkerMap.Clear();
        for (int i = 0; i < trackedImages.Count; i++)
        {
            int index = ((TrackedImage)trackedImages[i]).Index;

            TrackingState state = trackedImages[i].TrackingState;
            if (state != TrackingState.Tracking)
            {
                return;
            }

            Pose pose = ((TrackedImage)trackedImages[i]).CenterPose;

            if (MarkerMap.ContainsKey(index) == false)
            {
                MarkerMap.Add(index, pose);
                Debug.Log("Marker add: " + index + " pos: " + pose.position);
            }
            else
            {
                MarkerMap[index] = pose;
                Debug.Log("Marker update: " + pose.position);
            }
        }
    }

    private void GetTrackedImagesWithSLAM()
    {
        List<Trackable> trackedImages = new List<Trackable>();
        m_NativeSession.SessionApi.GetAllTrackables(trackedImages, ApiTrackableType.Image);
        for (int i = 0; i < trackedImages.Count; i++)
        {
            int index = ((TrackedImage)trackedImages[i]).Index;

            TrackingState state = trackedImages[i].TrackingState;
            if (state != TrackingState.Tracking)
            {
                return;
            }

            Pose pose = ((TrackedImage)trackedImages[i]).CenterPose;

            if (MarkerMap.ContainsKey(index) == false)
            {
                MarkerMap.Add(index, pose);
                Debug.Log("Marker add: " + index + " pos: " + pose.position);
            }
            else
            {
                MarkerMap[index] = pose;
                Debug.Log("Marker update: " + pose.position);
            }
        }
    }

    private void GetHandGestures<T>(List<T> AllTrackables) where T : Trackable
    {
        if (m_NativeSession == null)
            return;

        AllTrackables.Clear();

        List<Trackable> tempTrackableList = new List<Trackable>();
        m_NativeSession.SessionApi.GetAllTrackables(tempTrackableList, ApiTrackableType.HandGesture);

        for(int i = 0; i < tempTrackableList.Count; i++)
        {
            _SafeAdd<T>(tempTrackableList[i], AllTrackables);
        }
        
    }

    private void GetPlanes<T>(List<T> NewTrackables, List<T> AllTrackables) where T : Trackable
    {
        if (m_NativeSession == null)
            return;
        
        NewTrackables.Clear();
        AllTrackables.Clear();
        List<Trackable> tempTrackableList = new List<Trackable>();

        m_NativeSession.SessionApi.GetAllTrackables(tempTrackableList, ApiTrackableType.Plane);

        //Debug.Log ("Plane:" + tempTrackableList.Count);
        for (int i = 0; i < tempTrackableList.Count; i++)
        {
            _SafeAdd<T>(tempTrackableList[i], AllTrackables);
        }

        //get new planes
        int all = tempTrackableList.Count;
        if (all > mOldPlanes)
        {
            for (int i = mOldPlanes; i < tempTrackableList.Count; i++)
            {
                _SafeAdd<T>(tempTrackableList[i], NewTrackables);
            }
            mOldPlanes = all;
            return;
        }
        else if(all < mOldPlanes)
        {
            // begin to reset 
            mOldPlanes = 0;
            _RemovePlaneGameObject();
            for (int i = mOldPlanes; i < tempTrackableList.Count; i++)
            {
                _SafeAdd<T>(tempTrackableList[i], NewTrackables);
            }

            return;
        }
    }

    private void _SafeAdd<T>(Trackable trackable, List<T> trackables) where T : Trackable
    {
        if (trackable is T)
        {
            trackables.Add(trackable as T);
        }
    }

    private void _RemovePlaneGameObject()
    {
        foreach (GameObject obj in PlaneLists)
        {
            Destroy(obj);
        }

        PlaneLists.Clear();
    }
	
    public void ShowPlane(bool bShow)
    {
        if(pointCloudInstance)
        {
            pointCloudInstance.SetActive(bShow);
        }

        foreach (GameObject obj in PlaneLists)
        {
            obj.SetActive(bShow);
        }
    }

    public void EnableTrackedPlane(bool bEnable)
    {
        m_bEnableTrackedPlane = bEnable;
        foreach (GameObject obj in PlaneLists)
        {
            obj.SetActive(bEnable);
        }
    }

    public void EnablePointCloud(bool bEnable)
    {
        if (pointCloudInstance)
        {
            pointCloudInstance.SetActive(bEnable);
        }
    }

    public void EnableDepthTextureOcclusion(bool bUse)
    {
        UseDepthTextureOcclusion = bUse;
    }

    public int GetPlaneCount()
    {
        return m_AllPlanes.Count;
    }

    public List<TrackedHandGesture> GetHandGestures()
    {
        return m_AllHandGestures;
    }
}

#endif