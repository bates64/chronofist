using System;
using General;
using UI;
using UnityEngine;

namespace Physics
{
	    [RequireComponent (typeof (BoxCollider2D))]
	public class Controller2D : MonoBehaviour {

		public LayerMask collisionMask;

		const float SkinWidth = .015f;
		private const float MaxSlopeAngle = 80;

		private bool _isSlope;
		private BoxCollider2D _collider;
		private RaycastOrigins _raycastOrigins;
		private Direction _horizontal = new Direction(4, 0, Vector2.right,true);
		private Direction _vertical =  new Direction(4, 0, Vector2.up,false);
		public CollisionGroup Collisions = new CollisionGroup();
		
		#region Properties
		
		private Bounds Bounds
		{
			get
			{
				Bounds bounds = _collider.bounds;
				bounds.Expand (SkinWidth * -2); //Removes the skin before being used for any calculations.
				return bounds;
			}
		}

		#endregion

		#region Unity and Setup Functions

		private void Awake() 
		{
			_collider = GetComponent<BoxCollider2D> ();
			CalculateRaySpacing();
		}
		
		private void CalculateRaySpacing()
		{
			Bounds bounds = Bounds;
			_horizontal.RayCount = Mathf.Clamp (_horizontal.RayCount, 2, int.MaxValue);
			_vertical.RayCount = Mathf.Clamp (_vertical.RayCount, 2, int.MaxValue);
			_horizontal.RaySpacing = bounds.size.y / (_horizontal.RayCount - 1);
			_vertical.RaySpacing = bounds.size.x / (_vertical.RayCount - 1);
		}

		private void LateUpdate()
		{
			UpdateCollisions();
		}

		#endregion

		#region Movement Functions
		
		/// <summary>
		/// Moves the object by the specified distance preventing overlapping with colliders in the specified layer.
		/// </summary>
		/// <param name="velocity">The vector containing the distance we wanna move</param>
		public void Move(Vector2 velocity) 
		{
			//1: It updates the points where the check needs to happen
			UpdateRaycastOrigins();
			// 2: It checks for collisions on X if the object would move the target distance, if it cant, it trims the distance
			if (velocity.x != 0) Measure(_horizontal,_raycastOrigins.BottomRight,ref velocity.x,0,ref velocity);
			// 3: Same as X but for vertical collisions, except this time the check for collisions starts where it would be if it already moved on the X axis.
			if (velocity.y != 0) Measure(_vertical,_raycastOrigins.TopLeft,ref velocity.y,velocity.x,ref velocity);
			// 4: Performs the movement.
			transform.Translate (velocity);
			if(_isSlope) SlopeAdjustment();
		}
		
		/// <summary>
		/// Creates N number of raycasts towards the target direction, it checks for colliders of the specified layer in its trajectory and keeps track if a collision would happen.
		/// </summary>
		/// <param name="direction">direction we want to move</param>
		/// <param name="positiveOrigin">first corner that defines where our raycasts will generate from</param>
		/// <param name="distance">amount of distance we want to move</param>
		/// <param name="offset">amount of horizontal offset that needs to be added to the point of origin of raycasts</param>
		/// /// <param name="trueVelocity">a reference to the total velocity we are trying to move</param>
		private void Measure(Direction direction,Vector2 positiveOrigin, ref float distance, float offset,ref Vector2 trueVelocity)
		{
			Vector2 originalVelocity = trueVelocity;
			int sign = (int) Mathf.Sign(distance);
			float rayLength = Mathf.Abs(distance) + SkinWidth;
			bool isHit = false;
			for (int i = 0; i < direction.RayCount; i++)
			{
				Vector2 rayOrigin = (sign == -1) ? _raycastOrigins.BottomLeft : positiveOrigin;
				rayOrigin += Util.absPerpendicular(direction.DirectionVector) * (direction.RaySpacing * i + offset);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction.DirectionVector * sign, rayLength, collisionMask);
				Debug.DrawRay(rayOrigin, direction.DirectionVector * sign * rayLength,Color.red);
				if (hit)
				{
					isHit = hit;
					if (direction.ClimbSlopes)
					{
						float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
						if (i == 0 && slopeAngle <= MaxSlopeAngle)
						{
							ClimbSlope(ref trueVelocity,slopeAngle);
							continue;
						}
					}
					distance = (hit.distance - SkinWidth) * sign;
					rayLength = hit.distance;
				}
			}
			direction.NegativeCollision = (sign == -1 && isHit);
			direction.PositiveCollision = sign == 1 && isHit;
		}
		
