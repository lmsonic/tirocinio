using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tirocinio
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class Mover : MonoBehaviour
    {
        public enum SensorType{ Raycast,Spherecast,RaycastArray}
        [Header("Mover Options")]
        [Range(0,1)]
        public float stepHeightRatio=0.25f;
        [Header("Collider Options")]
        [SerializeField]
        public float colliderHeight = 2f;
        public float colliderThickness = 1f;
        public Vector3 colliderOffset = new Vector3(0f,0.5f,0f);
        [Header("Sensor Options")]
        public SensorType sensorType = SensorType.Raycast; 
        public bool isInDebugMode = false;
        [Header("Sensor Array Options")]
        [Range(1,5)]
        public int sensorArrayRows;
        [Range(1,15)]
        public int sensorArrayRayCount;
        public bool sensorArrayRowsAreOffset;

        private CapsuleCollider _collider;
        private Rigidbody _rigidbody;



        
    }
}
