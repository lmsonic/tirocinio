using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Tirocinio
{

    public class Locator : MonoBehaviour
    {
        // Start is called before the first frame update
        static public Locator Instance = null;

        public ObjectPooler ObjectPooler;
        public HexGeneration HexGeneration;
        private void Awake()
        {
            if (Instance)
                Destroy(gameObject);
            else
                Instance = this;

        }
    }
}
