using UnityEngine;

namespace RoadCrossing
{
	/// <summary>
	/// Defines a lane, which may have several moving objects in it. A lane can also have an item.
	/// </summary>
	public class RCGLane : MonoBehaviour
	{
		// Cache of this lanes instance Transform object.
		internal Transform thisTransform;
	
		// The start and end points of the lane
		public Vector3 laneStart = new Vector3(0, 0, -7);
		public Vector3 laneEnd = new Vector3(0, 0, 7);
	
		// Possible objects that will be used in this lane
		public Transform[] movingObjects;
	
		// An array of the objects that will be moving in this lane
		internal Transform[] movingObjectsList;
	
		// The minimum/maximum number of moving objects in a lane
		public Vector2 movingObjectsNumber = new Vector2(1, 3);

		// The minimum gap between objects. This is used to prevent objects from potentially overlapping eachother.
		public float minimumObjectGap = 1;

		// The movement speed of all the objects in a lane
		public Vector2 moveSpeed = new Vector2(0.5f, 1);
	
		// The movement direction of the objects in the lane
		internal int moveDirection = 1;

		//Should the starting position of the objects be random? If true, the object has a 50% of starting from the laneEnd and going to the laneStart
		public bool randomDirection = true;
	
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
		
			// Randomize the initial movement direction of the objects
			if( randomDirection == true && Random.value > 0.5f )
				moveDirection = -1;

			// Randomize the movement speed of the objects in the lane
			moveSpeed.x = Random.Range(moveSpeed.x, moveSpeed.y);
		
			// Add random objects to the moving object list
			// Calculate the length of the path of the moving objects in the lane
			float laneLength = Vector3.Distance(laneStart, laneEnd);
		
			// Set the number of moving objects in the lane randomly
			int currentMovingObjectsNumber = Mathf.FloorToInt(Random.Range(movingObjectsNumber.x, movingObjectsNumber.y));

			// Create a list that will contain all the moving objects in the lane
			movingObjectsList = new Transform[currentMovingObjectsNumber];

			// Create the moving objects and place them in the list
			for(int index = 0; index < currentMovingObjectsNumber; index++)
			{
				// Create a random moving object
				Transform newMovingObject = Instantiate(movingObjects[Mathf.FloorToInt(Random.Range(0, movingObjects.Length))]) as Transform;
			
				// Place the object at the start point, and look at the end of the lane
				newMovingObject.position = thisTransform.position + laneStart;
				newMovingObject.LookAt(thisTransform.position + laneEnd);

				// Spread the objects evenly along the lane
				newMovingObject.Translate(Vector3.forward * index * laneLength / currentMovingObjectsNumber, Space.Self);

				// Move each object randomly along the lane to make it more varied
				newMovingObject.Translate(Vector3.forward * Random.Range(minimumObjectGap, laneLength / movingObjectsNumber.x - minimumObjectGap), Space.Self);

				// Add the moving object to the list
				movingObjectsList[index] = newMovingObject;
			}

			// Go through all the moving objects and move them!
			foreach( Transform movingObject in movingObjectsList )
			{
				// Make the object look at the direction it is moving in
				if( moveDirection == 1 )
					movingObject.LookAt(thisTransform.position + laneEnd);
				else
					movingObject.LookAt(thisTransform.position + laneStart);
			
				// If there is an animation, play it
				if( movingObject.GetComponent<Animation>() )
				{
					// Set the animation speed base on the movement speed
					movingObject.GetComponent<Animation>()[movingObject.GetComponent<Animation>().clip.name].speed = moveSpeed.x;
				}
			}
		}
	
		/// <summary>
		/// Update is called every frame, if the MonoBehaviour is enabled.
		/// </summary>
		void Update()
		{
			// Go through all the moving objects in a lane...
			foreach( Transform movingObject in movingObjectsList )
			{
				// Check if the object moves normally, or in reverse
				if( movingObject )
				{
					if( moveDirection == 1 )
					{
						// Move the object towards the lane end point
						movingObject.position = Vector3.MoveTowards(movingObject.position, thisTransform.position + laneEnd, moveSpeed.x * Time.deltaTime);
					
						// If the object reaches the lane end point, reset it to the lane start point
						if( movingObject.position == thisTransform.position + laneEnd )
						{
							movingObject.position = thisTransform.position + laneStart;
							movingObject.LookAt(thisTransform.position + laneEnd);
						}
					}
					else
					{
						// Move the object towards the lane start point
						movingObject.position = Vector3.MoveTowards(movingObject.position, thisTransform.position + laneStart, moveSpeed.x * Time.deltaTime);
					
						// If the object reaches the lane start point, reset it to the lane end point
						if( movingObject.position == thisTransform.position + laneStart )
						{
							movingObject.position = thisTransform.position + laneEnd;
							movingObject.LookAt(thisTransform.position + laneStart);
						}
					}
				}
			}
		}
	
		/// <summary>
		/// Draws the start and end points of the moving objects in a lane.
		/// </summary>
		void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(transform.position + laneStart, 0.2f);
		
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(transform.position + laneEnd, 0.2f);
		}
	}
}