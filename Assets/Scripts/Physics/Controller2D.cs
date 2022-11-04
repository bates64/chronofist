using General;
using UI;
using UnityEngine;

namespace Physics
{
	    [RequireComponent (typeof (BoxCollider2D))]
	public class Controller2D : MonoBehaviour {

		public LayerMask collisionMask;

		const float SkinWidth = .015f;

		private BoxCollider2D _collider;
		protected float obj;
		private RaycastOrigins _raycastOrigins;
		private Direction _horizontal = new Direction(4, 0, Vector2.right);
		private Direction _vertical =  new Direction(4, 0, Vector2.up);
		public event Util.DVoid OnLanding;
		public event Util.DVoid OnTakeoff;
		public event Util.DVoid OnCeilingBump;

		public bool isGrounded => _vertical.NegativeCollision;
		
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
			_vertical.OnNegativeChange += RelayGroundEvents;
			_vertical.OnPositiveChange += RelayCeilingEvents;
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
		
		#endregion

		#region Movement Functions

		public void Move(Vector3 velocity, bool locked = false, bool skipSlope = false) 
		{
			LockCollisions(locked);
			UpdateRaycastOrigins();
			if (velocity.x != 0) Measure(_horizontal,_raycastOrigins.BottomRight,ref velocity.x,0,skipSlope);
			if (velocity.y != 0) Measure(_vertical,_raycastOrigins.TopLeft,ref velocity.y,velocity.x,skipSlope);
			transform.Translate (velocity);
			LockCollisions(false);
		}
		
		private void Measure(Direction direction,Vector2 positiveOrigin, ref float velocity, float offset,bool skipSlope)
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
			//if (sign == -1) direction.NegativeCollision = isHit;
			//if (sign == 1) direction.PositiveCollision = isHit;
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

		private void LockCollisions(bool locked)
		{
			_vertical.LockCollisions = locked;
			_horizontal.LockCollisions = locked;
		}

		private void RelayGroundEvents(bool isGround)
		{
			Debug.Log("Ground Event:" + isGround);
			if(isGround) OnLanding?.Invoke();
			else OnTakeoff?.Invoke();
		}

		private void RelayCeilingEvents(bool hasBumped)
		{
			Debug.Log("Ceiling Event:" + hasBumped);
			if (hasBumped)OnCeilingBump?.Invoke();
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
			private bool _lockCollisions;
			public event Util.DBool OnPositiveChange;
			public event Util.DBool OnNegativeChange;
			
			public Direction(int rCount,int rSpacing,Vector2 dir)
			{
				RayCount = rCount;
				RaySpacing = rSpacing;
				DirectionVector = dir;
				_negativeCollision = false;
				_positiveCollision = false;
				_lockCollisions = false;
				OnPositiveChange = null;
				OnNegativeChange = null;
			}

			public bool NegativeCollision
			{
				get => _negativeCollision;
				set
				{
					if (_lockCollisions) return;
					if (_negativeCollision != value) OnNegativeChange?.Invoke(value);
					_negativeCollision = value;
				}
			}

			public bool PositiveCollision
			{
				get => _positiveCollision;
				set
				{
					if (_lockCollisions) return;
					if (_positiveCollision != value) OnPositiveChange?.Invoke(value);
					_positiveCollision = value;
				}
			}
			
			public bool LockCollisions
			{
				get => _lockCollisions;
				set => _lockCollisions = value;
			}
			
			public void Reset()
			{
				_negativeCollision = false;
				_positiveCollision = false;
			}
		}

		#endregion
	}
}