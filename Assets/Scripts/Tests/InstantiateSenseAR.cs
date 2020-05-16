using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SenseAR;
using SenseARInternal;


public class InstantiateSenseAR : MonoBehaviour
{
    public GameObject SenseARPrefab;
    public GameObject senseARInstance;
    public SenseARSLAMSystem senseARSLAMController;
    public SenseARUpdateTextureOES updateTexture;
    public AndroidPermissionUtil andriodPermissionUtility;

    public Text debugTxt0;
    public Text debugTxt1;
    public Text debugTxt2;

    public NativeSession _nativeSession;
    public HandGestureApi _handgestureapi;

    //The Handle for the Handgesture
    public IntPtr _handgestureHandle;
    public HandGestureType _handGestureType;

    public IntPtr _listHandle;
    public IntPtr _trackableHandle;
    public List<Trackable> TrackList = new  List<Trackable>();

    void Start()
    {
        InitiateAPI();
    }

    public void InitiateAPI()
    {
        senseARInstance = null;
        senseARSLAMController = null;
        ApiArAvailability arAvailability = ApiArAvailability.AR_AVAILABILITY_SUPPORTED_NOT_INSTALLED;
        NativeSession.CheckApkAvailability(ref arAvailability);
        if(arAvailability != ApiArAvailability.AR_AVAILABILITY_SUPPORTED_INSTALLED)
        {
            ApiArInstallStatus status = ApiArInstallStatus.AR_INSTALL_STATUS_INSTALL_REQUESTED;
            NativeSession.RequestInstall(1,ref status);
        }
        ApiArStatus ret = NativeSession.CheckAuthorized(getAppId());
    }
    private void AsynCheckCapability()
    {ApiArStatus ret = NativeSession.CheckAuthorized(getAppId());}
    private string getAppId()
    {return "10010001107"; }
    public void GoSense()
    {
        if(senseARInstance==null)
        {
            senseARInstance = (GameObject)Instantiate(SenseARPrefab, transform.position,transform.rotation);
            senseARSLAMController = GameObject.Find("SlamController").GetComponent<SenseARSLAMSystem>();
            updateTexture = GameObject.Find("ARCamera").GetComponent<SenseARUpdateTextureOES>();
            senseARSLAMController.m_PointCloudPrefab.SetActive(true);
        }
    }

    void Update()
    {
        //Discord Help:
        // _nativeSession.SessionApi.GetAllTrackables(TrackList,ApiTrackableType.HandGesture);
        // if(TrackList[0] is TrackedHandGesture thg)
        // {
        //     var a = thg.HandGestureType;
        //     debugTxt0.text = "Tracklist Count Is:" + TrackList.Count.ToString()+"\n"+thg.HandGestureType.ToString();
        // }

        if(_nativeSession == null)
        {
            _nativeSession = senseARSLAMController.m_NativeSession;
            debugTxt0.text = ("No Native Session Set,attempting to set native session up");
        }else
        {
            _handgestureapi = _nativeSession.HandGestureApi;
            debugTxt0.text  = _nativeSession.SessionApi.GetTrackableCount(ApiTrackableType.HandGesture).ToString();
        }

    }
}
