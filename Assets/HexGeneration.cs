using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Tirocinio
{

    public class HexGeneration : MonoBehaviour
    {


        const int CENTER_HEX_INDEX = 0;
        const int TOP_HEX_INDEX = 1;
        const int TOP_RIGHT_HEX_INDEX = 6;
        public GameObject[] exits = new GameObject[6];
        public float exitProbability = 0.3f;
        public int hexIndex = 0;

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
                    GenerateExits(ExitDirection.DOWN_LEFT,ExitDirection.DOWN_RIGHT);
                    break;
                case HexPosition.UP_LEFT:
                    GenerateExits(ExitDirection.DOWN);
                    break;
                case HexPosition.DOWN_LEFT:
                    GenerateExits(ExitDirection.DOWN_RIGHT);
                    break;
                case HexPosition.DOWN:
                    GenerateExits(ExitDirection.UP_RIGHT);
                    break;
                case HexPosition.DOWN_RIGHT:
                    GenerateExits(ExitDirection.UP);
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
