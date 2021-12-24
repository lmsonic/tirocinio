
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

        public void GenerateChunk(){
            centerHex.SetColor(randomColor);
            GenerateHexes(centerHex);
            GenerateExits();
        }


        void GenerateHexes(Hex newCenterHex)
        {
            hexes[0] = newCenterHex;
            this.centerHex = newCenterHex;

            Transform centerHexTransform = newCenterHex.gameObject.transform;
            for (int i = 0; i < hexes.Length - 1; i++)
            {

                if (hexes[i + 1] != null) continue;

                Quaternion rotation = Quaternion.AngleAxis(-i * 60f, Vector3.up);
                Vector3 offset = Vector3.forward * Hex.hexRadius * 2f;
                offset = rotation * offset;

                GameObject hexGO = Locator.Instance.ObjectPooler.
                    GetPooledHex(newCenterHex.transform.position + offset, Quaternion.identity, centerHexTransform.parent);

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
                    if (hex.exits.ContainsKey(direction)) continue;

                    HexPosition hexPosition = HelperEnums.GetAdjacentHexPosition(hex.hexPosition, direction);
                    if (hexPosition == HexPosition.NONE) continue;

                    bool isOpen = Random.value < exitProbability;

                    Hex otherHex = hexes[(int)hexPosition];
                    hex.AddExit(direction, otherHex, isOpen);

                }
            }
        }

        private void OnDisable() {
            ClearNeighbours();
        }

        public void ClearNeighbours(){
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