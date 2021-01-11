using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
  float currentMelting = 100f;

  void setMeltingState(float change) {
    currentMelting -= change;
  }
}
