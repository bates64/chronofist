using General;
using UnityEngine;

namespace Physics {
    public class Controller2D : MonoBehaviour {
        private const float SkinWidth = .015f;
        public LayerMask collisionMask;
        private readonly Direction _horizontal = new(8, 0, Vector2.right);
        private readonly Direction _vertical = new(8, 0, Vector2.up);

        private BoxCollider2D _collider;
        private RaycastOrigins _raycastOrigins;

        public bool isGrounded => _vertical.NegativeCollision;
        public CollisionGroup collision { get; } = new();

        public float airTime { get; private set; }
        public float groundTime { get; private set; }

        #region Properties

        private Bounds Bounds {
            get {
                var bounds = _collider.bounds;
                bounds.Expand(SkinWidth * -2);
                return bounds;
            }
        }

        #endregion

        #region Unity and Setup Functions

        private void Awake() {
            _collider = GetComponentInChildren<BoxCollider2D>();
            CalculateRaySpacing();
        }

        private void LateUpdate() {
            UpdateCollisions();
            UpdateTimes();
        }

        private void UpdateCollisions() {
            collision.down = _vertical.NegativeCollision;
            collision.up = _vertical.PositiveCollision;
            collision.left = _horizontal.NegativeCollision;
            collision.right = _horizontal.PositiveCollision;
        }

        private void UpdateTimes() {
            var deltaTime = LocalTime.DeltaTimeAt(this);

            if (isGrounded) {
                groundTime += Time.deltaTime;
                airTime = 0f;
            } else {
                groundTime = 0f;
                airTime += Time.deltaTime;
            }
        }

        private void CalculateRaySpacing() {
            var bounds = Bounds;
            _horizontal.RayCount = Mathf.Clamp(_horizontal.RayCount, 2, int.MaxValue);
            _vertical.RayCount = Mathf.Clamp(_vertical.RayCount, 2, int.MaxValue);
            _horizontal.RaySpacing = bounds.size.y / (_horizontal.RayCount - 1);
            _vertical.RaySpacing = bounds.size.x / (_vertical.RayCount - 1);
        }

        #endregion

        #region Movement Functions

        public void Move(Vector2 velocity) {
            UpdateRaycastOrigins();
            ValidateVelocity(ref velocity);
            if (velocity.x != 0) {
                Measure(_horizontal, _raycastOrigins.BottomRight, ref velocity.x, 0);
            }

            if (velocity.y != 0) {
                Measure(_vertical, _raycastOrigins.TopLeft, ref velocity.y, velocity.x);
            }

            transform.Translate(velocity);
        }

        public bool CheckLeft(bool updateCollision = false) {
            UpdateRaycastOrigins();

            var x = -Util.PIXEL;
            Measure(_horizontal, _raycastOrigins.BottomLeft, ref x, 0, updateCollision);
            return Mathf.Abs(x) < Util.PIXEL;
        }

        public bool CheckRight(bool updateCollision = false) {
            UpdateRaycastOrigins();

            var x = Util.PIXEL;
            Measure(_horizontal, _raycastOrigins.BottomRight, ref x, 0, updateCollision);
            return Mathf.Abs(x) < Util.PIXEL;
        }

        private bool Measure(Direction direction, Vector2 positiveOrigin, ref float velocity, float offset,
            bool updateCollision = true) {
            var sign = (int)Mathf.Sign(velocity);
            var rayLength = Mathf.Abs(velocity) + SkinWidth;
            var isHit = false;
            for (var i = 0; i < direction.RayCount; i++) {
                var rayOrigin = sign == -1 ? _raycastOrigins.BottomLeft : positiveOrigin;
                rayOrigin += Util.absPerpendicular(direction.DirectionVector) * (direction.RaySpacing * i + offset);
                var hit = Physics2D.Raycast(rayOrigin, direction.DirectionVector * sign, rayLength, collisionMask);
                Debug.DrawRay(rayOrigin, direction.DirectionVector * sign * rayLength, Color.red);
                if (hit) {
                    isHit = hit;
                    velocity = (hit.distance - SkinWidth) * sign;
                    rayLength = hit.distance;
                }
            }

            if (!updateCollision) {
                return isHit;
            }

            direction.NegativeCollision = sign == -1 && isHit;
            direction.PositiveCollision = sign == 1 && isHit;
            return isHit;
        }

        private void ValidateVelocity(ref Vector2 velocity) {
            if (Mathf.Abs(velocity.x) < 0.005f) {
                velocity.x = 0;
            }

            if (Mathf.Abs(velocity.x) < 0.0005f) {
                velocity.x = 0;
            }
        }

        private void UpdateRaycastOrigins() {
            var bounds = Bounds;
            _raycastOrigins.BottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            _raycastOrigins.BottomRight = new Vector2(bounds.max.x, bounds.min.y);
            _raycastOrigins.TopLeft = new Vector2(bounds.min.x, bounds.max.y);
            _raycastOrigins.TopRight = new Vector2(bounds.max.x, bounds.max.y);
        }

        #endregion

        #region Structs

        private struct RaycastOrigins {
            public Vector2 TopLeft;
            public Vector2 TopRight;
            public Vector2 BottomLeft;
            public Vector2 BottomRight;
        }

        private class Direction {
            public readonly Vector2 DirectionVector;
            public int RayCount;
            public float RaySpacing;

            public Direction(int rCount, int rSpacing, Vector2 dir) {
                RayCount = rCount;
                RaySpacing = rSpacing;
                DirectionVector = dir;
                NegativeCollision = false;
                PositiveCollision = false;
            }

            public bool NegativeCollision { get; set; }

            public bool PositiveCollision { get; set; }
        }

        public class CollisionGroup {
            private bool _down;
            private bool _left;
            private bool _right;
            private bool _up;

            public bool down {
                get => _down;
                set {
                    if (_down != value) {
                        if (value) {
                            OnLanding?.Invoke();
                        } else {
                            OnTakeoff?.Invoke();
                        }
                    }

                    _down = value;
                }
            }

            public bool up {
                get => _up;
                set {
                    if (_up != value) {
                        if (value) {
                            OnCeilingBump?.Invoke();
                        }
                    }

                    _up = value;
                }
            }

            public bool left {
                get => _left;
                set {
                    if (_left != value) {
                        if (value) {
                            OnWallBump?.Invoke();
                            OnLeftWallBump?.Invoke();
                        }
                    }

                    _left = value;
                }
            }

            public bool right {
                get => _right;
                set {
                    if (_right != value) {
                        if (value) {
                            OnWallBump?.Invoke();
                            OnRightWallBump?.Invoke();
                        }
                    }

                    _right = value;
                }
            }

            public event Util.DVoid OnLanding;
            public event Util.DVoid OnTakeoff;
            public event Util.DVoid OnCeilingBump;
            public event Util.DVoid OnLeftWallBump;
            public event Util.DVoid OnRightWallBump;
            public event Util.DVoid OnWallBump;
        }

        #endregion
    }
}
