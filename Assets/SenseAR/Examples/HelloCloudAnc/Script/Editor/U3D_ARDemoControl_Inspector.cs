using UnityEngine;
using UnityEditor;
using SenseAR_Demo;

[CustomEditor(typeof(HelloARCloud_AnchorController))]
public class U3D_ARDemoControl_Inspector : Editor
{
    private HelloARCloud_AnchorController mtarget;
    private void OnEnable()
    {
        mtarget = target as HelloARCloud_AnchorController;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginVertical();

        GUILayout.BeginVertical("Box");
        GUILayout.Label("--------Control Config Prefab-------------");
        mtarget.SenseARBasePrefab = (GameObject)EditorGUILayout.ObjectField("ARBasePrefab",mtarget.SenseARBasePrefab,typeof(GameObject), false);
        mtarget.CloundRoom = mtarget.GetComponent<HelloARCloud_CloundRoomManager>();
        mtarget.CloundRoom = (HelloARCloud_CloundRoomManager)EditorGUILayout.ObjectField("CloudAnc", mtarget.CloundRoom, typeof(HelloARCloud_CloundRoomManager), true);
        GUILayout.EndVertical();

        GUILayout.Space(5.0f);

        GUILayout.BeginVertical("Box");
        GUILayout.Label("--------Cloud Ancho-------------");
        mtarget.bEnableUseClounAnc = EditorGUILayout.Toggle("Enable ClounAnc", mtarget.bEnableUseClounAnc);
        GUILayout.EndVertical();

        GUILayout.Space(5.0f);

        GUILayout.BeginVertical("Box");
        GUILayout.Label("--------Test-------------");
      
        if(GUILayout.Button("-------Play-------"))
        {
            if(Application.isPlaying)
            {
                
            }
        }
        GUILayout.EndVertical();
        GUILayout.EndVertical();


        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}