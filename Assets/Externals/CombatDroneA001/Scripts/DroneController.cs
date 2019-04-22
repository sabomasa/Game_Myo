using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum DroneStateTypes {
	Idle01,
	Idle02,
	LookRight,
	LookLeft,
	LookBackward,
	DualAttack,
	AttackRight,
	AttackLeft,
	HitFront,
	HitBack,
	HitRight,
	HitLeft,
	Dead
}

public enum CannonSideTypes {
	Right,
	Left
}

public class DroneController : MonoBehaviour {

	public bool IsAIControlled = false;
	public int DroneTeam = 1;

	public AudioSource DroneAudioSource;
	private float droneSpeakTimer = 0;
	private float droneSpeakTimerFreq = 0.25f;
	private int lastAudioClipIndex = 0;
	public AudioClip[] DroneSFXClipsArray;

	// Drone State Machine
	public DroneStateTypes CurrentDroneState = DroneStateTypes.Idle01;
	private float droneIdleTimer = 0;
	private float droneIdleTimerFreq = 2;

	// Attack Variables

	public KeyCode DualAttack_KeyCode = KeyCode.Alpha1;
	public KeyCode AttackRight_KeyCode = KeyCode.Alpha2;
	public KeyCode AttackLeft_KeyCode = KeyCode.Alpha3;
	public KeyCode TakeHitFront_KeyCode = KeyCode.I;
	public KeyCode TakeHitBack_KeyCode = KeyCode.K;
	public KeyCode TakeHitRight_KeyCode = KeyCode.L;
	public KeyCode TakeHitLeft_KeyCode = KeyCode.J;

	public GameObject CannonProjectilePrefab;
	public GameObject CannonLargeProjectilePrefab;
	public GameObject FireEffectPrefab;
	private bool droneAttacking = false;
	private float droneAttackTimer = 0;
	private float droneAttackTimerFreq = 1;
	private bool firedRight = false;
	private bool firedLeft = false;
	private float reloadTimer = 0;
	private float reloadTimerFreq = 1.0f;

	// Drone Taking Hits
	private bool droneBeingHit = false;
	private float droneHitTimer = 0;
	private float droneHitTimerFreq = 1;

	private bool firedEffect_HitFront = false;
	private bool firedEffect_HitBack = false;
	private bool firedEffect_HitRight = false;
	private bool firedEffect_HitLeft = false;
	public AudioSource TakeHitAudioSource;
	public ParticleSystem EffectPS_HitFront;
	public ParticleSystem EffectPS_HitBack;
	public ParticleSystem EffectPS_HitRight;
	public ParticleSystem EffectPS_HitLeft;

	// Drone Hit Points
	public bool DroneAlive = true;
	public bool TestDamagingDrone = false;
	public int DroneHitPoints = 90;
	public int DroneHitPointsMAX = 90;

	private bool droneExplosionSetup = false;
	private float droneExploTimer = 0;
	private float droneExploTimerFreq = 0.25f;
	private Transform[] droneTransforms;
	public GameObject DroneModelBaseGO;
	private int numberOfDroneExplo = 0;
	private int exploIndex = 0;

	public GameObject MainDroneModelGO;
	public GameObject RightArmGO;
	public GameObject LeftArmGO;

	public GameObject DroneShardExplosionPrefab;
	private void SimulateDroneExplosion() {
		if (!droneExplosionSetup) {
			myAnimator.enabled = false;
			myNavMeshAgent.enabled = false;
			gameObject.GetComponent<CapsuleCollider> ().enabled = false;
			if (DroneModelBaseGO != null) {
				droneTransforms = DroneModelBaseGO.GetComponentsInChildren<Transform> ();
				numberOfDroneExplo = droneTransforms.Length;
//				for (int i = 0; i < droneTransforms.Length; i++) {
//					droneTransforms [i].parent = null;
//				}
			}

			MainDroneModelGO.transform.parent = null;
			MainDroneModelGO.AddComponent<SphereCollider>();
			Rigidbody newRigid = MainDroneModelGO.AddComponent<Rigidbody> ();
			newRigid.mass = 1000;
			newRigid.drag = 0.5f;
			RightArmGO.AddComponent<BoxCollider> ();
			LeftArmGO.AddComponent<BoxCollider> ();

			droneExplosionSetup = true;
		} else {
			// Do Drone Destruction
			if (numberOfDroneExplo > 0) {
				if (droneExploTimer < droneExploTimerFreq) {
					droneExploTimer += Time.deltaTime;
				} else {
					// Do Next Drone Explo
					if (droneTransforms [exploIndex] != null) {						
//						Destroy (droneTransforms [exploIndex].gameObject, Random.Range(1.0f, 2.0f));
//						droneTransforms [exploIndex].gameObject.AddComponent<BoxCollider> ();
						DroneExplosionShard exploShard = droneTransforms [exploIndex].gameObject.AddComponent<DroneExplosionShard> ();
						exploShard.ExplosionPrefab = DroneShardExplosionPrefab;
						exploShard.ExplosionLifeTime = 10.0f;
						exploShard.LifeTime = Random.Range (0.5f, 1.0f);
//						Rigidbody newRigid = droneTransforms [exploIndex].gameObject.AddComponent<Rigidbody> ();
//						newRigid.mass = 1000;
//						newRigid.drag = 0.5f;
					}

					// Reset For Next
					exploIndex++;
					numberOfDroneExplo--;
					droneExploTimer = 0;
					droneExploTimerFreq = Random.Range (0.05f, 0.075f);
				}
			} else {
				Destroy (gameObject);
			}
		}
	}

	public float droneDeathTimer = 0;
	public GameObject DroneFinalExplosionPrefab;

