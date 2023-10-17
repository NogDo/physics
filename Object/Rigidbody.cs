using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Personal_Project_Game.Physics;

namespace Personal_Project_Game.Object
{

    public enum Shape
    {
        Circle = 0,
        Box = 1
    }

    public class Rigidbody
    {
        private Vector2 position;
        private Vector2 linearVelocity;
        private float rotation;
        private float rotationalVelocity;

        private Vector2 force;

        public float density;
        public float mass;
        public float inverseMass;
        public float restitution;
        public float area;

        public bool isStatic;

        public float radius;
        public float width;
        public float height;

        public Shape shape;

        public Vector2[] vertex;
        public Vector2[] changedVertex;
        public AABB aabb;

        private bool transformUpdate;
        private bool aabbUpdate;

        /// <summary>
        /// position값 가져오기, 바꾸기
        /// </summary>
        public Vector2 Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        public Vector2 LinearVelocity
        {
            get { return this.linearVelocity; }
            set { this.linearVelocity = value; }
        }

        /// <summary>
        /// 물리 강체 만들기
        /// </summary>
        /// <param name="position">강체의 포지션 값</param>
        /// <param name="density">밀도</param>
        /// <param name="mass">질량</param>
        /// <param name="restitution">반환</param>
        /// <param name="area">영역</param>
        /// <param name="isStatic">멈춰있는지 여부</param>
        /// <param name="radius">반지름 (원)</param>
        /// <param name="width">가로 길이 (박스)</param>
        /// <param name="height">세로 길이 (박스)</param>
        /// <param name="shape">모양</param>
        private Rigidbody(Vector2 position, float density, float mass, float restitution, float area, bool isStatic, float radius, float width, float height, Shape shape)
        {
            this.position = position;
            this.linearVelocity = Vector2.zero;
            this.rotation = 0.0f;
            this.rotationalVelocity = 0.0f;

            this.force = Vector2.zero;

            this.density = density;
            this.mass = mass;
            this.restitution = restitution;
            this.area = area;

            this.isStatic = isStatic;
            this.radius = radius;
            this.width = width;
            this.height = height;

            this.shape = shape;

            if (!this.isStatic)
            {
                this.inverseMass = 1.0f / this.mass;
            }
            else
            {
                this.inverseMass = 0.0f;
            }

            if(shape == Shape.Box)
            {
                this.vertex = CreateBoxVertex(this.width, this.height);
                this.changedVertex = new Vector2[this.vertex.Length];
            }
            else
            {
                this.vertex = null;
                this.changedVertex = null;
            }

            this.transformUpdate = true;
            this.aabbUpdate = true;
        }

        /// <summary>
        /// 박스 강체의 꼭짓점 구하기
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Vector2[] CreateBoxVertex(float width, float height)
        {
            float left = -width / 2.0f;
            float right = left + width;
            float bottom = -height / 2.0f;
            float top = bottom + height;

            Vector2[] vertex = new Vector2[4];
            vertex[0] = new Vector2(left, top);
            vertex[1] = new Vector2(right, top);
            vertex[2] = new Vector2(right, bottom);
            vertex[3] = new Vector2(left, bottom);

            return vertex;
        }

        /// <summary>
        /// 회전, 이동으로 인해 변한 꼭지점의 값을 최신화
        /// </summary>
        /// <returns></returns>
        public Vector2[] GetChangedVertex()
        {
            if (this.transformUpdate)
            {
                Transform transform = new Transform(this.position, this.rotation);

                for(int i = 0; i < this.vertex.Length; i++)
                {
                    Vector2 v = this.vertex[i];
                    this.changedVertex[i] = Vector2.Transform(v, transform);
                }
            }

            this.transformUpdate = false;
            return this.changedVertex;
        }

        /// <summary>
        /// 강체가 가지고있는 AABB값 가져오기
        /// </summary>
        /// <returns></returns>
        public AABB GetAABB()
        {
            if (aabbUpdate)
            {
                float minX = float.MaxValue;
                float minY = float.MaxValue;
                float maxX = float.MinValue;
                float maxY = float.MinValue;

                if (this.shape == Shape.Box)
                {
                    Vector2[] vertex = GetChangedVertex();

                    for (int i = 0; i < vertex.Length; i++)
                    {
                        Vector2 vector = vertex[i];

                        if (vector.x < minX)
                        {
                            minX = vector.x;
                        }

                        if (vector.x > maxX)
                        {
                            maxX = vector.x;
                        }

                        if (vector.y < minY)
                        {
                            minY = vector.y;
                        }

                        if (vector.y > maxY)
                        {
                            maxY = vector.y;
                        }
                    }
                }
                else if (this.shape == Shape.Circle)
                {
                    minX = this.position.x - this.radius;
                    minY = this.position.y - this.radius;
                    maxX = this.position.x + this.radius;
                    maxY = this.position.y + this.radius;
                }

                this.aabb =  new AABB(minX, minY, maxX, maxY);
            }

            this.aabbUpdate = false;
            return this.aabb;
        }

