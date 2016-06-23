using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using RoadCrossing.Types;

namespace RoadCrossing
{
	/// <summary>
	/// This script controls the game, starting it, following game progress, and finishing it with game over.
	/// It also creates lanes with moving objects and items as the palyer progresses.
	/// </summary>
	public class RCGGameController : MonoBehaviour
	{
		// The player object
		public Transform[] playerObjects;
		public int currentPlayer;
	
		// The camera that follows the player
		public Transform cameraObject;

		//The object that holds the movement buttons. If swipe controls are used, this object is deactivated
		public Transform moveButtonsObject;
		
		//Should swipe controls be used instead of click/tap controls?
		public bool swipeControls = false;
		
		//The start and end positions of the touches, when using swipe controls
		internal Vector3 swipeStart;
		internal Vector3 swipeEnd;
		
		//The swipe distance; How far we need to swipe before detecting movement
		public float swipeDistance = 10;

		//How long to wait before the swipe command is cancelled
		public float swipeTimeout = 1;
		internal float swipeTimeoutCount;
	
		// An array of lanes that that randomly appear as the player moves forward
		public Lane[] lanes;
		internal Lane[] lanesList;	
	
		// An array of the objects that can be dropped
		public ObjectDrop[] objectDrops;
		internal Transform[] objectDropList;

		// The offset left and right on the lane
		public int objectDropOffset = 4;
	
		// How many lanes to create before starting the game
		public int precreateLanes = 20;
		internal float nextLanePosition = 1;
	
		// The score and score text of the player
		public int score = 0;
		public Transform scoreText;
		internal int highScore = 0;

		//The player prefs record of the total coins we have ( not high score, but total coins we collected in all games )
		public string coinsPlayerPrefs = "Coins";
	
		//The overall game speed
		public float gameSpeed = 1;
	
		// How many points the player needs to collect before leveling up
		public int levelUpEveryCoins = 5;
		internal int increaseCount = 0;
	
		// This is the speed of the camera that keeps advancing on the player and kills him instantly if it reaches him
		public Transform deathLineObject;
		public float deathLineSpeed = 1;
		public float deathLineSpeedIncrease = 1;
		public float deathLineSpeedMax = 1.5f;

		// Various canvases for the UI
		public Transform gameCanvas;
		public Transform pauseCanvas;
		public Transform gameOverCanvas;
	
		// Is the game over?
		internal bool  isGameOver = false;
	
		// The level of the main menu that can be loaded after the game ends
		public string mainMenuLevelName = "MainMenu";
	
		// Various sounds
		public AudioClip soundLevelUp;
		public AudioClip soundGameOver;
	
		// The tag of the sound source
		public string soundSourceTag = "GameController";
		public string confirmButton = "Submit";
	
		// The button that pauses the game. Clicking on the pause button in the UI also pauses the game
		public string pauseButton = "Cancel";
		internal bool  isPaused = false;
		internal int index = 0;
	
		/// <summary>
		/// Start is only called once in the lifetime of the behaviour.
		/// The difference between Awake and Start is that Start is only called if the script instance is enabled.
		/// This allows you to delay any initialization code, until it is really needed.
		/// Awake is always called before any Start functions.
		/// This allows you to order initialization of scripts
		/// </summary>
		void Start()
		{
			// Update the score without adding to it
			ChangeScore(0);
		
			// Hide the game over canvas
			if( gameOverCanvas )
				gameOverCanvas.gameObject.SetActive(false);
		
			// Get the highscore for the player
			highScore = PlayerPrefs.GetInt(Application.loadedLevelName + "_HighScore", 0);
		
			// Calculate the chances for the objects to drop
			int totalLanes = 0;
			int totalLanesIndex = 0;
		
			// Calculate the total number of drops with their chances
			for(index = 0; index < lanes.Length; index++)
			{
				totalLanes += lanes[index].laneChance;
			}
		
			// Create a new list of the objects that can be dropped
			lanesList = new Lane[totalLanes];
		
			// Go through the list again and fill out each type of drop based on its drop chance
			for(index = 0; index < lanes.Length; index++)
			{
				int laneChanceCount = 0;
			
				while( laneChanceCount < lanes[index].laneChance )
				{
					lanesList[totalLanesIndex] = lanes[index];
				
					laneChanceCount++;
				
					totalLanesIndex++;
				}
			}
		
			// Calculate the chances for the objects to drop
			int totalDrops = 0;
			int totalDropsIndex = 0;
		
			// Calculate the total number of drops with their chances
			for(index = 0; index < objectDrops.Length; index++)
			{
				totalDrops += objectDrops[index].dropChance;
			}
		
			// Create a new list of the objects that can be dropped
			objectDropList = new Transform[totalDrops];
		
			// Go through the list again and fill out each type of drop based on its drop chance
			for(index = 0; index < objectDrops.Length; index++)
			{
				int dropChanceCount = 0;
			
				while( dropChanceCount < objectDrops[index].dropChance )
				{
					objectDropList[totalDropsIndex] = objectDrops[index].droppedObject;
				
					dropChanceCount++;
				
					totalDropsIndex++;
				}
			}

			//Get the currently selected player from PlayerPrefs
			currentPlayer = PlayerPrefs.GetInt("CurrentPlayer", currentPlayer);

			//Set the current player object
			SetPlayer(currentPlayer);

			// If the player object is not already assigned, Assign it from the "Player" tag
			//if( playerObjects[currentPlayer] == null )
				//playerObjects[currentPlayer] = GameObject.FindGameObjectWithTag("Player").transform;
		
			// If the player object is not already assigned, Assign it from the "Player" tag
			if( cameraObject == null )
				cameraObject = GameObject.FindGameObjectWithTag("MainCamera").transform;
		
			// Create a few lanes at the start of the game
			CreateLane(precreateLanes);
		
			// Pause the game at the start
			Pause();
		}
	
