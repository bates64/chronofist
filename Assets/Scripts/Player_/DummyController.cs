using System;
using Physics;
using UnityEngine;

namespace Player_
{
    public class DummyController : MonoBehaviour
    {
        private Controller2D _controller2D;
        
        private void Awake()
        {
            _controller2D = GetComponent<Controller2D>();
        }

        private void Update()
        {
            Vector3 dir = Vector3.zero;
            if (UnityEngine.Input.GetKey(KeyCode.RightArrow)) dir.x += 3;
            if (UnityEngine.Input.GetKey(KeyCode.LeftArrow)) dir.x -= 3;
            if (UnityEngine.Input.GetKey(KeyCode.UpArrow)) dir.y += 3;
            if (UnityEngine.Input.GetKey(KeyCode.DownArrow)) dir.y -= 3;
            _controller2D.Move(dir * Time.deltaTime);
        }
    }
}