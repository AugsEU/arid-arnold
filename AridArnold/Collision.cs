using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AridArnold
{
    struct Ray2f
    {
        public Ray2f(Vector2 _origin, Vector2 _direction)
        {
            origin = _origin;
            direction = _direction;
        }

        public Vector2 origin;
        public Vector2 direction;
     }

    struct Rect2f
    {
        public Rect2f(Vector2 _min, Vector2 _max)
        {
            min = _min;
            max = _max;
        }

        public float Width
        {
            get { return Math.Abs(max.X - min.X); }
        }

        public float Height
        {
            get { return Math.Abs(max.Y - min.Y); }
        }

        public Vector2 Centre
        {
            get { return (min + max)/2.0f; }
        }

        public static Rect2f operator +(Rect2f a, Rect2f b)
        {
            float minX = Math.Min(a.min.X, b.min.X);
            float minY = Math.Min(a.min.Y, b.min.Y);
            float maxX = Math.Max(a.max.X, b.max.X);
            float maxY = Math.Max(a.max.Y, b.max.Y);

            return new Rect2f(new Vector2(minX, minY), new Vector2(maxX, maxY));
        }

        public static Rect2f operator +(Rect2f rect, Vector2 vec)
        {
            rect.min += vec;
            rect.max += vec;

            return rect;
        }

        public Vector2 min;
        public Vector2 max;
    }

    struct CollisionResults
    {
        //t is our percentage of where it hit, null if not hit.
        public float? t;
        public Vector2 normal;

        public static CollisionResults None
        {
            get
            {
                CollisionResults none;

                none.t = null;
                none.normal = Vector2.Zero;

                return none;
            }
        }

        public bool Collided
        {
            get
            {
                return t.HasValue;
            }
        }
    }

    enum CollisionType
    {
        Ground,
        Wall,
        Ceiling
    }

    static class Collision2D
    {
        public static CollisionType GetCollisionType(Vector2 normal)
        {
            if (normal.Y == -1.0f && normal.X == 0.0f)
            {
                return CollisionType.Ground;
            }

            if (normal.Y == 1.0f && normal.X == 0.0f)
            {
                return CollisionType.Ceiling;
            }

            return CollisionType.Wall;
        }
        
        public static CollisionResults MovingRectVsRect(Rect2f movingRect, Vector2 displacement, Rect2f targetRect)
        {
            //Expand target rect
            Vector2 sizeVec = new Vector2(movingRect.Width * 0.5f, movingRect.Height * 0.5f);

            targetRect.min -= sizeVec;
            targetRect.max += sizeVec;

            return RayVsBox(new Ray2f(movingRect.Centre, displacement), targetRect);
        }

        public static CollisionResults MovingRectVsPlatform(Rect2f movingRect, Vector2 displacement, Vector2 platLeft, float length)
        {
            //Expand target rect
            Vector2 sizeVec = new Vector2(movingRect.Width * 0.5f, movingRect.Height * 0.5f);

            platLeft = platLeft - sizeVec;
            length += sizeVec.X * 2.0f;

            Ray2f platformRay = new Ray2f(platLeft, new Vector2(length, 0.0f));
            Ray2f movingRay = new Ray2f(movingRect.Centre, displacement);

            return RayVsRay(movingRay, platformRay);
        }

        public static CollisionResults RayVsRay(Ray2f checkRay, Ray2f targetRay)
        {
            CollisionResults results;
            results.t = null;
            results.normal = Vector2.Zero;

            float directionFactor = Util.Cross(checkRay.direction, targetRay.direction);

            if(directionFactor != 0.0f)
            { 
                float t = Util.Cross(targetRay.origin - checkRay.origin, targetRay.direction) / directionFactor;
                float u = Util.Cross(targetRay.origin - checkRay.origin, checkRay.direction) / directionFactor;

                if(1.00f >= t && t >= 0.0f && 1.00f >= u && u >= 0.0f)
                {
                    //A hit
                    results.t = t;
                    results.normal = Util.Perpendicular(targetRay.direction);

                    if(directionFactor > 0.0f)
                    {
                        results.normal = -results.normal;
                    }

                    results.normal.Normalize();

                    //Normalisation can fail so add this to fix it.
                    if(results.normal.X == 0.0f)
                    {
                        results.normal.Y = MathF.Sign(results.normal.Y);
                    }
                    else if (results.normal.Y == 0.0f)
                    {
                        results.normal.X = MathF.Sign(results.normal.X);
                    }
                }
            }

            return results;
        }

        //Checks if a 2d ray intersects a 2d box
        public static CollisionResults RayVsBox(Ray2f ray, Rect2f rect)
        {
            CollisionResults results;
            results.t = null;
            results.normal = Vector2.Zero;

            if(ray.direction == Vector2.Zero)
            {
                return results;
            }

            //Handle zero case
            if(ray.direction.X == 0.0f)
            {
                if (rect.min.X < ray.origin.X && ray.origin.X < rect.max.X)
                {
                    float y_min = (rect.min.Y - ray.origin.Y) / ray.direction.Y;
                    float y_max = (rect.max.Y - ray.origin.Y) / ray.direction.Y;

                    if (ray.direction.Y > 0.0f)
                    {
                        if (1.0f > y_min && y_min >= 0.0f)
                        {
                            results.t = y_min;
                            results.normal = new Vector2(0.0f, -1.0f);
                        }
                        else if (1.0f > y_max && y_max >= 0.0f)
                        {
                            results.t = y_max;
                            results.normal = new Vector2(0.0f, -1.0f);
                        }
                    }
                    else
                    {
                        if (1.0f > y_max && y_max >= 0.0f)
                        {
                            results.t = y_max;
                            results.normal = new Vector2(0.0f, 1.0f);
                        }
                        else if (1.0f > y_min && y_min >= 0.0f)
                        {
                            results.t = y_min;
                            results.normal = new Vector2(0.0f, 1.0f);
                        }
                    }
                }

                return results;
            }
            else if(ray.direction.Y == 0.0f)
            {
                if (rect.min.Y < ray.origin.Y && ray.origin.Y < rect.max.Y)
                {
                    float x_min = (rect.min.X - ray.origin.X) / ray.direction.X;
                    float x_max = (rect.max.X - ray.origin.X) / ray.direction.X;

                    if (ray.direction.X > 0.0f)
                    {
                        if (1.0f > x_min && x_min >= 0.0f)
                        {
                            results.t = x_min;
                            results.normal = new Vector2(-1.0f, 0.0f);
                        }
                        else if (1.0f > x_max && x_max >= 0.0f)
                        {
                            results.t = x_max;
                            results.normal = new Vector2(-1.0f, 0.0f);
                        }
                    }
                    else
                    {
                        if (1.0f > x_max && x_max >= 0.0f)
                        {
                            results.t = x_max;
                            results.normal = new Vector2(1.0f, 0.0f);
                        }
                        else if (1.0f > x_min && x_min >= 0.0f)
                        {
                            results.t = x_min;
                            results.normal = new Vector2(1.0f, 0.0f);
                        }
                    }
                }

                return results;
            }


            //Normal case
            float x_near = (rect.min.X - ray.origin.X) / ray.direction.X;
            float x_far = (rect.max.X - ray.origin.X) / ray.direction.X;

            float y_near = (rect.min.Y - ray.origin.Y) / ray.direction.Y;
            float y_far = (rect.max.Y - ray.origin.Y) / ray.direction.Y;

            if (x_far < x_near)
            {
                Util.Swap(ref x_near, ref x_far);
            }

            if (y_far < y_near)
            {
                Util.Swap(ref y_near, ref y_far);
            }


            bool no_intesect = x_near > y_far || y_near > x_far;
            if (no_intesect)
            {
                //No collision
                return results;
            }

            //Get nearest point with max because one is negative.
            float nearest_t = Math.Max(x_near, y_near);
            float farthest_t = Math.Min(x_far, y_far);

            if(farthest_t < 0.0f || nearest_t < 0.0f || nearest_t > 1.0f)
            {
                //No collision
                return results;
            }

            results.t = nearest_t;

            //Calculate normal
            if(x_near < y_near)
            {
                if(ray.direction.Y < 0.0f)
                {
                    results.normal = new Vector2(0.0f, 1.0f);
                }
                else
                {
                    results.normal = new Vector2(0.0f, -1.0f);
                }
            }
            else
            {
                if (ray.direction.X < 0.0f)
                {
                    results.normal = new Vector2(1.0f, 0.0f);
                }
                else
                {
                    results.normal = new Vector2(-1.0f, 0.0f);
                }
            }

            if (nearest_t == -0.0f && Vector2.Dot(ray.direction, results.normal) > 0.0f)
            {
                results.t = null;
            }

            return results;
        }

        public static bool BoxVsBox(Rect2f rect1, Rect2f rect2)
        {
            return rect1.min.X <= rect2.max.X && rect2.min.X <= rect1.max.X &&
                rect1.min.Y <= rect2.max.Y && rect2.min.Y <= rect1.max.Y;
        }

        public static bool BoxVsPoint(Rect2f rect, Vector2 point)
        {
            return rect.min.X <= point.X && point.X <= rect.max.X &&
                rect.min.Y <= point.Y && point.Y <= rect.max.Y;
        }

    }
}
