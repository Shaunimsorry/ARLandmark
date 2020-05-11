using SenseAR_Demo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelloARCloud_CloundDemo_UI : MonoBehaviour
{
    #region UI
    public GameObject mUI_GameStartPanel;
    public GameObject mUI_RoomHandlePanel;
    public GameObject mUI_CreateCloudAncPanel;
    public InputField mUI_RoomIDInput;
    public Button mUI_CreateRoomBtn;
    public Button mUI_JoinRoomBtn;
    public Button mUI_StartARBtn;
    public Button mUI_StopARBtn;

    public GameObject mRoomInfo;
    public Text mRoomIDTxt;
    public Text PlaceAncLogTxt;

    public Button mUISpawnDemoObjBtn;
    public Button mUILoadDemoObjBtn;
    #endregion

    public float fadeAniSecond = 2.0f;
    public Image fadefowImg;
    public Color pBeginColor;
    public Color pEndColor;
    public HelloARCloud_AnchorController ARControl;

    private void Start()
    {
        mUI_StartARBtn.onClick.AddListener(startARBtnClick);
        mUI_StopARBtn.onClick.AddListener(stopARBtnClick);

        mUI_CreateRoomBtn.onClick.AddListener(CreateRoomBtnClick);
        mUI_JoinRoomBtn.onClick.AddListener(joinBtnClick);
        mUI_RoomIDInput.onValueChanged.AddListener(RoomIDChange);

        mUISpawnDemoObjBtn.onClick.AddListener(SpawnDemoBtnClick);
        mUILoadDemoObjBtn.onClick.AddListener(LoadDemoBtnClick);
        Reset();
    }

    private void Reset()
    {
        mRoomInfo.SetActive(false);
        mRoomIDTxt.text = "";
        PlaceAncLogTxt.color = Color.black;
        PlaceAncLogTxt.text = " None CloudAnc";
        mUI_RoomHandlePanel.SetActive(false);
        mUI_CreateCloudAncPanel.SetActive(false);
        mUI_StartARBtn.gameObject.SetActive(true);
        mUI_StopARBtn.gameObject.SetActive(false);
        mUISpawnDemoObjBtn.gameObject.SetActive(false);
        mUILoadDemoObjBtn.gameObject.SetActive(false);
    }



    private void startARBtnClick()
    {
        mUI_StartARBtn.gameObject.SetActive(false);
        StartCoroutine(AsynWaitARCom());
    }

    private void stopARBtnClick()
    {
        ARControl.StopAR();
        Reset();
    }

    private IEnumerator AsynWaitARCom()
    {
        if(fadefowImg != null)
        {
            fadefowImg.color = pBeginColor;
            float waitSecond = fadeAniSecond;
            while (waitSecond > 0)
            {
                if(waitSecond < fadeAniSecond / 2.0f)
                {
                    ARControl.StartAR();
                }
                yield return new WaitForEndOfFrame();
                waitSecond -= Time.deltaTime;
                if (fadefowImg != null)
                    fadefowImg.color = Color.Lerp(pBeginColor, pEndColor,(fadeAniSecond - waitSecond)/ fadeAniSecond);
            }
            if (fadefowImg != null)
                GameObject.Destroy(fadefowImg.gameObject);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            ARControl.StartAR();
            yield return new WaitForSeconds(1.0f);
        }
        mUI_StopARBtn.gameObject.SetActive(true);
        mUI_RoomHandlePanel.SetActive(true);
    }

    private void CreateRoomBtnClick()
    {
        ARControl.CreateRoom(CreateRoomComplety);
      
        mUI_RoomHandlePanel.SetActive(false);
    }



    private void CreateRoomComplety(int pRoomID)
    {
        mRoomInfo.SetActive(true);
        mRoomIDTxt.text = pRoomID.ToString();
        PlaceAncLogTxt.color = Color.black;
        PlaceAncLogTxt.text = " None CloudAnc";
        mUI_CreateCloudAncPanel.SetActive(true);
        ARControl.BeginPlaceRoomCloudAnc(PlaceCloudAncCom, PlaceCloudAncFailure);
        PlaceAncLogTxt.color = Color.green;
        PlaceAncLogTxt.text = "please Click on the ground";
    }




    private void joinBtnClick()
    {
        int pRoomID = 1000;
        if (int.TryParse(mUI_RoomIDInput.text, out pRoomID))
        {
            ARControl.EnterRoom(joinRoomSuccess,pRoomID);
            mUI_RoomHandlePanel.SetActive(false);
            mUI_CreateCloudAncPanel.SetActive(true);
        }
    }

    private void joinRoomSuccess(int pRoomID)
    {
        mUI_RoomHandlePanel.SetActive(false);
        mUI_CreateCloudAncPanel.SetActive(true);
        mRoomInfo.SetActive(true);
        mRoomIDTxt.text = pRoomID.ToString();
        StartCoroutine(LoadCloudAnc());
       
    }

    private IEnumerator LoadCloudAnc()
    {
        yield return new WaitForSeconds(1.0f);
        PlaceAncLogTxt.color = Color.green;
        PlaceAncLogTxt.text = "re Loading Anc........!";
        ARControl.BeginLoadRoomCloudAnc(LoadCloudAncCom, LoadCloudAncFailure);
    }

    private void RoomIDChange(string arg0)
    {
       
    }



    private void PlaceCloudAncCom(int id, string error)
    {
        mUI_RoomHandlePanel.SetActive(false);

        PlaceAncLogTxt.color = Color.green;
        PlaceAncLogTxt.text = "Spawn CloudAnc Success!";

        mUISpawnDemoObjBtn.gameObject.SetActive(true);
        //Check Unit
    }

    private void PlaceCloudAncFailure(int id, string error)
    {
        PlaceAncLogTxt.color = Color.red;
        PlaceAncLogTxt.text = "Spawn CloudAnc Faliure!";
    }

    private void SpawnDemoBtnClick()
    {
        StartCoroutine(SpawnDemoMode());
    }


    private void LoadCloudAncCom(int id, string error)
    {
        PlaceAncLogTxt.color = Color.green;
        PlaceAncLogTxt.text = "Load CloudAnc Success!";

        mUILoadDemoObjBtn.gameObject.SetActive(true);
        mUISpawnDemoObjBtn.gameObject.SetActive(false);
    }


    private void LoadCloudAncFailure(int id, string error)
    {
        PlaceAncLogTxt.text = "Load CloudAnc Faliure!";
        //if Failure Auto ReLoad;
        StartCoroutine(LoadCloudAnc());
    }

    private void LoadDemoBtnClick()
    {
        StartCoroutine(LoadDemoMode());
    }

    public List<GameObject> spawndemoPrefab = new List<GameObject>();
    public float demoSpawnStep = 10.0f;
    public float demofirstSpawnOffset = 5.0f;
    public Vector3 mWorldScale = Vector3.one;

    public IEnumerator SpawnDemoMode()
    {
        HelloARCloud_CloundRoomManager CloundRoom = ARControl.GetComponent<HelloARCloud_CloundRoomManager>();
        if (CloundRoom != null && CloundRoom.isCanAddMode())
        {

            PlaceAncLogTxt.color = Color.green;
            PlaceAncLogTxt.text = "Spawnning the Demo Mode";
            int sIndex = 0,maxcout = spawndemoPrefab.Count;
            while(sIndex < maxcout)
            {
                Transform fR = CloundRoom.getCurrRoomCloud().transform;
                Vector3 vLoc = new Vector3(0.0f + demofirstSpawnOffset + sIndex * demoSpawnStep, 0.0f, 0.0f);
                Vector3 vWorldPos = fR.TransformPoint(vLoc);
                Quaternion vWorldRot = fR.rotation;
                CloundRoom.SpwanMode(spawndemoPrefab[sIndex], vWorldPos, vWorldRot,mWorldScale);
                sIndex++;
                yield return new WaitForSeconds(0.2f);
            }
            PlaceAncLogTxt.color = Color.green;
            PlaceAncLogTxt.text = "SpawnDemoMode Complety!";
        }
        yield return new WaitForEndOfFrame();
    }

    public IEnumerator LoadDemoMode()
    {
        HelloARCloud_CloundRoomManager CloundRoom = ARControl.GetComponent<HelloARCloud_CloundRoomManager>();
        if (CloundRoom != null && CloundRoom.isCanAddMode())
        {
            PlaceAncLogTxt.color = Color.green;
            PlaceAncLogTxt.text = "Loading the Demo Mode";
            int sIndex = 0, maxcout = spawndemoPrefab.Count;
            while (sIndex < maxcout)
            {
                Transform fR = CloundRoom.getCurrRoomCloud().transform;
                Vector3 vLoc = new Vector3(0.0f + demofirstSpawnOffset + sIndex * demoSpawnStep, 0.0f, 0.0f);
                Vector3 vWorldPos = fR.TransformPoint(vLoc);
                Quaternion vWorldRot = fR.rotation;
                CloundRoom.SpwanMode(spawndemoPrefab[sIndex], vWorldPos, vWorldRot, mWorldScale);
                yield return new WaitForSeconds(0.2f);
                sIndex++;
            }
            PlaceAncLogTxt.color = Color.green;
            PlaceAncLogTxt.text = "Load DemoMode Complety!";
            
        }
        yield return new WaitForEndOfFrame();
    }

}
