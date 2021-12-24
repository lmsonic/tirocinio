using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Tirocinio
{
    public class ResetCenterChunk : MonoBehaviour
    {
        // Start is called before the first frame update
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
