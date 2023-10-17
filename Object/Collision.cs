using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Personal_Project_Game.Physics;

namespace Personal_Project_Game.Object
{
    /// <summary>
    /// 충돌 강체
    /// </summary>
    public static class Collision
    {
        /// <summary>
        /// 원끼리 충돌체크
        /// </summary>
        /// <param name="centerA">원1의 중심좌표</param>
        /// <param name="radiusA">원1의 반지름</param>
        /// <param name="centerB">원2의 중심좌표</param>
        /// <param name="radiusB">원2의 반지름</param>
        /// <param name="normal">법선</param>
        /// <param name="depth">충돌 깊이</param>
        /// <returns></returns>
        public static bool CrossCircle(Vector2 centerA, float radiusA, Vector2 centerB, float radiusB, out Vector2 normal, out float depth)
        {
            normal = Vector2.zero;
            depth = 0.0f;

            float distance = PhysicsMath.Distance(centerA, centerB);
            float radiusSum = radiusA + radiusB;

            // 두개의 원의 중심사이의 거리가 두개의 원의 반지름의 합보다 큰 경우에는 두 원이 충돌하지 않은 판정이다. 고로 false값을 리턴
            if (distance >= radiusSum)
            {
                return false;
            }

            normal = PhysicsMath.Normalize(centerB - centerA);
            depth = radiusSum - distance;

            return true;
        }

        /// <summary>
        /// 다각형끼리 충돌체크
        /// </summary>
        /// <param name="centerA">다각형 A의 중심좌표</param>
        /// <param name="vertexA">다각형 A의 꼭짓점들</param>
        /// <param name="centerB">다각형 B의 중심좌표</param>
        /// <param name="vertexB">다각형 B의 꼭짓점들</param>
        /// <param name="normal">법선</param>
        /// <param name="depth">충돌 깊이</param>
        /// <returns></returns>
        public static bool CrossPolygon(Vector2 centerA, Vector2[] vertexA, Vector2 centerB, Vector2[] vertexB, out Vector2 normal, out float depth)
        {
            normal = Vector2.zero;
            depth = float.MaxValue;

            for (int i = 0; i < vertexA.Length; i++)
            {
                Vector2 vectorA = vertexA[i];
                Vector2 vectorB = vertexA[(i + 1) % vertexA.Length];

                Vector2 edge = vectorB - vectorA;
                Vector2 axis = new Vector2(-edge.y, edge.x);
                axis = PhysicsMath.Normalize(axis);

                ProjectVertex(vertexA, axis, out float minA, out float maxA);
                ProjectVertex(vertexB, axis, out float minB, out float maxB);

                if (minA >= maxB || minB >= maxA)
                {
                    return false;
                }

                float axisDepth = Math.Min(maxB - minA, maxA - minB);

                if (axisDepth < depth)
                {
                    depth = axisDepth;
                    normal = axis;
                }
            }

            for (int i = 0; i < vertexB.Length; i++)
            {
                Vector2 vectorA = vertexB[i];
                Vector2 vectorB = vertexB[(i + 1) % vertexB.Length];

                Vector2 edge = vectorB - vectorA;
                Vector2 axis = new Vector2(-edge.y, edge.x);
                axis = PhysicsMath.Normalize(axis);

                ProjectVertex(vertexA, axis, out float minA, out float maxA);
                ProjectVertex(vertexB, axis, out float minB, out float maxB);

                if (minA >= maxB || minB >= maxA)
                {
                    return false;
                }

                float axisDepth = Math.Min(maxB - minA, maxA - minB);

                if (axisDepth < depth)
                {
                    depth = axisDepth;
                    normal = axis;
                }
            }

            Vector2 direction = centerB - centerA;
            if (PhysicsMath.Dot(direction, normal) < 0.0f)
            {
                normal = -normal;
            }

            return true;
        }

