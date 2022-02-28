
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tirocinio
{
    public class Chunk : MonoBehaviour
    {
        public List<Hex> hexes = new List<Hex>();

        public Hex centerHex;


        static public float hexRadius = 10f;

        public float exitProbability = 0.5f;

        public int maxGenerationSteps = 4;




        public void Start()
        {
            hexRadius *= transform.localScale.x;
            Debug.Log("Hex Radius:" + hexRadius);

            Locator.Instance.ObjectPooler.AddCentralHex(centerHex.gameObject);
            hexes.Add(centerHex);
            //StartCoroutine(GenerateHexesRecursive(centerHex, maxGenerationSteps));
            StartCoroutine(GenerateHexesIterative(centerHex, maxGenerationSteps));
        }



        public void MoveCenterHex(Hex newCenter)
        {

        }


        IEnumerator GenerateHexesIterative(Hex pivotHex, int nSteps)
        {

            
                
                yield return GenerateLayer(pivotHex);
                for (int i = 0; i < 6; i++)
                {
                    Hex newPivot = pivotHex.neighbours[i];
                    yield return GenerateLayer(newPivot);
                    for (int j = 0; j < 6; j++){
                        yield return GenerateLayer(newPivot.neighbours[j]);
                    }
                }
            
            
            for (int i = 0; i < 6; i++)
            {
                Hex newPivot = pivotHex.neighbours[i];

                yield return GenerateLayer(newPivot);
                for (int j = 0; j < 6; j++)
                {
                    Hex newPivot2 = newPivot.neighbours[j];

                    yield return GenerateLayer(newPivot2);
                }
            }

            //GenerateExits();

        }



        IEnumerator GenerateLayer(Hex pivotHex)
        {
            yield return new WaitForSeconds(1f);
            pivotHex.SetColor(Color.red);
            yield return GenerateHexes(pivotHex);
            //yield return GenerateExits(pivotHex);
            yield return new WaitForSeconds(1f);
            pivotHex.SetColor(Color.white);
            for (int i = 0; i < pivotHex.neighbours.Length; i++)
            {
                pivotHex.neighbours[i]?.SetColor(Color.white);
            }


        }



        IEnumerator GenerateHexes(Hex pivotHex)
        {
            Transform centerHexTransform = pivotHex.gameObject.transform;
            for (int i = 0; i < 6; i++)
            {
                if (pivotHex.neighbours[i] != null) continue;

                yield return new WaitForSeconds(0.5f);
                //generates hex in correct position
                Quaternion rotation = Quaternion.AngleAxis(-i * 60f, Vector3.up);

                ExitDirection direction = (ExitDirection)i;
                ExitDirection oppositeDirection = HelperEnums.GetOppositeDirection(direction);



                Hex hex = MakeHex(pivotHex.transform.position, direction);

                hexes.Add(hex);
                pivotHex.neighbours[i] = hex;
                hex.neighbours[(int)oppositeDirection] = pivotHex;
                hex.SetColor(Color.yellow);

                AddExit(direction,pivotHex,hex);

            }
            yield return ConnectNeighbouringHexes(pivotHex);
        }

        IEnumerator ConnectNeighbouringHexes(Hex pivotHex)
        {
            for (int currentNeighborIndex = 0; currentNeighborIndex < 6; currentNeighborIndex++)
            {
                yield return new WaitForSeconds(0.5f);
                //Generates hexes in the directions where there is not an exit, in front
                int nextNeighbourIndex = (currentNeighborIndex + 1) % 6;
                ExitDirection nextDirection = (ExitDirection)((currentNeighborIndex + 2) % 6);
                ExitDirection oppositeDir = HelperEnums.GetOppositeDirection(nextDirection);

                Hex currentNeighbour = pivotHex.neighbours[currentNeighborIndex];
                Hex nextNeighbour = pivotHex.neighbours[nextNeighbourIndex];

                currentNeighbour.neighbours[(int)nextDirection] = nextNeighbour;
                nextNeighbour.neighbours[(int)oppositeDir] = currentNeighbour;
                
                AddExit(nextDirection,currentNeighbour,nextNeighbour);
            }
        }


        Hex MakeHex(Vector3 startPosition, ExitDirection direction)
        {
            Quaternion rotation = Quaternion.AngleAxis(-(int)direction * 60f, Vector3.up);
            Vector3 offset = Vector3.forward * hexRadius * Mathf.Sqrt(3f);
            offset = rotation * offset;

            //sets up the hex 
            GameObject hexGO = Locator.Instance.ObjectPooler.
                GetPooledHex(startPosition + offset, Quaternion.identity, transform);

            return hexGO.GetComponent<Hex>();
        }

        // IEnumerator GenerateHexesRecursive(Hex hexGenerationCenter, int stepsRemaining)
        // {
        //     Debug.Log("Recursion step " + stepsRemaining);
        //     if (stepsRemaining > 0)
        //     {
        //         yield return new WaitForSeconds(0.5f);
        //         Color randomColor = Random.ColorHSV();
        //         Transform centerHexTransform = hexGenerationCenter.gameObject.transform;
        //         Hex[] neighbours = new Hex[6];
        //         for (int i = 0; i < 6; i++)
        //         {
        //             //generates hex in correct position
        //             Quaternion rotation = Quaternion.AngleAxis(-i * 60f, Vector3.up);

        //             ExitDirection direction = (ExitDirection)i;

        //             if (hexGenerationCenter.exits.ContainsKey(direction))
        //             {
        //                 Exit exit = hexGenerationCenter.exits[direction];
        //                 neighbours[i] = exit.GetOtherHex(hexGenerationCenter);
        //                 continue;
        //             };

        //             Vector3 offset = Vector3.forward * hexRadius * Mathf.Sqrt(3f);
        //             offset = rotation * offset;

        //             //sets up the hex 
        //             GameObject hexGO = Locator.Instance.ObjectPooler.
        //                 GetPooledHex(hexGenerationCenter.transform.position + offset, Quaternion.identity, transform);

        //             Hex hex = hexGO.GetComponent<Hex>();

        //             hexes.Add(hex);
        //             neighbours[i] = hex;
        //             hex.SetColor(randomColor);

        //             //connect to center hex
        //             AddExit(direction, hexGenerationCenter, hex);

        //         }

        //         for (int i = 0; i < 6; i++)
        //         {
        //             yield return new WaitForSeconds(0.3f);
        //             //Generates hexes in the directions where there is not an exit, in front
        //             int nextIndex = (i + 1) % 6;
        //             ExitDirection nextDirection = (ExitDirection)((i + 2) % 6);
        //             ExitDirection oppositeDir = HelperEnums.GetOppositeDirection(nextDirection);
        //             if (neighbours[i].exits.ContainsKey(nextDirection)) continue;
        //             if (neighbours[nextIndex].exits.ContainsKey(oppositeDir)) continue;

        //             AddExit(nextDirection, neighbours[i], neighbours[nextIndex]);


        //         }

        //         for (int i = 0; i < 6; i++)
        //         {
        //             yield return new WaitForSeconds(1f);
        //             //Recursive step
        //             StartCoroutine(GenerateHexesRecursive(neighbours[i], stepsRemaining - 1));

        //         }
        //     }
        // }





        public void AddExit(ExitDirection direction, Hex hex1, Hex hex2)
        {
            ExitDirection oppositeDirection = HelperEnums.GetOppositeDirection(direction);

            Exit exit1 = hex1.exits[(int)direction];
            Exit exit2 = hex2.exits[(int)oppositeDirection];
            if (exit1 == null && exit2 != null)
            {
                hex1.exits[(int)direction] = exit2;
                return;
            }
            else if (exit1 != null && exit2 == null)
            {
                hex2.exits[(int)oppositeDirection] = exit1;
                return;
            }

            //Sets the exit in the correct rotation, offset from hex, and sets it both in the hex that
            //spawns it and in the hex that is connecting to it, in their exit dictionaries

            Exit exit = MakeExit(hex1.transform.position, direction);
            exit.Initialize(hex1, hex2);
            exit.SetColor(Color.black);

            bool isOpen = Random.value < exitProbability;
            if (isOpen) exit.Open(); else exit.Close();

            hex1.exits[(int)direction] = exit;

            hex2.exits[(int)oppositeDirection] = exit;


        }

        Exit MakeExit(Vector3 startPos, ExitDirection direction)
        {
            Quaternion rotation = Quaternion.AngleAxis(-(int)direction * 60f, Vector3.up);
            Vector3 offset = rotation * Vector3.forward * hexRadius * Mathf.Sqrt(3f) * 0.5f;

            GameObject exitGO = Locator.Instance.ObjectPooler.
                GetPooledExit(startPos + offset, rotation, transform);

            return exitGO.GetComponent<Exit>();

        }



        private void OnDisable()
        {

            for (int i = 0; i < hexes.Count; i++)
            {
                hexes[i].gameObject.SetActive(false);
            }
            hexes.Clear();
        }



    }

}