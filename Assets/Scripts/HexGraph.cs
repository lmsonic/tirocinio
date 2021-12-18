using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGraph : MonoBehaviour
{

    public enum HexPosition
    {
        CENTER, UP, UP_LEFT, DOWN_LEFT, DOWN, DOWN_RIGHT, UP_RIGHT, NONE
    }


    public enum ExitDirection
    {
        NORTH, NORTHWEST, SOUTHWEST, SOUTH, SOUTHEAST, NORTHEAST
    }

    public class Exit
    {
        public Hex hex1, hex2;
        static public GameObject prefab;
        GameObject gameObject;
        public Exit(Hex h1, Hex h2, Vector3 position, Quaternion rotation, Transform parent)
        {
            gameObject = Instantiate(prefab, position + Vector3.up, rotation, parent);
            hex1 = h1;
            hex2 = h2;
        }

    }
    public class Hex
    {
        public Dictionary<ExitDirection, Exit> exits = new Dictionary<ExitDirection, Exit>();
        static public GameObject prefab;

        public float hexRadius = 18f;

        public GameObject gameObject;

        public HexPosition hexPosition;
        public Hex(Vector3 position, Transform parent, HexPosition hexPosition)
        {
            this.hexPosition = hexPosition;
            gameObject = Instantiate(prefab, position, Quaternion.identity, parent);
        }

        public void SetHexPosition(HexPosition pos)
        {
            hexPosition = pos;
        }
        public void AddExit(ExitDirection direction, Hex otherHex)
        {
            Quaternion rotation = Quaternion.AngleAxis(-(int)direction * 60f, Vector3.up);
            Vector3 offset = rotation * Vector3.forward * hexRadius;

            Exit exit = new Exit(this, otherHex, gameObject.transform.position + offset / 2, rotation, gameObject.transform);
            exits[direction] = exit;

            ExitDirection oppositeDirection = (ExitDirection)(((int)direction + 3) % 6);
            otherHex.exits[oppositeDirection] = exit;
        }

        public HexPosition GetAdjacentHexPosition(ExitDirection direction)
        {
            switch (hexPosition)
            {
                case HexPosition.CENTER:
                    return (HexPosition)((int)direction + 1);
                case HexPosition.UP:
                    switch (direction)
                    {
                        case ExitDirection.SOUTHWEST:
                            return HexPosition.UP_LEFT;
                        case ExitDirection.SOUTH:
                            return HexPosition.CENTER;
                        case ExitDirection.SOUTHEAST:
                            return HexPosition.UP_RIGHT;
                        default:
                            return HexPosition.NONE;
                    }
                case HexPosition.UP_LEFT:
                    switch (direction)
                    {
                        case ExitDirection.NORTHEAST:
                            return HexPosition.UP;
                        case ExitDirection.SOUTHEAST:
                            return HexPosition.CENTER;
                        case ExitDirection.SOUTH:
                            return HexPosition.DOWN_LEFT;
                        default:
                            return HexPosition.NONE;
                    }
                case HexPosition.DOWN_LEFT:
                    switch (direction)
                    {
                        case ExitDirection.NORTH:
                            return HexPosition.UP_LEFT;
                        case ExitDirection.NORTHEAST:
                            return HexPosition.CENTER;
                        case ExitDirection.SOUTHEAST:
                            return HexPosition.DOWN;
                        default:
                            return HexPosition.NONE;
                    }
                case HexPosition.DOWN:
                    switch (direction)
                    {
                        case ExitDirection.NORTHWEST:
                            return HexPosition.DOWN_LEFT;
                        case ExitDirection.NORTH:
                            return HexPosition.CENTER;
                        case ExitDirection.NORTHEAST:
                            return HexPosition.DOWN_RIGHT;
                        default:
                            return HexPosition.NONE;
                    }
                case HexPosition.DOWN_RIGHT:
                    switch (direction)
                    {
                        case ExitDirection.SOUTHWEST:
                            return HexPosition.DOWN;
                        case ExitDirection.NORTHWEST:
                            return HexPosition.CENTER;
                        case ExitDirection.NORTH:
                            return HexPosition.UP_RIGHT;
                        default:
                            return HexPosition.NONE;
                    }
                case HexPosition.UP_RIGHT:
                    switch (direction)
                    {
                        case ExitDirection.SOUTH:
                            return HexPosition.DOWN_RIGHT;
                        case ExitDirection.SOUTHWEST:
                            return HexPosition.CENTER;
                        case ExitDirection.NORTHWEST:
                            return HexPosition.UP;
                        default:
                            return HexPosition.NONE;
                    }
                default:
                    return HexPosition.NONE;

            }
        }


    }


    public class Chunk
    {
        public Hex[] hexes = new Hex[7];
        static public float exitProbability = 0.5f;
        public Chunk(Hex centerHex)
        {
            GenerateHexes(centerHex);
            GenerateExits();
        }

        void GenerateHexes(Hex centerHex)
        {
            hexes[0] = centerHex;
            for (int i = 0; i < hexes.Length - 1; i++)
            {
                if (hexes[i + 1] != null) continue;

                Quaternion rotation = Quaternion.AngleAxis(-i * 60f, Vector3.up);
                Vector3 offset = Vector3.forward * centerHex.hexRadius;
                offset = rotation * offset;

                hexes[i + 1] = new Hex(centerHex.gameObject.transform.position + offset,
                                    centerHex.gameObject.transform.parent, (HexPosition)i + 1);
            }
        }

        public void MoveChunkCenter(Hex newCenter)
        {
            Hex[] oldHexes = new Hex[hexes.Length];
            for (int i = 0; i < hexes.Length; i++) //clearing hexes
            {
                oldHexes[i] = hexes[i];
                hexes[i] = null;
            }

            ExitDirection[] directionsToHexesToKeep = new ExitDirection[3];
            HexPosition[] hexPositionsToKeep = new HexPosition[3];
            HexPosition[] hexPositionsToDelete = new HexPosition[3];

            int j = 0;
            for (int i = 0; i < 6; i++)
            {
                //at this point the new center has not set its position
                HexPosition hexPosition = newCenter.GetAdjacentHexPosition((ExitDirection)i);
                if (hexPosition != HexPosition.NONE)
                {
                    directionsToHexesToKeep[j] = (ExitDirection)i;
                    hexPositionsToKeep[j] = hexPosition;
                    j++;
                    if (j > 2) break; //in any direction, only 3 hexes other than center are kept
                }
            }
            j = 0;
            for (int i = 1; i < 7; i++)
            {
                bool flagForDeletion = true;
                HexPosition pos = (HexPosition)i;
                if (pos == newCenter.hexPosition) continue;

                for (int k = 0; k < 3; k++)
                {
                    if (pos == hexPositionsToKeep[k])
                    {
                        flagForDeletion = false;
                    }
                }

                if (flagForDeletion)
                {
                    Debug.Log("Flagged for deletion: " + pos);
                    hexPositionsToDelete[j] = pos;
                    j++;
                    
                }
            }

            newCenter.SetHexPosition(HexPosition.CENTER);
            Debug.Log("MOVING CENTER");
            for (int i = 0; i < 3; i++) //saving old hexes
            {
                ExitDirection oldDirection = directionsToHexesToKeep[i];
                HexPosition oldHexPosition = hexPositionsToKeep[i];
                HexPosition newHexPosition = newCenter.GetAdjacentHexPosition(oldDirection);
                Debug.Log(oldHexPosition + "->" + newHexPosition);

                hexes[(int)newHexPosition] = oldHexes[(int)oldHexPosition];
                hexes[(int)newHexPosition].SetHexPosition(newHexPosition);
            }

            for (int i = 0; i < 3; i++)
            {
                Destroy(oldHexes[(int)hexPositionsToDelete[i]].gameObject);
            }   

            GenerateHexes(newCenter);
            GenerateExits();
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

                    if (Random.value < exitProbability)
                    {
                        Debug.Log(hex.hexPosition + " " + direction + " " + hexPosition);
                        Hex otherHex = hexes[(int)hexPosition];
                        hex.AddExit(direction, otherHex);
                    }
                }
            }
        }


    }


    public GameObject hexPrefab;
    public GameObject exitPrefab;

    public float exitProbability = 0.5f;

    public ExitDirection selectedDirection = ExitDirection.NORTH;

    Chunk chunk;

    private void Start()
    {
        Hex.prefab = hexPrefab;
        Exit.prefab = exitPrefab;
        Chunk.exitProbability = exitProbability;

        Hex centerHex = new Hex(transform.position, transform, HexPosition.CENTER);
        chunk = new Chunk(centerHex);


    }

    

}



