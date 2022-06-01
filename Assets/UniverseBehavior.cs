using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MinMaxRange {
	public float min;
	public float max;

	public MinMaxRange (float min, float max) {
		this.min = min;
		this.max = max;
	}
}

[System.Serializable]
public class UniverseJsonData {
	public float massTransferRate = 25.0f;
	public float massTransferRestriction = 50000.0f;
	public float heatScale = 0.01f;
	public float tailEmissionRate = 8.0f;
	public float gravConst = 0.0006674f;
	public float particleDensity = 990.0f;
	public int particleCount = 50;
	public float initialRadius = 100.0f;
	public int[] initialDistribution = new int[5] {2,1,1,1,1};
	public MinMaxRange initialMass = new MinMaxRange(100.0f, 500.0f);
}

public static class URef {
	public static float fourPi = Mathf.PI * 4.0f;
	public static float cubic = 1.0f / 3.0f;
	public static float minMassTransferRate = 0.1f;
}

public class UniverseBehavior : MonoBehaviour {

	public static InputBroker IB;

	public UniverseJsonData conf;

	public OrbitCamRestricted orbitCam;
	public FadeText timeFactorText;
	public Menu menu;
	public GameObject particalPrefab;

	private float timeAcc = 1.0f;
	private MinMaxRange timeRange = new MinMaxRange (0.5f, 16.0f);
	private string configFilename = "./universe.json";

	private List<Particle> particles;

	void Awake () {
		IB = new InputBroker ();
		IB.Init ();
	}

	void Start () {
		particles = new List<Particle> ();

		LoadConfigFile ();
		NewUniverse (true, conf.particleCount);
	}

	void Update () {
		IB.Update ();
		Control ();
	}

	void FixedUpdate () {
		CalculateParticleForces ();
	}

	void LateUpdate () {
		IB.PostUpdate ();
	}

	void Control () {
		if (!menu.isPaused ()) {
			if (IB.GetKeyDown (KeyCode.Period)) {
				UpdateTime (2.0f);
			} else if (IB.GetKeyDown (KeyCode.Comma)) {
				UpdateTime (0.5f);
			}
		}
		if (!menu.isPaused () || menu.isActionView ()) {
			if (IB.GetKeyDown (KeyCode.Minus)) {
				SetCameraTargetCycle (-1);
			} else if (IB.GetKeyDown (KeyCode.Equals)) {
				SetCameraTargetCycle (1);
			} else if (IB.GetKeyDown (KeyCode.Alpha1)) {
				SetCameraTargetByLargest (0);
			} else if (IB.GetKeyDown (KeyCode.Alpha2)) {
				SetCameraTargetByLargest (1);
			} else if (IB.GetKeyDown (KeyCode.Alpha3)) {
				SetCameraTargetByLargest (2);
			} else if (IB.GetKeyDown (KeyCode.Alpha4)) {
				SetCameraTargetByLargest (3);
			} else if (IB.GetKeyDown (KeyCode.Alpha5)) {
				SetCameraTargetByLargest (4);
			} else if (IB.GetKeyDown (KeyCode.Alpha6)) {
				SetCameraTargetByLargest (5);
			} else if (IB.GetKeyDown (KeyCode.Alpha7)) {
				SetCameraTargetByLargest (6);
			} else if (IB.GetKeyDown (KeyCode.Alpha8)) {
				SetCameraTargetByLargest (7);
			} else if (IB.GetKeyDown (KeyCode.Alpha9)) {
				SetCameraTargetByLargest (8);
			}
		}
	}

	void UpdateTime (float change) {
		timeAcc *= change;

		if (timeAcc > timeRange.max) {
			timeAcc = timeRange.max;
			change = 1.0f;
		} else if (timeAcc < timeRange.min) {
			timeAcc = timeRange.min;
			change = 1.0f;
		}

		timeFactorText.SetText ("Time Factor: " + timeAcc);

		if (change != 1.0f) {
			for (int i = 0; i < particles.Count; i++) {
				particles [i].UpdateTimeChange (change);
			}
		}
	}

	void SetCameraTargetByLargest (int rank) {
		Particle target = GetLargest (rank);

		if (target != null) {
			orbitCam.SetTarget (target.gameObject);
		}
	}

	void SetCameraTargetCycle (int cycleChange) {
		int index = 0;
		GameObject targetRaw = orbitCam.GetTarget ();
		Particle target = null;

		if (targetRaw != null) {
			target = targetRaw.GetComponent<Particle> ();
			index = particles.IndexOf (target);
		}

		index = (index + cycleChange) % particles.Count;
		if (index < 0) {
			index += particles.Count;
		}

		orbitCam.SetTarget (particles [index].gameObject);
	}

	void NewUniverse (bool randomize, int count) {
		timeAcc = 1.0f;
		ClearParticles ();
		CreateParticles (count);

		if (randomize) {
			RandomizeParticles ();
		}

		timeFactorText.SetText ("Time Factor: " + timeAcc);
	}

	void LoadConfigFile () {
		UniverseJsonData data = null;

		try {
			using (StreamReader r = new StreamReader (configFilename)) {
				data = JsonUtility.FromJson<UniverseJsonData> (r.ReadToEnd ());
				r.Close ();
			}

			SetJsonData (data);
		}
		catch (System.Exception e) {
			Debug.Log ("Universe Using Defaults: " + e.ToString ());
		}
	}

	void CreateParticles (int particleCount) {
		Particle newParticle = null;

		for (int i = 0; i < particleCount; i++) {
			newParticle = Instantiate (particalPrefab).GetComponent<Particle> ();
			particles.Add (newParticle);
			newParticle.universe = this;
		}
	}

	void ClearParticles () {
		for (int i = 0; i < particles.Count; i++) {
			Destroy (particles [i].gameObject);
		}
		particles.Clear ();
	}

