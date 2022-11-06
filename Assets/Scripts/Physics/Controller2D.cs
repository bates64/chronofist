using UnityEngine;
using System.Collections;
using General;

[RequireComponent (typeof (BoxCollider2D))]
public class Controller2D : MonoBehaviour {

	public LayerMask collisionMask;

	const float skinWidth = .015f;
	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;

	float maxClimbAngle = 80;
	float maxDescendAngle = 80;

	float horizontalRaySpacing;
	float verticalRaySpacing;

	BoxCollider2D collider;
	RaycastOrigins raycastOrigins;
	private CollisionInfo _tempCollisions = new CollisionInfo();
	private CollisionInfo _finalCollisions = new CollisionInfo();
	
	public CollisionInfo Collisions => _finalCollisions;
	
	void Awake() 
	{
		collider = GetComponent<BoxCollider2D> ();
		CalculateRaySpacing ();
		Collisions.OnLanding += print;
	}

	private void print()
	{
		print("Landing");
	}
	
	private void LateUpdate()
	{
		UpdateCollisions();
	}
	
	private void UpdateCollisions()
	{
		_finalCollisions.Below = _tempCollisions.Below || _tempCollisions.climbingSlope || _tempCollisions.descendingSlope;
		_finalCollisions.Above = _tempCollisions.Above;
		_finalCollisions.Left = _tempCollisions.Left;
		_finalCollisions.Right = _tempCollisions.Right;
		_finalCollisions.climbingSlope = _tempCollisions.climbingSlope;
		_finalCollisions.descendingSlope = _tempCollisions.descendingSlope;
		_finalCollisions.slopeAngle = _tempCollisions.slopeAngle;
		_finalCollisions.velocityOld = _tempCollisions.velocityOld;
		_finalCollisions.slopeAngleOld = _tempCollisions.slopeAngleOld;
	}
	
	public void Move(Vector3 velocity) {
		UpdateRaycastOrigins ();
		_tempCollisions.Reset ();
		_tempCollisions.velocityOld = velocity;
		if (velocity.y < 0) {
			DescendSlope(ref velocity);
		}
		if (velocity.x != 0) {
			HorizontalCollisions (ref velocity);
		}
		if (velocity.y != 0) {
			VerticalCollisions (ref velocity);
		}
		transform.Translate (velocity);
	}

	void HorizontalCollisions(ref Vector3 velocity) {
		float directionX = Mathf.Sign (velocity.x);
		float rayLength = Mathf.Abs (velocity.x) + skinWidth;
		
		for (int i = 0; i < horizontalRayCount; i ++) {
			Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength,Color.red);

			if (hit) {

				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

				if (i == 0 && slopeAngle <= maxClimbAngle) {
					if (_tempCollisions.descendingSlope) {
						_tempCollisions.descendingSlope = false;
						velocity = _tempCollisions.velocityOld;
					}
					float distanceToSlopeStart = 0;
					if (slopeAngle != _tempCollisions.slopeAngleOld) {
						distanceToSlopeStart = hit.distance-skinWidth;
						velocity.x -= distanceToSlopeStart * directionX;
					}
					ClimbSlope(ref velocity, slopeAngle);
					velocity.x += distanceToSlopeStart * directionX;
				}

				if (!_tempCollisions.climbingSlope || slopeAngle > maxClimbAngle) {
					velocity.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance;

					if (_tempCollisions.climbingSlope) {
						velocity.y = Mathf.Tan(_tempCollisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
					}

					_tempCollisions.Left = directionX == -1;
					_tempCollisions.Right = directionX == 1;
				}
			}
		}
	}
	
	void VerticalCollisions(ref Vector3 velocity) {
		float directionY = Mathf.Sign (velocity.y);
		float rayLength = Mathf.Abs (velocity.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i ++) {
			Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength,Color.red);

			if (hit) {
				velocity.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				if (_tempCollisions.climbingSlope) {
					velocity.x = velocity.y / Mathf.Tan(_tempCollisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
				}

				_tempCollisions.Below = directionY == -1;
				_tempCollisions.Above = directionY == 1;
			}
		}

		if (_tempCollisions.climbingSlope) {
			float directionX = Mathf.Sign(velocity.x);
			rayLength = Mathf.Abs(velocity.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight) + Vector2.up * velocity.y;
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin,Vector2.right * directionX,rayLength,collisionMask);

			if (hit) {
				float slopeAngle = Vector2.Angle(hit.normal,Vector2.up);
				if (slopeAngle != _tempCollisions.slopeAngle) {
					velocity.x = (hit.distance - skinWidth) * directionX;
					_tempCollisions.slopeAngle = slopeAngle;
				}
			}
		}
	}

	void ClimbSlope(ref Vector3 velocity, float slopeAngle) {
		float moveDistance = Mathf.Abs (velocity.x);
		float climbVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
		if (velocity.y <= climbVelocityY) 
		{
			velocity.y = climbVelocityY;
			velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
			_tempCollisions.Below = true;
			_tempCollisions.climbingSlope = true;
			_tempCollisions.slopeAngle = slopeAngle;
		}
	}

	void DescendSlope(ref Vector3 velocity) {
		float directionX = Mathf.Sign (velocity.x);
		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

		if (hit) {
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if (slopeAngle != 0 && slopeAngle <= maxDescendAngle) {
				if (Mathf.Sign(hit.normal.x) == directionX) {
					if (hit.distance - skinWidth * 2 <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x)) {
						float moveDistance = Mathf.Abs(velocity.x);
						float descendVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
						velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
						velocity.y -= descendVelocityY;

						_tempCollisions.slopeAngle = slopeAngle;
						_tempCollisions.descendingSlope = true;
						_tempCollisions.Below = true;
					}
				}
			}
		}
	}

	void UpdateRaycastOrigins() {
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2);

		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}

	void CalculateRaySpacing() {
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2);

		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

	struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	public class CollisionInfo {
		private bool above, below;
		private bool left, right;

		public bool climbingSlope;
		public bool descendingSlope;
		public float slopeAngle, slopeAngleOld;
		public Vector3 velocityOld;

		public event Util.DVoid OnLanding;
		public event Util.DVoid OnTakeOff;
		public event Util.DVoid OnBonk;
		public event Util.DVoid OnWallBump;
		
		public bool Below
		{
			get => below;
			set
			{
				if (below != value)
				{
					if(value) OnLanding?.Invoke();
					else OnTakeOff?.Invoke();
				}
				below = value;
			}
		}
		
		public bool Above
		{
			get => above;
			set
			{
				if (above != value)
				{
					if(value) OnBonk?.Invoke();
				}
				above = value;
			}
		}

		public bool Left
		{
			get => left;
			set
			{
				if (left != value)
				{
					if(value) OnWallBump?.Invoke();
				}
				left = value;
			}
		}
		
		public bool Right
		{
			get => right;
			set
			{
				if (right != value)
				{
					if(value) OnWallBump?.Invoke();
				}
				right = value;
			}
		}
		
		public void Reset() {
			above = below = false;
			left = right = false;
			climbingSlope = false;
			descendingSlope = false;

			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	}

}