        /// <summary>
        /// 강체 선형이동
        /// </summary>
        /// <param name="time">전프레임과 지금프레임의 간격</param>
        /// <param name="gravity">중력 값</param>
        /// <param name="interations">정확한 물리엔진을 위해 time을 나눠주는값</param>
        public void Step(float time, Vector2 gravity, int interations)
        {
            if (this.isStatic) 
            { 
                return; 
            }

            time /= (float)interations;

            // F = ma, 힘 = 질량 * 가속도 >> 이건 중력이 없고, 캐릭터를 직접 움직일 때 사용
            //Vector2 acceleration = this.force / this.mass;
            //this.linearVelocity += acceleration * time;

            this.linearVelocity += gravity * time;
            this.position += this.linearVelocity * time;
            this.linearVelocity -= linearVelocity * 0.0001f; // 공기마찰...
            //Console.WriteLine(linearVelocity);

            this.rotation += this.rotationalVelocity * time;

            this.force = Vector2.zero;
            this.transformUpdate = true;
            this.aabbUpdate = true;
        }

        /// <summary>
        /// 강체 이동
        /// </summary>
        /// <param name="amount"></param>
        public void Move(Vector2 amount)
        {
            this.position += amount;
            this.transformUpdate = true;
            this.aabbUpdate = true;
        }

        /// <summary>
        /// 강체가 원하는 포지션값으로 이동
        /// </summary>
        /// <param name="position"></param>
        public void MoveTo(Vector2 position)
        {
            this.position = position;
            this.transformUpdate = true;
            this.aabbUpdate = true;
        }
        
        /// <summary>
        /// 강체 회전
        /// </summary>
        /// <param name="amount"></param>
        public void Rotate(float amount)
        {
            this.rotation += amount;
            this.transformUpdate = true;
            this.aabbUpdate = true;
        }

        /// <summary>
        /// 강체에 물리적 힘을 가함
        /// </summary>
        /// <param name="amount"></param>
        public void AddForce(Vector2 amount)
        {
            this.force = amount;
        }

        /// <summary>
        /// 원 강체 만들기
        /// </summary>
        /// <param name="radius">반지름</param>
        /// <param name="position">위치값</param>
        /// <param name="density">밀도</param>
        /// <param name="isStatic">움직이는 개체인지 아닌지</param>
        /// <param name="restitution">반환</param>
        /// <param name="rigidbody">강체</param>
        /// <param name="Error">에러메세지</param>
        /// <returns></returns>
        public static bool CreateCircleBody(float radius, Vector2 position, float density, bool isStatic, float restitution, out Rigidbody rigidbody, out string Error)
        {
            rigidbody = null;
            Error = string.Empty;

            float area = radius * radius * (float)Math.PI;

            if (area < GameWorld.minBodySize)
            {
                Error = "원이 너무 작습니다. 최소크기 >> " + GameWorld.minBodySize.ToString();
                return false;
            }

            if (area > GameWorld.maxBodySize)
            {
                Error = "원이 너무 큽니다. 최대크기 >> " + GameWorld.maxBodySize.ToString();
                return false;
            }

            if (density < GameWorld.minDensity)
            {
                Error = "밀도가 너무 작습니다. 최소밀도 >> " + GameWorld.minDensity.ToString();
                return false;
            }

            if (density > GameWorld.maxDensity)
            {
                Error = "밀도가 너무 큽니다. 최대밀도 >> " + GameWorld.maxDensity.ToString();
                return false;
            }

            restitution = PhysicsMath.Clamp(restitution, 0.0f, 1.0f);

            // 질량 = 면적 * 깊이 * 밀도
            float mass = area * density;

            rigidbody = new Rigidbody(position, density, mass, restitution, area, isStatic, radius, 0.0f, 0.0f, Shape.Circle);

            return true;
        }

        /// <summary>
        /// 박스 강체 만들기
        /// </summary>
        /// <param name="width">너비</param>
        /// <param name="height">높이</param>
        /// <param name="position">위치값</param>
        /// <param name="density">밀도</param>
        /// <param name="isStatic">움직이는 개체인지 아닌지</param>
        /// <param name="restitution">반환</param>
        /// <param name="rigidbody">강체</param>
        /// <param name="Error">에러 메세지</param>
        /// <returns></returns>
        public static bool CreateBoxBody(float width, float height, Vector2 position, float density, bool isStatic, float restitution, out Rigidbody rigidbody, out string Error)
        {
            rigidbody = null;
            Error = string.Empty;

            float area = width * height;

            if (area < GameWorld.minBodySize)
            {
                Error = "박스가 너무 작습니다. 최소크기 >> " + GameWorld.minBodySize.ToString();
                return false;
            }

            if (area > GameWorld.maxBodySize)
            {
                Error = "박스가 너무 큽니다. 최대크기 >> " + GameWorld.maxBodySize.ToString();
                return false;
            }

            if (density < GameWorld.minDensity)
            {
                Error = "밀도가 너무 작습니다. 최소밀도 >> " + GameWorld.minDensity.ToString();
                return false;
            }

            if (density > GameWorld.maxDensity)
            {
                Error = "밀도가 너무 큽니다. 최대밀도 >> " + GameWorld.maxDensity.ToString();
                return false;
            }

            restitution = PhysicsMath.Clamp(restitution, 0.0f, 1.0f);

            // 질량 = 면적 * 깊이 * 밀도
            float mass = area * density;

            rigidbody = new Rigidbody(position, density, mass, restitution, area, isStatic, 0.0f, width, height, Shape.Box);

            return true;
        }
    }
}
