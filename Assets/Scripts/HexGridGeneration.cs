using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tirocinio
{

    public class HexGridGeneration : MonoBehaviour
    {


        public HexGeneration[] hexes = new HexGeneration[7];
        private ExitDirection selectedDirection = ExitDirection.NONE;
        Vector3[] adjacentHexPositionsOffsets = new Vector3[6]
        {
            new Vector3(0f,0f,18f),
            new Vector3(-16f,0f,9f),
            new Vector3(-16f,0f,-9f),
            new Vector3(0f,0f,-18f),
            new Vector3(16f,0f,-9f),
            new Vector3(16f,0f,9f),

        };

        private Dictionary<ExitDirection, int[]> directionNewIndices = new Dictionary<ExitDirection, int[]>()
        {                                        // 0   1   2   3   4   5   6
            {ExitDirection.NORTH, new int[]       { 4 , 0 , 3 , 2 , 1 , 6 , 5 }},
            {ExitDirection.NORTHWEST, new int[]   { 5 , 6 , 0 , 4 , 3 , 2 , 1 }},
            {ExitDirection.SOUTHWEST, new int[]   { 6 , 2 , 1 , 0 , 5 , 4 , 3 }},
            {ExitDirection.SOUTH, new int[]       { 1 , 4 , 3 , 2 , 0 , 6 , 5 }},
            {ExitDirection.SOUTHEAST, new int[]   { 2 , 6 , 5 , 4 , 3 , 0 , 1 }},
            {ExitDirection.NORTHEAST, new int[]   { 3 , 2 , 1 , 6 , 5 , 4 , 0 }},
        };

        private Dictionary<ExitDirection, int[]> directionTilesToRegenerate = new Dictionary<ExitDirection, int[]>()
        {
            {ExitDirection.NORTH, new int[]       { 4 , 3 , 5 }},
            {ExitDirection.NORTHWEST, new int[]   { 4 , 5 , 6 }},
            {ExitDirection.SOUTHWEST, new int[]   { 1 , 6 , 5 }},
            {ExitDirection.SOUTH, new int[]       { 2 , 1 , 6 }},
            {ExitDirection.SOUTHEAST, new int[]   { 1 , 2 , 3 }},
            {ExitDirection.NORTHEAST, new int[]   { 2 , 3 , 4 }},
            {ExitDirection.NONE, new int[]        { 0 , 1 , 2 , 3 , 4 , 5 , 6 }},
        };




        private void Start()
        {
            GenerateGrid();
        }

        public void SetSelectedDirection(int direction)
        {
            selectedDirection = (ExitDirection)direction;
        }
        public void GenerateGrid()
        {
            GenerateGrid(selectedDirection);
            for (int i = 0; i< hexes.Length; i++)
            {
                hexes[i].SetIndex(i);
            }


            int[] indicesToRegenerate = directionTilesToRegenerate[selectedDirection];
            foreach (int i in indicesToRegenerate)
            {
                hexes[i].GenerateHex();
            }
            
        }

        public void GenerateGrid(ExitDirection exitDirection)
        {
            if (exitDirection == ExitDirection.NONE)
            {
                return;
            }

            HexGeneration[] newHexes = new HexGeneration[7];
            var indices = directionNewIndices[exitDirection];
            for (int i = 0; i < 7; i++) 
            {
                newHexes[indices[i]] = hexes[i];
            }
            Vector3 newCenterPosition = newHexes[0].transform.position;
            for (int i = 0; i < 6; i++)
            {
                newHexes[i + 1].transform.position = newCenterPosition + adjacentHexPositionsOffsets[i];
            }

            hexes = newHexes;

            
            

        }




    }
}
