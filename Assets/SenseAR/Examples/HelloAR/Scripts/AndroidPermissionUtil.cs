using UnityEngine;
using System.Collections;
using UnityEngine.Android;


public class AndroidPermissionUtil : MonoBehaviour
{
    public bool cameraPermission = false;
#if PLATFORM_ANDROID
    // Use this for initialization
    void Start()
    {
        //检测是否有Camera的权限
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            cameraPermission = true;
        }

    }
#endif
}
