using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
  float currentMelting = 100f;
  const float kIceMelt = 60;
  const float kWaterMelt = 20;
  public GameObject ice;
  public GameObject water;
  public GameObject lava;

  void Start() {
    ice.SetActive(true);
    water.SetActive(false);
    lava.SetActive(false);
  }

  public void SetMeltingState(float change) {
    currentMelting -= change;
    if (currentMelting < kIceMelt && ice.activeSelf) {
      ice.SetActive(false);
      water.SetActive(true);
    }
    if (currentMelting < kWaterMelt && water.activeSelf) {
      water.SetActive(false);
      lava.SetActive(true);
    }
  }
}
