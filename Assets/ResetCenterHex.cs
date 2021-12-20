using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Tirocinio
{
    public class ResetCenterHex : MonoBehaviour
    {
        // Start is called before the first frame update
        public Hex hex;

        public event Action<Hex> PlayerNotOnCenterHex;
        private void Start() {
            PlayerNotOnCenterHex+=Locator.Instance.Chunk.PlayerNotOnCenterHex;
        }

        private void OnTriggerEnter(Collider other) {
            if (other.tag == "Player")
            {
                if (hex.hexPosition != HexPosition.CENTER)
                {
                    PlayerNotOnCenterHex.Invoke(hex);
                }
            }
        }
    }
}
