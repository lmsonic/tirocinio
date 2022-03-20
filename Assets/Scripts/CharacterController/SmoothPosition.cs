using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tirocinio
{
    public class SmoothPosition : MonoBehaviour
    {

        public enum UpdateType { Update, LateUpdate }
        public enum SmoothType { Lerp, SmoothDamp }
        public GameObject targetObject;
        public float lerpSpeed = 20f;
        public float smoothDampTime = 0.04f;
        public UpdateType updateType;
        public SmoothType smoothType;

        Vector3 lastFramePosition;

        private void Start()
        {
            lastFramePosition = transform.position;
        }

        private void Update()
        {
            if (updateType == UpdateType.LateUpdate) return;


        }

        private void LateUpdate()
        {
            if (updateType == UpdateType.Update) return;
        }

        Vector3 smoothDampVelocity;
        void Smooth()
        {
            Vector3 tmp;
            switch (smoothType)
            {
                case SmoothType.Lerp:
                    tmp = transform.position;
                    transform.position = Vector3.Lerp(lastFramePosition, transform.position, lerpSpeed);
                    lastFramePosition = tmp;
                    break;
                case SmoothType.SmoothDamp:
                    tmp = transform.position;
                    transform.position = Vector3.SmoothDamp(lastFramePosition, transform.position, ref smoothDampVelocity, smoothDampTime);
                    lastFramePosition = tmp;
                    break;

            }
        }





    }
}
