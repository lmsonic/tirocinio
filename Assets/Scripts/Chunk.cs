
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



        public void Start()
        {
            Locator.Instance.ObjectPooler.AddCentralHex(centerHex.gameObject);
        }

        public void GenerateChunk(){
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


        public ChunkPosition GetAdjacentChunkPosition(ExitDirection direction)
        {
            switch (chunkPosition)
            {
                case ChunkPosition.CENTER:
                    return (ChunkPosition)((int)direction + 1);
                case ChunkPosition.UP:
                    switch (direction)
                    {
                        case ExitDirection.SOUTHWEST:
                            return ChunkPosition.UP_LEFT;
                        case ExitDirection.SOUTH:
                            return ChunkPosition.CENTER;
                        case ExitDirection.SOUTHEAST:
                            return ChunkPosition.UP_RIGHT;
                        default:
                            return ChunkPosition.NONE;
                    }
                case ChunkPosition.UP_LEFT:
                    switch (direction)
                    {
                        case ExitDirection.NORTHEAST:
                            return ChunkPosition.UP;
                        case ExitDirection.SOUTHEAST:
                            return ChunkPosition.CENTER;
                        case ExitDirection.SOUTH:
                            return ChunkPosition.DOWN_LEFT;
                        default:
                            return ChunkPosition.NONE;
                    }
                case ChunkPosition.DOWN_LEFT:
                    switch (direction)
                    {
                        case ExitDirection.NORTH:
                            return ChunkPosition.UP_LEFT;
                        case ExitDirection.NORTHEAST:
                            return ChunkPosition.CENTER;
                        case ExitDirection.SOUTHEAST:
                            return ChunkPosition.DOWN;
                        default:
                            return ChunkPosition.NONE;
                    }
                case ChunkPosition.DOWN:
                    switch (direction)
                    {
                        case ExitDirection.NORTHWEST:
                            return ChunkPosition.DOWN_LEFT;
                        case ExitDirection.NORTH:
                            return ChunkPosition.CENTER;
                        case ExitDirection.NORTHEAST:
                            return ChunkPosition.DOWN_RIGHT;
                        default:
                            return ChunkPosition.NONE;
                    }
                case ChunkPosition.DOWN_RIGHT:
                    switch (direction)
                    {
                        case ExitDirection.SOUTHWEST:
                            return ChunkPosition.DOWN;
                        case ExitDirection.NORTHWEST:
                            return ChunkPosition.CENTER;
                        case ExitDirection.NORTH:
                            return ChunkPosition.UP_RIGHT;
                        default:
                            return ChunkPosition.NONE;
                    }
                case ChunkPosition.UP_RIGHT:
                    switch (direction)
                    {
                        case ExitDirection.SOUTH:
                            return ChunkPosition.DOWN_RIGHT;
                        case ExitDirection.SOUTHWEST:
                            return ChunkPosition.CENTER;
                        case ExitDirection.NORTHWEST:
                            return ChunkPosition.UP;
                        default:
                            return ChunkPosition.NONE;
                    }
                default:
                    return ChunkPosition.NONE;

            }
        }
    }

}