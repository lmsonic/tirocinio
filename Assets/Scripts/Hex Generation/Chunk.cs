
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tirocinio
{
    public class Chunk : MonoBehaviour
    {
        public List<Hex> hexes = new List<Hex>();

        public GameObject exitPrefab;
        public GameObject hexPrefab;

        private ObjectPool<PoolObject> exitPool;

        private ObjectPool<PoolObject> hexPool;
        public Hex centerHex;


        static public float hexRadius = 10f;

        public float exitProbability = 0.5f;

        public int renderDistance = 4;

        public int regenerateHexDistance = 2;




        public void Start()
        {

            hexRadius *= transform.localScale.x;
            Debug.Log("Hex Radius:" + hexRadius);

            exitPool = new ObjectPool<PoolObject>(exitPrefab, 50);
            hexPool = new ObjectPool<PoolObject>(hexPrefab, 50);
            centerHex.Initialize(hexPool.Push);

            hexes.Add(centerHex);
            //StartCoroutine(GenerateHexesRecursive(centerHex, maxGenerationSteps));
            GenerateHexesIterative(centerHex, renderDistance);
        }


        public void RegenerateHexes(float newRenderDistance)
        {
            renderDistance = (int)newRenderDistance;

            Dictionary<Hex, int> distances = BFSDistances(hexes, centerHex);

            DeleteFarHexes(distances);
            GenerateCloseHexes(distances);
            GenerateExits();
        }

        public void MoveCenterHex(Hex newCenter)
        {

            Dictionary<Hex, int> distances = BFSDistances(hexes, newCenter);
            if (distances[centerHex] < regenerateHexDistance) return;
            centerHex = newCenter;

            DeleteFarHexes(distances);
            GenerateCloseHexes(distances);
            GenerateExits();


        }

        void GenerateCloseHexes(Dictionary<Hex, int> distances)
        {
            List<Hex> outerLayer = new List<Hex>();
            do
            {
                distances = BFSDistances(hexes, centerHex);
                outerLayer.Clear();
                foreach (Hex hex in hexes)
                {
                    if (distances[hex] < renderDistance)
                    {
                        foreach (Hex neighbour in hex.neighbours)
                            if (neighbour == null)
                                outerLayer.Add(hex);
                    }

                }

                for (int i = 0; i < outerLayer.Count; i++)
                {
                    GenerateHexes(outerLayer[i]);
                }

            }
            while (outerLayer.Count > 0);
        }

        void DeleteFarHexes(Dictionary<Hex, int> distances)
        {
            foreach (var pair in distances)
            {
                if (pair.Value >= renderDistance)
                {
                    Hex hex = pair.Key;
                    hex.ClearExits();
                    hex.ClearNeighbours();
                    hex.gameObject.SetActive(false);
                    hexes.Remove(hex);

                }
            }
        }





        Dictionary<Hex, int> BFSDistances(List<Hex> graph, Hex start)
        {

            if (!graph.Contains(start))
            {
                Debug.LogError("Start node is not present in graph");
                return null;
            }


            Dictionary<Hex, int> distances = new Dictionary<Hex, int>();


            var queue = new Queue<Hex>();
            queue.Enqueue(start);
            distances[start] = 0;

            while (queue.Count > 0)
            {
                Hex hex = queue.Dequeue();

                foreach (var neighbor in hex.neighbours)
                {
                    if (neighbor != null && !distances.ContainsKey(neighbor))
                    {
                        distances[neighbor] = distances[hex] + 1;
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return distances;

        }




        void GenerateHexesIterative(Hex pivotHex, int nSteps) // n = nSteps , N = number of hexes
        //O( 1 + 3n^2-3n + 18*N^2 )
        {
            //1+ 0,6,18,36,60,90 calls of GenerateHexes()
            List<Hex> outerLayer = GenerateHexes(pivotHex);//o(1)
            for (int i = 0; i < nSteps - 1; i++)// n = nSteps in this 
            //O(6*sigma(n-1)) = O(6*n*(n-1)/2) = O(3n^2-3n)
            {
                List<Hex> generatedHexes = new List<Hex>();
                for (int j = 0; j < outerLayer.Count; j++)
                {
                    List<Hex> hexLayer = GenerateHexes(outerLayer[j]);
                    generatedHexes.AddRange(hexLayer);
                }
                outerLayer = generatedHexes;
            }

            GenerateExits();//O(18*N^2) where N is number of hexes


        }





        void GenerateExits()//O(18*n^2)
        {
            for (int i = 0; i < hexes.Count; i++)//O(n^2)
            {
                for (int j = 0; j < hexes.Count; j++)//O(n)
                {
                    if (i == j) continue;
                    if (AreNeighbours(hexes[i], hexes[j]))//O(6)
                    {
                        if (!HaveExitConnection(hexes[i], hexes[j]))//O(12)
                        {
                            AddExit(hexes[i], hexes[j]);

                        }
                    }
                }
            }
        }

        bool HaveExitConnection(Hex hex1, Hex hex2)//O(12) worst case
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

        bool AreNeighbours(Hex hex1, Hex hex2)//O(6)
        {
            for (int i = 0; i < 6; i++)
            {
                if (hex1.neighbours[i] == hex2) return true;
                if (hex2.neighbours[i] == hex1) return true;
            }
            return false;
        }


        List<Hex> GenerateHexes(Hex pivotHex)//O(12)
        {
            Debug.Log("Call GenerateHexes()");
            Transform centerHexTransform = pivotHex.gameObject.transform;
            List<Hex> generatedHexes = new List<Hex>();
            for (int i = 0; i < 6; i++)//O(6)
            {
                if (pivotHex.neighbours[i] != null) continue;

                //generates hex in correct position
                Quaternion rotation = Quaternion.AngleAxis(-i * 60f, Vector3.up);

                ExitDirection direction = (ExitDirection)i;
                ExitDirection oppositeDirection = HelperEnums.GetOppositeDirection(direction);



                Hex hex = MakeHex(pivotHex.transform.position, direction);

                hexes.Add(hex);
                generatedHexes.Add(hex);
                pivotHex.neighbours[i] = hex;
                hex.neighbours[(int)oppositeDirection] = pivotHex;


            }
            ConnectNeighbouringHexes(pivotHex);//O(6)

            return generatedHexes;
        }

        void ConnectNeighbouringHexes(Hex pivotHex)//O(6)
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


        Hex MakeHex(Vector3 startPosition, ExitDirection direction)//O(1)
        {
            Quaternion rotation = Quaternion.AngleAxis(-(int)direction * 60f, Vector3.up);
            Vector3 offset = Vector3.forward * hexRadius * Mathf.Sqrt(3f);
            offset = rotation * offset;

            Hex hex =(Hex)hexPool.Pull(startPosition + offset, Quaternion.identity, transform);
            hex.transform.localScale = Vector3.one;

            return hex;
        }


        public void AddExit(Hex hex1, Hex hex2)//O(1)
        {

            //Sets the exit in the correct rotation, offset from hex, and sets it both in the hex that
            //spawns it and in the hex that is connecting to it, in their exit dictionaries

            Exit exit = MakeExit(hex1.transform.position, hex2.transform.position);
            exit.Init(hex1, hex2);
            exit.SetColor(Color.black);

            bool isOpen = Random.value < exitProbability;
            if (isOpen) exit.Open(); else exit.Close();

            exit.transform.parent = hex1.transform;
            hex1.exits.Add(exit);
            hex2.exits.Add(exit);

        }

        Exit MakeExit(Vector3 hex1Pos, Vector3 hex2Pos)//O(1)
        {

            Vector3 middlePos = (hex1Pos + hex2Pos) * 0.5f;
            Quaternion rotation = Quaternion.LookRotation(hex1Pos - hex2Pos, Vector3.up);

            Exit exit = (Exit)exitPool.Pull(middlePos, rotation, transform);
            exit.transform.localScale = Vector3.one;

            return exit;

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