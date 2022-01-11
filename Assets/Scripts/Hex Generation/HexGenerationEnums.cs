using UnityEngine;
namespace Tirocinio
{

    public enum ChunkPosition
    {
        CENTER, UP, UP_LEFT, DOWN_LEFT, DOWN, DOWN_RIGHT, UP_RIGHT, NONE
    }
    public enum HexPosition
    {
        CENTER, UP, UP_LEFT, DOWN_LEFT, DOWN, DOWN_RIGHT, UP_RIGHT, NONE
    }


    public enum ExitDirection
    {
        NORTH, NORTHWEST, SOUTHWEST, SOUTH, SOUTHEAST, NORTHEAST

    }

    class HelperEnums : MonoBehaviour
    {
        static public ExitDirection GetOppositeDirection(ExitDirection dir) => (ExitDirection)(((int)dir + 3) % 6);

        static public ChunkPosition GetAdjacentChunkPosition(ChunkPosition chunkPosition, ExitDirection direction)
        {
            switch (chunkPosition)
            {
                case ChunkPosition.CENTER:
                    return (ChunkPosition)((int)direction + 1);
                case ChunkPosition.UP:
                    switch (direction)
                    {
                        case ExitDirection.SOUTHWEST:
                            return ChunkPosition.UP_LEFT;
                        case ExitDirection.SOUTH:
                            return ChunkPosition.CENTER;
                        case ExitDirection.SOUTHEAST:
                            return ChunkPosition.UP_RIGHT;
                        default:
                            return ChunkPosition.NONE;
                    }
                case ChunkPosition.UP_LEFT:
                    switch (direction)
                    {
                        case ExitDirection.NORTHEAST:
                            return ChunkPosition.UP;
                        case ExitDirection.SOUTHEAST:
                            return ChunkPosition.CENTER;
                        case ExitDirection.SOUTH:
                            return ChunkPosition.DOWN_LEFT;
                        default:
                            return ChunkPosition.NONE;
                    }
                case ChunkPosition.DOWN_LEFT:
                    switch (direction)
                    {
                        case ExitDirection.NORTH:
                            return ChunkPosition.UP_LEFT;
                        case ExitDirection.NORTHEAST:
                            return ChunkPosition.CENTER;
                        case ExitDirection.SOUTHEAST:
                            return ChunkPosition.DOWN;
                        default:
                            return ChunkPosition.NONE;
                    }
                case ChunkPosition.DOWN:
                    switch (direction)
                    {
                        case ExitDirection.NORTHWEST:
                            return ChunkPosition.DOWN_LEFT;
                        case ExitDirection.NORTH:
                            return ChunkPosition.CENTER;
                        case ExitDirection.NORTHEAST:
                            return ChunkPosition.DOWN_RIGHT;
                        default:
                            return ChunkPosition.NONE;
                    }
                case ChunkPosition.DOWN_RIGHT:
                    switch (direction)
                    {
                        case ExitDirection.SOUTHWEST:
                            return ChunkPosition.DOWN;
                        case ExitDirection.NORTHWEST:
                            return ChunkPosition.CENTER;
                        case ExitDirection.NORTH:
                            return ChunkPosition.UP_RIGHT;
                        default:
                            return ChunkPosition.NONE;
                    }
                case ChunkPosition.UP_RIGHT:
                    switch (direction)
                    {
                        case ExitDirection.SOUTH:
                            return ChunkPosition.DOWN_RIGHT;
                        case ExitDirection.SOUTHWEST:
                            return ChunkPosition.CENTER;
                        case ExitDirection.NORTHWEST:
                            return ChunkPosition.UP;
                        default:
                            return ChunkPosition.NONE;
                    }
                default:
                    return ChunkPosition.NONE;

            }
        }

