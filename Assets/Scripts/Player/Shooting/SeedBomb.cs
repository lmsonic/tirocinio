using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Tirocinio
{
    public class SeedBomb : PoolObject
    {

        public LayerMask paintableTerrain;
        public float timeToDisappear = 5f;
        Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }
        private void OnEnable()
        {
            Invoke("Disable", timeToDisappear);
            rb.velocity = Vector3.zero;
        }

        void Disable()
        {
            gameObject.SetActive(false);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (paintableTerrain.Contains(other.gameObject.layer))
            {
                Locator.Instance.GrassPainter.AddGrass(other.GetContact(0).point, other.GetContact(0).normal);

            }
            gameObject.SetActive(false);
        }


    }
}
