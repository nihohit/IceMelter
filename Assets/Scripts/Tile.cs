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
  private ParticleSystem cloudParticles;

  void Start() {
    ice.SetActive(true);
    water.SetActive(false);
    lava.SetActive(false);
    cloudParticles = GetComponent<ParticleSystem>();
    cloudParticles.Play();
  }

  public void PopulateNeighbours() {
    var radius = GetComponent<CapsuleCollider>().radius * 1.5f;
    neighbours = Physics.OverlapSphere(transform.position, radius).Select(collider => collider.GetComponent<Tile>()).ToList();
  }

  float currentUpdateHeat = 0f;
  float heatReceived = 0f;

  private float heatUpdate() {
    var accumulator = currentUpdateHeat;
    foreach (var tile in neighbours) {
      accumulator += tile.HeatReleased();
    }
    return accumulator;
  }

  private void Update() {
    if (currentMelting < kWaterMelt) {
      return;
    }
    heatReceived = heatUpdate() * Time.deltaTime;
    currentUpdateHeat = 0f;
    currentMelting -= heatReceived;
    var emission = cloudParticles.emission;
    emission.rateOverTime = VaporReleased() / Time.deltaTime;


    if (currentMelting < kIceMelt && ice.activeSelf) {
      ice.SetActive(false);
      water.SetActive(true);
    }
    if (currentMelting < kWaterMelt && water.activeSelf) {
      cloudParticles.Stop();
      water.SetActive(false);
      lava.SetActive(true);
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
      return heatReceived / 4;
    }
    return heatReceived;
  }
}
