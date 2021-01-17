using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Tile : MonoBehaviour {
  float currentMelting = 8f;
  const float kIceMelt = 6;
  const float kWaterMelt = 1;
  public GameObject ice;
  public GameObject water;
  public GameObject lava;
  public List<Tile> neighbours;

  void Start() {
    ice.SetActive(true);
    water.SetActive(false);
    lava.SetActive(false);
  }

  public void PopulateNeighbours() {
    var radius = GetComponent<CapsuleCollider>().radius * 1.5f;
    neighbours = Physics.OverlapSphere(transform.position, radius).Select(collider => collider.GetComponent<Tile>()).ToList();
  }

  float currentUpdateHeat = 0f;
  float heatReceived = 0f;
  bool needsUpdate = true;

  private void Update() {
    if (!needsUpdate) {
      return;
    }
    heatReceived = (currentUpdateHeat + neighbours.Aggregate(0f, (acc, tile) => acc + tile.HeatReleased())) * Time.deltaTime;
    currentUpdateHeat = 0f;
    currentMelting -= heatReceived;
    if (currentMelting < kIceMelt && ice.activeSelf) {
      ice.SetActive(false);
      water.SetActive(true);
    }
    if (currentMelting < kWaterMelt && water.activeSelf) {
      water.SetActive(false);
      lava.SetActive(true);
      needsUpdate = false;
    }
  }

  public void SetMeltingState(float change) {
    currentUpdateHeat = change;
  }

  public float HeatReleased() {
    if (currentMelting > kIceMelt) {
      return 0;
    }
    if (currentMelting < kWaterMelt) {
      return 0.25f;
    }
    return (kIceMelt - currentMelting) / 20;
  }

  public float VaporReleased() {
    if (currentMelting < kWaterMelt) {
      return 0;
    }
    if (currentMelting > kIceMelt) {
      return heatReceived / 2;
    }
    return heatReceived;
  }
}
