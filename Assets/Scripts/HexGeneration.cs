using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Tirocinio
{
    public class HexGeneration : MonoBehaviour
    {

        public Chunk[] chunks = new Chunk[7];

        public Chunk centerChunk;


        private void Start()
        {
            Hex.hexRadius *= transform.localScale.x;
            Locator.Instance.ObjectPooler.AddCentralChunk(centerChunk.gameObject);
            GenerateChunks(centerChunk);
        }

        void GenerateChunks(Chunk center)
        {
            chunks[0] = center;
            this.centerChunk = center;
            center.GenerateChunk();
            Transform centerChunkTransform = center.gameObject.transform;
            for (int i = 0; i < chunks.Length - 1; i++)
            {

                if (chunks[i + 1] != null) continue;

                Quaternion rotation = Quaternion.AngleAxis(-i * 60f + 20f, Vector3.up);
                Vector3 offset = Vector3.forward * Hex.hexRadius * 5.2f;
                offset = rotation * offset;

                GameObject chunkGO = Locator.Instance.ObjectPooler.
                    GetPooledChunk(center.transform.position + offset, Quaternion.identity, centerChunkTransform.parent);


                Chunk chunk = chunkGO.GetComponent<Chunk>();
                chunk.SetChunkPosition((ChunkPosition)(i + 1));
                chunk.GenerateChunk();
                for (int j = 0; j < 6; j++)
                {
                    ExitDirection direction = (ExitDirection)i;
                    ChunkPosition adjacentPosition = chunk.GetAdjacentChunkPosition(direction);
                    if (adjacentPosition == ChunkPosition.NONE) continue;

                    Chunk neighbourChunk = chunks[(int)adjacentPosition];

                    chunk.neighbours[direction] = neighbourChunk;
                    neighbourChunk.neighbours[(ExitDirection)((int)direction + 3 % 6)] = chunk;
                }

                chunks[i + 1] = chunk;
            }

        }

        public void MoveGridCenter(Chunk newCenter)
        {

            Chunk[] newChunks = new Chunk[7];
            for (int i = 0; i < newChunks.Length; i++)//initialize to null
            {
                newChunks[i] = null;
            }

            List<Chunk> chunksToKeep = new List<Chunk>();

            foreach (KeyValuePair<ExitDirection, Chunk> entry in newCenter.neighbours)
            {
                //saving and setting up adjacent hexes
                ExitDirection direction = entry.Key;
                Chunk neighbour = entry.Value;

                ChunkPosition chunkPositionFromCenter = (ChunkPosition)(int)(direction + 1);


                neighbour.SetChunkPosition(chunkPositionFromCenter);
                newChunks[(int)chunkPositionFromCenter] = neighbour;

                chunksToKeep.Add(neighbour);

            }

            //saving and setting up adjacent hexes
            newCenter.SetChunkPosition(ChunkPosition.CENTER);
            chunksToKeep.Add(newCenter);


            for (int i = 0; i < chunks.Length; i++) // deleting exits
            {
                ChunkPosition hexPosition = (ChunkPosition)i;
                Chunk chunk = chunks[i];

                if (chunksToKeep.Contains(chunk)) continue;

                chunk.gameObject.SetActive(false);

            }

            chunks = newChunks;

            GenerateChunks(newCenter);
        }



    }

}
