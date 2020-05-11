using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SenseAR;
using SenseARInternal;
[CustomEditor(typeof(ARCoreSessionConfig))]
public class LoadARSessionConfig : Editor
{
    private SerializedObject obj;
    private ARCoreSessionConfig config;
    private SerializedProperty MatchCameraFramerate;
    private SerializedProperty SLAMAlgorithmMode;
    private SerializedProperty SLAMStreamMode;
    private SerializedProperty HandGestureAlgorithmMode;
    private SerializedProperty HandGestureStreamMode;
    private SerializedProperty ImageTrackingAlgorithmMode;
    private SerializedProperty ImageTrackingStreamMode;
    private SerializedProperty PlaneFindingAlgorithmMode;
    private SerializedProperty PlaneFinding;
    private SerializedProperty PlaneFindingStreamMode;
    private SerializedProperty LightEstimationAlgorithmMode;
    private SerializedProperty LightEstimation;
    private SerializedProperty LightEstimationStreamMode;
    private SerializedProperty CloudAnchorAlgorithmMode;
    private SerializedProperty CloudAnchorStreamMode;

    void OnEnable()
    {
        obj = new SerializedObject(target);
        MatchCameraFramerate = obj.FindProperty("MatchCameraFramerate");
        SLAMAlgorithmMode = obj.FindProperty("SLAMAlgorithmMode");
        SLAMStreamMode = obj.FindProperty("SLAMStreamMode");
        HandGestureAlgorithmMode = obj.FindProperty("HandGestureAlgorithmMode");
        HandGestureStreamMode = obj.FindProperty("HandGestureStreamMode");
        ImageTrackingAlgorithmMode = obj.FindProperty("ImageTrackingAlgorithmMode");
        ImageTrackingStreamMode = obj.FindProperty("ImageTrackingStreamMode");
        PlaneFindingAlgorithmMode = obj.FindProperty("PlaneFindingAlgorithmMode");
        PlaneFinding = obj.FindProperty("PlaneFinding");
        PlaneFindingStreamMode = obj.FindProperty("PlaneFindingStreamMode");
        LightEstimationAlgorithmMode = obj.FindProperty("LightEstimationAlgorithmMode");
        LightEstimation = obj.FindProperty("LightEstimation");
        LightEstimationStreamMode = obj.FindProperty("LightEstimationStreamMode");
        CloudAnchorAlgorithmMode = obj.FindProperty("CloudAnchorAlgorithmMode");
        CloudAnchorStreamMode = obj.FindProperty("CloudAnchorStreamMode");
    }
    public override void OnInspectorGUI()
    {
        obj.Update();
        EditorGUI.BeginChangeCheck();

        config = (ARCoreSessionConfig)target;
        EditorGUILayout.PropertyField(MatchCameraFramerate);
        config.SLAMAlgorithmMode = (ApiAlgorithmMode)EditorGUILayout.EnumPopup("SLAMAlgorithmMode", config.SLAMAlgorithmMode);
        if (config.SLAMAlgorithmMode == ApiAlgorithmMode.Enabled)
        {
            EditorGUILayout.PropertyField(SLAMStreamMode);
        }

        config.HandGestureAlgorithmMode = (ApiAlgorithmMode)EditorGUILayout.EnumPopup("HandGestureAlgorithmMode", config.HandGestureAlgorithmMode);
        if (config.HandGestureAlgorithmMode == ApiAlgorithmMode.Enabled)
        {
            EditorGUILayout.PropertyField(HandGestureStreamMode);
        }

        config.ImageTrackingAlgorithmMode = (ApiAlgorithmMode)EditorGUILayout.EnumPopup("ImageTrackingAlgorithmMode", config.ImageTrackingAlgorithmMode);
        if (config.ImageTrackingAlgorithmMode == ApiAlgorithmMode.Enabled)
        {
            EditorGUILayout.PropertyField(ImageTrackingStreamMode);
        }

        config.PlaneFindingAlgorithmMode = (ApiAlgorithmMode)EditorGUILayout.EnumPopup("PlaneFindingAlgorithmMode", config.PlaneFindingAlgorithmMode);
        if (config.PlaneFindingAlgorithmMode == ApiAlgorithmMode.Enabled)
        {
            EditorGUILayout.PropertyField(PlaneFinding);
            EditorGUILayout.PropertyField(PlaneFindingStreamMode);
        }

        config.LightEstimationAlgorithmMode = (ApiAlgorithmMode)EditorGUILayout.EnumPopup("LightEstimationAlgorithmMode", config.LightEstimationAlgorithmMode);
        if (config.LightEstimationAlgorithmMode == ApiAlgorithmMode.Enabled)
        {
            EditorGUILayout.PropertyField(LightEstimation);
            EditorGUILayout.PropertyField(LightEstimationStreamMode);
        }

        config.CloudAnchorAlgorithmMode = (ApiAlgorithmMode)EditorGUILayout.EnumPopup("CloudAnchorAlgorithmMode", config.CloudAnchorAlgorithmMode);
        if (config.CloudAnchorAlgorithmMode == ApiAlgorithmMode.Enabled)
        {
            EditorGUILayout.PropertyField(CloudAnchorStreamMode);
        }

        if (EditorGUI.EndChangeCheck())
        {
            obj.ApplyModifiedProperties();
        }
    }
}
