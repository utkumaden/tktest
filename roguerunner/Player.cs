using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using OpenTK.Input;

namespace roguerunner
{
    public class Player
    {
        public PointF Location;
        public PlayerState State;
        public static readonly SizeF Size = new SizeF(0.1f, 0.1f);
        public RectangleF BoundingBox => new RectangleF(Location, Size);

        public bool Grounded { get => State.HasFlag(PlayerState.Grounded); set => State = value ? State | PlayerState.Grounded : State & ~PlayerState.Grounded; }
        public bool Headbump { get => State.HasFlag(PlayerState.Headbump); set => State = value ? State | PlayerState.Headbump : State & ~PlayerState.Headbump; }
        public bool LeftStick { get => State.HasFlag(PlayerState.LeftStick); set => State = value ? State | PlayerState.LeftStick : State & ~PlayerState.LeftStick; }
        public bool RightStick { get => State.HasFlag(PlayerState.RightStick); set => State = value ? State | PlayerState.RightStick : State & ~PlayerState.RightStick; }

        public float VerticalSpeed;
        public float HorizontalSpeed;

        public const float UpGravity = -0.002f;
        public const float DownGravity = -0.004f;

        public const float UpSpeed = 0.03f;
        public const float DownSpeedCap = -0.06f;

        public const float StrafeAcceleration = 0.01f;
        public const float StrafeSlow = 0.015f;
        public const float StrafeCap = 0.025f;

        public const float WallSpeed = 30f;
        public const float WallSlip = 10f;

        public void Jump()
        {
            VerticalSpeed = UpSpeed;
        }

        public void WallJumpLeft()
        {
            VerticalSpeed = UpSpeed;
            HorizontalSpeed = -WallSpeed;
        }

        public void WallJumpRight()
        {
            VerticalSpeed = UpSpeed;
            HorizontalSpeed = WallSpeed;
        }

        public void Upkeep(bool jumping, float running)
        {
            if (jumping)
            {
                VerticalSpeed += UpGravity;
            }
            else
            {
                VerticalSpeed += DownGravity;
            }

            if (VerticalSpeed < DownSpeedCap)
            {
                VerticalSpeed = DownSpeedCap;
            }

            if (running == 0)
            {
                HorizontalSpeed -= Math.Sign(HorizontalSpeed) * StrafeSlow;
                if (Math.Abs(HorizontalSpeed) < StrafeSlow)
                    HorizontalSpeed = 0;
            }
            else
            {
                HorizontalSpeed += StrafeAcceleration * running;
            }

            if (Math.Abs(HorizontalSpeed) > StrafeCap)
            {
                HorizontalSpeed = Math.Sign(HorizontalSpeed) * StrafeCap;
            }

            if (Headbump)
                VerticalSpeed = -VerticalSpeed;
            if (Grounded)
                VerticalSpeed = 0;
            if (LeftStick || RightStick)
                HorizontalSpeed = 0;

            Location.X += HorizontalSpeed;
            Location.Y += VerticalSpeed;
        }
    }
}
