using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Tirocinio
{

    public class Hex : PoolObject
    {

        public List<Exit> exits;
        public Hex[] neighbours = new Hex[6];



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

        public void ClearNeighbours()
        {
            for (int i = 0; i < neighbours.Length; i++)
            {
                ExitDirection oppositeDirection = HelperEnums.GetOppositeDirection((ExitDirection)i);
                Hex neighbour = neighbours[i];
                if (neighbour)
                    neighbour.neighbours[(int)oppositeDirection] = null;
            }
            neighbours = new Hex[6];
        }





        private void OnDrawGizmos()
        {
            for (int i = 0; i < neighbours.Length; i++)
            {
                if (neighbours[i])
                {
                    Vector3 direction = (neighbours[i].transform.position - transform.position);
                    Debug.DrawRay(transform.position, direction * 0.3f, Color.black);
                }
            }

        }









    }
}

