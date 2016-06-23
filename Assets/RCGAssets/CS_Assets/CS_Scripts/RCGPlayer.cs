using UnityEngine;
using RoadCrossing.Types;

namespace RoadCrossing
{
	/// <summary>
	/// Defines the player, its speed and movement limits, as well as the different types of deaths it may suffer.
	/// </summary>
	public class RCGPlayer : MonoBehaviour
	{
		internal Transform thisTransform;
		internal GameObject gameController;
	
		// The player's movement speed, and variables that check if the player is moving now, where it came from, and where it's going to
		public float speed = 0.1f;
		internal bool  isMoving = false;
		internal Vector3 previousPosition;
		internal Vector3 targetPosition;

		// The movement limits object. This object contains some colliders that bounce the player back into the game area
		public Transform moveLimits;
		
		// Various animations for the player
		public AnimationClip animationMove;
		public AnimationClip animationCoin;
	
		// Death effects that show when the player is killed
		public Transform[] deathEffect;
	
		// The player can't move or perform actions for a few seconds
		public float moveDelay = 0;
	
		// Various sounds and their source
		public AudioClip[] soundMove;
		public AudioClip[] soundCoin;
		public string soundSourceTag = "GameController";
	
		/// <summary>
		/// Start is only called once in the lifetime of the behaviour.
		/// The difference between Awake and Start is that Start is only called if the script instance is enabled.
		/// This allows you to delay any initialization code, until it is really needed.
		/// Awake is always called before any Start functions.
		/// This allows you to order initialization of scripts
		/// </summary>
		void Start()
		{
			thisTransform = transform;

			targetPosition = thisTransform.position;
		
			gameController = GameObject.FindGameObjectWithTag("GameController");
		}
	
		/// <summary>
		/// Update is called every frame, if the MonoBehaviour is enabled.
		/// </summary>
		void Update()
		{
			// Keep the move limits object moving forward/backward along with the player
			if( moveLimits )
			{
				Vector3 newPosition = new Vector3();

				newPosition.x = thisTransform.position.x;

				moveLimits.position = newPosition;
			}

			// Count down the move delay
			if( moveDelay > 0 )
				moveDelay -= Time.deltaTime;
		
			// If the player is not already moving, it can move
			if( isMoving == false && moveDelay <= 0 && Time.timeScale > 0 )
			{
				// You can move left/right only if you are not already moving forward/backwards
				if( Input.GetAxisRaw("Vertical") == 0 )
				{
					// Moving right
					if( Input.GetAxisRaw("Horizontal") > 0 )
					{
						// Move one step to the right
						Move("right");
					}
				
					// Moving left
					if( Input.GetAxisRaw("Horizontal") < 0 )
					{
						// Move one step to the left
						Move("left");
					}
				}
			
				// You can move forward/backwards only if you are not already moving left/right
				if( Input.GetAxisRaw("Horizontal") == 0 )
				{
					// Moving forward
					if( Input.GetAxisRaw("Vertical") > 0 )
					{
						// Move one step forward
						Move("forward");
					}
				
					// Moving backwards
					if( Input.GetAxisRaw("Vertical") < 0 )
					{
						// Move one step backwards
						Move("backward");
					}
				}
			}
			else // Otherwise, move the player to its target position
			{
				// Keep moving towards the target position until we reach it
				if( thisTransform.position != targetPosition )
				{
					// Move this object towards the target position
					thisTransform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);
				}
				else
				{
					// The object is not moving anymore
					isMoving = false;
				}
			}
		}
	
