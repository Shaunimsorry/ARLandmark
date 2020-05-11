using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading;
//SenseAR
using SenseAR;
using SenseARInternal;
//MapBox
using Mapbox;
using Mapbox.Utils;
using Mapbox.Unity.Location;


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
    public Text gpsText0;

    //MapBox API Interface
    public LocationProviderFactory mapboxLocationFactory;
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

        //Starting Location For Compass and GPS
        Input.location.Start();
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

            //Attempt Disabling The PointCloud as it is visually distracting
            slamController.m_PointCloudPrefab.SetActive(false);
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
        
        //Live Mapbox Location Debugging
        var locationProvider = mapboxLocationFactory.DefaultLocationProvider;
        gpsText0.text = "Current Location: " + "\n" + locationProvider.CurrentLocation.LatitudeLongitude.ToString() +"\n" + "Current Heading: " + Input.compass.trueHeading.ToString();


        //SenseAR Camera Testing
        //Log The Camera's Position to the GUI [For Test Purposes]
        var unityCamPos = GameObject.Find("ARCamera").transform.position.ToString();
        var unityCamCompPos = GameObject.Find("ARCamera").GetComponent<Camera>().transform.position.ToString();
        var SenseARCompPos = GameObject.Find("ARCamera").GetComponent<SenseARCameraPose>().transform.position.ToString();
        //TextOutPUT
        debugTXT1.text = unityCamPos+"\n"+unityCamPos+"\n"+SenseARCompPos;
    }

}
