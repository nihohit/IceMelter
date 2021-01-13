using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {
  private static readonly Quaternion kTopRotation = Quaternion.Euler(90, 0, 0);
  public const float kRadius = 8;
  public const float kSpaceshipHeight = 3;
  public GameObject spaceship;

  void Awake() {
    var tilePrefab = Resources.Load<GameObject>("Tile");
    float surfaceArea = 4 * Mathf.PI * Mathf.Pow(kRadius, 2);
    float tileRadius = tilePrefab.GetComponent<CapsuleCollider>().radius * 3;
    float tileSurface = Mathf.PI * Mathf.Pow(tileRadius, 2); ;
    int numberOfTiles = Mathf.FloorToInt(surfaceArea / tileSurface);

    var Ncount = 0;
    var a = 4 * Mathf.PI / numberOfTiles;
    var d = Mathf.Sqrt(a);
    var M_theta = Mathf.Round(Mathf.PI / d);
    var d_theta = Mathf.PI / M_theta;
    var d_phi = a / (d_theta);
    for (var m = 0; m < M_theta; ++m) {
      var _theta = Mathf.PI * (m + 0.5f) / M_theta;
      var M_phi = Mathf.Round(2 * Mathf.PI * Mathf.Sin(_theta) / d_phi);
      for (var n = 0; n < M_phi; ++n) {
        var _phi = 2 * Mathf.PI * n / M_phi;
        Vector3 position = (new Vector3(Mathf.Sin(_theta) * Mathf.Cos(_phi), Mathf.Sin(_theta) * Mathf.Sin(_phi), Mathf.Cos(_theta)) * kRadius) + transform.position;
        Quaternion rotation = Quaternion.LookRotation(position - transform.position) * kTopRotation;
        var tile = Instantiate(tilePrefab, position, rotation);
        tile.transform.parent = transform;

        ++Ncount;
      }
    }

    spaceship.transform.position = new Vector3(0, kSpaceshipHeight + kRadius, 0);
  }

  //     for (int i = 0; i<numberOfTiles; ++i) {
  //       Vector3 position = Vector3.zero;
  //   Quaternion rotation = Quaternion.identity;
  //   var tile = Instantiate(tilePrefab, position, rotation);
  //   tile.transform.parent = transform;
  //     }
  //   }

  // Update is called once per frame
  void Update() { }
}
