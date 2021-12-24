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
            centerChunk = center;
            centerChunk.GenerateChunk();
            Transform centerChunkTransform = center.gameObject.transform;
            for (int i = 0; i < chunks.Length - 1; i++)
            {

                if (chunks[i + 1] != null) continue;

                Quaternion rotation = Quaternion.AngleAxis(-i * 60f + 19f, Vector3.up);
                Vector3 offset = Vector3.forward * Hex.hexRadius * 5.3f;
                offset = rotation * offset;

                GameObject chunkGO = Locator.Instance.ObjectPooler.
                    GetPooledChunk(center.transform.position + offset, Quaternion.identity, centerChunkTransform.parent);


                Chunk chunk = chunkGO.GetComponent<Chunk>();
                chunk.SetChunkPosition((ChunkPosition)(i + 1));
                chunk.GenerateChunk();
                chunks[i + 1] = chunk;
            }
            for (int i = 0; i < chunks.Length; i++)
            {
                GenerateNeighbours(chunks[i]);
            }
            

        }

        void GenerateNeighbours(Chunk chunk)
        {
            for (int i = 0; i < 6; i++)
            {
                ExitDirection direction = (ExitDirection)i;
                ChunkPosition adjacentPosition = HelperEnums.GetAdjacentChunkPosition(chunk.chunkPosition,direction);
                if (adjacentPosition == ChunkPosition.NONE) continue;

                Chunk neighbourChunk = chunks[(int)adjacentPosition];

                chunk.neighbours[direction] = neighbourChunk;
                ExitDirection oppositeDirection = HelperEnums.GetOppositeDirection(direction);
                Debug.Log(direction + "<->" + oppositeDirection);
                neighbourChunk.neighbours[oppositeDirection] = chunk;
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

                chunk.ClearNeighbours();
                chunk.gameObject.SetActive(false);

            }

            chunks = newChunks;

            GenerateChunks(newCenter);
        }



    }

}
