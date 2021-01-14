using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {
  private static readonly Quaternion kTopRotation = Quaternion.Euler(90, 0, 0);
  public const float kRadius = 8;
  public const float kSpaceshipHeight = 5 + kRadius;
  public GameObject spaceship;

  private Vector3 positionVectorForAngle(float phi, float theta, float radius) {
    return (new Vector3(Mathf.Sin(theta) * Mathf.Cos(phi), Mathf.Sin(theta) * Mathf.Sin(phi), Mathf.Cos(theta)) * radius) + transform.position;
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
        Vector3 position = positionVectorForAngle(phi, theta, kRadius);
        Quaternion rotation = Quaternion.LookRotation(position - transform.position) * kTopRotation;
        var tile = Instantiate(tilePrefab, position, rotation);
        tile.transform.parent = transform;

        ++Ncount;
      }
    }

    spaceship.transform.position = positionVectorForAngle(0, 0, kSpaceshipHeight);
  }

  private float angle = 0;
  private const float kTimeToCircle = 10;
  private const float kSpaceshipSpeed = (2 * Mathf.PI) / kTimeToCircle; //2*PI in degress is 360, so you get 5 seconds to complete a circle

  // Update is called once per frame
  void Update() {
    angle -= kSpaceshipSpeed * Time.deltaTime; //if you want to switch direction, use -= instead of +=
    spaceship.transform.position = positionVectorForAngle(0, angle, kSpaceshipHeight);
    spaceship.transform.rotation = Quaternion.LookRotation(spaceship.transform.position - transform.position) * kTopRotation;
  }
}
