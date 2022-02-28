using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tirocinio
{
    public class Hex : MonoBehaviour
    {

        public Exit[] exits = new Exit[6];
        public Hex[] neighbours = new Hex[6];
        public Renderer rend;


        public void SetColor(Color color) => rend.material.color = color;

        public Color GetColor() {return rend.material.color;}

        

        public void ClearExits()
        {
            for (int i = 0; i < 6; i++)
            {
                Exit exit = exits[i];
                Hex otherHex = exit.GetOtherHex(this);
                ExitDirection oppositeDirection = HelperEnums.GetOppositeDirection((ExitDirection)i);
                otherHex.exits[(int)oppositeDirection]= null;
                exit.gameObject.SetActive(false);
                exits[i] = null;
            }
            
        }

        // private void OnDrawGizmos() {
        //     for (int i = 0; i < neighbours.Length; i++)
        //     {
        //         if (neighbours[i])
        //             Debug.DrawLine(transform.position,neighbours[i].transform.position,Color.green);
        //     }

        // }




        




    }

}