		/// <summary>
		/// Update is called every frame, if the MonoBehaviour is enabled.
		/// </summary>
		void Update()
		{
			// If the game is over, listen for the Restart and MainMenu buttons
			if( isGameOver == true )
			{
				// The jump button restarts the game
				if( Input.GetButtonDown(confirmButton) )
				{
					Restart();
				}
			
				// The pause button goes to the main menu
				if( Input.GetButtonDown(pauseButton) )
				{
					MainMenu();
				}
			}
			else
			{
				// Toggle pause/unpause in the game
				if( Input.GetButtonDown(pauseButton) )
				{
					if( isPaused == true )
						Unpause();
					else
						Pause();
				}

				//Using swipe controls to move the player
				if ( swipeControls == true )
				{
					if ( swipeTimeoutCount > 0 )    swipeTimeoutCount -= Time.deltaTime;
					
					//Check touches on the screen
					foreach ( Touch touch in Input.touches )
					{
						//Check the touch position at the beginning
						if ( touch.phase == TouchPhase.Began )
						{
							swipeStart = touch.position;
							swipeEnd = touch.position;
							
							swipeTimeoutCount = swipeTimeout;
						}
						
						//Check swiping motion
						if ( touch.phase == TouchPhase.Moved )
						{
							swipeEnd = touch.position;
						}
						
						//Check the touch position at the end, and move the player accordingly
						if( touch.phase == TouchPhase.Ended && swipeTimeoutCount > 0 )
						{
							if( (swipeStart.x - swipeEnd.x) > swipeDistance && (swipeStart.y - swipeEnd.y) < -swipeDistance ) //Swipe left
							{
								MovePlayer("left");
							}
							else if((swipeStart.x - swipeEnd.x) < -swipeDistance && (swipeStart.y - swipeEnd.y) > swipeDistance ) //Swipe right
							{
								MovePlayer("right");
							}
							else if((swipeStart.y - swipeEnd.y) < -swipeDistance && (swipeStart.x - swipeEnd.x) < -swipeDistance ) //Swipe up
							{
								MovePlayer("forward");
							}
							else if((swipeStart.y - swipeEnd.y) > swipeDistance && (swipeStart.x - swipeEnd.x) > swipeDistance ) //Swipe down
							{
								MovePlayer("backward");
							}
						}
					}
				}
			}
			
			// If the camera moved forward enough, create another lane
			if( nextLanePosition - cameraObject.position.x < precreateLanes )
				CreateLane(1);
			
			if( cameraObject )
			{
				// Make the camera chase the player in all directions
				if( playerObjects[currentPlayer] )
					cameraObject.position = new Vector3(Mathf.Lerp(cameraObject.position.x, playerObjects[currentPlayer].position.x, Time.deltaTime * 3), 0, Mathf.Lerp(cameraObject.position.z, playerObjects[currentPlayer].position.z, Time.deltaTime * 3));
			
				// Make the death line chase the player forward only
				if( deathLineObject )
				{
					Vector3 newVector3 = deathLineObject.position;

					if( cameraObject.position.x > deathLineObject.position.x )
						newVector3.x = cameraObject.position.x;

					if( isGameOver == false )
						newVector3.x += deathLineSpeed * Time.deltaTime;
			
					deathLineObject.position = newVector3;
				}
			}
		}
	
		/// <summary>
		/// Creates a lane, sometimes reversing the paths of the moving objects in it
		/// </summary>
		/// <param name="laneCount">Lane count.</param>
		void CreateLane(int laneCount)
		{
			// Create a few lanes at the start of the game
			while( laneCount > 0 )
			{
				laneCount--;
			
				// Choose a random lane from the list
				int randomLane = Mathf.FloorToInt(Random.Range(0, lanesList.Length));
			
				// Create a random lane from the list of available lanes
				Transform newLane = Instantiate(lanesList[randomLane].laneObject, new Vector3(nextLanePosition, 0, 0), Quaternion.identity) as Transform;

				if( Random.value < lanesList[randomLane].itemChance )
				{ 
					Transform newObject = Instantiate(objectDropList[Mathf.FloorToInt(Random.Range(0, objectDropList.Length))]) as Transform;

					Vector3 newVector = new Vector3();

					newVector = newLane.position;

					newVector.z += Mathf.Round(Random.Range(-objectDropOffset, objectDropOffset));

					newObject.position = newVector;
				}
			
				// Go to the next lane position
				nextLanePosition += lanesList[randomLane].laneWidth;
			}
		}

