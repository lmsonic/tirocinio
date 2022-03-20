using UnityEngine;
using UnityEditor;

namespace Tirocinio
{
    [CustomEditor(typeof(Mover))]
    public class MoverEditor : Editor
    {
        SerializedProperty stepHeightRatio;
        SerializedProperty colliderHeight;
        SerializedProperty colliderThickness;
        SerializedProperty colliderOffset;
        SerializedProperty sensorType;
        SerializedProperty isInDebugMode;
        SerializedProperty sensorArrayRows;
        SerializedProperty sensorArrayRayCount;
        SerializedProperty sensorArrayRowsAreOffset;
        SerializedProperty capsuleMesh;
        CapsuleCollider collider;

        private void OnEnable()
        {

            stepHeightRatio = serializedObject.FindProperty("stepHeightRatio");

            colliderHeight = serializedObject.FindProperty("colliderHeight");
            colliderThickness = serializedObject.FindProperty("colliderThickness");
            colliderOffset = serializedObject.FindProperty("colliderOffset");

            sensorType = serializedObject.FindProperty("sensorType");
            isInDebugMode = serializedObject.FindProperty("isInDebugMode");

            sensorArrayRows = serializedObject.FindProperty("sensorArrayRows");
            sensorArrayRayCount = serializedObject.FindProperty("sensorArrayRayCount");
            sensorArrayRowsAreOffset = serializedObject.FindProperty("sensorArrayRowsAreOffset");

            capsuleMesh = serializedObject.FindProperty("capsuleMesh");

            collider = (target as MonoBehaviour).GetComponent<CapsuleCollider>();
        }
        public override void OnInspectorGUI()
        {

            serializedObject.Update();
            EditorGUILayout.PropertyField(stepHeightRatio);


            EditorGUILayout.LabelField("Collider Options", EditorStyles.boldLabel);
            colliderHeight.floatValue = EditorGUILayout.FloatField("Collider Height", colliderHeight.floatValue);
            colliderThickness.floatValue = EditorGUILayout.FloatField("Collider Thickness", colliderThickness.floatValue);
            colliderOffset.vector3Value = EditorGUILayout.Vector3Field("Collider Offset", colliderOffset.vector3Value);

            EditorGUILayout.PropertyField(sensorType);
            EditorGUILayout.PropertyField(isInDebugMode);

            EditorGUILayout.PropertyField(sensorArrayRows);
            EditorGUILayout.PropertyField(sensorArrayRayCount);
            EditorGUILayout.PropertyField(sensorArrayRowsAreOffset);

            EditorGUILayout.PropertyField(capsuleMesh);

            collider.height = colliderHeight.floatValue;
            collider.radius = colliderThickness.floatValue;
            collider.center = colliderOffset.vector3Value;

            serializedObject.ApplyModifiedProperties();
        }
    }
}