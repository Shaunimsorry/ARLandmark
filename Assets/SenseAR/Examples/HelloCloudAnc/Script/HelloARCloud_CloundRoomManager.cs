using SenseAR;
using SenseARInternal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SenseAR_Demo
{

    public class HelloARCloud_CloundRoomManager : MonoBehaviour
    {
        public string cloud_key = "580114249204891648";
        public string cloud_sto = "90061f8a2162424b8d789d5cb912c585";
        public string AncTag = "ARObj";
        public int Layer = 10;
        public GameObject prefabAncPrefab;
        public bool bAutoAttachToCloudAnc = true;



        public delegate void CloudRoomEvent(int id);
        public delegate void RoomCloudAncEvent(int id,string error);

        private CloudRoomEvent CreateRoomSuccessEvent;
        private CloudRoomEvent EnterRoomSuccessEvent;
        private RoomCloudAncEvent regedistRoomCloudAncSuccessEvent;
        private RoomCloudAncEvent regedistRoomCloudAncFailureEvent;

        private RoomCloudAncEvent LoadRoomCloudAncSuccessEvent;
        private RoomCloudAncEvent LoadRoomCloudAncFailureEvent;

        /// <summary>
        /// is Room Factory
        /// </summary>
        private bool isRoommer = false;
        public int   CloudRoomID { get; private set; }
        private SenseAR_CloudAnchorProxy currRoomCloudAnc;
        public SenseAR_CloudAnchorProxy getCurrRoomCloud()
        {
            return currRoomCloudAnc;
        }


        private List<GameObject> currControlAncEntityList = new List<GameObject>();

        public bool IsRoommer()
        {
            return isRoommer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pSuccessCall"> Create Over CallBack</param>
        public void createRoom(CloudRoomEvent pSuccessCall)
        {
            Session.SetKeyAndSecret(cloud_key, cloud_sto);
            CreateRoomSuccessEvent = null;
            CreateRoomSuccessEvent += pSuccessCall;
            CloudRoomID = ARCloudClient.createRoom().Value;
            isRoommer = true;
            NotifyCloudRoomCreateSuccess();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pSuccessCall"> Enter Over CallBack</param>
        /// <param name="pRoomID"></param>
        /// <param name="isAutoSyncCloud"></param>
        public void EnterRoom(CloudRoomEvent pSuccessCall, int pRoomID)
        {
            Session.SetKeyAndSecret(cloud_key, cloud_sto);
            EnterRoomSuccessEvent = null;
            EnterRoomSuccessEvent += pSuccessCall;
            CloudRoomID = pRoomID;
            isRoommer = false;
            NotifyCloudRoomEnterSuccess();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pEntityPrefab"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        public void SpwanMode(GameObject pEntityPrefab, Vector3 pos, Quaternion rot)
        {
            GameObject m_ModelObj = MonoBehaviour.Instantiate(pEntityPrefab, pos, rot);
            AddOneMode(m_ModelObj);
        }

        public void SpwanMode(GameObject pEntityPrefab, Vector3 pos, Quaternion rot,Vector3 worldScale)
        {
            GameObject m_ModelObj = MonoBehaviour.Instantiate(pEntityPrefab, pos, rot);
            m_ModelObj.transform.localScale = worldScale;
            AddOneMode(m_ModelObj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m_ModelObj"></param>
        public void AddOneMode(GameObject m_ModelObj)
        {
            //Chahe World, then Transform Parent, then Reseve
            GameObject m_AnchorObj = new GameObject("anchor");
            Vector3 vWorld = m_ModelObj.transform.position;
            Quaternion vWorldRot = m_ModelObj.transform.rotation;

            m_ModelObj.transform.parent = m_AnchorObj.transform;
            m_ModelObj.transform.localPosition = Vector3.zero;
            m_ModelObj.transform.localRotation = Quaternion.identity;

            m_ModelObj.tag = AncTag;
            m_ModelObj.layer = Layer;

            m_AnchorObj.transform.position = vWorld;
            m_AnchorObj.transform.rotation = vWorldRot;

            if (bAutoAttachToCloudAnc)
            {
                currControlAncEntityList.Add(m_ModelObj);
                m_AnchorObj.transform.parent = currRoomCloudAnc.transform;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAnchor"></param>
        public void RegisterRoomCloudAnc(Anchor pAnchor, RoomCloudAncEvent pSuccessCall, RoomCloudAncEvent pFailureCall)
        {
            regedistRoomCloudAncFailureEvent = pFailureCall;
            regedistRoomCloudAncSuccessEvent = pSuccessCall;

            if (currRoomCloudAnc == null)
            {
                GameObject vAncObj = GameObject.Instantiate<GameObject>(prefabAncPrefab);
                currRoomCloudAnc = vAncObj.GetComponent<SenseAR_CloudAnchorProxy>();
            }
            if (currRoomCloudAnc.isCanCreatFromClient())
            {
                currRoomCloudAnc.ReBindCloudCreateSuccessEvent(RegisterRoomCloudOver);
                currRoomCloudAnc.ReBindCloudCreateFailureEvent(RegisterRoomCloudOver);
                currRoomCloudAnc.CreatFromClient(pAnchor);
            }
            else
            {
                NotifyRegisterRoomCloudFailure();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAnchor"></param>
        public void LoadRoomCloudAnc(RoomCloudAncEvent pSuccessCall, RoomCloudAncEvent pFailureCall)
        {
            LoadRoomCloudAncSuccessEvent = pSuccessCall;
            LoadRoomCloudAncFailureEvent = pFailureCall;
            if (currRoomCloudAnc == null)
            {
                GameObject vAncObj = GameObject.Instantiate<GameObject>(prefabAncPrefab);
                currRoomCloudAnc = vAncObj.GetComponent<SenseAR_CloudAnchorProxy>();
            }
            if (currRoomCloudAnc.isCanCreatFromClient())
            {
                currRoomCloudAnc.ReBindSyncFromCloudSuccessEvent(LoadRoomCloudAncOver);
                currRoomCloudAnc.ReBindSyncFromCloudFailureEvent(LoadRoomCloudAncOver);
                RetStr vAncContentCloud = ARCloudClient.enterRoom(CloudRoomID);
                currRoomCloudAnc.ReaderFormCloud(vAncContentCloud);
            }
            else
            {
                NotifyLoadRoomCloudAncFailure();
            }
        }

        public void ResetALL()
        {
            if(currRoomCloudAnc != null)
            {
                currRoomCloudAnc.clear();
                GameObject.Destroy(currRoomCloudAnc);
            }
            regedistRoomCloudAncSuccessEvent = null;
            regedistRoomCloudAncFailureEvent = null;
            LoadRoomCloudAncSuccessEvent = null;
            LoadRoomCloudAncFailureEvent = null;
            isRoommer = false;
            CloudRoomID = -1;
        }

        internal void SpwanMode(GameObject gameObject, Vector3 vWorldPos, Quaternion vWorldRot, object mWorldScale)
        {
            throw new NotImplementedException();
        }

        private void NotifyCloudRoomCreateSuccess()
        {
            if(CreateRoomSuccessEvent != null)
                CreateRoomSuccessEvent.Invoke(CloudRoomID);
        }

        private void NotifyCloudRoomEnterSuccess()
        {
            if(EnterRoomSuccessEvent != null)
                EnterRoomSuccessEvent.Invoke(CloudRoomID);
        }

        private void RegisterRoomCloudOver()
        {
            if(currRoomCloudAnc.IsSetupCloudPoint())
            {
                ARCloudClient.saveAnchorId(CloudRoomID, currRoomCloudAnc.GetAncContentStr());
                NotifyRegisterRoomCloudSuccess();
            }
            else
            {
                NotifyRegisterRoomCloudFailure();
            }
        }

        private void NotifyRegisterRoomCloudSuccess()
        {
            if(regedistRoomCloudAncSuccessEvent != null)
                regedistRoomCloudAncSuccessEvent.Invoke(CloudRoomID, "succ");
        }

        private void NotifyRegisterRoomCloudFailure()
        {
            if(regedistRoomCloudAncFailureEvent != null)
                regedistRoomCloudAncFailureEvent.Invoke(CloudRoomID, "err-0");
        }

      

        private void LoadRoomCloudAncOver()
        {
            if (currRoomCloudAnc.IsSetupCloudPoint())
            {
                NotifLoadRoomCloudAncSuccess();
            }
            else
            {
                NotifyLoadRoomCloudAncFailure();
            }
        }

        private void NotifLoadRoomCloudAncSuccess()
        {
            if(LoadRoomCloudAncSuccessEvent != null)
                LoadRoomCloudAncSuccessEvent.Invoke(CloudRoomID,"success");
        }

        private void NotifyLoadRoomCloudAncFailure()
        {
            if(LoadRoomCloudAncFailureEvent != null)
                LoadRoomCloudAncFailureEvent.Invoke(CloudRoomID,"err - 0");
        }

        internal bool isCanAddMode()
        {
            return currRoomCloudAnc != null && currRoomCloudAnc.IsSetupCloudPoint();
        }
    }
}
