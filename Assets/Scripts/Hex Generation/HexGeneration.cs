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
            //if HexGeneration is scaled, everything scales correctly
            Hex.hexRadius *= transform.localScale.x;
            Debug.Log("Hex Radius:" + Hex.hexRadius);

            Locator.Instance.ObjectPooler.AddCentralChunk(centerChunk.gameObject);
            GenerateChunks(centerChunk);
        }



        void GenerateChunks(Chunk center)
        {
            //Setting up center chunk
            chunks[0] = center;
            centerChunk = center;
            centerChunk.GenerateChunk();
            Transform centerChunkTransform = center.gameObject.transform;

            for (int i = 0; i < chunks.Length - 1; i++)
            {
                //if chunk has been generated, continue
                if (chunks[i + 1] != null) continue;

                //sets up chunk in correct position
 
                Quaternion rotation = Quaternion.Euler(0f,-i * 60f - 40.9f,0f);

                Vector3 offset = Vector3.forward * Hex.hexRadius * Mathf.Sqrt(85f) * 0.4975f;
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
            //Generate exits

        }

        void GenerateNeighbours(Chunk chunk)
        {
            //Connects chunks neighbour dictionary correctly
            for (int i = 0; i < 6; i++)
            {
                ExitDirection direction = (ExitDirection)i;
                ChunkPosition adjacentPosition = HelperEnums.GetAdjacentChunkPosition(chunk.chunkPosition, direction);
                //if adjacent position is out of the 7 chunk grid, continue
                if (adjacentPosition == ChunkPosition.NONE) continue;

                Chunk neighbourChunk = chunks[(int)adjacentPosition];

                chunk.neighbours[direction] = neighbourChunk;
                ExitDirection oppositeDirection = HelperEnums.GetOppositeDirection(direction);
                neighbourChunk.neighbours[oppositeDirection] = chunk;
            }

        }

        public void MoveGridCenter(Chunk newCenter)
        {
            //Changes chunk grid center, keeeping chunks that are spawned and adjecent to the new center, and
            //deleting old ones

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


            for (int i = 0; i < chunks.Length; i++)
            {
                // deleting exits of the chunks that will be deleted, and the corrisponding chunk
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
