using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tirocinio
{
    public class Hex : MonoBehaviour
    {

        public Dictionary<ExitDirection, Exit> exits = new Dictionary<ExitDirection, Exit>();

        


        public Renderer rend;


        public void SetColor(Color color) => rend.material.color = color;

        public Color GetColor() {return rend.material.color;}

        

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


        




    }

}
