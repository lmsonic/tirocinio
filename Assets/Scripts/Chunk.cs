using UnityEngine;
namespace Tirocinio
{
    public class Chunk : MonoBehaviour
    {
        public Hex[] hexes = new Hex[7];

        public GameObject hexPrefab;
        public Hex centerHex;

        public float exitProbability = 0.5f;

        public ExitDirection selectedDirection = ExitDirection.NORTH;

        public void ChangeDirection(int indexDirection)
        {
            selectedDirection = (ExitDirection)indexDirection;
        }

        public void MoveInDirection()
        {
            HexPosition pos = hexes[0].GetAdjacentHexPosition(selectedDirection);
            MoveChunkCenter(hexes[(int)pos]);
        }

        public void Start()
        {
            GenerateHexes(centerHex);
            GenerateExits();
        }

        void GenerateHexes(Hex centerHex)
        {
            hexes[0] = centerHex;
            Transform centerHexTransform = centerHex.gameObject.transform;
            for (int i = 0; i < hexes.Length - 1; i++)
            {
                if (hexes[i + 1] != null) continue;

                Quaternion rotation = Quaternion.AngleAxis(-i * 60f, Vector3.up);
                Vector3 offset = Vector3.forward * centerHex.hexRadius;
                offset = rotation * offset;

                GameObject hexGO = Instantiate(hexPrefab,
                                        centerHexTransform.position + offset, Quaternion.identity,
                                            centerHexTransform.parent);
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

                    if (Random.value < exitProbability)
                    {
                        Debug.Log(hex.hexPosition + " " + direction + " " + hexPosition);
                        Hex otherHex = hexes[(int)hexPosition];
                        hex.AddExit(direction, otherHex);
                    }
                }
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



    }

}