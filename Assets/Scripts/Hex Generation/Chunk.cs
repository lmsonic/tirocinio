
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


        public void Start()
        {
            hexRadius *= transform.localScale.x;
            Debug.Log("Hex Radius:" + hexRadius);
            Locator.Instance.ObjectPooler.AddCentralHex(centerHex.gameObject);
            hexes.Add(centerHex);
            GenerateHexesRecursive(centerHex, 4);
        }

        void GenerateHexesRecursive(Hex hexGenerationCenter, int stepsRemaining)
        {
            if (stepsRemaining > 0)
            {
                Debug.Log("steps remaining " + stepsRemaining);
                Color randomColor = Random.ColorHSV();
                Transform centerHexTransform = hexGenerationCenter.gameObject.transform;
                hexGenerationCenter.SetColor(randomColor);
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
                    Debug.Log(direction);

                    Vector3 offset = Vector3.forward * hexRadius * Mathf.Sqrt(3f);
                    offset = rotation * offset;

                    //sets up the hex 
                    GameObject hexGO = Locator.Instance.ObjectPooler.
                        GetPooledHex(hexGenerationCenter.transform.position + offset, Quaternion.identity, centerHexTransform);

                    Hex hex = hexGO.GetComponent<Hex>();
                    hex.SetColor(randomColor);

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

                    if (neighbours[i].exits.ContainsKey(nextDirection)) continue;

                    AddExit(nextDirection, neighbours[i], neighbours[nextIndex]);

                }

                for (int i = 0; i < 6; i++)
                {
                    //Recursive step
                    GenerateHexesRecursive(neighbours[i], stepsRemaining - 1);
                }
            }
        }





        public void AddExit(ExitDirection direction, Hex hex1, Hex hex2)
        {
            ExitDirection oppositeDirection = HelperEnums.GetOppositeDirection(direction);
            //Checking if an exit exists already
            if (hex1.exits.ContainsKey(direction) && !hex2.exits.ContainsKey(oppositeDirection))
            {
                Exit ex = hex1.exits[direction];
                if (ex.GetOtherHex(hex1)==hex2)
                    hex2.exits[oppositeDirection] = ex;
                else
                    Debug.LogError("Exit is already connected to another hex");
                return;
            }
            if (hex2.exits.ContainsKey(oppositeDirection) && !hex1.exits.ContainsKey(direction))
            {
                Exit ex = hex2.exits[oppositeDirection];
                if (ex.GetOtherHex(hex2)==hex1)
                    hex1.exits[direction] = ex;
                else
                    Debug.LogError("Exit is already connected to another hex");
                return;
            }


            //Sets the exit in the correct rotation, offset from hex, and sets it both in the hex that
            //spawns it and in the hex that is connecting to it, in their exit dictionaries

            Quaternion rotation = Quaternion.AngleAxis(-(int)direction * 60f, Vector3.up);
            Vector3 offset = rotation * Vector3.forward * hexRadius * Mathf.Sqrt(3f) * 0.5f;

            GameObject exitGO = Locator.Instance.ObjectPooler.
                GetPooledExit(hex1.transform.position + offset, rotation, hex1.transform);

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