using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tirocinio
{
    public class PlayerShooting : MonoBehaviour
    {
        public GameObject seedBombPrefab;

        public Transform shootingOrigin;

        public float forceMultiplier = 10f;

        PlayerInput playerInput;

        ObjectPool<PoolObject> seedBombPool;

        private void OnEnable()
        {
            playerInput.Enable();
        }

        private void OnDisable()
        {
            playerInput.Disable();
        }

        private void Awake()
        {
            playerInput = new PlayerInput();

            playerInput.Player.Shoot.started += Shoot;

            seedBombPool = new ObjectPool<PoolObject>(seedBombPrefab, 10);
        }

        void Shoot(InputAction.CallbackContext ctx)
        {

            GameObject go = seedBombPool.PullGameObject(shootingOrigin.position);
            Rigidbody rb = go.GetComponent<Rigidbody>();

            Vector3 force = (transform.forward + transform.up).normalized * forceMultiplier;
            rb.AddForce(force, ForceMode.VelocityChange);


        }
    }
}
