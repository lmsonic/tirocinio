using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tirocinio
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerShooting : MonoBehaviour
    {
        public GameObject seedBombPrefab;

        public Transform shootingOrigin;

        public float forceMultiplier = 10f;

        PlayerInput playerInput;

        ObjectPool<PoolObject> seedBombPool;

        public float shootTime = 0.5f;
        float timer = 0f;

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            seedBombPool = new ObjectPool<PoolObject>(seedBombPrefab, 10);
        }

        private void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (playerInput.IsShooting && timer > shootTime)
            {
                Shoot();
                timer = 0f;
            }
        }



        void Shoot()
        {

            GameObject go = seedBombPool.PullGameObject(shootingOrigin.position);
            Rigidbody rb = go.GetComponent<Rigidbody>();

            Vector3 force = (transform.forward + transform.up).normalized * forceMultiplier;
            rb.AddForce(force, ForceMode.VelocityChange);

        }
    }
}
