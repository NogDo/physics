using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Personal_Project_Game.Physics;
using Personal_Project_Game.Object;

namespace Personal_Project_Game
{
    class GameWorld
    {
        public static float minBodySize = 1.0f * 1.0f;
        public static float maxBodySize = 1280.0f * 720.0f;

        public static float minDensity = 0.5f;
        public static float maxDensity = 21.4f;

        public static int minInteration = 1;
        public static int maxInteration = 128;

        private List<Rigidbody> list_Rigidbody;
        private Vector2 gravity;

        public int BodyCount
        {
            get { return this.list_Rigidbody.Count; }
        }

        public GameWorld()
        {
            this.gravity = new Vector2(0.0f, 9.81f);
            this.list_Rigidbody = new List<Rigidbody>();
        }

        public void AddRigidbody(Rigidbody rigidbody)
        {
            this.list_Rigidbody.Add(rigidbody);
        }

        public bool RemoveRigidbody(Rigidbody rigidbody)
        {
            return this.list_Rigidbody.Remove(rigidbody);
        }

        public bool GetRigidbody(int index, out Rigidbody rigidbody)
        {
            rigidbody = null;
            if (index < 0 || index > this.list_Rigidbody.Count)
            {
                return false;
            }

            rigidbody = list_Rigidbody[index];
            return true;
        }

        public void Step(float time, int interations)
        {
            interations = PhysicsMath.Clamp(interations, minInteration, maxInteration);

            for (int ii = 0; ii < interations; ii++)
            {
                // 이동처리
                for (int i = 0; i < this.list_Rigidbody.Count; i++)
                {
                    this.list_Rigidbody[i].Step(time, this.gravity, interations);
                }

                // 충돌처리
                for (int i = 0; i < this.list_Rigidbody.Count - 1; i++)
                {
                    Rigidbody rigidbodyA = this.list_Rigidbody[i];

                    for (int j = i + 1; j < this.list_Rigidbody.Count; j++)
                    {
                        Rigidbody rigidbodyB = this.list_Rigidbody[j];

                        if (rigidbodyA.isStatic && rigidbodyB.isStatic)
                        {
                            continue;
                        }

                        if (OnCollide(rigidbodyA, rigidbodyB, out Vector2 normal, out float depth))
                        {
                            if (rigidbodyA.isStatic)
                            {
                                rigidbodyB.Move(normal * depth);
                            }
                            else if (rigidbodyB.isStatic)
                            {
                                rigidbodyA.Move(-normal * depth);
                            }
                            else
                            {
                                rigidbodyA.Move(-normal * depth / 2.0f);
                                rigidbodyB.Move(normal * depth / 2.0f);
                            }

                            ResolveCollision(rigidbodyA, rigidbodyB, normal, depth);
                        }
                    }
                }
            }
        }

        public void ResolveCollision(Rigidbody rigidbodyA, Rigidbody rigidbodyB, Vector2 normal, float depth)
        {
            Vector2 relativeVelocity = rigidbodyB.LinearVelocity - rigidbodyA.LinearVelocity;

            if (PhysicsMath.Dot(relativeVelocity, normal) > 0.0f)
            {
                return;
            }

            float e = Math.Min(rigidbodyA.restitution, rigidbodyB.restitution);
            float j = -(1.0f + e) * PhysicsMath.Dot(relativeVelocity, normal);
            j /= rigidbodyA.inverseMass * 1.15f + rigidbodyB.inverseMass * 1.15f;

            Vector2 impulse = j * normal;

            rigidbodyA.LinearVelocity -= impulse * rigidbodyA.inverseMass;
            rigidbodyB.LinearVelocity += impulse * rigidbodyB.inverseMass;
        }

        public bool OnCollide(Rigidbody rigidbodyA, Rigidbody rigidbodyB, out Vector2 normal, out float depth)
        {
            normal = Vector2.zero;
            depth = 0.0f;

            Shape shapeA = rigidbodyA.shape;
            Shape shapeB = rigidbodyB.shape;

            if (shapeA == Shape.Box)
            {
                if (shapeB == Shape.Box)
                {
                    return Collision.CrossPolygon(rigidbodyA.Position, rigidbodyA.GetChangedVertex(), rigidbodyB.Position, rigidbodyB.GetChangedVertex(), out normal, out depth);
                }
                else if (shapeB == Shape.Circle)
                {
                    bool result = Collision.CrossCirclePolygon(rigidbodyB.Position, rigidbodyB.radius, rigidbodyA.Position, rigidbodyA.GetChangedVertex(), out normal, out depth);
                    normal = -normal;

                    return result;
                }
            }
            else if (shapeA == Shape.Circle)
            {
                if (shapeB == Shape.Box)
                {
                    return Collision.CrossCirclePolygon(rigidbodyA.Position, rigidbodyA.radius, rigidbodyB.Position, rigidbodyB.GetChangedVertex(), out normal, out depth);
                }
                else if (shapeB == Shape.Circle)
                {
                    return Collision.CrossCircle(rigidbodyA.Position, rigidbodyA.radius, rigidbodyB.Position, rigidbodyB.radius, out normal, out depth);
                }
            }

            return false;
        }
    }
}