        static public HexPosition GetAdjacentHexPosition(HexPosition hexPosition, ExitDirection direction)
        {
            //Trying to understand the modular mathematics for this problem
            /*
            Debug.Log("hexPosition: " + hexPosition + ", direction: " + direction);


            if (hexPosition == HexPosition.CENTER)
            {
                Debug.Log("Adjacent From Center Position: " + (HexPosition)(direction + 1));
                return (HexPosition)(direction + 1);
            }



            ExitDirection directionFromCenter = (ExitDirection)( hexPosition - 1) ;
            ExitDirection directionToCenter = GetOppositeDirection(directionFromCenter);
            ExitDirection directionAfterCenter = (ExitDirection)(((int)directionToCenter + 1) % 6);
            ExitDirection directionBeforeCenter = (ExitDirection)(((int)directionToCenter - 1) % 6);
            Debug.Log("fromCenter: " + directionFromCenter + " ,toCenter: " + directionToCenter +
                      " ,afterCenter: " + directionAfterCenter + " ,beforeCenter: " + directionBeforeCenter);

            if (direction == directionToCenter)
            {
                Debug.Log("Adjacent Center Position: " + HexPosition.CENTER);
                return HexPosition.CENTER;
            }
            else if (direction == directionAfterCenter)
            {
                HexPosition nextPosition =  (HexPosition)(((int)hexPosition +1) % 7);
                if (nextPosition == HexPosition.CENTER)
                    nextPosition = HexPosition.UP;
                Debug.Log("Adjacent After Position: " + nextPosition);
                return nextPosition;
            }
            else if (direction == directionBeforeCenter)
            {
                HexPosition beforePosition =  (HexPosition)(((int)hexPosition -1) % 7);
                if (beforePosition == HexPosition.CENTER)
                    beforePosition = HexPosition.UP_RIGHT;
                Debug.Log("Adjacent After Position: " + beforePosition);
                return beforePosition;
            }
            else
            {
                Debug.Log("Adjacent Position: NOT FOUND");
                return HexPosition.NONE;
            }
            */

            //Brute forcing it
            switch (hexPosition)
            {
                case HexPosition.CENTER: // (0, d) -> d+1
                    return (HexPosition)((int)direction + 1);
                case HexPosition.UP:


                    switch (direction)
                    {
                        case ExitDirection.SOUTHWEST:
                            return HexPosition.UP_LEFT; // (1, 2) -> 2
                        case ExitDirection.SOUTH:
                            return HexPosition.CENTER; // (1, 3) -> 0
                        case ExitDirection.SOUTHEAST:
                            return HexPosition.UP_RIGHT; // (1, 4) -> 6
                        default:
                            return HexPosition.NONE;
                    }
                case HexPosition.UP_LEFT:
                    switch (direction)
                    {
                        case ExitDirection.SOUTH:
                            return HexPosition.DOWN_LEFT; // (2, 3) -> 3
                        case ExitDirection.NORTHEAST:
                            return HexPosition.UP; // (2, 5) -> 1
                        case ExitDirection.SOUTHEAST:
                            return HexPosition.CENTER; // (2, 4) -> 0
                        default:
                            return HexPosition.NONE;
                    }
                case HexPosition.DOWN_LEFT:
                    switch (direction)
                    {
                        case ExitDirection.NORTH:
                            return HexPosition.UP_LEFT;// (3, 0) -> 2
                        case ExitDirection.SOUTHEAST:
                            return HexPosition.DOWN;// (3, 4) -> 4
                        case ExitDirection.NORTHEAST:
                            return HexPosition.CENTER;// (3, 5) -> 0
                        default:
                            return HexPosition.NONE;
                    }
                case HexPosition.DOWN:
                    switch (direction)
                    {
                        case ExitDirection.NORTH:
                            return HexPosition.CENTER;// (4, 0) -> 0
                        case ExitDirection.NORTHWEST:
                            return HexPosition.DOWN_LEFT;// (4, 1) -> 3
                        case ExitDirection.NORTHEAST:
                            return HexPosition.DOWN_RIGHT;// (4, 5) -> 5
                        default:
                            return HexPosition.NONE;
                    }
                case HexPosition.DOWN_RIGHT:
                    switch (direction)
                    {
                        case ExitDirection.NORTH:
                            return HexPosition.UP_RIGHT;// (5, 0) -> 6
                        case ExitDirection.NORTHWEST:
                            return HexPosition.CENTER;// (5, 1) -> 0
                        case ExitDirection.SOUTHWEST:
                            return HexPosition.DOWN; // (5, 2) -> 4
                        default:
                            return HexPosition.NONE;
                    }
                case HexPosition.UP_RIGHT:
                    switch (direction)
                    {
                        case ExitDirection.NORTHWEST:
                            return HexPosition.UP; // (6, 1) -> 1
                        case ExitDirection.SOUTHWEST:
                            return HexPosition.CENTER; // (6, 2) -> 0
                        case ExitDirection.SOUTH:
                            return HexPosition.DOWN_RIGHT; // (6, 3) -> 5
                        default:
                            return HexPosition.NONE;
                    }
                default:
                    return HexPosition.NONE;

            }
        }

