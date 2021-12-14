using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGraph : MonoBehaviour
{

    public enum ExitDirection
    {
        NORTH, NORTHWEST, SOUTHWEST, SOUTH, SOUTHEAST, NORTHEAST
    }
    public class Hex
    {
        public Dictionary<ExitDirection, Exit> exits = new Dictionary<ExitDirection, Exit>();
        static public GameObject prefab;

        const float hexRadius = 18f;

        GameObject gameObject;

        public Hex(Vector3 position, Transform parent)
        {
            gameObject = Instantiate(prefab, position, Quaternion.identity, parent);
        }
        public void AddExit(ExitDirection direction)
        {
            Quaternion rotation = Quaternion.AngleAxis((int)direction * 60f, Vector3.up);
            Vector3 offset = rotation * Vector3.forward * hexRadius;
            Hex hex = new Hex(gameObject.transform.position + offset, gameObject.transform.parent);
            Exit exit = new Exit(this, hex, gameObject.transform.position + offset / 2, rotation, gameObject.transform);
            exits[direction] = exit;
            ExitDirection oppositeDirection = (ExitDirection)(((int)direction + 3) % 6);
            hex.exits[oppositeDirection] = exit;
        }


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


    public GameObject hexPrefab;
    public GameObject exitPrefab;

    Hex originalHex;
    public int generationSteps = 3;
    public float exitProbability = 0.5f;

    private void Start()
    {
        Hex.prefab = hexPrefab;
        Exit.prefab = exitPrefab;

        originalHex = new Hex(transform.position, transform);


        for (int i = 0; i < 6; i++)
        {
            ExitDirection direction = (ExitDirection)i;
            if (originalHex.exits.ContainsKey(direction)) continue;

            if (Random.value < exitProbability)
                originalHex.AddExit(direction);
        }
        foreach (KeyValuePair<ExitDirection, Exit> entry in originalHex.exits)
        {
            ExitDirection direction = entry.Key;
            Exit exit = entry.Value;

            Hex otherHex = (exit.hex1 == originalHex) ? exit.hex2 : exit.hex1;

            for (int i = 0; i < 6; i++)
            {
                ExitDirection dir = (ExitDirection)i;
                if (otherHex.exits.ContainsKey(dir)) continue;

                if (Random.value < exitProbability)
                    otherHex.AddExit(dir);
            }
        }

    }

}



