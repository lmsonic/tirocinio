using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Tirocinio
{

    public class HexGeneration : MonoBehaviour
    {


        public GameObject[] exits = new GameObject[6];
        public float exitProbability = 0.3f;
        int hexIndex = 0;

        // Start is called before the first frame update
        void Awake()
        {
            Color randomColor = Random.ColorHSV();
            foreach (GameObject exit in exits)
            {
                exit.GetComponent<Renderer>().material.color = randomColor;
            }

        }


        public void SetIndex(int index)
        {
            hexIndex = index;
        }
        public void GenerateHex()
        {
            for (int i = 0; i < exits.Length; i++)
            {
                exits[i].SetActive(false);
            }
            switch ((HexPosition)hexIndex)
            {
                case HexPosition.CENTER:
                    GenerateExits();
                    break;
                case HexPosition.UP:
                    GenerateExits(ExitDirection.SOUTHWEST,ExitDirection.SOUTHEAST);
                    break;
                case HexPosition.UP_LEFT:
                    GenerateExits(ExitDirection.SOUTH);
                    break;
                case HexPosition.DOWN_LEFT:
                    GenerateExits(ExitDirection.SOUTHEAST);
                    break;
                case HexPosition.DOWN:
                    GenerateExits(ExitDirection.NORTHEAST);
                    break;
                case HexPosition.DOWN_RIGHT:
                    GenerateExits(ExitDirection.NORTH);
                    break;
                case HexPosition.UP_RIGHT:
                    break;

            }
        }

        void GenerateExits()
        {

                for (int i = 0; i < exits.Length; i++)
                {
                    if (Random.value < exitProbability)
                    {
                        exits[i].SetActive(true);
                    }
                }


        }

        void GenerateExits(params ExitDirection[] exitDirections)
        {
            foreach (ExitDirection direction in exitDirections)
            {
                if (Random.value < exitProbability)
                {
                    exits[(int)direction].SetActive(true);
                }

            }
        }
    }

}
