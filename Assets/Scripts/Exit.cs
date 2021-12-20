using System;

using UnityEngine;
using UnityEngine.Events;
namespace Tirocinio
{
    public class Exit : MonoBehaviour
    {
        public Hex hex1, hex2;

        public Renderer rend;

        public Collider coll;

        public event Action<Exit> OnExitEntered;

        private void Start()
        {
            OnExitEntered += Locator.Instance.Chunk.MoveCenterFromExit;
        }
        public void Open()
        {
            rend.enabled = false;
            coll.isTrigger = true;
        }

        public void Close()
        {
            rend.enabled = true;
            coll.isTrigger = false;
        }
        public void Initialize(Hex h1, Hex h2)
        {
            hex1 = h1;
            hex2 = h2;
        }

        public Hex GetOtherHex(Hex hex)
        {
            if (hex != hex1 && hex != hex2)
            {
                Debug.Log("Trying to get an hex from an hex not adjacent");
                return null;
            }

            return hex1 == hex ? hex2 : hex1;

        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
                OnExitEntered.Invoke(this);
        }




    }
}