using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
	public float jumpSpeed = 10f;
	public float maxHorzVel = 10;
	public float minFOV = 60f;
	public float maxFOV = 100f;
	public float fovSpeed = 200;
	public float maxFocusTime = 3f;
	
	public int focusStep = 1;

	private Joystick mJoyStick;
	private float maxVel = 10;
	private float maxYVel = 12;
	private Vector3 vel;
	private Vector3 accl;
	private Vector3 pos;
	private static float gravity = 25f;
	private float rayLength = 0.2f;
	private bool grounded = false;
	private bool wallSticking = false;
	private int groundMask;
	private int sideWallMask;
	private int enemyMask;
	private RaycastHit ground;
	private RaycastHit sideWall;
	private float fixedDeltaTime;

	// For double jumps not actual grounded is required
	private bool fakeGrounded = false;
	private CinemachineVirtualCamera cinemachine;
	private float fov;
	private float axis;
	private bool slingShoot = false;
	private float slowTimeFactor = 0.2f;
	private float slowTimeElapsed;
	private int slowTimeStep;
	private bool animated = false;
	private float animateTimer = 0f;

	private static Bounce instance;
	public static Bounce Instance
	{
		get { return instance; }
	}

	public Vector2 getPosition
	{
		get{ return pos; }
	}

	private void Awake()
	{
		if (instance != null)
			Destroy(this.gameObject);
		else
			instance = this;

		cinemachine = FindObjectOfType<CinemachineVirtualCamera>();
	}

	void Start()
    {
		mJoyStick = FindObjectOfType<Joystick>();
		fov = minFOV;
		pos = transform.position;
		vel = new Vector3(0, 0, 0);
		accl = new Vector3(0, 0, 0);
		groundMask = LayerMask.GetMask("Ground");
		sideWallMask = LayerMask.GetMask("SideWall");
		enemyMask = LayerMask.GetMask("Enemy");
		fixedDeltaTime = Time.fixedDeltaTime;
		slowTimeStep = 0;
		slowTimeElapsed = 0f;
	}

	public float getFocusTime
	{
		get { return slowTimeElapsed; }
	}

	public int getFocusTimeStep
	{
		get { return slowTimeStep; }
	}

    // Update is called once per frame
    void LateUpdate()
    {
		if (Input.GetButtonDown("Jump"))
		{
			if (grounded)
				accl.y = jumpSpeed;
			else if (wallSticking && fakeGrounded)
			{
				vel = Vector2.zero;
				accl.y = jumpSpeed * 1.8f;
				float direcX = sideWall.transform.position.x - transform.position.x;
				accl.x = -direcX * jumpSpeed/8;
				ParticleManager.getInstance.ParticlesOnCollision();
				animated = true;
			}

			fakeGrounded = false;
		}

		if (Input.GetButtonDown("Focus") && slowTimeElapsed <= slowTimeStep && !grounded)
		{
			if (slowTimeStep < maxFocusTime)
				slowTimeStep += focusStep;
		}
		else if (Input.GetButton("Focus") && slowTimeElapsed < slowTimeStep && !grounded)
		{
			slowTimeElapsed += Time.deltaTime / Time.timeScale;

			if (slowTimeElapsed > slowTimeStep)
				slowTimeElapsed = slowTimeStep;

			if (Time.timeScale > slowTimeFactor)
			{
				Time.timeScale -= 0.05f;
				Time.fixedDeltaTime = Time.timeScale * 0.02f;
				axis += Mathf.Log(fixedDeltaTime * fovSpeed);				
				vel = Vector2.zero;
				slingShoot = true;
			}
		}
		else if (Input.GetButtonUp("Focus"))
		{
			if (slowTimeElapsed < maxFocusTime && slingShoot)
			{
				slowTimeElapsed = slowTimeStep;
				DoDoubleJump();
			}
		}
		else
		{
			Time.timeScale = 1;
			Time.fixedDeltaTime = fixedDeltaTime;
			axis -= Mathf.Log(fixedDeltaTime * fovSpeed);
		}

		axis = Mathf.Clamp(axis, 0, maxFOV - minFOV);
		fov = axis + minFOV;
		cinemachine.m_Lens.FieldOfView = Mathf.Lerp(cinemachine.m_Lens.FieldOfView, fov, 0.2f);
		cinemachine.m_Lens.OrthographicSize = Mathf.Lerp(cinemachine.m_Lens.OrthographicSize, fov/10, 0.2f);
	}

	private void DoDoubleJump()
	{
		vel = Vector2.zero;
		accl = Vector2.zero;
		accl.y = jumpSpeed * 1.5f;
		slingShoot = false;
	}

	private void ResetFocus()
	{
		slowTimeStep = 0;
		slowTimeElapsed = 0;
	}

	private void AddFocus(int addStep)
	{
		if (slowTimeElapsed > 0)
		{
			slowTimeElapsed -= addStep;
			slowTimeStep = (int)slowTimeElapsed;
		}
	}

	private void FixedUpdate()
	{
		//Grounding the ball
		if (Physics.Raycast(transform.position, -transform.up, out ground, transform.localScale.y / 2 + rayLength, groundMask))
		{
			grounded = true;
			fakeGrounded = true;
			ResetFocus();
		}
		else
			grounded = false;

		if (!grounded)
			accl.y -= gravity * Time.fixedDeltaTime;
		else if(vel.y < 0)
		{
			pos.y = ground.transform.position.y + ground.transform.localScale.y / 2 + transform.localScale.y / 2;
			vel.y = 0;
			accl = Vector2.zero;
		}

		//upperwall
		RaycastHit upperWall;
		if (Physics.Raycast(transform.position, transform.up, out upperWall, transform.localScale.y / 2, groundMask))
		{
			pos.y = upperWall.transform.position.y - upperWall.transform.localScale.y / 2 - transform.localScale.y / 2;
			vel.y *= -1;
			accl = Vector2.zero;
		}

		//Sidewall collision
		if (Physics.Raycast(transform.position, vel.normalized.x * transform.right, out sideWall, transform.localScale.y / 2 + rayLength, groundMask))
		{
			float direcX = sideWall.transform.position.x - transform.position.x;
			direcX = direcX / Mathf.Abs(direcX);
			pos.x = sideWall.transform.position.x - direcX * (sideWall.transform.localScale.x / 2 + transform.localScale.x / 2);
			vel.x = 0;
			wallSticking = true;
		}
		else
			wallSticking = false;		

		RaycastHit enemy;
		if (vel.y < 0 && Physics.Raycast(transform.position, -transform.up, out enemy, transform.localScale.y / 2 + rayLength, enemyMask))
		{
			accl.y = jumpSpeed * 1.5f;
			vel.y = 0;
			enemy.transform.GetComponent<Enemy>().kill();
			fakeGrounded = true;
			CameraShake.getInstance.shake();
			AddFocus(1);
			ParticleManager.getInstance.ParticlesOnCollision();
		}

		if (!animated)
		{


#if UNITY_ANDROID
			if (mJoyStick != null)
				vel.x = mJoyStick.Horizontal * maxVel;
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
			vel.x = Input.GetAxis("Horizontal") * maxVel;
#endif

			//vel.x = InputHandler.horz * maxVel;
		}
		else
		{
			animateTimer += Time.deltaTime;
			if (animateTimer > 0.8f)
			{
				animateTimer = 0;
				animated = false;
			}
		}

		if (Mathf.Abs(vel.x) > maxVel)
			vel.x = Mathf.Lerp(vel.x, maxVel, 0.2f);

		vel.y = Mathf.Clamp(vel.y, -maxVel, maxVel);
		vel += accl * Time.fixedDeltaTime;
		pos += vel * Time.fixedDeltaTime;
		transform.position = pos;
	}
}
