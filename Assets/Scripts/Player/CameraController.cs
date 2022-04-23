using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tirocinio
{
    public class CameraController : MonoBehaviour
    {
        public Transform cameraTransform;
        Vector3 baseCameraOffset;
        [Range(0f, 30f)]
        public float cameraDistance = 5f;

        PlayerInput playerInput;

        public float minPitch = -60f, maxPitch = 60f;

        float pitch, yaw;

        public float rotationSpeed;




        private void Start()
        {
            playerInput = GetComponent<PlayerInput>();
            baseCameraOffset = cameraTransform.localPosition;
        }

        private void Update()
        {
            //pitch is between -180 and 180
            pitch = (pitch % 360 + 180) % 360 - 180;

            yaw += rotationSpeed * playerInput.LookInput.x * Time.deltaTime;
            pitch -= rotationSpeed * playerInput.LookInput.y * Time.deltaTime;

            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            cameraTransform.rotation = Quaternion.Euler(pitch, yaw, 0f);

            cameraTransform.position = transform.position + transform.rotation * baseCameraOffset - cameraTransform.forward * cameraDistance;
        }




    }
}