		private void UpdateCollisions()
		{
			Collisions.Down = _vertical.NegativeCollision;
			Collisions.Up = _vertical.PositiveCollision;
			Collisions.Left = _horizontal.NegativeCollision;
			Collisions.Right = _horizontal.PositiveCollision;
		}
		
		private void ClimbSlope(ref Vector2 velocity, float slopeAngle)
		{
			float moveDistance = Mathf.Abs(velocity.x);
			float slopeVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
			if (slopeVelocityY >= velocity.y)
			{
				velocity.y = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
				_isSlope = true;
			}
			velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
		}

		private void SlopeAdjustment()
		{
			_isSlope = false;
			Move(new Vector2(0,-SkinWidth * 2));
		}
		
		private void UpdateRaycastOrigins()
		{
			Bounds bounds = Bounds;
			_raycastOrigins.BottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
			_raycastOrigins.BottomRight = new Vector2 (bounds.max.x, bounds.min.y);
			_raycastOrigins.TopLeft = new Vector2 (bounds.min.x, bounds.max.y);
			_raycastOrigins.TopRight = new Vector2 (bounds.max.x, bounds.max.y);
		}

		#endregion
		
		#region Structs

		struct RaycastOrigins
		{
			public Vector2 TopLeft;
			public Vector2 TopRight;
			public Vector2 BottomLeft; 
			public Vector2 BottomRight;
		}

		private class Direction
		{
			public int RayCount;
			public float RaySpacing;
			public readonly Vector2 DirectionVector;
			private bool _positiveCollision;
			private bool _negativeCollision;
			private bool _climbSlopes;
			
			public Direction(int rCount,int rSpacing,Vector2 dir,bool climbSlopes)
			{
				RayCount = rCount;
				RaySpacing = rSpacing;
				DirectionVector = dir;
				_negativeCollision = false;
				_positiveCollision = false;
				_climbSlopes = climbSlopes;
			}

			public bool ClimbSlopes => _climbSlopes;
			
			public bool NegativeCollision
			{
				get => _negativeCollision;
				set
				{ 
					_negativeCollision = value;
				} 
			}

			public bool PositiveCollision
			{
				get => _positiveCollision;
				set => _positiveCollision = value;
			}

			public void Reset()
			{
				_negativeCollision = false;
				_positiveCollision = false;
			}
		}

		public struct CollisionGroup
		{
			private bool _down;
			private bool _up;
			private bool _left;
			private bool _right;
			
			public event Util.DVoid OnLanding;
			public event Util.DVoid OnTakeoff;
			public event Util.DVoid OnCeilingBump;
			public event Util.DVoid OnLeftWallBump;
			public event Util.DVoid OnRightWallBump;
			public event Util.DVoid OnWallBump;
			
			public bool Down
			{
				get => _down;
				set
				{
					if (_down != value)
					{
						if (value) OnLanding?.Invoke();
						else OnTakeoff?.Invoke();
					}
					_down = value;
				}
			}
			
			public bool Up
			{
				get => _up;
				set
				{
					if (_up != value)
					{
						if (value) OnCeilingBump?.Invoke();
					}
					_up = value;
				}
			}

			public bool Left
			{
				get => _left;
				set
				{
					if (_left != value)
					{
						if (value)
						{
							OnWallBump?.Invoke();
							OnLeftWallBump?.Invoke();
						}
					}
					_up = value;
				}
			}

			public bool Right
			{
				get => _right;
				set
				{
					if (_right != value)
					{
						if (value)
						{
							OnWallBump?.Invoke();
							OnRightWallBump?.Invoke();
						}
					}
					_up = value;
				}
			}
			
		}
		
		#endregion
	}
}