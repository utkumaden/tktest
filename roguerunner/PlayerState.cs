using System;
using System.Collections.Generic;
using System.Text;

namespace roguerunner
{
    [Flags]
    public enum PlayerState
    {
        Headbump = 1,
        Grounded = 2,
        LeftStick = 4,
        RightStick = 8,
    }
}
