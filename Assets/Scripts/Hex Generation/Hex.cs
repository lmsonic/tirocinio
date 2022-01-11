using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tirocinio
{
    public class Hex : MonoBehaviour
    {

        public Dictionary<ExitDirection, Exit> exits = new Dictionary<ExitDirection, Exit>();


        static public float hexRadius = 8.75f;

        public HexPosition hexPosition;


        public void SetHexPosition(HexPosition pos) => hexPosition = pos;

        public Renderer rend;


        public void SetColor(Color color) => rend.material.color = color;

        public void AddExit(ExitDirection direction, Hex otherHex, bool isOpen)
        {
            //Sets the exit in the correct rotation, offset from hex, and sets it both in the hex that
            //spawns it and in the hex that is connecting to it, in their exit dictionaries
            Quaternion rotation = Quaternion.AngleAxis(-(int)direction * 60f, Vector3.up);
            Vector3 offset = rotation * Vector3.forward * hexRadius;

            GameObject exitGO = Locator.Instance.ObjectPooler.
                GetPooledExit(transform.position + offset, rotation, transform);

            Exit exit = exitGO.GetComponent<Exit>();
            exit.Initialize(this, otherHex);
            exit.SetColor(rend.material.color);

            if (isOpen) exit.Open(); else exit.Close();

            exits[direction] = exit;
            ExitDirection oppositeDirection = HelperEnums.GetOppositeDirection(direction);
            otherHex.exits[oppositeDirection] = exit;
        }

        public void ClearExits()
        {
            //Despawning every exit for an hex in the correct way
            foreach (KeyValuePair<ExitDirection, Exit> entry in exits)
            {
                Exit exit = entry.Value;
                Hex otherHex = exit.GetOtherHex(this);
                ExitDirection oppositeDirection = HelperEnums.GetOppositeDirection(entry.Key);
                otherHex.exits.Remove(oppositeDirection);
                exit.gameObject.SetActive(false);
            }
            exits.Clear();
        }

        private void OnDisable()
        {
            ClearExits();
        }




    }

}
