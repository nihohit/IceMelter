using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Planet : MonoBehaviour {

  [SerializeField, Range(0f, 10f)]
  float maxVerticalSpeed = 5;

  [SerializeField, Range(0f, 10f)]
  float maxHorizontalSpeed = 5f;

  [SerializeField, Range(0f, 10f)]
  float maxAcceleration = 5f;

  [SerializeField, Range(0f, 1f)]
  float bounciness = 0.5f;

  private static readonly Quaternion kTopRotation = Quaternion.Euler(90, 0, 0);
  public const float kRadius = 8;
  public const float kSpaceshipHeight = 5 + kRadius;
  public const float kBearInitialHeight = 0.5f + kRadius;
  public const float kCameraDistance = 20;
  public GameObject spaceship;
  public GameObject player;

  private Vector3 positionVectorForAngle(float phi, float theta, float radius) {
    return (new Vector3(Mathf.Sin(theta) * Mathf.Cos(phi), Mathf.Sin(theta) * Mathf.Sin(phi), Mathf.Cos(theta)) * radius) + transform.position;
  }

  private void adjustPositionAndFacePlanet(GameObject gameObject, float phi, float theta, float radius) {
    Vector3 position = positionVectorForAngle(phi, theta, radius);
    Quaternion rotation = Quaternion.LookRotation(position - transform.position) * kTopRotation;
    gameObject.transform.position = position;
    gameObject.transform.rotation = rotation;
  }

  void Awake() {
    var tilePrefab = Resources.Load<GameObject>("Tile");
    float surfaceArea = 4 * Mathf.PI * Mathf.Pow(kRadius, 2);
    float tileRadius = tilePrefab.GetComponent<CapsuleCollider>().radius;
    float tileSurface = Mathf.PI * Mathf.Pow(tileRadius, 2);
    int numberOfTiles = Mathf.FloorToInt(surfaceArea / tileSurface);

    var Ncount = 0;
    var a = 4 * Mathf.PI / numberOfTiles;
    var d = Mathf.Sqrt(a);
    var M_theta = Mathf.Round(Mathf.PI / d);
    var d_theta = Mathf.PI / M_theta;
    var d_phi = a / d_theta;
    for (var m = 0; m < M_theta; ++m) {
      var theta = Mathf.PI * (m + 0.5f) / M_theta;
      var M_phi = Mathf.Round(2 * Mathf.PI * Mathf.Sin(theta) / d_phi);
      for (var n = 0; n < M_phi; ++n) {
        var phi = 2 * Mathf.PI * n / M_phi;
        var tile = Instantiate(tilePrefab);
        tile.transform.parent = transform;
        adjustPositionAndFacePlanet(tile, phi, theta, kRadius);
        ++Ncount;
      }
    }

    spaceship.transform.position = positionVectorForAngle(0, 0, kSpaceshipHeight);
    adjustPositionAndFacePlanet(player, 0, 0, kBearInitialHeight);
  }

  // Update is called once per frame
  void Update() {
    adjustSpaceshipPosition();
    adjustPlayerPosition();
    meltTile();
  }

  private RaycastHit hit;

  private void meltTile() {
    var ray = new Ray(transform.position, player.transform.position);
    Physics.Raycast(ray, out hit);
    if (hit.collider) {
      var tile = hit.collider.GetComponent<Tile>();
      tile.SetMeltingState((1.5f - Vector3.Distance(player.transform.position, hit.point)) * Time.deltaTime);
    }
  }

  private Vector2 direction;
  private Vector2 velocity;

  private void adjustPlayerPosition() {
    Vector3 desiredVelocity = new Vector2(direction.x * maxHorizontalSpeed, direction.y * maxVerticalSpeed);

    float maxSpeedChange = maxAcceleration * Time.deltaTime;
    velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
    velocity.y = Mathf.MoveTowards(velocity.y, desiredVelocity.y, maxSpeedChange);
    var lastPosition = player.transform.position;
    var verticalChange = Vector3.up * velocity.y;
    var offset = lastPosition - transform.position;
    offset.y = 0;
    var horizontalChange = Vector3.Normalize(Vector3.Cross(offset, Vector3.up)) * velocity.x;
    horizontalChange.y = 0;
    var unnormalizedPosition = lastPosition + verticalChange + horizontalChange;
    var currentPosition = Vector3.Normalize(unnormalizedPosition - transform.position) * kRadius;
    player.transform.position = currentPosition;
    if (currentPosition != lastPosition) {
      player.transform.rotation = Quaternion.LookRotation(currentPosition - transform.position) * kTopRotation;
    }
    var cameraPosition = currentPosition - transform.position;
    cameraPosition.y = transform.position.y;
    Camera.main.transform.position = Vector3.Normalize(cameraPosition) * kCameraDistance;
    Camera.main.transform.LookAt(cameraPosition);
  }

  private float spaceshipPhi = 0;
  private float spaceshipTheta = 0;
  private const float kTimeToCircle = 10;
  private const float kSpaceshipSpeed = (2 * Mathf.PI) / kTimeToCircle; //2*PI in degress is 360, so you get 5 seconds to complete a circle

  private void adjustSpaceshipPosition() {
    spaceshipTheta -= kSpaceshipSpeed * Time.deltaTime; //if you want to switch direction, use -= instead of +=
    adjustPositionAndFacePlanet(spaceship, spaceshipPhi, spaceshipTheta, kSpaceshipHeight);
  }

  // Update is called once per frame
  public void OnMove(InputValue input) {
    direction = input.Get<Vector2>();
  }
}
