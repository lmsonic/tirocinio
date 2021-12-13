using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tirocinio
{

    public class HexGridGeneration : MonoBehaviour
    {
        private ExitDirection selectedDirection = ExitDirection.NONE;

        
        public HexGeneration[] hexes = new HexGeneration[7];
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
        {                                       // 0   1   2   3   4   5   6
            {ExitDirection.UP, new int[]         { 4 , 0 , 3 , 2 , 1 , 6 , 5 }},
            {ExitDirection.UP_LEFT, new int[]    { 5 , 6 , 0 , 4 , 3 , 2 , 1 }},
            {ExitDirection.DOWN_LEFT, new int[]  { 6 , 2 , 1 , 0 , 5 , 4 , 3 }},
            {ExitDirection.DOWN, new int[]       { 1 , 4 , 3 , 2 , 0 , 6 , 5 }},
            {ExitDirection.DOWN_RIGHT, new int[] { 2 , 6 , 5 , 4 , 3 , 0 , 1 }},
            {ExitDirection.UP_RIGHT, new int[]   { 3 , 2 , 1 , 6 , 5 , 4 , 0 }},
        };

        

        private List<HexPosition> newList;

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
            for (int i = 0; i < hexes.Length; i++)
            {

                hexes[i].SetIndex(i);
                hexes[i].GenerateHex();
            }
        }
        
        public void GenerateGrid(ExitDirection exitDirection)
        {
            if (exitDirection == ExitDirection.NONE)
            {
                for (int i = 0; i < hexes.Length; i++)
                {

                    hexes[i].SetIndex(i);
                    hexes[i].GenerateHex();
                }

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
                newHexes[i+1].transform.position = newCenterPosition + adjacentHexPositionsOffsets[i];
            }

            hexes = newHexes;
            
        }

        


    }
}
