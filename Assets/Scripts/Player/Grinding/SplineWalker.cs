using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Tirocinio
{
    public class SplineWalker : MonoBehaviour
    {
        public enum SplineWalkerMode
        {
            Once,
            Loop,
            PingPong
        }
        public BezierSpline spline;
        public float duration;
        private float progress;

        public bool lookForward;

        public SplineWalkerMode mode;
        private bool goingForward;

        private void Update()
        {
            if (goingForward)
            {
                progress += Time.deltaTime / duration;
                if (progress > 1f)
                {
                    switch (mode)
                    {
                        case SplineWalkerMode.Once:
                            progress = 1f; break;
                        case SplineWalkerMode.Loop:
                            progress -= 1f; break;
                        case SplineWalkerMode.PingPong:
                            progress = 2f - progress;
                            goingForward = false;
                            break;
                    }

                }

            }
            else
            {
                progress -= Time.deltaTime / duration;
                if (progress < 0f)
                {
                    progress = -progress;
                    goingForward = true;

                }
            }

            Vector3 position = spline.GetPoint(progress);
            transform.localPosition = position;
            if (lookForward)
            {
                transform.LookAt(position + spline.GetDirection(progress));
            }
        }
    }
}
