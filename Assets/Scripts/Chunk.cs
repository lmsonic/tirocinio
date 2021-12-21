
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tirocinio
{
    public class Chunk : MonoBehaviour
    {
        public Hex[] hexes = new Hex[7];

        public Hex centerHex;

        public float exitProbability = 0.5f;



        public void PlayerNotOnCenterHex(Hex newCenter){
            MoveChunkCenter(newCenter);
        }


        public void Start()
        {
            Hex.hexRadius *= transform.localScale.x;
            Locator.Instance.ObjectPooler.AddCentralHex(centerHex.gameObject);
            GenerateHexes(centerHex);
            GenerateExits();
        }

        void GenerateHexes(Hex newCenterHex)
        {
            hexes[0] = newCenterHex;
            this.centerHex = newCenterHex;
            Transform centerHexTransform = newCenterHex.gameObject.transform;
            for (int i = 0; i < hexes.Length-1; i++)
            {

                if (hexes[i+1] != null) continue;

                Quaternion rotation = Quaternion.AngleAxis(-i * 60f, Vector3.up);
                Vector3 offset = Vector3.forward * Hex.hexRadius * 2f;
                offset = rotation * offset;

                GameObject hexGO = Locator.Instance.ObjectPooler.
                    GetPooledHex(newCenterHex.transform.position + offset, Quaternion.identity, centerHexTransform.parent);


                Hex hex = hexGO.GetComponent<Hex>();
                hex.SetHexPosition((HexPosition)(i + 1));
                hexes[i + 1] = hex;
            }
        }

        public void GenerateExits()
        {
            for (int i = 0; i < hexes.Length; i++)
            {
                Hex hex = hexes[i];
                for (int j = 0; j < 6; j++)
                {
                    ExitDirection direction = (ExitDirection)j;
                    if (hex.exits.ContainsKey(direction)) continue;

                    HexPosition hexPosition = hex.GetAdjacentHexPosition(direction);
                    if (hexPosition == HexPosition.NONE) continue;

                    bool isOpen = Random.value < exitProbability;

                    Hex otherHex = hexes[(int)hexPosition];
                    hex.AddExit(direction, otherHex, isOpen);

                }
            }
        }

        public void MoveChunkCenter(Hex newCenter)
        {
            Hex[] newHexes = new Hex[7];
            for (int i = 0; i < newHexes.Length; i++)//initialize to null
            {
                newHexes[i] = null;
            }

            List<Hex> hexesToKeep = new List<Hex>();

            foreach (KeyValuePair<ExitDirection, Exit> entry in newCenter.exits)
            {
                //saving and setting up adjacent hexes
                ExitDirection direction = entry.Key;
                Exit exit = entry.Value;

                HexPosition hexPositionFromCenter = (HexPosition)(int)(direction + 1);

                Hex adjacentHex = exit.GetOtherHex(newCenter);

                adjacentHex.SetHexPosition(hexPositionFromCenter);
                newHexes[(int)hexPositionFromCenter] = adjacentHex;

                hexesToKeep.Add(adjacentHex);

            }
            //saving and setting up adjacent hexes
            newCenter.SetHexPosition(HexPosition.CENTER);

            hexesToKeep.Add(newCenter);


            for (int i = 0; i < hexes.Length; i++) // deleting exits
            {
                HexPosition hexPosition = (HexPosition)i;
                Hex hex = hexes[i];

                if (hexesToKeep.Contains(hex)) continue;

                hex.gameObject.SetActive(false);

            }

            hexes = newHexes;


            GenerateHexes(newCenter);
            GenerateExits();
        }

    }

}