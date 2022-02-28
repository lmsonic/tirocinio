using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tirocinio
{
    public class Hex : MonoBehaviour
    {

        public List<Exit> exits;
        public Hex[] neighbours = new Hex[6];
        public Renderer rend;


        public void SetColor(Color color) => rend.material.color = color;

        public Color GetColor() {return rend.material.color;}

        

        public void ClearExits()
        {
            foreach (Exit exit in exits)
            {
                Hex otherHex = exit.GetOtherHex(this);
                otherHex.exits.Remove(exit);
                exit.gameObject.SetActive(false);
            }
            exits.Clear();
            
        }

        private void OnDrawGizmos() {
            for (int i = 0; i < neighbours.Length; i++)
            {
                if (neighbours[i]){
                    Vector3 direction = (neighbours[i].transform.position - transform.position);
                    Debug.DrawRay(transform.position,direction*0.3f,Color.black);
                }
            }

        }




        




    }

}