        /// <summary>
        /// 원과 다각형 충돌체크
        /// </summary>
        /// <param name="centerA">원A의 중심좌표</param>
        /// <param name="radiusA">원A의 반지름</param>
        /// <param name="centerB">다각형B의 중심좌표</param>
        /// <param name="vertexB">다각형B의 꼭짓점들</param>
        /// <param name="normal">법선</param>
        /// <param name="depth">충돌 깊이</param>
        /// <returns></returns>
        public static bool CrossCirclePolygon(Vector2 centerA, float radiusA, Vector2 centerB, Vector2[] vertexB, out Vector2 normal, out float depth)
        {
            normal = Vector2.zero;
            depth = float.MaxValue;
            Vector2 axis = Vector2.zero;
            float axisDepth = 0f;
            float minA, maxA, minB, maxB;

            for (int i = 0; i < vertexB.Length; i++)
            {
                Vector2 vectorA = vertexB[i];
                Vector2 vectorB = vertexB[(i + 1) % vertexB.Length];

                Vector2 edge = vectorB - vectorA;
                axis = new Vector2(-edge.y, edge.x);
                axis = PhysicsMath.Normalize(axis);

                ProjectVertex(vertexB, axis, out minA, out maxA);
                ProjectCircle(centerA, radiusA, axis, out minB, out maxB);

                if (minA >= maxB || minB >= maxA)
                {
                    return false;
                }

                axisDepth = Math.Min(maxB - minA, maxA - minB);

                if (axisDepth < depth)
                {
                    depth = axisDepth;
                    normal = axis;
                }
            }

            int index = FindClosestPointIndex(centerA, vertexB);
            Vector2 closestPoint = vertexB[index];

            axis = closestPoint - centerA;
            axis = PhysicsMath.Normalize(axis);

            ProjectVertex(vertexB, axis, out minA, out maxA);
            ProjectCircle(centerA, radiusA, axis, out minB, out maxB);

            if (minA >= maxB || minB >= maxA)
            {
                return false;
            }

            axisDepth = Math.Min(maxB - minA, maxA - minB);

            if (axisDepth < depth)
            {
                depth = axisDepth;
                normal = axis;
            }

            Vector2 direction = centerB - centerA;
            if (PhysicsMath.Dot(direction, normal) < 0.0f)
            {
                normal = -normal;
            }

            return true;
        }

        /// <summary>
        /// 강체의 최소, 최대 꼭지점찾기
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="axis"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        private static void ProjectVertex(Vector2[] vertex, Vector2 axis, out float min, out float max)
        {
            min = float.MaxValue;
            max = float.MinValue;

            for (int i = 0; i < vertex.Length; i++)
            {
                Vector2 vector = vertex[i];
                float proj = PhysicsMath.Dot(vector, axis);

                if (proj < min)
                {
                    min = proj;
                }

                if (proj > max)
                {
                    max = proj;
                }
            }
        }

        /// <summary>
        /// 강체와 가장 가까운 점의 인덱스 찾기
        /// </summary>
        /// <param name="circleCenter"></param>
        /// <param name="vertex"></param>
        /// <returns></returns>
        private static int FindClosestPointIndex(Vector2 circleCenter, Vector2[] vertex)
        {
            int index = -1;
            float minDistance = float.MaxValue;

            for (int i = 0; i < vertex.Length; i++)
            {
                Vector2 vector = vertex[i];
                float distance = PhysicsMath.Distance(circleCenter, vector);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    index = i;
                }
            }

            return index;
        }

        /// <summary>
        /// 강체의 최소, 최대 점 찾기
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <param name="axis"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        private static void ProjectCircle(Vector2 center, float radius, Vector2 axis, out float min, out float max)
        {
            Vector2 direction = PhysicsMath.Normalize(axis);
            Vector2 directionRadius = direction * radius;

            Vector2 point1 = center + directionRadius;
            Vector2 point2 = center - directionRadius;

            min = PhysicsMath.Dot(point1, axis);
            max = PhysicsMath.Dot(point2, axis);

            if (min > max)
            {
                float temp = min;
                min = max;
                max = temp;
            }
        }
    }
}
