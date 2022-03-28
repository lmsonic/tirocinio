using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Tirocinio
{
    public class SeedBomb : PoolObject
    {
        public GameObject grassDecal;
        public float timeToDisappear = 10f;
        Rigidbody rb;

        private void Awake() {
            rb = GetComponent<Rigidbody>();
        }   
        private void OnEnable() {
            Invoke("Disable",timeToDisappear);
            rb.velocity = Vector3.zero;
        }




        void Disable(){
            gameObject.SetActive(false);
        }

        private void OnCollisionEnter(Collision other) {
            if (other.gameObject.CompareTag("Dirt")){
                Instantiate(grassDecal,other.GetContact(0).point + Vector3.up,grassDecal.transform.rotation);
                Disable();
            }
        }

        
    }
}