	// Drone Damage Materials
	private List<Renderer> droneMainRenderers;
	public Material Drone_Material01;
	public int NumberMainRends = 0;

	private List<Renderer> droneEyeRenderers;
	public Material DroneEye_Material01;
	public int NumberEyeRends = 0;

	public Material DroneDamage_Material01;
	public Material DroneDestroyed_Material01;

	public ParticleSystem HoverEffectsPS;
	public Light HoverEffectsLight;

	private void SetupMaterials () {		
		Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer> ();
		droneMainRenderers = new List<Renderer> ();
		droneEyeRenderers = new List<Renderer> ();
		if (renderers.Length > 0) {
			for (int i = 0; i < renderers.Length; i++) {
				if (renderers [i].sharedMaterial == Drone_Material01) {
					droneMainRenderers.Add (renderers [i]);
					NumberMainRends++;
				} else if (renderers [i].sharedMaterial == DroneEye_Material01) {
					droneEyeRenderers.Add (renderers [i]);
					NumberEyeRends++;
				}
			}
		}
	}

	public ParticleSystem DamageSmokePS;
	public ParticleSystem[] DamageSparksPSArray;

	public AudioSource DamageSoundAudioSource;
	public AudioClip[] DamageSoundClips;
	private float damageSFXPlayTimer = 0;
	private float damageSFXPlayTimerFreq = 2.0f;

	private bool hasPlayedDamageExplosion = false;
	public ParticleSystem DamageExplosionPS;
	public AudioSource DamageExplosionSFX;

	private void UpdateDamageEffects() {
		if (DroneHitPoints > DroneHitPointsMAX * 0.75f) {
			hasPlayedDamageExplosion = false;
			// Hit Point State 01 - No Damage
			if (DamageSmokePS != null) {
				if (DamageSmokePS.isPlaying) {
					DamageSmokePS.Stop ();
				}			
			}
			if (DamageSparksPSArray != null) {
				if (DamageSparksPSArray.Length > 0) {
					for (int i = 0; i < DamageSparksPSArray.Length; i++) {
						if (DamageSparksPSArray[i].isPlaying) {
							DamageSparksPSArray[i].Stop ();
						}
					}
				}
			}
		} else if (DroneHitPoints <= DroneHitPointsMAX * 0.75f && DroneHitPoints >= 1) {
			if (!hasPlayedDamageExplosion) {
				if (DamageExplosionPS != null) {
					DamageExplosionPS.Play ();
				}
				if (DamageExplosionSFX != null) {
					DamageExplosionSFX.Play ();
				}
				hasPlayedDamageExplosion = true;
			}
			// Hit Point State 01 - No Damage
			for (int i = 0; i < droneMainRenderers.Count; i++) {
				if (droneMainRenderers [i].sharedMaterial != DroneDamage_Material01)
					droneMainRenderers [i].sharedMaterial = DroneDamage_Material01;
			}
			if (DamageSmokePS != null) {
				if (!DamageSmokePS.isPlaying) {
					DamageSmokePS.Play ();
				}			
			}
			if (DamageSparksPSArray != null) {
				if (DamageSparksPSArray.Length > 0) {
					for (int i = 0; i < DamageSparksPSArray.Length; i++) {
						if (!DamageSparksPSArray[i].isPlaying) {
							DamageSparksPSArray[i].Play ();
						}
					}
				}
			}

			// Randomize Damage SFX
			if (DamageSoundAudioSource != null) {
				if (damageSFXPlayTimer < damageSFXPlayTimerFreq) {
					damageSFXPlayTimer += Time.deltaTime;
				} else {
					// Play Damage SoundFX
					int randomIndex = Random.Range(0, DamageSoundClips.Length);
					DamageSoundAudioSource.clip = DamageSoundClips[randomIndex];
					DamageSoundAudioSource.Play ();
					damageSFXPlayTimer = 0;
					damageSFXPlayTimerFreq = Random.Range (2.0f, 3.5f);
				}
			}
		} else {
			if (!hasPlayedDamageExplosion) {
				if (DamageExplosionPS != null) {
					DamageExplosionPS.Play ();
				}
				if (DamageExplosionSFX != null) {
					DamageExplosionSFX.Play ();
				}
				hasPlayedDamageExplosion = true;
			}
			// Hit Point State 01 - No Damage
			for (int i = 0; i < droneMainRenderers.Count; i++) {
				if (droneMainRenderers [i].sharedMaterial != DroneDestroyed_Material01)
					droneMainRenderers [i].sharedMaterial = DroneDestroyed_Material01;
			}
			if (DamageSmokePS != null) {
				if (DamageSmokePS.isPlaying) {
					DamageSmokePS.Stop ();
				}			
			}
			if (DamageSparksPSArray != null) {
				if (DamageSparksPSArray.Length > 0) {
					for (int i = 0; i < DamageSparksPSArray.Length; i++) {
						if (DamageSparksPSArray[i].isPlaying) {
							DamageSparksPSArray[i].Stop ();
						}
					}
				}
			}
		}
	}
	private void UpdateMaterialsBasedOnHitPoints() {
		if (DroneHitPoints > DroneHitPointsMAX * 0.75f) {
			// Hit Point State 01 - No Damage
			for (int i = 0; i < droneMainRenderers.Count; i++) {
				if (droneMainRenderers [i].sharedMaterial != Drone_Material01)
					droneMainRenderers [i].sharedMaterial = Drone_Material01;
			}
		} else if (DroneHitPoints <= DroneHitPointsMAX * 0.75f) {
			// Hit Point State 01 - No Damage
			for (int i = 0; i < droneMainRenderers.Count; i++) {
				if (droneMainRenderers [i].sharedMaterial != DroneDamage_Material01)
					droneMainRenderers [i].sharedMaterial = DroneDamage_Material01;
			}
		}
	}