		/// <summary>
		/// Moves an object from its current position to a target position, over time
		/// </summary>
		/// <param name="moveDirection">Move direction.</param>
		void Move(string moveDirection)
		{
			if( isMoving == false && moveDelay <= 0 )
			{
				// The object is moving
				isMoving = true;
			
				switch( moveDirection.ToLower() )
				{
					case "forward":
						// Turn to the front
						Vector3 newEulerAngle = new Vector3();
						newEulerAngle.y = 0;
						thisTransform.eulerAngles = newEulerAngle;
				
						// Set the new target position to move to
						targetPosition = thisTransform.position + new Vector3(1, 0, 0);
				
						// Make sure the player lands on the grid 
						targetPosition.x = Mathf.Round(targetPosition.x);
						targetPosition.z = Mathf.Round(targetPosition.z);
				
						// Register the last position the player was at, so we can return to it if the path is blocked
						previousPosition = thisTransform.position;
				
						break;
				
					case "backward":
						// Turn to the back
						newEulerAngle = new Vector3();
						newEulerAngle.y = 180;
						thisTransform.eulerAngles = newEulerAngle;

						// Register the last position the player was at, so we can return to it if the path is blocked
						previousPosition = thisTransform.position;
				
						// Make sure the player lands on the grid 
						targetPosition.x = Mathf.Round(targetPosition.x);
						targetPosition.z = Mathf.Round(targetPosition.z);
				
						// Set the new target position to move to
						targetPosition = thisTransform.position + new Vector3(-1, 0, 0);
				
						break;
				
					case "right":
						// Turn to the right
						newEulerAngle = new Vector3();
						newEulerAngle.y = 90;
						thisTransform.eulerAngles = newEulerAngle;

						// Register the last position the player was at, so we can return to it if the path is blocked
						previousPosition = thisTransform.position;
				
						// Make sure the player lands on the grid 
						targetPosition.x = Mathf.Round(targetPosition.x);
						targetPosition.z = Mathf.Round(targetPosition.z);
				
						// Set the new target position to move to
						targetPosition = thisTransform.position + new Vector3(0, 0, -1);
				
						break;
				
					case "left":
						// Turn to the left
						newEulerAngle = new Vector3();
						newEulerAngle.y = -90;
						thisTransform.eulerAngles = newEulerAngle;
				
						// Register the last position the player was at, so we can return to it if the path is blocked
						previousPosition = thisTransform.position;
				
						// Make sure the player lands on the grid 
						targetPosition.x = Mathf.Round(targetPosition.x);
						targetPosition.z = Mathf.Round(targetPosition.z);
				
						// Set the new target position to move to
						targetPosition = thisTransform.position + new Vector3(0, 0, 1);
				
						break;
				
					default:
						// Turn to the front
						newEulerAngle = new Vector3();
						newEulerAngle.y = 0;
						thisTransform.eulerAngles = newEulerAngle;
				
						// Set the new target position to move to
						targetPosition = thisTransform.position + new Vector3(1, 0, 0);
				
						targetPosition.Normalize();
				
						// Register the last position the player was at, so we can return to it if the path is blocked
						previousPosition = thisTransform.position;
				
						break;
				}
			
				// If there is an animation, play it
				if( GetComponent<Animation>() && animationMove )
				{
					// Stop the animation
					GetComponent<Animation>().Stop();
				
					// Play the animation
					GetComponent<Animation>().Play(animationMove.name);
				
					// Set the animation speed base on the movement speed
					GetComponent<Animation>()[animationMove.name].speed = speed;
				
					// If there is a sound source and more than one sound assigned, play one of them from the source
					if( soundSourceTag != string.Empty && soundMove.Length > 0 )
						GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent<AudioSource>().PlayOneShot(soundMove[Mathf.FloorToInt(Random.value * soundMove.Length)]);
				}
			}
		}
	
		/// <summary>
		/// Cancels the player's current move, bouncing it back to it's previous position 
		/// </summary>
		/// <returns><c>true</c> if this instance cancel move the specified moveDelayTime; otherwise, <c>false</c>.</returns>
		/// <param name="moveDelayTime">Move delay time.</param>
		void CancelMove(float moveDelayTime)
		{
			// If there is an animation, play it
			if( GetComponent<Animation>() && animationMove )
			{
				// Set the animation speed base on the movement speed
				GetComponent<Animation>()[animationMove.name].speed = -speed;
			}
		
			// Set the previous positoin as the target position to move to
			targetPosition = previousPosition;
		
			// If there is a move delay, prevent movement for a while
			moveDelay = moveDelayTime;
		}
	
		//This function animates a coin added to the player
		/// <summary>
		/// Adds the coin/changes score.
		/// </summary>
		/// <param name="addValue">value to add to score</param>
		void AddCoin(int addValue)
		{
			gameController.SendMessage("ChangeScore", addValue);
		
			// If there is an animation, play it
			if( GetComponent<Animation>() && animationCoin && animationMove )
			{
				// Play the animation
				GetComponent<Animation>()[animationCoin.name].layer = 1; 
				GetComponent<Animation>().Play(animationCoin.name); 
				GetComponent<Animation>()[animationCoin.name].weight = 0.5f;
			}
		
			// If there is a sound source and more than one sound assigned, play one of them from the source
			if( soundSourceTag != string.Empty && soundCoin.Length > 0 )
				GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent<AudioSource>().PlayOneShot(soundCoin[Mathf.FloorToInt(Random.value * soundCoin.Length)]);
		}
	
		//This function destroys the player, and triggers the game over event
		/// <summary>
		/// Destroys the player, and triggers the game over event
		/// </summary>
		/// <param name="deathType">Death effect type.</param>
		void Die(int deathType)
		{
			// Call the game over function from the game controller
			gameController.SendMessage("GameOver", 0.5f);
		
			// If we have death effects...
			if( deathEffect.Length > 0 )
			{
				// Create the correct death effect
				Instantiate(deathEffect[deathType], thisTransform.position, thisTransform.rotation);
			}
		
			// Remove this object
			Destroy(gameObject);
		}
	}
}