        static public HexPosition GetAdjacentHexPositionWithOtherChunks(HexPosition hexPosition, ExitDirection direction)
        {
            //Brute forcing it for connections with other chunks
            switch (hexPosition)
            {
                case HexPosition.CENTER: // (0, d) -> d+1
                    return (HexPosition)((int)direction + 1);
                case HexPosition.UP:
                    switch (direction)
                    {
                        case ExitDirection.SOUTHWEST:
                            return HexPosition.UP_LEFT; // (1, d) -> d+1
                        case ExitDirection.SOUTH:
                            return HexPosition.CENTER;
                        case ExitDirection.SOUTHEAST:
                            return HexPosition.UP_RIGHT;
                        case ExitDirection.NORTHWEST:
                            return HexPosition.DOWN;
                        case ExitDirection.NORTH:
                            return HexPosition.DOWN_RIGHT;
                        case ExitDirection.NORTHEAST:
                            return HexPosition.DOWN_LEFT;
                        default:
                            return HexPosition.NONE;
                    }
                case HexPosition.UP_LEFT:
                    switch (direction)
                    {
                        case ExitDirection.NORTHEAST:
                            return HexPosition.UP;
                        case ExitDirection.SOUTHEAST:
                            return HexPosition.CENTER;
                        case ExitDirection.SOUTH:
                            return HexPosition.DOWN_LEFT;
                        case ExitDirection.SOUTHWEST:
                            return HexPosition.DOWN_RIGHT;
                        case ExitDirection.NORTHWEST:
                            return HexPosition.UP_RIGHT;
                        case ExitDirection.NORTH:
                            return HexPosition.DOWN;
                        default:
                            return HexPosition.NONE;
                    }
                case HexPosition.DOWN_LEFT:
                    switch (direction)
                    {
                        case ExitDirection.NORTH:
                            return HexPosition.UP_LEFT;
                        case ExitDirection.NORTHEAST:
                            return HexPosition.CENTER;
                        case ExitDirection.SOUTHEAST:
                            return HexPosition.DOWN;
                        case ExitDirection.SOUTH:
                            return HexPosition.UP_RIGHT;
                        case ExitDirection.SOUTHWEST:
                            return HexPosition.UP;
                        case ExitDirection.NORTHWEST:
                            return HexPosition.DOWN_RIGHT;
                        default:
                            return HexPosition.NONE;
                    }
                case HexPosition.DOWN:
                    switch (direction)
                    {
                        case ExitDirection.NORTHWEST:
                            return HexPosition.DOWN_LEFT;
                        case ExitDirection.NORTH:
                            return HexPosition.CENTER;
                        case ExitDirection.NORTHEAST:
                            return HexPosition.DOWN_RIGHT;
                        case ExitDirection.SOUTHEAST:
                            return HexPosition.UP;
                        case ExitDirection.SOUTH:
                            return HexPosition.UP_LEFT;
                        case ExitDirection.SOUTHWEST:
                            return HexPosition.UP_RIGHT;
                        default:
                            return HexPosition.NONE;
                    }
                case HexPosition.DOWN_RIGHT:
                    switch (direction)
                    {
                        case ExitDirection.SOUTHWEST:
                            return HexPosition.DOWN;
                        case ExitDirection.NORTHWEST:
                            return HexPosition.CENTER;
                        case ExitDirection.NORTH:
                            return HexPosition.UP_RIGHT;
                        case ExitDirection.SOUTH:
                            return HexPosition.UP;
                        case ExitDirection.SOUTHEAST:
                            return HexPosition.DOWN_LEFT;
                        case ExitDirection.NORTHEAST:
                            return HexPosition.UP_LEFT;
                        default:
                            return HexPosition.NONE;
                    }
                case HexPosition.UP_RIGHT:
                    switch (direction)
                    {
                        case ExitDirection.SOUTH:
                            return HexPosition.DOWN_RIGHT;
                        case ExitDirection.SOUTHWEST:
                            return HexPosition.CENTER;
                        case ExitDirection.NORTHWEST:
                            return HexPosition.UP;
                        case ExitDirection.SOUTHEAST:
                            return HexPosition.UP_LEFT;
                        case ExitDirection.NORTHEAST:
                            return HexPosition.DOWN;
                        case ExitDirection.NORTH:
                            return HexPosition.DOWN_LEFT;
                        default:
                            return HexPosition.NONE;
                    }
                default:
                    return HexPosition.NONE;

            }


        }

    }
}




