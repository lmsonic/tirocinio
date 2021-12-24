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

    class HelperEnums
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
            switch (hexPosition)
            {
                case HexPosition.CENTER:
                    return (HexPosition)((int)direction + 1);
                case HexPosition.UP:
                    switch (direction)
                    {
                        case ExitDirection.SOUTHWEST:
                            return HexPosition.UP_LEFT;
                        case ExitDirection.SOUTH:
                            return HexPosition.CENTER;
                        case ExitDirection.SOUTHEAST:
                            return HexPosition.UP_RIGHT;
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
                        default:
                            return HexPosition.NONE;
                    }
                default:
                    return HexPosition.NONE;

            }
        }

        static public HexPosition GetAdjacentHexPositionWithOtherChunks(HexPosition hexPosition, ExitDirection direction)
        {
            switch (hexPosition)
            {
                case HexPosition.CENTER:
                    return (HexPosition)((int)direction + 1);
                case HexPosition.UP:
                    switch (direction)
                    {
                        case ExitDirection.SOUTHWEST:
                            return HexPosition.UP_LEFT;
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




