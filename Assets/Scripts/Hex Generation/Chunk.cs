
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
            GenerateHexesRecursive(centerHex, maxGenerationSteps);
        }

        public void MoveCenterHex(Hex newCenter)
        {
            float distance = (centerHex.transform.position - newCenter.transform.position).magnitude;
            float despawnDistance = hexRadius * Mathf.Sqrt(3) * maxGenerationSteps * 0.5f;
            Debug.Log(distance + "/" + despawnDistance);
            if (distance > despawnDistance)
            {
                centerHex = newCenter;
                for (int i = hexes.Count - 1; i >= 0; i--)
                {
                    GenerateHexesRecursive(newCenter, maxGenerationSteps);

                    /*if ((hexes[i].transform.position - centerHex.transform.position).magnitude > despawnDistance*2f)
                   {
                       hexes[i].ClearExits();
                       hexes[i].gameObject.SetActive(false);
                       hexes.RemoveAt(i);
                   } */

                }


            }
        }




        void GenerateHexesRecursive(Hex hexGenerationCenter, int stepsRemaining)
        {
            Debug.Log("Recursion step " + stepsRemaining);
            if (stepsRemaining > 0)
            {
                Transform centerHexTransform = hexGenerationCenter.gameObject.transform;
                Hex[] neighbours = new Hex[6];
                for (int i = 0; i < 6; i++)
                {
                    //generates hex in correct position
                    Quaternion rotation = Quaternion.AngleAxis(-i * 60f, Vector3.up);

                    ExitDirection direction = (ExitDirection)i;

                    if (hexGenerationCenter.exits.ContainsKey(direction))
                    {
                        Exit exit = hexGenerationCenter.exits[direction];
                        neighbours[i] = exit.GetOtherHex(hexGenerationCenter);
                        continue;
                    };

                    Vector3 offset = Vector3.forward * hexRadius * Mathf.Sqrt(3f);
                    offset = rotation * offset;

                    //sets up the hex 
                    GameObject hexGO = Locator.Instance.ObjectPooler.
                        GetPooledHex(hexGenerationCenter.transform.position + offset, Quaternion.identity, transform);

                    Hex hex = hexGO.GetComponent<Hex>();

                    hexes.Add(hex);
                    neighbours[i] = hex;

                    //connect to center hex
                    AddExit(direction, hexGenerationCenter, hex);

                }

                for (int i = 0; i < 6; i++)
                {
                    //Generates hexes in the directions where there is not an exit, in front
                    int nextIndex = (i + 1) % 6;
                    ExitDirection nextDirection = (ExitDirection)((i + 2) % 6);
                    ExitDirection oppositeDir = HelperEnums.GetOppositeDirection(nextDirection);
                    if (neighbours[i].exits.ContainsKey(nextDirection)) continue;
                    if (neighbours[nextIndex].exits.ContainsKey(oppositeDir)) continue;

                    AddExit(nextDirection, neighbours[i], neighbours[nextIndex]);


                }

                for (int i = 0; i < 6; i++)
                {
                    //Recursive step
                    GenerateHexesRecursive(neighbours[i], stepsRemaining - 1);

                }
            }
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
            //Checking if an exit exists already
            if (hex1.exits.ContainsKey(direction) && !hex2.exits.ContainsKey(oppositeDirection))
            {
                Exit ex = hex1.exits[direction];
                if (ex.GetOtherHex(hex1) == hex2)
                    hex2.exits[oppositeDirection] = ex;
                else
                {
                    Debug.LogWarning("Exit is already connected to another hex");


                }
                return;
            }
            if (hex2.exits.ContainsKey(oppositeDirection) && !hex1.exits.ContainsKey(direction))
            {
                Exit ex = hex2.exits[oppositeDirection];
                if (ex.GetOtherHex(hex2) == hex1)
                    hex1.exits[direction] = ex;
                else
                {
                    Debug.LogWarning("Exit is already connected to another hex");

                }
                return;
            }


            //Sets the exit in the correct rotation, offset from hex, and sets it both in the hex that
            //spawns it and in the hex that is connecting to it, in their exit dictionaries

            Quaternion rotation = Quaternion.AngleAxis(-(int)direction * 60f, Vector3.up);
            Vector3 offset = rotation * Vector3.forward * hexRadius * Mathf.Sqrt(3f) * 0.5f;

            GameObject exitGO = Locator.Instance.ObjectPooler.
                GetPooledExit(hex1.transform.position + offset, rotation, transform);

            Exit exit = exitGO.GetComponent<Exit>();
            exit.Initialize(hex1, hex2);
            exit.SetColor(Color.black);

            bool isOpen = Random.value < exitProbability;
            if (isOpen) exit.Open(); else exit.Close();

            hex1.exits[direction] = exit;

            hex2.exits[oppositeDirection] = exit;


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