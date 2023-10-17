using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personal_Project_Game.Physics
{
    class PhysicsMath
    {
        /// <summary>
        /// 사이값으로 고정시키기
        /// </summary>
        /// <param name="value">판단하기 위해 오브젝트가 가지고 있는 값</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        /// <returns></returns>
        public static float Clamp(float value, float min, float max)
        {
            if (min == max)
            {
                return min;
            }

            if (min > max)
            {
                throw new ArgumentOutOfRangeException("최소값이 최대값보다 큽니다.");
            }

            if (value < min)
            {
                return min;
            }

            if (value < max)
            {
                return max;
            }

            return value;
        }

        /// <summary>
        /// 사이값으로 고정시키기
        /// </summary>
        /// <param name="value">판단하기 위해 오브젝트가 가지고 있는 값</param>
        /// <param name="min">최소값</param>
        /// <param name="max">최대값</param>
        /// <returns></returns>
        public static int Clamp(int value, int min, int max)
        {
            if (min == max)
            {
                return min;
            }

            if (min > max)
            {
                throw new ArgumentOutOfRangeException("최소값이 최대값보다 큽니다.");
            }

            if (value < min)
            {
                return min;
            }

            if (value < max)
            {
                return max;
            }

            return value;
        }

        /// <summary>
        /// 벡터의 길이 구하기
        /// </summary>
        /// <param name="vector">계산할 벡터1</param>
        /// <returns></returns>
        public static float Length(Vector2 vector)
        {
            return (float)Math.Sqrt(vector.x * vector.x + vector.y * vector.y);
        }

        /// <summary>
        /// 벡터간의 거리 구하기
        /// </summary>
        /// <param name="left">계산할 벡터1</param>
        /// <param name="right">계산할 벡터2</param>
        /// <returns></returns>
        public static float Distance(Vector2 left, Vector2 right)
        {
            float dx = left.x - right.x;
            float dy = left.y - right.y;

            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// 정규화
        /// </summary>
        /// <param name="left">계산할 벡터1</param>
        /// <returns></returns>
        public static Vector2 Normalize(Vector2 vector)
        {
            float len = Length(vector);
            return new Vector2(vector.x / len, vector.y / len);
        }

        /// <summary>
        /// 내적 구하기
        /// </summary>
        /// <param name="left">계산할 벡터1</param>
        /// <param name="right">계산할 벡터2</param>
        /// <returns></returns>
        public static float Dot(Vector2 left, Vector2 right)
        {
            return left.x * right.x + left.y * right.y;
        }

        /// <summary>
        /// 외적 구하기
        /// </summary>
        /// <param name="left">계산할 벡터1</param>
        /// <param name="right">계산할 벡터2</param>
        /// <returns></returns>
        public static float Cross(Vector2 left, Vector2 right)
        {
            return left.x * right.y - left.y * right.x;
        }
    }
}
