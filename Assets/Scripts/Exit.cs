using UnityEngine;
namespace Tirocinio
{
    public class Exit : MonoBehaviour
    {
        public Hex hex1, hex2;

        public void Initialize(Hex h1, Hex h2)
        {
            hex1 = h1;
            hex2 = h2;
        }

    }
}