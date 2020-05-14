using System.Collections.Generic;
using UnityEngine;
using SenseAR;
using UnityEngine.UI;
using SenseARInternal;
using SenseAR.HelloAR;

public class SenseARSLAMSystem : MonoBehaviour
{

    private NativeSession m_NativeSession;
    public Camera m_Camera;

    public GameObject m_TrackedPlanePrefab;
    private bool m_bEnableTrackedPlane = true;

	public GameObject m_PointCloudPrefab;

    private List<TrackedPlane> m_NewPlanes = new List<TrackedPlane>();
    private List<TrackedPlane> m_AllPlanes = new List<TrackedPlane>();
    private List<TrackedHandGesture> m_AllHandGestures = new List<TrackedHandGesture>();
    private List<GameObject> PlaneLists = new List<GameObject>();
    GameObject pointCloudInstance = null;
    private byte[] m_SLAMDebugInfo ;

    private int mOldPlanes = 0;

    private bool UseDepthTextureOcclusion = true;

    public Dictionary<int, Pose> MarkerMap = new Dictionary<int, Pose>();


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

    }

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

        if (GetComponentInParent<SenseARSession>().SessionConfig.SLAMAlgorithmMode == ApiAlgorithmMode.Enabled && GetComponentInParent<SenseARSession>().SessionConfig.PlaneFindingAlgorithmMode == ApiAlgorithmMode.Enabled)
        {
            if (m_TrackedPlanePrefab)
            {
                GetPlanes(m_NewPlanes, m_AllPlanes);

                for (int i = 0; i < m_NewPlanes.Count; i++)
                {
                    GameObject planeObject = Instantiate(m_TrackedPlanePrefab, Vector3.zero, Quaternion.identity, transform);
                    planeObject.GetComponent<TrackedPlaneVisualizer>().Initialize(m_NewPlanes[i]);
                    PlaneLists.Add(planeObject);
                }
            }
        }

        if(GetComponentInParent<SenseARSession>().SessionConfig.HandGestureAlgorithmMode == ApiAlgorithmMode.Enabled)
        {
            GetHandGestures(m_AllHandGestures);
        }

        if (GetComponentInParent<SenseARSession>().SessionConfig.ImageTrackingAlgorithmMode == ApiAlgorithmMode.Enabled && GetComponentInParent<SenseARSession>().SessionConfig.ImageTrackingAlgorithmMode == ApiAlgorithmMode.Enabled)
        {
            GetTrackedImages();
        }

        if (GetComponentInParent<SenseARSession>().SessionConfig.ImageTrackingAlgorithmMode == ApiAlgorithmMode.Enabled && GetComponentInParent<SenseARSession>().SessionConfig.ImageTrackingAlgorithmMode == ApiAlgorithmMode.Enabled)

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
        for (int i = 0; i < tempTrackableList.Count; i++)
        {
            _SafeAdd<T>(tempTrackableList[i], AllTrackables);
        }
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
