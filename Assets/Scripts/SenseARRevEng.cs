using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading;
using SenseAR;
using SenseARInternal;


public class SenseARRevEng : MonoBehaviour
{
    public GameObject SenseArPrefab;
    private GameObject SenseARInstance;
    private GameObject axisInstance;
    private SenseARSLAMController slamController;
    private SenseARUpdateTexture updateTexture;
    public AndroidPermissionUtil permissionUtil;

    //Slam UI CTRL and setup Debug On Screen Text Objects
    public Button startSlam;
    public Button stopSlam;
    public Text debugTXT0;
    public Text debugTXT1;

    void start()
    {
        //Init
        SenseARInstance = null;
        axisInstance = null;
        slamController = null;

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
    {
        ApiArStatus ret = NativeSession.CheckAuthorized(getAppId());
        debugTXT1.text = "HelloAR Controller Ret= "+ ret;
    }

    private string getAppId()
    {
        return "10010001107";
    }

    public void StartSenseAR()
    {
        if(SenseARInstance==null)
        {
            SenseARInstance = (GameObject)Instantiate(SenseArPrefab, transform.position,transform.rotation);
            slamController = GameObject.Find("SlamController").GetComponent<SenseARSLAMController>();
            updateTexture = GameObject.Find("ARCamera").GetComponent<SenseARUpdateTexture>();
        }
    }

    public void StopSenseAR()
    {
        slamController = null;
        Destroy(SenseARInstance);
        SenseARInstance = null;
        
        debugTXT1.text = "Sense AR Destroyed";
        debugTXT0.text = "Sense AR Destroyed";
    }

    void Update()
    {
        if(slamController)
        {
            //Start Outputting Slam Info to screen
            debugTXT0.text = slamController.GetSLAMDebugStr();
        }

    }
}
