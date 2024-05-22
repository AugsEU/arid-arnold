#define EXPERIMENTAL_COLLISION_REMOVALS

namespace GMTK2023
{
	/// <summary>
	/// Type of collision.
	/// </summary>
	enum CollisionType
	{
		Ground,
		Wall,
		Ceiling
	}


	/// <summary>
	/// Represents a finite ray in world space.
	/// Note it has a finite length
	/// </summary>
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





	/// <summary>
	/// Rectangle in world space used for colliders
	/// </summary>
	struct Rect2f
	{
		public Rect2f(Vector2 vec1, Vector2 vec2)
		{
			min = new Vector2(MathF.Min(vec1.X, vec2.X), MathF.Min(vec1.Y, vec2.Y));
			max = new Vector2(MathF.Max(vec1.X, vec2.X), MathF.Max(vec1.Y, vec2.Y));
		}

		public Rect2f(Rectangle rect)
		{
			min = new Vector2(rect.X, rect.Y);
			max = new Vector2(rect.X + rect.Width, rect.Y + rect.Height);
		}

		public Rect2f(Vector2 _min, Texture2D texture)
		{
			min = _min;
			max = new Vector2(_min.X + texture.Width, _min.Y + texture.Height);
		}

		public Rect2f(Vector2 _min, float width, float height)
		{
			min = _min;
			max = new Vector2(_min.X + width, _min.Y + height);
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
			get { return (min + max) / 2.0f; }
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





	/// <summary>
	/// Results of a ray collision
	/// </summary>
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





	/// <summary>
	/// Utility methods for doing collisions
	/// </summary>
	static class Collision2D
	{
		/// <summary>
		/// Classify angle of collision
		/// </summary>
		/// <param name="normal">Normal vector of collision</param>
		/// <returns>Collision type(Wall, Ground or Ceiling</returns>
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



		/// <summary>
		/// Compare if moving a rectangle will hit another rectangle
		/// </summary>
		/// <param name="movingRect">Rectangle that will be moving</param>
		/// <param name="displacement">How far it is moving</param>
		/// <param name="targetRect">Rectangle that is static</param>
		/// <returns>Collision results of collision</returns>
		public static CollisionResults MovingRectVsRect(Rect2f movingRect, Vector2 displacement, Rect2f targetRect)
		{
			//Expand target rect
			Vector2 sizeVec = new Vector2(movingRect.Width * 0.5f, movingRect.Height * 0.5f);

			targetRect.min -= sizeVec;
			targetRect.max += sizeVec;

			return RayVsBox(new Ray2f(movingRect.Centre, displacement), targetRect);
		}



		/// <summary>
		/// Check if two rays intersect
		/// </summary>
		/// <param name="checkRay">Ray to check</param>
		/// <param name="targetRay">Second ray to check</param>
		/// <returns>Collision results of their collision</returns>
		public static CollisionResults RayVsRay(Ray2f checkRay, Ray2f targetRay)
		{
			CollisionResults results;
			results.t = null;
			results.normal = Vector2.Zero;

			float directionFactor = MonoMath.Cross(checkRay.direction, targetRay.direction);

			if (directionFactor != 0.0f)
			{
				float t = MonoMath.Cross(targetRay.origin - checkRay.origin, targetRay.direction) / directionFactor;
				float u = MonoMath.Cross(targetRay.origin - checkRay.origin, checkRay.direction) / directionFactor;

				if (1.00f >= t && t >= 0.0f && 1.00f >= u && u >= 0.0f)
				{
					//A hit
					results.t = t;
					results.normal = MonoMath.Perpendicular(targetRay.direction);

					if (directionFactor > 0.0f)
					{
						results.normal = -results.normal;
					}

					results.normal.Normalize();

					//Normalisation can fail so add this to fix it.
					if (results.normal.X == 0.0f)
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



		/// <summary>
		/// Check if a ray intersects a box
		/// </summary>
		/// <param name="ray">Ray to check</param>
		/// <param name="rect">Box to check</param>
		/// <returns>Collision results of their collision</returns>
		public static CollisionResults RayVsBox(Ray2f ray, Rect2f rect)
		{
			CollisionResults results;
			results.t = null;
			results.normal = Vector2.Zero;

			if (BoxVsPointOpen(rect, ray.origin))
			{
				return CollisionResults.None;
			}

			if (ray.direction == Vector2.Zero)
			{
				return CollisionResults.None;
			}

			//Handle zero case
			if (ray.direction.X == 0.0f)
			{
				//Vertical case, check if we are within X bounds.
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
#if !EXPERIMENTAL_COLLISION_REMOVALS
                        else if (1.0f > y_max && y_max >= 0.0f)
                        {
                            results.t = y_max;
                            results.normal = new Vector2(0.0f, -1.0f);
                        }
#endif
					}
					//Going up
					else
					{
						if (1.0f > y_max && y_max >= 0.0f)
						{
							results.t = y_max;
							results.normal = new Vector2(0.0f, 1.0f);
						}
#if !EXPERIMENTAL_COLLISION_REMOVALS
                        else if (1.0f > y_min && y_min >= 0.0f)
                        {
                            results.t = y_min;
                            results.normal = new Vector2(0.0f, 1.0f);
                        }
#endif
					}
				}

				return results;
			}
			else if (ray.direction.Y == 0.0f)
			{
				//Horizontal case check if we are in Y bounds.
				if (rect.min.Y < ray.origin.Y && ray.origin.Y < rect.max.Y)
				{
					float x_min = (rect.min.X - ray.origin.X) / ray.direction.X;
					float x_max = (rect.max.X - ray.origin.X) / ray.direction.X;

					//Going right
					if (ray.direction.X > 0.0f)
					{
						if (1.0f > x_min && x_min >= 0.0f)
						{
							results.t = x_min;
							results.normal = new Vector2(-1.0f, 0.0f);
						}
#if !EXPERIMENTAL_COLLISION_REMOVALS
                        else if (1.0f > x_max && x_max >= 0.0f)
                        {
                            results.t = x_max;
                            results.normal = new Vector2(-1.0f, 0.0f);
                        }
#endif
					}
					else
					{
						if (1.0f > x_max && x_max >= 0.0f)
						{
							results.t = x_max;
							results.normal = new Vector2(1.0f, 0.0f);
						}
#if !EXPERIMENTAL_COLLISION_REMOVALS
                        else if (1.0f > x_min && x_min >= 0.0f)
                        {
                            results.t = x_min;
                            results.normal = new Vector2(1.0f, 0.0f);
                        }
#endif
					}
				}

				if (BoxVsPoint(rect, ray.origin))
				{
					results.normal = CardinalFromCentre(rect, ray.origin);
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
				MonoAlg.Swap(ref x_near, ref x_far);
			}

			if (y_far < y_near)
			{
				MonoAlg.Swap(ref y_near, ref y_far);
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

			if (farthest_t < 0.0f || nearest_t < 0.0f || nearest_t > 1.0f)
			{
				//No collision
				return CollisionResults.None;
			}

			results.t = nearest_t;

			//Calculate normal
			if (x_near < y_near)
			{
				if (ray.direction.Y < 0.0f)
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

			if (nearest_t == 1.0f)
			{
				results.t = null;
			}

			if (nearest_t == -0.0f && Vector2.Dot(ray.direction, results.normal) > 0.0f)
			{
				results.t = null;
			}

			return results;
		}



		/// <summary>
		/// Check if two rectangles overlap
		/// </summary>
		/// <param name="rect1">First rectangle</param>
		/// <param name="rect2">Second rectangle</param>
		/// <returns>True if they overlap(inclusive)</returns>
		public static bool BoxVsBox(Rect2f rect1, Rect2f rect2)
		{
			return rect1.min.X <= rect2.max.X && rect2.min.X <= rect1.max.X &&
				rect1.min.Y <= rect2.max.Y && rect2.min.Y <= rect1.max.Y;
		}



		/// <summary>
		/// Check if a point is within another rectangle as a closed set
		/// </summary>
		/// <param name="rect">Rectangle to check</param>
		/// <param name="point">Point to check</param>
		/// <returns>True if the point is within the rectangle(inclusive)</returns>
		public static bool BoxVsPoint(Rect2f rect, Vector2 point)
		{
			return rect.min.X <= point.X && point.X <= rect.max.X &&
				rect.min.Y <= point.Y && point.Y <= rect.max.Y;
		}



		/// <summary>
		/// Check if a point is within another rectangle as an open set
		/// </summary>
		/// <param name="rect">Rectangle to check</param>
		/// <param name="point">Point to check</param>
		/// <returns>True if the point is within the rectangle(not inclusive)</returns>
		public static bool BoxVsPointOpen(Rect2f rect, Vector2 point)
		{
			return rect.min.X < point.X && point.X < rect.max.X &&
				rect.min.Y < point.Y && point.Y < rect.max.Y;
		}



		/// <summary>
		/// Gets cardinal direction away from centre.
		/// E.g. a point above the centre will yield an upwards vector
		/// </summary>
		/// <param name="rect">Rectangle to check</param>
		/// <param name="point">Point to check</param>
		/// <returns>Cardinal direction</returns>
		public static Vector2 CardinalFromCentre(Rect2f rect, Vector2 point)
		{
			point -= rect.Centre;

			float gradient = (rect.max.Y - rect.min.Y) / (rect.max.X - rect.min.X);

			if (point.Y > gradient * point.X)
			{
				if (point.Y > -gradient * point.X)
				{
					return new Vector2(0.0f, 1.0f);
				}
				else
				{
					return new Vector2(-1.0f, 0.0f);
				}
			}
			else
			{
				if (point.Y > -gradient * point.X)
				{
					return new Vector2(1.0f, 0.0f);
				}
				else
				{
					return new Vector2(0.0f, -1.0f);
				}
			}
		}
	}
}
