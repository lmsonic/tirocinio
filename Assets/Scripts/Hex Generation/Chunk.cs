
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


        public Dictionary<ExitDirection, Chunk> neighbours = new Dictionary<ExitDirection, Chunk>();


        public ChunkPosition chunkPosition = ChunkPosition.CENTER;

        public void SetChunkPosition(ChunkPosition pos) => chunkPosition = pos;

        Color randomColor;
        private void Awake() {
            randomColor = Random.ColorHSV();
        }
        public void Start()
        {
            Locator.Instance.ObjectPooler.AddCentralHex(centerHex.gameObject);
        }

        public void GenerateChunk()
        {
            centerHex.SetColor(randomColor);

            GenerateHexes(centerHex);
            GenerateExits();
        }


        void GenerateHexes(Hex newCenterHex)
        {
            //setting up new center hex
            hexes[0] = newCenterHex;
            this.centerHex = newCenterHex;
            Transform centerHexTransform = newCenterHex.gameObject.transform;

            for (int i = 0; i < hexes.Length - 1; i++)
            {
                //if hex has been already generated, continue
                if (hexes[i + 1] != null) continue;

                //generates hex in correct position
                Quaternion rotation = Quaternion.AngleAxis(-i * 60f, Vector3.up);
                Vector3 offset = Vector3.forward * Hex.hexRadius * Mathf.Sqrt(3f);
                offset = rotation * offset;

                GameObject hexGO = Locator.Instance.ObjectPooler.
                    GetPooledHex(newCenterHex.transform.position + offset, Quaternion.identity, centerHexTransform.parent);

                //sets up the hex 
                Hex hex = hexGO.GetComponent<Hex>();
                hex.SetHexPosition((HexPosition)(i + 1));
                hex.SetColor(randomColor);
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
                    //if hex has already exit in that direction, continue
                    if (hex.exits.ContainsKey(direction)) continue;

                    HexPosition hexPosition = HelperEnums.GetAdjacentHexPosition(hex.hexPosition, direction);
                    //if hex which would be connected is out the chunk, continue
                    //TODO: make exits work with different chunks
                    if (hexPosition == HexPosition.NONE) continue;

                    bool isOpen = Random.value < exitProbability;

                    Hex otherHex = hexes[(int)hexPosition];
                    hex.AddExit(direction, otherHex, isOpen);

                }
            }
        }

        private void OnDisable()
        {
            ClearNeighbours();
        }

        public void ClearNeighbours()
        {
            //Clears up neighbour dictionary correctly
            foreach (KeyValuePair<ExitDirection, Chunk> entry in neighbours)
            {
                ExitDirection direction = entry.Key;
                Chunk chunk = entry.Value;
                ExitDirection oppositeDirection = HelperEnums.GetOppositeDirection(direction);
                chunk.neighbours.Remove(oppositeDirection);
            }
            neighbours.Clear();
        }


    }

}