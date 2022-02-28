
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
            GenerateHexesIterative(centerHex, maxGenerationSteps);
        }



        public void MoveCenterHex(Hex newCenter)
        {

        }


        void GenerateHexesIterative(Hex pivotHex, int nSteps)
        {



            GenerateHexes(pivotHex);
            for (int i = 0; i < 6; i++)
            {
                Hex newPivot = pivotHex.neighbours[i];
                GenerateHexes(newPivot);
                for (int j = 0; j < 6; j++)
                {
                    GenerateHexes(newPivot.neighbours[j]);
                }
            }

             GenerateExits();

        }





        void GenerateExits()
        {
            for (int i = 0; i < hexes.Count; i++)
            {
                for (int j = 0; j < hexes.Count; j++)
                {
                    if (i == j) continue;
                    if (AreNeighbours(hexes[i], hexes[j]))
                    {
                        if (!HaveExitConnection(hexes[i], hexes[j]))
                        {
                            AddExit(hexes[i], hexes[j]);

                        }
                    }
                }
            }
        }

        bool HaveExitConnection(Hex hex1, Hex hex2)
        {
            foreach (Exit exit in hex1.exits)
            {
                if (exit?.GetOtherHex(hex1) == hex2)
                    return true;
            }
            foreach (Exit exit in hex2.exits)
            {
                if (exit?.GetOtherHex(hex2) == hex1)
                    return true;
            }
            return false;
        }

        bool AreNeighbours(Hex hex1, Hex hex2)
        {
            for (int i = 0; i < 6; i++)
            {
                if (hex1.neighbours[i] == hex2) return true;
                if (hex2.neighbours[i] == hex1) return true;
            }
            return false;
        }


        void GenerateHexes(Hex pivotHex)
        {

            Transform centerHexTransform = pivotHex.gameObject.transform;
            for (int i = 0; i < 6; i++)
            {
                if (pivotHex.neighbours[i] != null) continue;

                //generates hex in correct position
                Quaternion rotation = Quaternion.AngleAxis(-i * 60f, Vector3.up);

                ExitDirection direction = (ExitDirection)i;
                ExitDirection oppositeDirection = HelperEnums.GetOppositeDirection(direction);



                Hex hex = MakeHex(pivotHex.transform.position, direction);

                hexes.Add(hex);
                pivotHex.neighbours[i] = hex;
                hex.neighbours[(int)oppositeDirection] = pivotHex;


            }
            ConnectNeighbouringHexes(pivotHex);


        }

        void ConnectNeighbouringHexes(Hex pivotHex)
        {
            for (int currentNeighborIndex = 0; currentNeighborIndex < 6; currentNeighborIndex++)
            {
                //Generates hexes in the directions where there is not an exit, in front
                int nextNeighbourIndex = (currentNeighborIndex + 1) % 6;
                ExitDirection nextDirection = (ExitDirection)((currentNeighborIndex + 2) % 6);
                ExitDirection oppositeDir = HelperEnums.GetOppositeDirection(nextDirection);

                Hex currentNeighbour = pivotHex.neighbours[currentNeighborIndex];
                Hex nextNeighbour = pivotHex.neighbours[nextNeighbourIndex];

                currentNeighbour.neighbours[(int)nextDirection] = nextNeighbour;
                nextNeighbour.neighbours[(int)oppositeDir] = currentNeighbour;



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


        public void AddExit(Hex hex1, Hex hex2)
        {

            //Sets the exit in the correct rotation, offset from hex, and sets it both in the hex that
            //spawns it and in the hex that is connecting to it, in their exit dictionaries

            Exit exit = MakeExit(hex1.transform.position, hex2.transform.position);
            exit.Initialize(hex1, hex2);
            exit.SetColor(Color.black);

            bool isOpen = Random.value < exitProbability;
            if (isOpen) exit.Open(); else exit.Close();

            exit.transform.parent = hex1.transform;
            hex1.exits.Add(exit);
            hex2.exits.Add(exit);


        }

        Exit MakeExit(Vector3 hex1Pos, Vector3 hex2Pos)
        {
            Vector3 middlePos = (hex1Pos + hex2Pos) * 0.5f;
            Quaternion rotation = Quaternion.LookRotation(hex1Pos - hex2Pos, Vector3.up);

            GameObject exitGO = Locator.Instance.ObjectPooler.
                GetPooledExit(middlePos, rotation, transform);

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