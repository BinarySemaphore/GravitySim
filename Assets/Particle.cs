using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ParticleInit {
	public Vector3 location = Vector3.zero;
	public Vector3 velocity = Vector3.zero;
	public Vector3 angularVelocity = Vector3.zero;
}

[System.Serializable]
public class ParticleConf {
	public float mass = 1.0f;
	public float heat = 0.0f;
	public float density = 1.0f;
}

[System.Serializable]
public class ParticleJson {
	public ParticleConf stats;
	public ParticleInit vectors;
}

public static class PRef {
	public static string particleTag = "Particle";
}

public class Particle : MonoBehaviour {

	public ParticleConf conf;
	public UniverseBehavior universe;

	private float maxEmissionRate;
	private Rigidbody body;
	private MeshRenderer mesh;
	private ParticleSystem pSystem;
	private ParticleSystem.EmissionModule pSystemEm;

	void Awake () {
		body = GetComponent<Rigidbody> ();
		mesh = GetComponent<MeshRenderer> ();
		pSystem = GetComponentInChildren<ParticleSystem> ();

		pSystemEm = pSystem.emission;
		maxEmissionRate = pSystemEm.rate.constantMax;

		var rate = new UnityEngine.ParticleSystem.MinMaxCurve ();
		rate.constantMax = 0;
		pSystemEm.rate = rate;
	}

	void Start () {
		UpdateObject ();
	}

	void Update () {
	}

	void FixedUpdate () {
		UpdateEmissionRate ();
	}

	void OnCollisionStay (Collision col) {
		if (col.gameObject != this && col.gameObject.tag == PRef.particleTag) {
			universe.HandleCollision (col, this);
		}
	}

	void UpdateEmissionRate () {
		var rate = new ParticleSystem.MinMaxCurve ();

		rate.constantMax = body.velocity.magnitude * universe.GetEmissionRate ();

		if (rate.constantMax > maxEmissionRate) {
			rate.constantMax = maxEmissionRate;
		}

		pSystemEm.rate = rate;
	}

	public void UpdateObject () {
		float diameter = 1.0f;
		Color color = mesh.material.color;

		body.mass = conf.mass;
		diameter = Mathf.Pow (
			(
				conf.mass / conf.density * 3.0f
			) / URef.fourPi
			, URef.cubic
		);
		color.r = 0.1f + (
			conf.heat * conf.density / 1000.0f
		);
		color.g = 0.3f;
		color.b = 0.7f;

		transform.localScale = new Vector3 (
			diameter,
			diameter,
			diameter
		);
		mesh.material.color = color;

		color.a = 0.25f;

		pSystem.startColor = color;
	}


	public void UpdateTimeChange (float change) {
		body.velocity *= change;
		pSystem.startLifetime /= change;
	}

	public Rigidbody GetBody () {
		return body;
	}

	public void SetJsonData (ParticleJson data) {
		conf = data.stats;
		transform.position = data.vectors.location;
		body.velocity = data.vectors.velocity;
		body.angularVelocity = data.vectors.angularVelocity;
	}

	public ParticleJson GetJsonData () {
		ParticleJson data = new ParticleJson ();
		data.vectors = new ParticleInit ();

		data.stats = conf;
		data.vectors.location = transform.position;
		data.vectors.velocity = body.velocity;
		data.vectors.angularVelocity = body.angularVelocity;

		return data;
	}
}
