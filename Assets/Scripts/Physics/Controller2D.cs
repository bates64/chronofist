using General;
using UnityEngine;

namespace Physics {
	[RequireComponent (typeof (BoxCollider2D))]
	public class Controller2D : MonoBehaviour {
		public LayerMask collisionMask;

		const float SkinWidth = .015f;

		private BoxCollider2D _collider;
		private RaycastOrigins _raycastOrigins;
		private Direction _horizontal = new Direction(8, 0, Vector2.right);
		private Direction _vertical =  new Direction(8, 0, Vector2.up);
		private CollisionGroup _collisions = new CollisionGroup();

		public bool isGrounded => _vertical.NegativeCollision;
		public CollisionGroup collision => _collisions;
        public float airTime { get; private set; } = 0f;
        public float groundTime { get; private set; } = 0f;

		#region Properties

		private Bounds Bounds
		{
			get
			{
				Bounds bounds = _collider.bounds;
				bounds.Expand (SkinWidth * -2);
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

		private void LateUpdate()
		{
			UpdateCollisions();
            UpdateTimes();
		}

		private void UpdateCollisions()
		{
			collision.down = _vertical.NegativeCollision;
			collision.up = _vertical.PositiveCollision;
			collision.left = _horizontal.NegativeCollision;
			collision.right = _horizontal.PositiveCollision;
		}

        private void UpdateTimes() {
            float deltaTime = LocalTime.DeltaTimeAt(this);

            if (isGrounded) {
                groundTime += Time.deltaTime;
                airTime = 0f;
            } else {
                groundTime = 0f;
                airTime += Time.deltaTime;
            }
        }

		private void CalculateRaySpacing()
		{
			Bounds bounds = Bounds;
			_horizontal.RayCount = Mathf.Clamp (_horizontal.RayCount, 2, int.MaxValue);
			_vertical.RayCount = Mathf.Clamp (_vertical.RayCount, 2, int.MaxValue);
			_horizontal.RaySpacing = bounds.size.y / (_horizontal.RayCount - 1);
			_vertical.RaySpacing = bounds.size.x / (_vertical.RayCount - 1);
		}

		#endregion

		#region Movement Functions

		public void Move(Vector3 velocity)
		{
			UpdateRaycastOrigins();
			if (velocity.x != 0) Measure(_horizontal,_raycastOrigins.BottomRight,ref velocity.x,0);
			if (velocity.y != 0) Measure(_vertical,_raycastOrigins.TopLeft,ref velocity.y,velocity.x);
			transform.Translate (velocity);
		}

		private void Measure(Direction direction,Vector2 positiveOrigin, ref float velocity, float offset)
		{
			int sign = (int) Mathf.Sign(velocity);
			float rayLength = Mathf.Abs(velocity) + SkinWidth;
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
					velocity = (hit.distance - SkinWidth) * sign;
					rayLength = hit.distance;
				}
			}
			direction.NegativeCollision = sign == -1 && isHit;
			direction.PositiveCollision = sign == 1 && isHit;
		}

		private void ClimbSlope(ref Vector3 velocity, float slopeAngle)
		{
			//velocity.y = Mathf.Sign()
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

			public Direction(int rCount,int rSpacing,Vector2 dir)
			{
				RayCount = rCount;
				RaySpacing = rSpacing;
				DirectionVector = dir;
				_negativeCollision = false;
				_positiveCollision = false;
			}

			public bool NegativeCollision
			{
				get => _negativeCollision;
				set => _negativeCollision = value;
			}

			public bool PositiveCollision
			{
				get => _positiveCollision;
				set => _positiveCollision = value;
			}

		}

		public class CollisionGroup
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

			public bool down
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

			public bool up
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

			public bool left
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

			public bool right
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
