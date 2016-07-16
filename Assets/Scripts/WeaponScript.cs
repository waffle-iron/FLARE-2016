using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponScript : MonoBehaviour
{

	public Player_WeaponADSAdjust ADS;
	public UI_WeaponCrosshair CrosshairDisp;
	public GameObject FPSCamera;

	public delegate void gunFireEvent ();
	public static event gunFireEvent triggerFireEvent;

	//GUN TYPE AND SETTINGS
	public enum weaponTypeSet
	{
		Pistol,
		Rifle,
		SMG,
		LMG,
		Shotgun,
		Launcher}
	;
	//Type of weapon (defines characteristics)
	public weaponTypeSet type = weaponTypeSet.Pistol;

	public enum cyclicTypeSet
	{
		Semi,
		Burst,
		Automatic,
		Bolt_Action,
		UNAVALIABLE}
	;
	//Cyclic type
	public cyclicTypeSet cyclicType = cyclicTypeSet.Automatic;
		
	public bool firesProjectile = false;

	//BULLET INACCURACY SETTINGS
	[Tooltip ("Size of spread while ADS initially. A value of 0-10 is recommended.")]
	public float
		initial_ADS_Spread = 10;
	[Tooltip ("Size of spread while HIP initially. A value of 45-100 is recommended.")]
	public float
		initial_HIP_Spread = 45;
	[Tooltip ("Maximum size of spread. A value of 150-300 is recommended.")]
	public float
		max_Spread = 300;
	[Tooltip ("Speed that the spread changes. A value of 200/250/300 is recommended.")]
	public float
		spread_Speed = 250;
	float spread;
	[Tooltip ("Amount of spread added per shot. A value of 20-40 is recommended. This will depend on spread_Speed.")]
	public int
		spreadPerShot = 20;
	//BULLET FIRING SETTINGS
	public int ammoPerClip = 30;
	public int ammoInClip = 0;
	
	public float RPM = 350.0f;
	float RPS = 0.0f;
	float RSEC = 0.0f;
	float temporaryFireTimer = 0.0f;
	bool canFire = false;
	public int burstLength = 3;
	//amount of times fired
	public int pelletsPerShot = 1;
	//Adjust above 1 if you want a shotgun
	
	
	float force = 0.0f;
	public float forceModifier = 20.0f;
	
	float damage = 0.0f;
	
	[Tooltip ("Set the actual damage for distance on a curve. Make sure the line extends beyond the maximum distance that this gun can shoot!")]
	public AnimationCurve
		damageCurve;
	//100 is equal to 100% of dropoff length
	
	
	float distanceToTarget = 0;
	
	//BULLET PHYSICS STUFF
	[Tooltip ("Maximum distance of this weapon.")]
	public float
		range = 300.0f;
	// Flight distance on which to calculate trajectory.
	public float muzzleVelocity = 90.0f;
	// Velocity of a projectile leaving the barrel. METRES SECOND
	float minimumDistance = 5.0f;
	// Minimal distance for a ballistic calculation. (requires less cpu)
	
	private int steps = 10;
	// How many straight lines to check. (more rays = smoother curve)
	private int rays = 10;
	// How many rays are cast per straight line check.
	
	float stepDist = 30.0f;
	float stepTime = 3.0f;
	float rayDist = 10.0f;
	float rayTime = 1.0f;
	
	public LayerMask world;
	// Layers that can be hit by the bullet.
	
	public bool impactParticles = false;
	public GameObject worldHitFX;
	public GameObject waterHitFX;
	public GameObject bloodHitFX;
	
	public bool impactHoles = false;
	public GameObject bulletHoles;

	public GameObject projectilePrefab;
	public Transform projectileSpawnPos;
	private List<Vector3> projectileTrack = new List<Vector3> ();
	public MeshRenderer dummyProjectile;

	//END BULLET PHYSICS
	void  Start ()
	{	
		RPS = RPM / 60.0f;
		RSEC = 1.0f / RPS;	
		
		//SETS UP BULLET PHYSICS
		stepDist = range / steps;
		stepTime = stepDist / muzzleVelocity;
		rayDist = range / (steps * rays);
		rayTime = rayDist / muzzleVelocity;
		minimumDistance = rayDist + 1.0f;	
	}

	void FixedUpdate ()
	{
		temporaryFireTimer -= Time.deltaTime; //Subtracts it at the rate of 1 per second.

		//For "zooming" of spread when aiming
		if (ADS.isADS) {
			
			spread = Mathf.Clamp (spread, initial_ADS_Spread, initial_HIP_Spread); //Contains the spread rate
			
			if (spread > initial_ADS_Spread) {
				spread -= spread_Speed * Time.deltaTime;
			} else {
				spread = initial_ADS_Spread;
			}
			
		} else {  
			
			spread = Mathf.Clamp (spread, initial_HIP_Spread, max_Spread); //Contains the spread rate
			
			if (spread < initial_HIP_Spread) {
				spread += spread_Speed * Time.deltaTime;
			} else {
				spread -= spread_Speed * Time.deltaTime;
			}
			
		}
		if (CrosshairDisp != null)
			CrosshairDisp.spread = spread; //Sends the spread rate to the display script for visual feedback
	}

	void  Update ()
	{
        temporaryFireTimer = Mathf.Clamp01 (temporaryFireTimer); //ASSUMING IT NEVER TAKES LONGER THAN 1 SEC
				
		
		if (temporaryFireTimer <= 0 && ammoInClip > 0 && (PLAYER_CaptureMouse.caught)) { //Checks if bullet delay is over and there is ammo in the clip
			canFire = true;
			if (firesProjectile) {
				dummyProjectile.enabled = true;
			}
		} else {
			canFire = false;
		}
		
		//Checks if there is ammo in the bag
		//Checks if there is no ammo and reloads it
		if (ammoInClip <= 0) {
            ammoInClip = ammoPerClip;
		}
		
		
		
		
		//THE FOLLOWING GETS INPUT BASED ON CYCLIC TYPE
		if (cyclicType == cyclicTypeSet.Semi) {
			if (canFire) {
				if (Input.GetButtonDown ("Fire")) {
					fire ();
				}
			}
		}
		
		if (cyclicType == cyclicTypeSet.Burst) {
			if (canFire) {
				if (Input.GetButtonDown ("Fire")) {
					StartCoroutine ("fireBurst", burstLength);
				}
			}
		}
		
		if (cyclicType == cyclicTypeSet.Automatic) {
			if (canFire) {
				if (Input.GetButton ("Fire")) {
					fire ();
				}
			}
		}
	}

	
	
	
	
	
	
	
	
	
	
	IEnumerator  fireBurst (int length)
	{
		for (int i = 0; i < length; i++) {
			if (canFire) {
				fire ();
				yield return new WaitForSeconds (RSEC);
			}
		}
	}

	void  fire ()
	{
		temporaryFireTimer += RSEC;
		ammoInClip--;	
				
		//triggerFireEvent ();
				
		for (int i = 0; i < pelletsPerShot; i++) { //Fires multiple times if pellets is above 1, for shotguns
			//Get innacuracy from recoil function
			StartCoroutine ("CalculateTrajectory", recoil ());
		}

		if (firesProjectile) {
			projectileTrack.Clear ();
			dummyProjectile.enabled = false;
			projectileTrack.Add (projectileSpawnPos.position);
		}
	}

	
	Vector3  recoil ()
	{
		Vector2 randomPointInSpread = Random.insideUnitCircle * ((spread / 2000) * range); //Calculates spread
		spread += (spreadPerShot / pelletsPerShot); //Adds spread that affects the next shot
		
		return transform.TransformDirection (new Vector3 (randomPointInSpread.x, randomPointInSpread.y, range));
	}
	
	
	
	
	/*The following bullet physics code was not done by me and is hard to debug! BUT ITS SO COOL!*/

	
	
	IEnumerator  CalculateTrajectory (Vector3 dir0)
	{
		Vector3 src0 = FPSCamera.transform.position;
				
		// Start raycasting from our weapon"s muzzle:
		Vector3 dir = transform.forward;
		Vector3 src = transform.position;
		
		dir = dir0;
		src = src0;
		
		// Check wether the target is close enough to neglect the ballistic trajectory:
		bool doBallisticCheck = false;
		RaycastHit ray0;
		if (Physics.Raycast (src0, dir0, out ray0, minimumDistance, world) && !firesProjectile) {
			AddImpactEffects (ray0.point, dir, dir, ray0.collider.transform.gameObject);
			doBallisticCheck = false;
		} else {
			doBallisticCheck = true;
		}
		
		// If it"s too far away, do the ballistics stuff:
		if (doBallisticCheck == true) {
			
			bool targetHit = false;
			// Check for each line:
			for (int s = 0; s < steps; s++) {
				
				if (targetHit == false) {
					// Calculate the coordinate of the starting and ending point:
					
					// Check for each step on the line:
					for (int r = 0; r < steps; r++) {
						if (s == 0 && r == 0) {
						} else {
							if (!firesProjectile) {
								yield return new WaitForSeconds (rayTime);
							}
						}
						if (targetHit == false) {
							float nextTime = s * stepTime + r * rayTime;	//(r + 1)
							Vector3 nextPos = GetTrajectoryCoord (dir0, src0, nextTime);
							RaycastHit ray;
							Debug.DrawLine (src, nextPos, Color.yellow, stepTime);
							if (Physics.Linecast (src, nextPos, out ray, world)) {
								src = ray.point;
								if (firesProjectile) {
									projectileTrack.Add (src);
								}
								if (!firesProjectile) {
									AddImpactEffects (ray.point, dir, Vector3.Normalize (src - nextPos), ray.collider.transform.gameObject);
								} else {
									SpawnProjectile ();
								}
								targetHit = true;
							} else {
								src = nextPos;
								if (firesProjectile) {
									projectileTrack.Add (src);
								}
							}
						}
					}
				}
			}
			if (targetHit == false && firesProjectile) {
				SpawnProjectile ();
			}
		}
	}

	void SpawnProjectile ()
	{
		//PlayerWeapons.CmdSpawnProjectile (projectileSpawnPos.position, projectileSpawnPos.rotation, projectileTrack.ToArray (), muzzleVelocity, Client_Credentials._name);
	}

	Vector3 GetTrajectoryCoord (Vector3 d0, Vector3 s0, float t)
	{
		// Trajectory: y = y0 + v0y * (∆x / v0x) - 0.5f * g * (∆x / v0x)²
		// With x = x0 + v0x * t
		// And  y = y0 + v0y * t - 0.5f * g * t²
		// We know the time (steps * rays * rayDist), so:
		
		// 1. Get the "horizontal" direction (xDir):
		// (xDir is always perpendicular to gravity)
		Vector3 xDir = Vector3.Normalize (new Vector3 (d0.x, 0.0f, d0.z));
		
		// 2. Get the initial velocity along xDir and Vec3.up:
		float inversion = 1.0f;	// (Major bug fix)
		if (Vector3.Angle (Vector3.up, d0) >= 90.0f) {
			inversion = -1.0f;
		}
		float ang = Vector3.Angle (xDir, d0) * Mathf.Deg2Rad;
		float v0y = Mathf.Sin (ang) * muzzleVelocity * inversion;
		float v0x = Mathf.Cos (ang) * muzzleVelocity;
		
		// 3. Get the vertical position after t seconds:
		float y = v0y * t - 0.5f * -Physics.gravity.y * t * t;
		
		// 4. Get the horizontal position along xDir:
		float x = v0x * t;
		
		// 5. get the final position on the trajectory:
		Vector3 pos = Vector3.up * y + xDir * x + s0;
		return pos;
	}
	
	// Add bullet impact effects where the target was hit:
	void  AddImpactEffects (Vector3 pos, Vector3 dir, Vector3 norm, GameObject baseTarget)
	{
		GameObject target = null;

		if (baseTarget.transform.parent != null) {
		    target = baseTarget.transform.root.gameObject;
	    } else {
			target = baseTarget.transform.gameObject;
		}

		Quaternion rotation = Quaternion.FromToRotation (Vector3.up, pos);

		GameObject impactFX = null;
		damage = damageCurve.Evaluate (Vector3.Distance (target.transform.position, transform.position)) / pelletsPerShot;
		distanceToTarget = Vector3.Distance (target.transform.position, transform.position);

			if (impactParticles == true) {
				if (target.tag == "Water") {
					impactFX = waterHitFX;
				} else {
					impactFX = worldHitFX;
					Instantiate (impactFX, pos, rotation);
				}
			}
		
	}
	
	
	
	
	
	
	
	
}