using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Personal_Project_Game.Object;

namespace Personal_Project_Game.Physics
{
    public struct Vector2
    {
        public float x, y;
        public static Vector2 zero = new Vector2(0, 0);

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Vector끼리 더하기
        /// </summary>
        /// <param name="left">계산할 벡터1</param>
        /// <param name="right">계산할 벡터2</param>
        /// <returns></returns>
        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            return new Vector2(left.x + right.x, left.y + right.y);
        }

        /// <summary>
        /// Vector끼리 빼기
        /// </summary>
        /// <param name="left">계산할 벡터1</param>
        /// <param name="right">계산할 벡터2</param>
        /// <returns></returns>
        public static Vector2 operator -(Vector2 left, Vector2 right)
        {
            return new Vector2(left.x - right.x, left.y - right.y);
        }

        /// <summary>
        /// Vector 음수로
        /// </summary>
        /// <param name="left">계산할 벡터1</param>
        /// <returns></returns>
        public static Vector2 operator -(Vector2 left)
        {
            return new Vector2(-left.x, -left.y);
        }

        /// <summary>
        /// Vector끼리 곱하기
        /// </summary>
        /// <param name="left">계산할 벡터1</param>
        /// <param name="right">계산할 벡터2</param>
        /// <returns></returns>
        public static Vector2 operator *(Vector2 left, Vector2 right)
        {
            return new Vector2(left.x * right.x, left.y * right.y);
        }

        /// <summary>
        /// Vector와 float의 곱계산
        /// </summary>
        /// <param name="left">계산할 벡터1</param>
        /// <param name="power">계산할 float값</param>
        /// <returns></returns>
        public static Vector2 operator *(Vector2 left, float power)
        {
            return new Vector2(left.x * power, left.y * power);
        }

        public static Vector2 operator *(float power, Vector2 left)
        {
            return new Vector2(left.x * power, left.y * power);
        }

        /// <summary>
        /// Vector끼리 나누기
        /// </summary>
        /// <param name="left">계산할 벡터1</param>
        /// <param name="right">계산할 벡터2</param>
        /// <returns></returns>
        public static Vector2 operator /(Vector2 left, Vector2 right)
        {
            return new Vector2(left.x / right.x, left.y / right.y);
        }

        /// <summary>
        /// Vector와 flaot의 나눗셈계산
        /// </summary>
        /// <param name="left">계산할 벡터1</param>
        /// <param name="power">계산할 float값</param>
        /// <returns></returns>
        public static Vector2 operator /(Vector2 left, float power)
        {
            return new Vector2(left.x / power, left.y / power);
        }

        public static Vector2 Transform(Vector2 vector, Transform transform)
        {
            // 2D 회전 공식
            // x1 = (x0 * cos) - (y0 * sin), y1 = (x0 * sin) + (y0 * cos)

            return new Vector2(
                vector.x * transform.cos - vector.y * transform.sin + transform.positionX,
                vector.x * transform.sin + vector.y * transform.cos + transform.positionY
                );
        }

        public bool Equals(Vector2 otherVector)
        {
            return this.x == otherVector.x && this.y == otherVector.y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vector2 other)
            {
                return this.Equals(other);
            }

            return base.Equals(obj);
        }
    }
}
