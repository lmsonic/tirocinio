using UnityEngine;

namespace Tirocinio
{
    [RequireComponent(typeof(Mover))]
    public class Controller : MonoBehaviour
    {
        private Mover mover;
        private void Awake() {
            mover= GetComponent<Mover>();
        }

        private void FixedUpdate() {
            mover.CheckForGround();
        }

    }
}