	// Drone Targeting
	public Transform CurrentTarget;

	// Drone Animation
	private bool hasAnimator = false;
	private Animator myAnimator;

	// Drone Cannons
	public Transform Cannon_RightTransform;
	private ParticleSystem cannonFirePS_Right;
	private AudioSource cannonFireAudioSource_Right;
	public Transform Cannon_LeftTransform;
	private ParticleSystem cannonFirePS_Left;
	private AudioSource cannonFireAudioSource_Left;

	// Drone Nav Mesh Agent
	private NavMeshAgent myNavMeshAgent;

	// Use this for initialization
	void Start () {
		myAnimator = gameObject.GetComponentInChildren<Animator> ();
		if (myAnimator != null) {
			hasAnimator = true;
		}
		myNavMeshAgent = gameObject.GetComponent<NavMeshAgent> ();

		// Setup Materials
		SetupMaterials();

		// Set Drone Hit Points and Alive Bool
		DroneHitPoints = DroneHitPointsMAX;
		DroneAlive = true;

		// Setup Cannon Barrels
		GameObject newCannonBarrelRightGO = new GameObject ("CannonBarrel_Right");
		newCannonBarrelRightGO.transform.parent = Cannon_RightTransform;
		newCannonBarrelRightGO.transform.localPosition = new Vector3 (0, 0, 1.5f);

		GameObject newFireEffect_Right = GameObject.Instantiate (FireEffectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		newFireEffect_Right.transform.parent = newCannonBarrelRightGO.transform;
		newFireEffect_Right.transform.localPosition = Vector3.zero;

		cannonFirePS_Right = newFireEffect_Right.GetComponent<ParticleSystem> ();
		cannonFireAudioSource_Right = newFireEffect_Right.GetComponent<AudioSource> ();

		GameObject newCannonBarrelLeftGO = new GameObject ("CannonBarrel_Left");
		newCannonBarrelLeftGO.transform.parent = Cannon_LeftTransform;
		newCannonBarrelLeftGO.transform.localPosition = new Vector3 (0, 0, 1.5f);

		GameObject newFireEffect_Left = GameObject.Instantiate (FireEffectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		newFireEffect_Left.transform.parent = newCannonBarrelLeftGO.transform;
		newFireEffect_Left.transform.localPosition = Vector3.zero;

		cannonFirePS_Left = newFireEffect_Left.GetComponent<ParticleSystem> ();
		cannonFireAudioSource_Left = newFireEffect_Left.GetComponent<AudioSource> ();

	}
	
	// Update is called once per frame
	void Update () {

		// Testing Drone Damage
		if (Input.GetKeyUp(KeyCode.Backspace)) {
			TestDamagingDrone = true;
		}
		if (TestDamagingDrone) { 
			// Damage Drone
			TakeDamage(10);
			TestDamagingDrone = false;
		}

		// Process Drone Hit Points and Alive Bool
		ProcessDroneHitPoints();

		if (DroneAlive) {
			if (CurrentTarget == null) {
				// Find New Target
				if (DroneTeamManager.GlobalAccess != null) {
					CurrentTarget = DroneTeamManager.GlobalAccess.GetTarget (DroneTeam);
				}
			}
			if (myNavMeshAgent != null) {
				if (CurrentTarget != null) {
					//myNavMeshAgent.SetDestination (CurrentTarget.position);
				}
			}
			// Update Drone Audio SFX
			UpdateDroneSoundFX ();

			if (IsAIControlled) {
				// Drone AI Basic
				if (CurrentTarget != null) {
					if (Vector3.Distance (gameObject.transform.position, CurrentTarget.position) < 38) {
						if (HasLOSOnTarget ()) {
							if (!droneAttacking) {
								int randomAttack = Random.Range (0, 3);
								if (randomAttack == 0) {
									DoDualAttack ();
								} else if (randomAttack == 1) {
									DoAttackRight ();
								} else if (randomAttack == 2) {
									DoAttackLeft ();
								}
							}
						}
					}
				}
			} else {
				// Process AI or Keyboard Input
				ProcessKeyboardInput ();
			}
		} else {
			// Destroy Drone
			SimulateDroneExplosion ();
		}

		// Update Drone Animation State Machine
		if (hasAnimator) {
			UpdateDroneAnimation ();
		}

	}

	private bool HasLOSOnTarget() {
		return inLineOfSight(CurrentTarget);
	}

	private RaycastHit hit;
	private float fov = 60.0f;
	private bool inLineOfSight(Transform target) {
		if (Vector3.Angle (target.position - transform.position, transform.forward) <= fov &&
		    Physics.Linecast (transform.position, target.position, out hit) && hit.collider.transform == target) {
			return true;
		}

		return false;
	}

	private void TakeDamage(float damageIn) {
		DroneHitPoints -= (int)damageIn;
	}

	private void ProcessDroneHitPoints() {
		if (DroneAlive) {
			if (DroneHitPoints < 0) {
				DroneHitPoints = 0;
			}

			// Update Effects Based on Hit Points
			UpdateDamageEffects();
			// Update Damage Materials
			UpdateMaterialsBasedOnHitPoints ();

			if (DroneHitPoints == 0) {
				KillDrone ();
			}
		}
	}

	private void KillDrone() {

		if (DroneTeamManager.GlobalAccess != null) {
			DroneTeamManager.GlobalAccess.UnregisterDrone (DroneTeam, this);
		}

		// Spawn Drone Final Explosion
		if (DroneFinalExplosionPrefab != null) {
			GameObject newDroneExplosion = GameObject.Instantiate (DroneFinalExplosionPrefab, transform.position, transform.rotation) as GameObject;
			Destroy (newDroneExplosion, 20);
		}

		// Hide Hover Effects
		if (HoverEffectsPS != null) {
			HoverEffectsPS.Stop();
		}
		if (HoverEffectsLight != null) {
			HoverEffectsLight.enabled = false;
		}

		myAnimator.SetBool ("Dead", true);
		CurrentDroneState = DroneStateTypes.Dead;
		// Update Effects Based on Hit Points
		UpdateDamageEffects();
		// Kill Drone - No Hit Points Left
		DroneAlive = false;
	}

	private void ProcessKeyboardInput() {
		// Take Hits Via Input
		if (Input.GetKeyUp (TakeHitFront_KeyCode)) {
			if (!droneBeingHit) {
				TakeHitFront ();
			}
		}
		if (Input.GetKeyUp (TakeHitBack_KeyCode)) {
			if (!droneBeingHit) {
				TakeHitBack ();
			}
		}
		if (Input.GetKeyUp (TakeHitRight_KeyCode)) {
			if (!droneBeingHit) {
				TakeHitRight ();
			}
		}
		if (Input.GetKeyUp (TakeHitLeft_KeyCode)) {
			if (!droneBeingHit) {
				TakeHitLeft ();
			}
		}
		// Update Drone Animation State
		if (reloadTimer < reloadTimerFreq) {
			reloadTimer += Time.deltaTime;
		} else {
			if (Input.GetKeyUp (DualAttack_KeyCode)) {			
				if (!droneAttacking) {
					if (!droneBeingHit) {
						if (CurrentTarget != null) {
							DoDualAttack ();
						}
					}
				}
			}
			if (Input.GetKeyUp (AttackRight_KeyCode)) {			
				if (!droneAttacking) {
					if (!droneBeingHit) {
						if (CurrentTarget != null) {
							DoAttackRight ();
						}
					}
				}
			}
			if (Input.GetKeyUp (AttackLeft_KeyCode)) {			
				if (!droneAttacking) {
					if (!droneBeingHit) {
						if (CurrentTarget != null) {
							DoAttackLeft ();
						}
					}
				}
			}
		}
	}

	private void UpdateDroneAnimation() {
		
		// Drone In Idle State
		if (CurrentDroneState == DroneStateTypes.Dead) {
			droneDeathTimer += Time.deltaTime;

			if (droneDeathTimer > 0.25f) {
				myAnimator.SetBool ("Dead", false);
			}

		} else {
			if (CurrentDroneState == DroneStateTypes.Idle01 || CurrentDroneState == DroneStateTypes.Idle02 ||
			   CurrentDroneState == DroneStateTypes.LookLeft || CurrentDroneState == DroneStateTypes.LookRight
			   || CurrentDroneState == DroneStateTypes.LookBackward) {
				if (droneIdleTimer < droneIdleTimerFreq) {
					droneIdleTimer += Time.deltaTime;
				} else {
					// Change Drone Idle Animation - Randomized
					int droneIdleRandomState = Random.Range (0, 2);
					CurrentDroneState = (DroneStateTypes)droneIdleRandomState;

					// Set Drone Animator State
					myAnimator.SetInteger ("DroneState", (int)CurrentDroneState);

					droneIdleTimerFreq = 2;
					droneIdleTimer = 0;
				}
			} else if (CurrentDroneState == DroneStateTypes.DualAttack) {
				if (droneAttackTimer < droneAttackTimerFreq) {
					if (droneAttackTimer > 0.1f)
						myAnimator.SetBool ("DualAttack", false);

					// Do Attacks
					if (droneAttackTimer > 0.33f) {
						if (!firedRight) {
							// Fire Right Cannon
							FireCannon (CannonSideTypes.Right);
							firedRight = true;
						}
					}
					if (droneAttackTimer > 0.75f) {
						if (!firedLeft) {
							// Fire Left Cannon
							FireCannon (CannonSideTypes.Left);
							firedLeft = true;
						}
					}

					droneAttackTimer += Time.deltaTime;
				} else {
					CurrentDroneState = DroneStateTypes.Idle01;
					reloadTimer = 0;
					droneAttacking = false;
				}
			} else if (CurrentDroneState == DroneStateTypes.AttackRight) {
				if (droneAttackTimer < droneAttackTimerFreq) {
					if (droneAttackTimer > 0.1f)
						myAnimator.SetBool ("AttackRight", false);

					// Do Attacks
					if (droneAttackTimer > 0.33f) {
						if (!firedRight) {
							// Fire Right Cannon
							FireCannon (CannonSideTypes.Right);
							firedRight = true;
						}
					}

					droneAttackTimer += Time.deltaTime;
				} else {
					CurrentDroneState = DroneStateTypes.Idle01;
					reloadTimer = 0;
					droneAttacking = false;
				}
			} else if (CurrentDroneState == DroneStateTypes.AttackLeft) {
				if (droneAttackTimer < droneAttackTimerFreq) {
					if (droneAttackTimer > 0.1f)
						myAnimator.SetBool ("AttackLeft", false);

					// Do Attacks
					if (droneAttackTimer > 0.33f) {
						if (!firedLeft) {
							// Fire Left Cannon
							FireCannon (CannonSideTypes.Left);
							firedLeft = true;
						}
					}

					droneAttackTimer += Time.deltaTime;
				} else {
					CurrentDroneState = DroneStateTypes.Idle01;
					reloadTimer = 0;
					droneAttacking = false;
				}
			} else if (CurrentDroneState == DroneStateTypes.HitFront) {
				// Took A Hit From The Front
				if (droneHitTimer < droneHitTimerFreq) {
					if (droneHitTimer > 0.1f)
						myAnimator.SetBool ("HitFront", false);

					// Play Hit Front Effects
					if (!firedEffect_HitFront) {
						if (EffectPS_HitFront != null) {
							EffectPS_HitFront.Play ();
						}
						firedEffect_HitFront = true;
					}

					droneHitTimer += Time.deltaTime;
				} else {
					CurrentDroneState = DroneStateTypes.Idle01;
					droneBeingHit = false;
				}
			} else if (CurrentDroneState == DroneStateTypes.HitBack) {
				// Took A Hit From The Back
				if (droneHitTimer < droneHitTimerFreq) {
					if (droneHitTimer > 0.1f)
						myAnimator.SetBool ("HitBack", false);

					// Play Hit Back Effects
					if (!firedEffect_HitBack) {
						if (EffectPS_HitBack != null) {
							EffectPS_HitBack.Play ();
						}
						firedEffect_HitBack = true;
					}

					droneHitTimer += Time.deltaTime;
				} else {
					CurrentDroneState = DroneStateTypes.Idle01;
					droneBeingHit = false;
				}
			} else if (CurrentDroneState == DroneStateTypes.HitRight) {
				// Took A Hit From The Right
				if (droneHitTimer < droneHitTimerFreq) {
					if (droneHitTimer > 0.1f)
						myAnimator.SetBool ("HitRight", false);

					// Play Hit Right Effects
					if (!firedEffect_HitRight) {
						if (EffectPS_HitRight != null) {
							EffectPS_HitRight.Play ();
						}
						firedEffect_HitRight = true;
					}

					droneHitTimer += Time.deltaTime;
				} else {
					CurrentDroneState = DroneStateTypes.Idle01;
					droneBeingHit = false;
				}
			} else if (CurrentDroneState == DroneStateTypes.HitLeft) {
				// Took A Hit From The Left
				if (droneHitTimer < droneHitTimerFreq) {
					if (droneHitTimer > 0.1f)
						myAnimator.SetBool ("HitLeft", false);

					// Play Hit Left Effects
					if (!firedEffect_HitLeft) {
						if (EffectPS_HitLeft != null) {
							EffectPS_HitLeft.Play ();
						}
						firedEffect_HitLeft = true;
					}

					droneHitTimer += Time.deltaTime;
				} else {
					CurrentDroneState = DroneStateTypes.Idle01;
					droneBeingHit = false;
				}
			}
		}
	}

	private void TakeHitFront() {
		// HitFront
		// Set Drone Animator State
		droneHitTimer = 0;
		myAnimator.SetBool ("HitFront", true);
		CurrentDroneState = DroneStateTypes.HitFront;
		droneBeingHit = true;
		firedEffect_HitFront = false;

		if (TakeHitAudioSource != null) {
			TakeHitAudioSource.Play ();
		}
	}
	private void TakeHitBack() {
		// HitBack
		// Set Drone Animator State
		droneHitTimer = 0;
		myAnimator.SetBool ("HitBack", true);
		CurrentDroneState = DroneStateTypes.HitBack;
		droneBeingHit = true;
		firedEffect_HitBack = false;

		if (TakeHitAudioSource != null) {
			TakeHitAudioSource.Play ();
		}
	}
	private void TakeHitRight() {
		// HitRight
		// Set Drone Animator State
		droneHitTimer = 0;
		myAnimator.SetBool ("HitRight", true);
		CurrentDroneState = DroneStateTypes.HitRight;
		droneBeingHit = true;
		firedEffect_HitRight = false;

		if (TakeHitAudioSource != null) {
			TakeHitAudioSource.Play ();
		}
	}
	private void TakeHitLeft() {
		// HitLeft
		// Set Drone Animator State
		droneHitTimer = 0;
		myAnimator.SetBool ("HitLeft", true);
		CurrentDroneState = DroneStateTypes.HitLeft;
		droneBeingHit = true;
		firedEffect_HitLeft = false;

		if (TakeHitAudioSource != null) {
			TakeHitAudioSource.Play ();
		}
	}

	private void DoDualAttack() {
		// DualAttack
		// Set Drone Animator State
		droneAttackTimer = 0;
		myAnimator.SetBool ("DualAttack", true);
		CurrentDroneState = DroneStateTypes.DualAttack;
		firedRight = false;
		firedLeft = false;
		droneAttacking = true;
	}

	private void DoAttackRight() {
		// Attack Right 01
		// Set Drone Animator State
		droneAttackTimer = 0;
		myAnimator.SetBool ("AttackRight", true);
		CurrentDroneState = DroneStateTypes.AttackRight;
		firedRight = false;
		firedLeft = false;
		droneAttacking = true;
	}

	private void DoAttackLeft() {
		// Attack Left 01
		// Set Drone Animator State
		droneAttackTimer = 0;
		myAnimator.SetBool ("AttackLeft", true);
		CurrentDroneState = DroneStateTypes.AttackLeft;
		firedRight = false;
		firedLeft = false;
		droneAttacking = true;
	}

	private void DoRandomAttack() {
		int randomAttack = Random.Range (1, 4);
		if (randomAttack == 1) {
			// DualAttack

			// Set Drone Animator State
			droneAttackTimer = 0;
			myAnimator.SetBool ("DualAttack", true);
			CurrentDroneState = DroneStateTypes.DualAttack;
			firedRight = false;
			firedLeft = false;
			droneAttacking = true;
		} else if (randomAttack == 2) {
			// Attack Right 01
			// Set Drone Animator State
			droneAttackTimer = 0;
			myAnimator.SetBool ("AttackRight", true);
			CurrentDroneState = DroneStateTypes.AttackRight;
			firedRight = false;
			firedLeft = false;
			droneAttacking = true;
		} else if (randomAttack == 3) {
			// Attack Left 01
			// Set Drone Animator State
			droneAttackTimer = 0;
			myAnimator.SetBool ("AttackLeft", true);
			CurrentDroneState = DroneStateTypes.AttackLeft;
			firedRight = false;
			firedLeft = false;
			droneAttacking = true;
		} else {
			// Drone Attack 01
			// Set Drone Animator State
			droneAttackTimer = 0;
			myAnimator.SetBool ("DualAttack", true);
			CurrentDroneState = DroneStateTypes.DualAttack;
			firedRight = false;
			firedLeft = false;
			droneAttacking = true;
		}
	}

	private void FireCannon(CannonSideTypes sideType) {
		switch (mode) {
			//遅くて大きい弾
			case 1:
				if (sideType == CannonSideTypes.Right) {
					if (cannonFirePS_Right != null) {
						cannonFirePS_Right.Play();
					}
					if (cannonFireAudioSource_Right != null) {
						cannonFireAudioSource_Right.Play();
					}
					if (CannonProjectilePrefab != null) {
						Quaternion fireDirection = Quaternion.LookRotation(CurrentTarget.position - Cannon_RightTransform.position + new Vector3(0f, 0.5f, 0f));
						GameObject newProj = GameObject.Instantiate(CannonLargeProjectilePrefab, Cannon_RightTransform.position, fireDirection) as GameObject;
						newProj.GetComponent<DroneProjectileLarge>().ProjectileVelocity = 2f;
					}

				} else if (sideType == CannonSideTypes.Left) {
					if (cannonFirePS_Left != null) {
						cannonFirePS_Left.Play();
					}
					if (cannonFireAudioSource_Left != null) {
						cannonFireAudioSource_Left.Play();
					}
					if (CannonProjectilePrefab != null) {
						Quaternion fireDirection = Quaternion.LookRotation(CurrentTarget.position - Cannon_LeftTransform.position + new Vector3(0f, 0.5f, 0f));
						GameObject newProj = GameObject.Instantiate(CannonLargeProjectilePrefab, Cannon_LeftTransform.position, fireDirection) as GameObject;
						newProj.GetComponent<DroneProjectileLarge>().ProjectileVelocity = 2f;
					}
				}
				break;
			//複数WAYの弾
			case 2:
				//way数は3~5、角度は30°~60°でランダム
				int way = Random.Range(3, 6);
				int angle = Random.Range(15, 31);
				for (int i = 1; i <= way; i++) {
					float cannonAngle = -angle + i * ( angle * 2 ) / way;

					if (sideType == CannonSideTypes.Right) {
						if (cannonFirePS_Right != null) {
							cannonFirePS_Right.Play();
						}
						if (cannonFireAudioSource_Right != null) {
							cannonFireAudioSource_Right.Play();
						}
						if (CannonProjectilePrefab != null) {
							Quaternion fireDirection = Quaternion.LookRotation(CurrentTarget.position - Cannon_RightTransform.position + new Vector3(0f, 0.5f, 0f));
							Vector3 direction = fireDirection.eulerAngles;
							direction.y += cannonAngle;
							fireDirection = Quaternion.Euler(direction);
							GameObject newProj = GameObject.Instantiate(CannonProjectilePrefab, Cannon_RightTransform.position, fireDirection) as GameObject;
							newProj.GetComponent<DroneProjectile>().ProjectileVelocity = 7.5f;
						}
					} else if (sideType == CannonSideTypes.Left) {
						if (cannonFirePS_Left != null) {
							cannonFirePS_Left.Play();
						}
						if (cannonFireAudioSource_Left != null) {
							cannonFireAudioSource_Left.Play();
						}
						if (CannonProjectilePrefab != null) {
							Quaternion fireDirection = Quaternion.LookRotation(CurrentTarget.position - Cannon_LeftTransform.position + new Vector3(0f, 0.5f, 0f));
							Vector3 direction = fireDirection.eulerAngles;
							direction.y += cannonAngle;
							fireDirection = Quaternion.Euler(direction);
							GameObject newProj = GameObject.Instantiate(CannonProjectilePrefab, Cannon_LeftTransform.position, fireDirection) as GameObject;
							newProj.GetComponent<DroneProjectile>().ProjectileVelocity = 7.5f;
						}
					}
				}
				break;
			//半分爆撃(敵から見て左)
			case 3:
				//50発打つ
				for(int i = 1; i <= 48; i++) {
					//弾のターゲットと速度
					Vector3 targetPosition = new Vector3(i % 12 - 12, Random.Range(0f, 0.5f), i - 15);
					float speed = Random.Range(1f, 3f);
					float accel = Random.Range(0.1f, 0.5f);

					if (sideType == CannonSideTypes.Right) {
						if (cannonFirePS_Right != null) {
							cannonFirePS_Right.Play();
						}
						if (cannonFireAudioSource_Right != null) {
							cannonFireAudioSource_Right.Play();
						}
						if (CannonProjectilePrefab != null) {
							Quaternion fireDirection = Quaternion.LookRotation(targetPosition - Cannon_RightTransform.position + new Vector3(0f, 0.5f, 0f));
							GameObject newProj = GameObject.Instantiate(CannonLargeProjectilePrefab, Cannon_RightTransform.position, fireDirection) as GameObject;
							newProj.GetComponent<DroneProjectileLarge>().ProjectileVelocity = speed;
							newProj.GetComponent<DroneProjectileLarge>().accel = accel;
						}
					} else if (sideType == CannonSideTypes.Left) {
						if (cannonFirePS_Left != null) {
							cannonFirePS_Left.Play();
						}
						if (cannonFireAudioSource_Left != null) {
							cannonFireAudioSource_Left.Play();
						}
						if (CannonProjectilePrefab != null) {
							Quaternion fireDirection = Quaternion.LookRotation(targetPosition - Cannon_LeftTransform.position + new Vector3(0f, 0.5f, 0f));
							GameObject newProj = GameObject.Instantiate(CannonLargeProjectilePrefab, Cannon_LeftTransform.position, fireDirection) as GameObject;
							newProj.GetComponent<DroneProjectileLarge>().ProjectileVelocity = speed;
							newProj.GetComponent<DroneProjectileLarge>().accel = accel;
						}
					}
				}
				break;
			//半分爆撃(敵から見て右)
			case 4:
				//50発打つ
				for (int i = 1; i <= 48; i++) {
					//弾のターゲットと速度
					Vector3 targetPosition = new Vector3(i % 12, Random.Range(0f, 0.5f), i - 15);
					float speed = Random.Range(1f, 3f);
					float accel = Random.Range(0.1f, 0.5f);

					if (sideType == CannonSideTypes.Right) {
						if (cannonFirePS_Right != null) {
							cannonFirePS_Right.Play();
						}
						if (cannonFireAudioSource_Right != null) {
							cannonFireAudioSource_Right.Play();
						}
						if (CannonProjectilePrefab != null) {
							Quaternion fireDirection = Quaternion.LookRotation(targetPosition - Cannon_RightTransform.position + new Vector3(0f, 0.5f, 0f));
							GameObject newProj = GameObject.Instantiate(CannonLargeProjectilePrefab, Cannon_RightTransform.position, fireDirection) as GameObject;
							newProj.GetComponent<DroneProjectileLarge>().ProjectileVelocity = speed;
							newProj.GetComponent<DroneProjectileLarge>().accel = accel;
						}
					} else if (sideType == CannonSideTypes.Left) {
						if (cannonFirePS_Left != null) {
							cannonFirePS_Left.Play();
						}
						if (cannonFireAudioSource_Left != null) {
							cannonFireAudioSource_Left.Play();
						}
						if (CannonProjectilePrefab != null) {
							Quaternion fireDirection = Quaternion.LookRotation(targetPosition - Cannon_LeftTransform.position + new Vector3(0f, 0.5f, 0f));
							GameObject newProj = GameObject.Instantiate(CannonLargeProjectilePrefab, Cannon_LeftTransform.position, fireDirection) as GameObject;
							newProj.GetComponent<DroneProjectileLarge>().ProjectileVelocity = speed;
							newProj.GetComponent<DroneProjectileLarge>().accel = accel;
						}
					}
				}
				break;
			//半分爆撃2連(敵から見て左)
			case 5:
				//50発打つ
				for (int i = 1; i <= 22; i++) {
					//弾のターゲットと速度
					Vector3 targetPosition = new Vector3(i % 13 - 10, Random.Range(0f, 0.8f), Random.Range(0, 30));
					float speed = Random.Range(1f, 2f);
					float accel = Random.Range(0.2f, 0.2f);

					if (sideType == CannonSideTypes.Right) {
						if (cannonFirePS_Right != null) {
							cannonFirePS_Right.Play();
						}
						if (cannonFireAudioSource_Right != null) {
							cannonFireAudioSource_Right.Play();
						}
						if (CannonProjectilePrefab != null) {
							Quaternion fireDirection = Quaternion.LookRotation(targetPosition - Cannon_RightTransform.position + new Vector3(0f, 0.5f, 0f));
							GameObject newProj = GameObject.Instantiate(CannonLargeProjectilePrefab, Cannon_RightTransform.position, fireDirection) as GameObject;
							newProj.GetComponent<DroneProjectileLarge>().ProjectileVelocity = speed;
							newProj.GetComponent<DroneProjectileLarge>().accel = accel;
						}
					} else if (sideType == CannonSideTypes.Left) {
						if (cannonFirePS_Left != null) {
							cannonFirePS_Left.Play();
						}
						if (cannonFireAudioSource_Left != null) {
							cannonFireAudioSource_Left.Play();
						}
						if (CannonProjectilePrefab != null) {
							Quaternion fireDirection = Quaternion.LookRotation(targetPosition - Cannon_LeftTransform.position + new Vector3(0f, 0.5f, 0f));
							GameObject newProj = GameObject.Instantiate(CannonLargeProjectilePrefab, Cannon_LeftTransform.position, fireDirection) as GameObject;
							newProj.GetComponent<DroneProjectileLarge>().ProjectileVelocity = speed;
							newProj.GetComponent<DroneProjectileLarge>().accel = accel;
						}
					}
				}
				break;
			//半分爆撃2連(敵から見て右)
			case 6:
				//50発打つ
				for (int i = 1; i <= 22; i++) {
					//弾のターゲットと速度
					Vector3 targetPosition = new Vector3(i % 13 - 2, Random.Range(0f, 0.8f), Random.Range(0, 30));
					float speed = Random.Range(1f, 2f);
					float accel = Random.Range(0.2f, 0.2f);

					if (sideType == CannonSideTypes.Right) {
						if (cannonFirePS_Right != null) {
							cannonFirePS_Right.Play();
						}
						if (cannonFireAudioSource_Right != null) {
							cannonFireAudioSource_Right.Play();
						}
						if (CannonProjectilePrefab != null) {
							Quaternion fireDirection = Quaternion.LookRotation(targetPosition - Cannon_RightTransform.position + new Vector3(0f, 0.5f, 0f));
							GameObject newProj = GameObject.Instantiate(CannonLargeProjectilePrefab, Cannon_RightTransform.position, fireDirection) as GameObject;
							newProj.GetComponent<DroneProjectileLarge>().ProjectileVelocity = speed;
							newProj.GetComponent<DroneProjectileLarge>().accel = accel;
						}
					} else if (sideType == CannonSideTypes.Left) {
						if (cannonFirePS_Left != null) {
							cannonFirePS_Left.Play();
						}
						if (cannonFireAudioSource_Left != null) {
							cannonFireAudioSource_Left.Play();
						}
						if (CannonProjectilePrefab != null) {
							Quaternion fireDirection = Quaternion.LookRotation(targetPosition - Cannon_LeftTransform.position + new Vector3(0f, 0.5f, 0f));
							GameObject newProj = GameObject.Instantiate(CannonLargeProjectilePrefab, Cannon_LeftTransform.position, fireDirection) as GameObject;
							newProj.GetComponent<DroneProjectileLarge>().ProjectileVelocity = speed;
							newProj.GetComponent<DroneProjectileLarge>().accel = accel;
						}
					}
				}
				break;
			//元々のやつ
			default:
				if (sideType == CannonSideTypes.Right) {
					//Debug.Log ("Firing Right Cannon!");

					if (cannonFirePS_Right != null) {
						cannonFirePS_Right.Play();
					}
					if (cannonFireAudioSource_Right != null) {
						cannonFireAudioSource_Right.Play();
					}
					if (CannonProjectilePrefab != null) {
						Quaternion fireDirection = Quaternion.LookRotation(CurrentTarget.position - Cannon_RightTransform.position + new Vector3(0f, 0.5f, 0f));
						GameObject newProj = GameObject.Instantiate(CannonProjectilePrefab, Cannon_RightTransform.position, fireDirection) as GameObject;
					}

				} else if (sideType == CannonSideTypes.Left) {
					//Debug.Log ("Firing Left Cannon!");

					if (cannonFirePS_Left != null) {
						cannonFirePS_Left.Play();
					}
					if (cannonFireAudioSource_Left != null) {
						cannonFireAudioSource_Left.Play();
					}
					if (CannonProjectilePrefab != null) {
						Quaternion fireDirection = Quaternion.LookRotation(CurrentTarget.position - Cannon_LeftTransform.position + new Vector3(0f, 0.5f, 0f));
						GameObject newProj = GameObject.Instantiate(CannonProjectilePrefab, Cannon_LeftTransform.position, fireDirection) as GameObject;
					}
				}
				break;
		}
	}

	private void UpdateDroneSoundFX() {
		if (DroneAudioSource != null) {
			if (DroneSFXClipsArray.Length > 0) {
		
				// Update Drone Audio
				if (!DroneAudioSource.isPlaying) {
					// Done Not Speaking
					if (droneSpeakTimer < droneSpeakTimerFreq) {
						droneSpeakTimer += Time.deltaTime;
					} else {
						// Decide What and When To Speak Next
						int randomAudioClip = Random.Range (0, DroneSFXClipsArray.Length);
						if (randomAudioClip == lastAudioClipIndex) {
							randomAudioClip = Random.Range (0, DroneSFXClipsArray.Length);
						}
						if (randomAudioClip == lastAudioClipIndex) {
							randomAudioClip = Random.Range (0, DroneSFXClipsArray.Length);
						}
						DroneAudioSource.clip = DroneSFXClipsArray [randomAudioClip];
						lastAudioClipIndex = randomAudioClip;

						DroneAudioSource.Play ();

						// Reset Drone Speak Timer
						droneSpeakTimer = 0;
						droneSpeakTimerFreq = Random.Range (1.25f, 2.45f);
					}
				} else {
					droneSpeakTimer = 0;
				}

			}
		}
	}

	// Damage and Hit Functions
	public void TakeHit(float damageIn, Vector3 hitPosition) {
		// Drone Hit
		Debug.Log("Drone Taking a Hit! Damage: " + damageIn.ToString() + " From Angle: " + GetHitAngle(hitPosition).ToString());

		// Take Damage to Hit Points
		TakeDamage(damageIn);

	}
	void Damage(float damageIn) {
		// Drone Hit
		Debug.Log("Drone Hit! Damage: " + damageIn.ToString());

		// Take Damage to Hit Points
		TakeDamage(damageIn);
	}

	private float GetHitAngle(Vector3 hitPosition) {
		float hitAngle = 0;

		//hitAngle = Quaternion.FromToRotation(Vector3.up, transform.position - hitPosition).eulerAngles.z;
		hitAngle = Quaternion.FromToRotation(transform.rotation.eulerAngles, transform.position - hitPosition).eulerAngles.y;

		return hitAngle;
	}

	//オリジナル
	//playerBullet側から呼び出す、衝突関連
	public void TakeHitDamage(float damage) {
		//ダメージを食らう
		TakeDamage(damage);
		//前からの被弾モーション
		TakeHitFront();
	}
	//FireCannonのモード分け(1:大きくて遅い,2:複数way,3:半分爆撃)
	public int mode = 2;
	//攻撃
	public void AttackRight() {
		mode = 0;
		DoAttackRight();
	}
	public void AttackSlow() {
		mode = 1;
		DoDualAttack();
	}
	public void AttackWay() {
		mode = 2;
		DoAttackLeft();
	}
	public void AttackAreaBombingLeft() {
		mode = 3;
		DoAttackLeft();
	}
	public void AttackAreaBombingRight() {
		mode = 4;
		DoAttackRight();
	}
	public void AttackAreaBombingLeftSlow() {
		mode = 5;
		DoAttackLeft();
	}
	public void AttackAreaBombingRightSlow() {
		mode = 6;
		DoAttackRight();
	}

	public void StayCool() {
		myAnimator.SetBool("DualAttack", false);
		myAnimator.SetBool("AttackRight", false);
		myAnimator.SetBool("AttackLeft", false);
		myAnimator.SetBool("HitFront", false);
		CurrentDroneState = DroneStateTypes.Idle01;
	}
}
