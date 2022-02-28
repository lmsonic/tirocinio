using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Tirocinio
{
    public class ResetCenterChunk : MonoBehaviour
    {

        //Class is used to generate new chunks when player collides with collider attached to this class
        public Hex hex;

        public event Action<Hex> PlayerNotOnCenterChunk;
        private void Start()
        {
            //PlayerNotOnCenterChunk += Locator.Instance.Chunk.MoveCenterHex;
        }

        // private void OnTriggerEnter(Collider other)
        // {
        //     if (other.CompareTag("Player"))
        //     {

        //         PlayerNotOnCenterChunk.Invoke(hex);
        //     }
        // }

    }
}
