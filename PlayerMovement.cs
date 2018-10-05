using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof (ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour
{
	[SerializeField] float walkMoveStopRadius = 0.2f;
	[SerializeField] float attackMoveStopRadius = 5f;
	ThirdPersonCharacter thirdPersonCharacter;   // A reference to the ThirdPersonCharacter on the object
    CameraRaycaster cameraRaycaster;
    Vector3 currentDestination, clickPoint;

	private bool isInDirectMode = false;
	private bool isJumping;
	

	private void Start()
    {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
        currentDestination = transform.position;
    }

    // Fixed update is called in sync with physics
    private void FixedUpdate()
	{
		if (Input.GetKeyDown(KeyCode.G))  // G for gamepad. TODO allow player to remap later
		{
			currentDestination = transform.position;
			isInDirectMode = !isInDirectMode; //Toggle mode
			Debug.Log("Control mode changed");
		}

		if (isInDirectMode)
		{
			ProcessDirectMovement();
		}
		else
		{
			
			ProcessMouseMovement();
		}
		if (!isJumping)
		{
			isJumping = Input.GetButtonDown("Jump");
		}
	}

	private void ProcessDirectMovement()
	{
		currentDestination = transform.position;
		bool crouch = Input.GetKey(KeyCode.C);
		bool sprint = Input.GetKey(KeyCode.LeftShift);
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		// calculate camera relative direction to move:
		Vector3 cameraFoward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
		Vector3 movement = v * cameraFoward + h * Camera.main.transform.right;
		IsSprinting(sprint);

		thirdPersonCharacter.Move(movement, crouch, isJumping);
		isJumping = false;
	}


	private void ProcessMouseMovement()
	{
		bool sprint = Input.GetKey(KeyCode.LeftShift);
		bool crouch = Input.GetKey(KeyCode.C);
		if (Input.GetMouseButton(0))
		{
			clickPoint = cameraRaycaster.hit.point;
			print("Cursor raycast hit layer: " + cameraRaycaster.layerHit);
			switch (cameraRaycaster.layerHit)
			{
				case Layer.Walkable:
					currentDestination = ShortDestination(clickPoint, walkMoveStopRadius);
					break;
				case Layer.Enemy:
					currentDestination = ShortDestination(clickPoint, attackMoveStopRadius);
					break;
				default:
					print("SHOULDNT BE HERE!");
					return;

			}
		}
		WalkToDestination(sprint, crouch);
	}

	private void IsSprinting(bool sprinting)
	{
		if (sprinting)
		{
			thirdPersonCharacter.m_MoveSpeedMultiplier = 1.15f;
			thirdPersonCharacter.m_AnimSpeedMultiplier = 1.35f;
		}
		else if (!sprinting)
		{
			thirdPersonCharacter.m_MoveSpeedMultiplier = 1.2f;
			thirdPersonCharacter.m_AnimSpeedMultiplier = .8f;
		}
	}

	private void WalkToDestination(bool sprint, bool crouch)
	{
		var playerToClickPoint = currentDestination - transform.position;
		if (playerToClickPoint.magnitude >= 0)
		{
			IsSprinting(sprint);
			thirdPersonCharacter.Move(playerToClickPoint, crouch, isJumping);
			isJumping = false;
		}
		else
		{
			IsSprinting(sprint);
			thirdPersonCharacter.Move(Vector3.zero, crouch, isJumping);
			isJumping = false;
		}
	}

	Vector3 ShortDestination( Vector3 destination, float shortening)
	{
		Vector3 reductionVector = (destination - transform.position).normalized * shortening;
		return destination - reductionVector;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.black;
		Gizmos.DrawLine(transform.position, currentDestination);
		Gizmos.DrawSphere(currentDestination, 0.15f);
		Gizmos.DrawSphere(clickPoint, 0.12f);
		Gizmos.DrawWireSphere(transform.position, attackMoveStopRadius);


	}
}


