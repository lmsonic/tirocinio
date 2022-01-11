using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Tirocinio
{
    public class ResetCenterChunk : MonoBehaviour
    {
        //Class is used to generate new chunks when player collides with collider attached to this class
        public Chunk chunk;

        public event Action<Chunk> PlayerNotOnCenterChunk;
        private void Start() {
            PlayerNotOnCenterChunk+=Locator.Instance.HexGeneration.MoveGridCenter;
        }

        private void OnTriggerStay(Collider other) {
            if (other.tag == "Player")
            {
                if (chunk.chunkPosition != ChunkPosition.CENTER)
                {
                    PlayerNotOnCenterChunk.Invoke(chunk);
                }
            }
        }
    }
}
