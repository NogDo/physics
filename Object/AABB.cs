using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Personal_Project_Game.Physics;

namespace Personal_Project_Game.Object
{
    public struct AABB
    {
        public Vector2 Min;
        public Vector2 Max;

        public AABB(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }

        public AABB(float minX, float minY, float maxX, float maxY)
        {
            Min = new Vector2(minX, minY);
            Max = new Vector2(maxX, maxY);
        }
    }
}
