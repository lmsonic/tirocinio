using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tirocinio
{
    public class CameraFollow : MonoBehaviour
    {
        // Start is called before the first frame update
        public Transform target;

        Vector3 offset;

        private void Start() {

            offset = transform.position - target.position;
            
        }
        private void LateUpdate() {
            Quaternion rotation =  Quaternion.Euler(0f,target.eulerAngles.y,0f);
            transform.position = target.position + rotation * offset;

            transform.LookAt(target);

        }
    }
}
