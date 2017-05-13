using UnityEngine;
using System.Collections;

public class ParticleAutoDestroy : MonoBehaviour {

	void Start () {
		Destroy (gameObject, 3.0f);		// Auto destroy particle system object after 3 seconds
	}
}
