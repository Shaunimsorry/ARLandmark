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

    public bool planeTracking = true;
    public bool pointClouds = true;

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

        if(planeTracking)
        {
            senseARInstance.GetComponent<SenseARSession>().SessionConfig.PlaneFindingAlgorithmMode = ApiAlgorithmMode.Enabled;
        }else
        {
            senseARInstance.GetComponent<SenseARSession>().SessionConfig.PlaneFindingAlgorithmMode = ApiAlgorithmMode.Disabled;
        }
    }
}
