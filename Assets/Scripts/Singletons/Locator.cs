using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Tirocinio
{

    public class Locator : MonoBehaviour
    {
        //This class is used as a singleton to locate objects that function as singleton, as members of this class
        static public Locator Instance = null;

        public Chunk Chunk;
        public GrassPainter GrassPainter;

        private void Awake()
        {
            if (Instance)
                Destroy(gameObject);
            else
                Instance = this;

        }

    }
}
