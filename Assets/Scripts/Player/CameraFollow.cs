using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tirocinio
{
    public class CameraFollow : MonoBehaviour
    {

        public float lerpSpeed = 20f;

        private void Awake()
        {
            if (Camera.main == null) return;

            GameObject mainCamera = Camera.main.gameObject;

            mainCamera.transform.position = transform.position;

            mainCamera.transform.rotation = transform.rotation;

        }

        private void LateUpdate()
        {
            if (Camera.main == null) return;

            GameObject mainCamera = Camera.main.gameObject;

            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, transform.position, lerpSpeed * Time.deltaTime);


            mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, transform.rotation, lerpSpeed * Time.deltaTime);

        }
    }
}
