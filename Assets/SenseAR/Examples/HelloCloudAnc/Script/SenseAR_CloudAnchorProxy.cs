using UnityEngine;
using System.Collections;
using SenseAR;
using SenseARInternal;
using System;

/// <summary>
/// this is one Cloud Anc Proxy
/// Auto Save and Sync with Cloud
/// </summary>
public class SenseAR_CloudAnchorProxy : MonoBehaviour
{
    public delegate void CloudAncProxyEvent();
    private CloudAncProxyEvent AncCreateSuccessEvent;
    private CloudAncProxyEvent AncCreateFailureEvent;

    private CloudAncProxyEvent AncSyncFromCloudSuccessEvent;
    private CloudAncProxyEvent AncSyncFromCloudFailureEvent;

    private Transform transformCache;
    private bool isInHostRoomIng = false;
    private bool isInResolveRoomIng = false;
    private bool mIsSetupCloudPoint = false;
    public bool IsSetupCloudPoint() { return mIsSetupCloudPoint; }
    private Anchor mCurrRootCloudAncho;

    public TextMesh AncInfo;
    public GameObject AncMesh;

    private IntPtr m_NativeAncID;
    private string mAncContent;

    public string GetAncContentStr()
    {
        return mAncContent;
    }
    // Use this for initialization
    public void ReBindCloudCreateSuccessEvent(CloudAncProxyEvent pCall)
    {
        AncCreateSuccessEvent = null;
        AncCreateSuccessEvent += pCall;
    }

    public void ReBindCloudCreateFailureEvent(CloudAncProxyEvent pCall)
    {
        AncCreateFailureEvent = null;
        AncCreateFailureEvent += pCall;
    }

    public void ReBindSyncFromCloudSuccessEvent(CloudAncProxyEvent pCall)
    {
        AncSyncFromCloudSuccessEvent = null;
        AncSyncFromCloudSuccessEvent += pCall;
    }

    public void ReBindSyncFromCloudFailureEvent(CloudAncProxyEvent pCall)
    {
        AncSyncFromCloudFailureEvent = null;
        AncSyncFromCloudFailureEvent += pCall;
    }

    public bool isCanCreatFromClient()
    {
        return !mIsSetupCloudPoint && !isInHostRoomIng;
    }

    public void CreatFromClient(Anchor pNactiveAnc)
    {
        //Loca Anchor to Show
        this.transform.parent = pNactiveAnc.transform;
        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.identity;
        AncInfo.gameObject.SetActive(true);
        AncMesh.gameObject.SetActive(true);

        m_NativeAncID = pNactiveAnc.m_AnchorNativeHandle;
        if (mIsSetupCloudPoint || isInHostRoomIng)
        {
            return;
        }
        StopCoroutine(AsyncRegisterCloudAnc());
        StartCoroutine(AsyncRegisterCloudAnc());
    }

    public void ReaderFormCloud(RetStr pCloudAncContent)
    {
        mAncContent = pCloudAncContent.Value;
        if (mIsSetupCloudPoint || isInHostRoomIng)
        {
            return;
        }
        StopCoroutine(AsncResloveCloudAnc());
        StartCoroutine(AsncResloveCloudAnc());
    }

    public void clear()
    {
        StopCoroutine(AsyncRegisterCloudAnc());
        StopCoroutine(AsncResloveCloudAnc());
        AncInfo = null;
        AncMesh = null;
        AncCreateSuccessEvent = null;
        AncCreateFailureEvent = null;
        AncSyncFromCloudSuccessEvent = null;
        AncSyncFromCloudFailureEvent = null;
    }

    private void Awake()
    {
        transformCache = transform;
    }


    private IEnumerator AsyncRegisterCloudAnc()
    {
        isInHostRoomIng = true;
        Anchor vTempAnc = Session.HostAndAcquireNewCloudAnchor(m_NativeAncID);
        //if (vTempAnc.m_AnchorNativeHandle == IntPtr.Zero)
            //NotifyAncCreateFailure();


        while (Session.GetCloudAnchorState(vTempAnc.m_AnchorNativeHandle) == ApiCloudAnchorState.TaskInProgress)
        {
            //Debug.Log("justsoso hosting");
            yield return new WaitForEndOfFrame();
        }
        ApiCloudAnchorState state = Session.GetCloudAnchorState(vTempAnc.m_AnchorNativeHandle);
        Debug.Log("justsoso host result" + state);
        if (state == ApiCloudAnchorState.Success)
        {
            mAncContent = Session.AcquireCloudAnchorId(vTempAnc.m_AnchorNativeHandle);
            AncInfo.gameObject.SetActive(true);
            AncMesh.gameObject.SetActive(true);
            RefreshAnc(vTempAnc);
            NotifyAncCreateSuccess();

        }
        else
        {
            NotifyAncCreateFailure();
        }
        yield return new WaitForEndOfFrame();
        isInHostRoomIng = false;
    }

    private IEnumerator AsncResloveCloudAnc()
    {
        isInResolveRoomIng = true;
        Anchor vTempAnc = Session.ResolveAndAcquireNewCloudAnchor(mAncContent);
        while (Session.GetCloudAnchorState(vTempAnc.m_AnchorNativeHandle) == ApiCloudAnchorState.TaskInProgress)
        {
            yield return new WaitForEndOfFrame();
        }
        if (Session.GetCloudAnchorState(vTempAnc.m_AnchorNativeHandle) == ApiCloudAnchorState.Success)
        {
            AncInfo.gameObject.SetActive(true);
            AncMesh.gameObject.SetActive(true);
            RefreshAnc(vTempAnc);
            NotifyAncSyncCloudSuccess();
        }
        else
        {
            NotifyAncSyncCloudFailure();
        }
        yield return new WaitForEndOfFrame();
        isInResolveRoomIng = false;
    }

    private void RefreshAnc(Anchor vTempAnc)
    {
        if (vTempAnc == null || vTempAnc.m_AnchorNativeHandle == IntPtr.Zero)
        {
            return;
        }
        mCurrRootCloudAncho = vTempAnc;
        transformCache.parent = vTempAnc.transform;
        transformCache.localPosition = Vector3.zero;
        transformCache.localRotation = Quaternion.identity;
        transformCache.localScale = Vector3.one;
        mIsSetupCloudPoint = true;
    }


    private void NotifyAncCreateSuccess()
    {
        if(AncCreateSuccessEvent != null)
            AncCreateSuccessEvent.Invoke();
    }

    private void NotifyAncCreateFailure()
    {
        if(AncCreateFailureEvent != null)
            AncCreateFailureEvent.Invoke();
    }

    private void NotifyAncSyncCloudSuccess()
    {
        if(AncSyncFromCloudSuccessEvent != null)
            AncSyncFromCloudSuccessEvent.Invoke();
    }

    private void NotifyAncSyncCloudFailure()
    {
        if(AncSyncFromCloudFailureEvent != null)
            AncSyncFromCloudFailureEvent.Invoke();
    }


    private void Update()
    {
        if (mIsSetupCloudPoint)
        {
            AncInfo.text = string.Format("cloAch pos = {0}", transformCache.position);
        }
    }


}
