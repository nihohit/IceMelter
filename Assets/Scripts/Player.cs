using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {

  [SerializeField, Range(0f, 100f)]
  float maxSpeed = 10f;

  [SerializeField, Range(0f, 100f)]
  float maxAcceleration = 10f;

  [SerializeField, Range(0f, 1f)]
  float bounciness = 0.5f;

  // [SerializeField]
  // Rect allowedArea = new Rect(-5f, -5f, 10f, 10f);

  Vector2 direction;
  Vector3 velocity;

  private void Update() {
    Vector3 displacement = velocity * Time.deltaTime;
    transform.localPosition = transform.localPosition + displacement;
    Vector3 desiredVelocity =
    new Vector3(direction.x, 0f, direction.y) * maxSpeed;

    float maxSpeedChange = maxAcceleration * Time.deltaTime;
    velocity.x =
        Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
    velocity.z =
        Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);

    // if (newPosition.x < allowedArea.xMin) {
    //     newPosition.x = allowedArea.xMin;
    //     velocity.x = -velocity.x * bounciness;
    // }
    // else if (newPosition.x > allowedArea.xMax) {
    //     newPosition.x = allowedArea.xMax;
    //     velocity.x = -velocity.x * bounciness;
    // }
    // if (newPosition.z < allowedArea.yMin) {
    //     newPosition.z = allowedArea.yMin;
    //     velocity.z = -velocity.z * bounciness;
    // }
    // else if (newPosition.z > allowedArea.yMax) {
    //     newPosition.z = allowedArea.yMax;
    //     velocity.z = -velocity.z * bounciness;
    // }
  }

  // Update is called once per frame
  public void OnMove(InputValue input) {
    direction = input.Get<Vector2>();
  }
}