	void RandomizeParticles () {
		float distance = 0.0f;
		float distpow = 0.0f;
		float curdistpow = 0.0f;
		float total = 0.0f;
		Vector3 location = Vector3.right;
        Vector3 tangVelocity = Vector3.zero;
        Quaternion sphereCoordinates = Quaternion.identity;
        ParticleJson data = new ParticleJson ();

		distpow = Mathf.Pow (10, conf.initialDistribution.Length - 1);
		curdistpow = 0.0f;
		total = 0.0f;
		for (int i = 0; i < conf.initialDistribution.Length; i++) {
			total += conf.initialDistribution [i];
		}

		for (int i = 0; i < particles.Count; i++) {
			data.vectors = new ParticleInit ();
            data.stats = new ParticleConf
            {
                density = conf.particleDensity,
                heat = 0.0f,
                mass = Random.Range(conf.initialMass.min, conf.initialMass.max)
            };
            
            location = Quaternion.Euler(new Vector3(
                Random.Range(-360.0f, 360.0f),
                Random.Range(-360.0f, 360.0f),
                Random.Range(-360.0f, 360.0f)
            )) * location;

            distance = 0.0f;
			for (int j = 0; j < conf.initialDistribution.Length; j++) {
				curdistpow = distpow / Mathf.Pow (10, j);
				for (int k = 0; k < conf.initialDistribution [j]; k++) {
					distance += Random.Range (0.0f, curdistpow);
				}
			}
			distance /= distpow * total;
			distance *= conf.initialRadius;

            tangVelocity = Quaternion.AngleAxis(
                Random.Range(1.0f, 30.0f),
                sphereCoordinates * Vector3.up
            ) * location - location;

			data.vectors.location = location * distance;
            data.vectors.velocity = tangVelocity;
			data.vectors.angularVelocity = Vector3.zero;

			particles [i].SetJsonData (data);
		}
	}

	Particle GetLargest (int rank) {
		if (particles.Count == 0) {
			return null;
		}

		List<Particle> masses = new List<Particle> ();
		for (int i = 0; i < particles.Count; i++) {
			for (int j = 0; j <= masses.Count; j++) {
				if (j == masses.Count || particles [i].conf.mass >= masses [j].conf.mass) {
					masses.Insert (j, particles [i]);
					break;
				}
			}
		}

		if (rank >= masses.Count) {
			rank = masses.Count;
		}

		return masses [rank];
	}

	void CalculateParticleForces () {
		float force = 0.0f;
		float distance = 0.0f;
		float timeAccPow2 = timeAcc * timeAcc;
		Vector3 direction = new Vector3 ();
		Particle pA = null;
		Particle pB = null;
		Rigidbody pABody = null;
		Rigidbody pBBody = null;

		for (int i = 0; i < particles.Count; i++) {
			pA = particles [i];
			pABody = pA.GetBody ();

			for (int j = i + 1; j < particles.Count; j++) {
				if (i == j) {
					continue;
				}

				pB = particles [j];
				pBBody = pB.GetBody ();

				direction = pB.transform.position - pA.transform.position;
				distance = direction.magnitude;

				direction.Normalize ();

				force = conf.gravConst * (pA.conf.mass * pB.conf.mass) / (distance * distance);
				direction = timeAccPow2 * force * direction;

				pABody.AddForce (direction);
				pBBody.AddForce (-direction);
			}
		}
	}

	public void HandleCollision (Collision col, Particle p) {
		float massChange = 1.0f;
		float heatChange = 1.0f;
		Particle other = col.gameObject.GetComponent<Particle> ();

		if (p.conf.mass < other.conf.mass) {
			return;
		}

		massChange = (
			(
				(
					URef.minMassTransferRate + conf.massTransferRate
				) * (
					p.conf.mass - other.conf.mass
				)
			) / (
				col.relativeVelocity.magnitude * conf.massTransferRestriction
			)
		) * timeAcc;

		if (massChange > other.conf.mass) {
			massChange = other.conf.mass;
		} else if (massChange <= 0f) {
			massChange = URef.minMassTransferRate;
		}

		heatChange = (
			col.relativeVelocity.magnitude * massChange
		) * conf.heatScale * 0.5f;

		other.conf.mass -= massChange;
		other.conf.heat += heatChange;
		p.conf.mass += massChange;
		p.conf.heat += heatChange;

		other.UpdateObject ();
		p.UpdateObject ();

		if (other.conf.mass <= 0.0f) {
			RemoveParticle (other);
			Destroy (other.gameObject);
		}
	}

	public void RemoveParticle (Particle particle) {
		particles.Remove (particle);
	}

	public float GetEmissionRate () {
		return timeAcc * conf.tailEmissionRate;
	}

	public void Reset () {
		LoadConfigFile ();
		NewUniverse (true, conf.particleCount);
		orbitCam.Reset ();
	}

	public void CreateFromData (UniverseJsonData udata, ParticleJson[] pdata) {
		SetJsonData (udata);
		NewUniverse (false, pdata.Length);

		for (int i = 0; i < pdata.Length; i++) {
			particles [i].SetJsonData (pdata [i]);
		}

		SetCameraTargetByLargest (0);
	}

	public void SetJsonData (UniverseJsonData data) {
		conf = data;
	}

	public UniverseJsonData GetJsonData () {
		return conf;
	}

	public ParticleJson[] GetParticlesJsonData () {
		ParticleJson[] data = new ParticleJson[particles.Count];

		for (int i = 0; i < data.Length; i++) {
			data [i] = particles [i].GetJsonData ();
			data [i].vectors.velocity /= timeAcc;
			data [i].vectors.angularVelocity /= timeAcc;
		}

		return data;
	}
}
