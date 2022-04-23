using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;


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

        public Vector2 min;
        public Vector2 max;
    }

    struct CollisionResults
    {
        //t is our percentage of where it hit, null if not hit.
        public float? t;
        public Vector2 normal;
    }



    static class Collision2D
    {
        
        public static CollisionResults MovingRectVsRect(Rect2f movingRect, Vector2 displacement, Rect2f targetRect)
        {
            //Expand target rect
            Vector2 sizeVec = new Vector2(movingRect.Width * 0.5f, movingRect.Height * 0.5f);

            targetRect.min -= sizeVec;
            targetRect.max += sizeVec;

            return RayVsBox(new Ray2f(movingRect.Centre, displacement), targetRect);
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

            if(ray.direction.X < 0.0f)
            {
                //Debug.WriteLine("Negative X");
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

            if(farthest_t < 0.0f || nearest_t > 1.0f)
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

    }
}
