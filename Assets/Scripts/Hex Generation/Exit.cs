using System;

using UnityEngine;
using UnityEngine.Events;

namespace Tirocinio
{

    public class Exit : PoolObject
    {
        public Hex hex1, hex2;

        public Renderer wallRenderer;

        public Collider wallCollider;

        public Renderer roadRenderer;

        public Collider roadCollider;

        bool isOpen = false;

        public void Open()
        {//when open, the exit is invisible and the collider becomes a trigger
            isOpen = true;

            if (wallRenderer)
                wallRenderer.enabled = false;
            if (wallCollider)
                wallCollider.enabled = false;

            if (roadRenderer)
                roadRenderer.enabled = true;
            if (roadCollider)
                roadCollider.enabled = true;
        }

        public void Close()
        {//when closed, the exit wall is visible and the collider blocks the player
            isOpen = false;

            if (wallRenderer)
                wallRenderer.enabled = true;
            if (wallCollider)
                wallCollider.enabled = true;

            if (roadRenderer)
                roadRenderer.enabled = false;
            if (roadCollider)
                roadCollider.enabled = false;

        }

        public void Init(Hex h1, Hex h2)
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

        private void OnDrawGizmos()
        {
            if (isOpen)
            {
                //Debug lines for open exit connections
                Debug.DrawLine(hex1.transform.position, hex2.transform.position, Color.magenta);
            }
        }







    }
}