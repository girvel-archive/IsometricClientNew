using System;
using UnityEngine;

namespace Assets.Code.Common
{
    public static class Keys
    {
        public static KeyCode GetKeyFromCoordinates(int x, int y)
        {
            switch (y)
            {
                case 0:
                    switch (x)
                    {
                        case 0:
                            return KeyCode.Q;
                        case 1:
                            return KeyCode.W;
                        case 2:
                            return KeyCode.E;
                        case 3:
                            return KeyCode.R;
                        case 4:
                            return KeyCode.T;
                    }
                    throw new ArgumentException();

                case 1:
                    switch (x)
                    {
                        case 0:
                            return KeyCode.A;
                        case 1:
                            return KeyCode.S;
                        case 2:
                            return KeyCode.D;
                        case 3:
                            return KeyCode.F;
                        case 4:
                            return KeyCode.G;
                    }
                    throw new ArgumentException();

                case 2:
                    switch (x)
                    {
                        case 0:
                            return KeyCode.Z;
                        case 1:
                            return KeyCode.X;
                        case 2:
                            return KeyCode.C;
                        case 3:
                            return KeyCode.V;
                        case 4:
                            return KeyCode.B;
                    }
                    throw new ArgumentException();

                default:
                    throw new ArgumentException();
            }
        }
    }
}