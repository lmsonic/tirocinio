using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tirocinio
{
    public class Hex : MonoBehaviour
    {
        public Dictionary<ExitDirection, Exit> exits = new Dictionary<ExitDirection, Exit>();


        static public float hexRadius = 8f;

        public HexPosition hexPosition;

        
        public void SetHexPosition(HexPosition pos)
        {
            hexPosition = pos;
        }

        public void AddExit(ExitDirection direction, Hex otherHex, bool isOpen)
        {
            Quaternion rotation = Quaternion.AngleAxis(-(int)direction * 60f, Vector3.up);
            Vector3 offset = rotation * Vector3.forward * hexRadius;

            GameObject exitGO = Locator.Instance.ObjectPooler.
                GetPooledExit(transform.position + offset, rotation, transform);
            
            Exit exit = exitGO.GetComponent<Exit>();
            exit.Initialize(this, otherHex);

            if (isOpen) exit.Open(); else exit.Close();

            exits[direction] = exit;
            ExitDirection oppositeDirection = (ExitDirection)(((int)direction + 3) % 6);
            otherHex.exits[oppositeDirection] = exit;
        }

        public void ClearExits()
        {
            foreach (KeyValuePair<ExitDirection, Exit> entry in exits)
            {
                Exit exit = entry.Value;
                Hex otherHex = exit.GetOtherHex(this);
                ExitDirection oppositeDirection = (ExitDirection)(((int)entry.Key + 3) % 6);
                otherHex.exits.Remove(oppositeDirection);
                exit.gameObject.SetActive(false);
            }
            exits.Clear();
        }

        private void OnDisable()
        {
            ClearExits();
        }

        public HexPosition GetAdjacentHexPosition(ExitDirection direction)
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


    }

}
