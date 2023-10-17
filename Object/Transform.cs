using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Personal_Project_Game.Physics;

namespace Personal_Project_Game.Object
{
    public struct Transform
    {
        public float positionX;
        public float positionY;
        public float sin;
        public float cos;

        public static Transform zero = new Transform(0.0f, 0.0f, 0.0f);

        public Transform(Vector2 position, float angle)
        {
            positionX = position.x;
            positionY = position.y;
            sin = (float)Math.Sin(angle);
            cos = (float)Math.Cos(angle);
        }

        public Transform(float x, float y, float angle)
        {
            positionX = x;
            positionY = y;
            sin = (float)Math.Sin(angle);
            cos = (float)Math.Cos(angle);
        }
    }
}