		/// <summary>
		/// Updates the players score, considers whether the player has also leveld up
		/// </summary>
		/// <param name="changeValue">Value to add to the current score.</param>
		void ChangeScore(int changeValue)
		{
			// Change the score
			score += changeValue;
		
			// Update the score text
			if( scoreText )
				scoreText.GetComponent<Text>().text = score.ToString();
		
			// Increase the counter to the next level
			increaseCount += changeValue;
		
			// If we reached the required number of points, level up!
			if( increaseCount >= levelUpEveryCoins )
			{
				increaseCount -= levelUpEveryCoins;
			
				LevelUp();
			}
		}

		/// <summary>
		/// Levels up and increases the difficulty of the game.
		/// </summary>
		void LevelUp()
		{
			// Increase the speed of the death line ( the moving fog ), but never above the maximum allowed value
			if( deathLineSpeed + deathLineSpeedIncrease < deathLineSpeedMax )
				deathLineSpeed += deathLineSpeedIncrease;	

			// If there is a source and a sound, play it from the source
			if( soundSourceTag != string.Empty && soundLevelUp )
				GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent<AudioSource>().PlayOneShot(soundLevelUp);
		}
	
		/// <summary>
		/// Pauses the game.
		/// </summary>
		void Pause()
		{
			isPaused = true;
		
			// Set timescale to 0, preventing anything from moving
			Time.timeScale = 0;
		
			// Show the pause screen and hide the game screen
			if( pauseCanvas )
				pauseCanvas.gameObject.SetActive(true);

			if( gameCanvas )
				gameCanvas.gameObject.SetActive(false);
		}
	
		/// <summary>
		/// Unpauses the game
		/// </summary>
		void Unpause()
		{
			isPaused = false;
		
			// Set timescale back to the current game speed
			Time.timeScale = gameSpeed;
		
			// Hide the pause screen and show the game screen
			if( pauseCanvas )
				pauseCanvas.gameObject.SetActive(false);

			if( gameCanvas )
				gameCanvas.gameObject.SetActive(true);
		}
	
		/// <summary>
		/// Handles when the game is over.
		/// </summary>
		/// <returns>Yields for a period of time to allow execution to continue then continues through the game over text/gui display</returns>
		/// <param name="delay">The delay of the yield in seconds</param>
		IEnumerator GameOver(float delay)
		{
			yield return new WaitForSeconds(delay);
		
			isGameOver = true;
		
			// Remove the pause and game screens
			if( pauseCanvas )
				Destroy(pauseCanvas.gameObject);

			if( gameCanvas )
				Destroy(gameCanvas.gameObject);

			//Get the number of coins we have
			int totalCoins = PlayerPrefs.GetInt( coinsPlayerPrefs, 0);
			
			//Add to the number of coins we collected in this game
			totalCoins += score;
			
			//Record the number of coins we have
			PlayerPrefs.SetInt( coinsPlayerPrefs, totalCoins);
		
			// Show the game over screen
			if( gameOverCanvas )
			{
				// Show the game over screen
				gameOverCanvas.gameObject.SetActive(true);
			
				// Write the score text
				gameOverCanvas.Find("TextScore").GetComponent<Text>().text = "SCORE " + score.ToString();
			
				// Check if we got a high score
				if( score > highScore )
				{
					highScore = score;
				
					// Register the new high score
					PlayerPrefs.SetInt(Application.loadedLevelName + "_HighScore", score);
				}
			
				// Write the high sscore text
				gameOverCanvas.Find("TextHighScore").GetComponent<Text>().text = "HIGH SCORE " + highScore.ToString();
			}
		}
	
		/// <summary>
		/// Reloads the current loaded level.
		/// </summary>
		void Restart()
		{
			Application.LoadLevel(Application.loadedLevelName);
		}
	
		/// <summary>
		/// Loads and returns the user/player to the main menu.
		/// </summary>
		void MainMenu()
		{
			Application.LoadLevel(mainMenuLevelName);
		}

		/// <summary>
		/// Activates the selected player, while deactivating all the others
		/// </summary>
		/// <param name="playerNumber">The number of the player to be activated</param>
		void SetPlayer( int playerNumber )
		{
			//Go through all the players, and hide each one except the current player
			for(index = 0; index < playerObjects.Length; index++)
			{
				if ( index != playerNumber )    
					playerObjects[index].gameObject.SetActive(false);
				else    
					playerObjects[index].gameObject.SetActive(true);
			}
		}

		/// <summary>
		/// Send a move command to the current player
		/// </summary>
		/// <param name="moveDirection">The direction the player should move in</param>
		void MovePlayer( string moveDirection )
		{
			//If there is a current player, send a move message with a direction
			if ( playerObjects[currentPlayer] )    playerObjects[currentPlayer].SendMessage("Move", moveDirection);
		}
	}
}

