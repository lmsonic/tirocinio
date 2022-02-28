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

        bool isOpen = false;

        public void SetColor(Color color) => rend.material.color = color;


        public void Open()
        {//when open, the exit is invisible and the collider becomes a trigger
            isOpen=true;
            rend.enabled = false;
            coll.isTrigger = true;
            coll.enabled = false;
        }

        public void Close()
        {//when closed, the exit wall is visible and the collider blocks the player
            isOpen=false;
            rend.enabled = true;
            coll.isTrigger = false;
            coll.enabled = true;
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

        private void OnDrawGizmos() {
            if (isOpen){
                //Debug lines for open exit connections
                Debug.DrawLine(hex1.transform.position,hex2.transform.position,Color.magenta);
            }
        }







    }
}