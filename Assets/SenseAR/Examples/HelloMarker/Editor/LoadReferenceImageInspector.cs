using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(HelloMarkerController))]
public class LoadReferenceImInspector : Editor
{
    private SerializedObject obj;
    private HelloMarkerController markerController;
    private SerializedProperty type;
    private SerializedProperty textures;
    private SerializedProperty config;
    private SerializedProperty patts;
    private SerializedProperty path;
    private SerializedProperty SenseARBasePrefab;
    private SerializedProperty AndyAndroidPrefab;
    private SerializedProperty AxisPrefab;
    private SerializedProperty InfoDisplay;
    private SerializedProperty shotButton;

    void OnEnable()
    {
        obj = new SerializedObject(target);
        type = obj.FindProperty("type");
        textures = obj.FindProperty("textures");
        config = obj.FindProperty("config");
        patts = obj.FindProperty("patts");
        path = obj.FindProperty("path");
        SenseARBasePrefab = obj.FindProperty("SenseARBasePrefab");
        AndyAndroidPrefab = obj.FindProperty("AndyAndroidPrefab");
        AxisPrefab = obj.FindProperty("AxisPrefab");
        InfoDisplay = obj.FindProperty("InfoDisplay");
        shotButton = obj.FindProperty("shotButton");
    }
    public override void OnInspectorGUI()
    {
        obj.Update();
        EditorGUI.BeginChangeCheck();

        markerController = (HelloMarkerController)target;
        EditorGUILayout.PropertyField(SenseARBasePrefab);
        EditorGUILayout.PropertyField(AndyAndroidPrefab);
        EditorGUILayout.PropertyField(AxisPrefab);
        EditorGUILayout.PropertyField(InfoDisplay);
        EditorGUILayout.PropertyField(shotButton);

        markerController.type = (HelloMarkerController.ReferenceImageType)EditorGUILayout.EnumPopup("ReferecenImageType", markerController.type);

        if (markerController.type == HelloMarkerController.ReferenceImageType.Database)
        {
            EditorGUILayout.PropertyField(path);
        }
        else if(markerController.type == HelloMarkerController.ReferenceImageType.Patt)
        {
            EditorGUILayout.PropertyField(config);
            EditorGUILayout.PropertyField(patts, true);
        }
        else
        {
            EditorGUILayout.PropertyField(textures, true);
        }

        if (EditorGUI.EndChangeCheck())
        {
            obj.ApplyModifiedProperties();
        }
    }
}  