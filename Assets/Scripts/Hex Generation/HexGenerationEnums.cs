using UnityEngine;
namespace Tirocinio
{


    public enum ExitDirection
    {
        NORTH, NORTHWEST, SOUTHWEST, SOUTH, SOUTHEAST, NORTHEAST

    }

    class HelperEnums : MonoBehaviour
    {
        static public ExitDirection GetOppositeDirection(ExitDirection dir) => (ExitDirection)(((int)dir + 3) % 6);

        
    }
}




