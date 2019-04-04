using System;
using UnityEngine;
using System.Collections;

[Serializable]
public struct CollisionInfo {
	public bool above, below;
	public bool left, right;

	public bool climbingSlope;
	public bool descendingSlope;
	public bool slidingDownMaxSlope;

	public float slopeAngle, slopeAngleOld;
	public Vector2 slopeNormal;
	public Vector2 moveAmountOld;
	public int faceDir;
	public bool fallingThroughPlatform;



	public Collider2D curGroundCollider; 
		
	public void Reset() {
		above = below = false;
		left = right = false;
		climbingSlope = false;
		descendingSlope = false;
		slidingDownMaxSlope = false;
		slopeNormal = Vector2.zero;

		slopeAngleOld = slopeAngle;
		slopeAngle = 0;
	}
}

public class Controller2D : RaycastController {

	float maxSlopeAngle = 80;
	public float maxSlopeAngleWalking = 45f;
	public float maxSlopeAngleClimbing = 90f;

	public bool isClimbing;
	public bool isWalking;
	public bool isFalling;
	public bool isRising;

	public CollisionInfo collisions;
	[HideInInspector]
	public Vector2 playerInput;

	public override void Start() {
		base.Start ();
		collisions.faceDir = 1;
	}

	public void Move(Vector2 moveAmount, bool standingOnPlatform) {
		Move (moveAmount, Vector2.zero, standingOnPlatform);
	}

	public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false) {
		UpdateRaycastOrigins ();

		collisions.Reset ();
		collisions.moveAmountOld = moveAmount;
		playerInput = input;

		if (isClimbing)
		{
			maxSlopeAngle = maxSlopeAngleClimbing;
		}
		else
		{
			maxSlopeAngle = maxSlopeAngleWalking;
		}

		if (moveAmount.y < 0) {
			DescendSlope(ref moveAmount);
		}

		if (moveAmount.x != 0) {
			collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
		}

		HorizontalCollisions (ref moveAmount);
		VerticalCollisions (ref moveAmount);

		transform.Translate (moveAmount);
		
		
		if (standingOnPlatform) {
			collisions.below = true;
		}
		
		if (input.x != 0 && collisions.below)
		{
			isWalking = true;
		}
		else
		{
			isWalking = false;
		}

		if (!collisions.below)
		{
			if (moveAmount.y > 0)
			{
				isRising = true;
				isFalling = false;
			}
			else if(moveAmount.y < 0)
			{
				isFalling = true;
				isRising = false;
			}
			else
			{
				isFalling = false;
				isRising = false;
			}
		}
		else
		{
			isFalling = false;
			isRising = false;
		}
	}

	#region Collision Handling
	void HorizontalCollisions(ref Vector2 moveAmount) {
		float directionX = collisions.faceDir;
		float rayLength = Mathf.Abs (moveAmount.x) + skinWidth;

		// for wall climbing since the x movement may be zero at that situation
		if (Mathf.Abs(moveAmount.x) < skinWidth || collisions.slopeAngleOld != 0) {
			// this multiplier was increased to account for slow movement on very slight slopes
			rayLength = 10f * skinWidth;
		}
		
		for (int i = 0; i < horizontalRayCount; i ++) {
			Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.right * directionX,Color.red);

			foreach (RaycastHit2D hit in hits)
			{
				if (hit.distance == 0) {
					continue;
				}

				if (hit.collider.CompareTag("Through") && playerInput.y == -1f)
				{
					// ignore horizontal collisions on Through platforms if we're trying to fall through them
					continue;
				}
				
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

				if (i == 0 && slopeAngle <= maxSlopeAngle) {
					// if it happens to be descending and climbing at the same time, there is a lag 
					// since descending function will decrease x movement but player will not descend (because it's climbing)
					// that's why we restore its previous movement for climbing to work (EP 5 12:39)
					if (collisions.descendingSlope && moveAmount.x != 0f) {
						collisions.descendingSlope = false;
						moveAmount = collisions.moveAmountOld;
					}
					float distanceToSlopeStart = 0;
					bool longEnough = false;
					if (slopeAngle != collisions.slopeAngleOld) {
						distanceToSlopeStart = hit.distance-skinWidth;
						if (moveAmount.x > skinWidth)
						{
							longEnough = true;
							moveAmount.x -= distanceToSlopeStart * directionX;
						}
					}
					ClimbSlope(ref moveAmount, slopeAngle, hit.normal);
					if (longEnough)
					{
						moveAmount.x += distanceToSlopeStart * directionX;
					}

					if (collisions.climbingSlope)
					{
						collisions.curGroundCollider = hit.collider;
						collisions.fallingThroughPlatform = false;
					}
				}

				// ignore a horizontal "Through" collider if...
				//		we're shooting out of our feet (i == 0) and the slope is angled down (hit.normal.y < 0f)
				//		OR
				//		we're not shooting out of our feet (i != 0) and we're hitting a collider that we aren't standing on (hit.collider != collisions.curGroundCollider)
				bool ignoreHorizontalCollider = hit.collider.CompareTag("Through") &&
				                                ((i == 0 && hit.normal.y < 0f) ||
				                                 (i != 0 && hit.collider != collisions.curGroundCollider));

				if ((!collisions.climbingSlope || slopeAngle > maxSlopeAngle) && !ignoreHorizontalCollider) {
					if(moveAmount.x != 0)
						moveAmount.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance;

					// when you are climbing a slope and run into a wall, you need this chunk to adjust y movement to avoid jagging
					if (collisions.climbingSlope) {
						moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
					}

					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
				}
			}
		}
	}

	void VerticalCollisions(ref Vector2 moveAmount) {
		float directionY = Mathf.Sign (moveAmount.y);
		float rayLength = Mathf.Abs (moveAmount.y) + skinWidth;
		float originalRayLength = rayLength;

		Vector2 lastNormal = Vector2.down;
		for (int i = 0; i < verticalRayCount; i ++) {

			Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
			RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.up * directionY,Color.red);

			foreach (RaycastHit2D hit in hits)
			{
				if (hit.collider.CompareTag("Through")) {
					
					// always ignore if player is holding down
					if (playerInput.y == -1) {
						
						collisions.fallingThroughPlatform = true;
						continue;
					}

					// ignore if we're moving up or if we're inside the collider (which would make hit.distance 0)
					if ((directionY == 1 || hit.distance == 0))
					{
						continue;
					}
					
					// adding a amount of time for controller to accelerate in case the platform moves faster than it
					if (collisions.fallingThroughPlatform && hit.collider == collisions.curGroundCollider) {
						continue;
					}
					
					// if we hit a slope that's angled left, we should ignore every ray except the last one
					// otherwise, we should ignore every ray that isn't the first one
					// and we should only do this if we're hitting a slope that we're not standing on
					//		(this is so we don't accidentally ignore a curved slope that we're trying to climb/descend
					if (((hit.normal.x < 0f && i != verticalRayCount - 1) || (hit.normal.x > 0f && i != 0)) 
					    && hit.collider != collisions.curGroundCollider)
					{
						// we figured out that we should maybe ignore this ray, but now we need to handle a special case:
						// if we are landing on a surface with normals that have different X directions, we will fall through it even though it is a solid surface
						// so we should only fall through a surface if the normal resulting from the previous hit is in the same direction as the new hit
						// this means that we will still ignore slopes correctly if needed, but we will never ignore a hill/bump/hump
						
						
						if (lastNormal == Vector2.down) // check to see if lastNormal has not been updated yet
						{
							lastNormal = hit.normal;
							// this is the first ray that successfully hit the slope
							// it might be ray 0, but maybe not
							// if the slope is facing left, we should ignore this ray (because we only consider the last ray)
							// otherwise, we should ignore anything that isn't the first ray (because we only care about the first ray if the slope is facing right)
							if(hit.normal.x < 0f)
								continue;
							else if(i != 0)
								continue;
						}
						else
						{
							// if the normal of the last hit is in the same direction of our current normal, then we can go ahead and ignore this ray as planned
							if (Mathf.Sign(hit.normal.x) == Mathf.Sign(lastNormal.x))
							{
								lastNormal = hit.normal;
								continue;
							}
						}
					}

				}
				
				moveAmount.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				if (collisions.climbingSlope) {
					moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * collisions.faceDir;
				}

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;

				collisions.curGroundCollider = hit.collider;

				if (playerInput.y != -1)
				{
					collisions.fallingThroughPlatform = false;
				}
				

			}
			
		}

		if (collisions.climbingSlope) {
			float directionX = collisions.faceDir;
			rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
			RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin,Vector2.right * directionX,rayLength,collisionMask);

			foreach (RaycastHit2D hit in hits)
			{
				if (hit.distance != 0f) // ignore rays that are cast inside a collider
				{
					float slopeAngle = Vector2.Angle(hit.normal,Vector2.up);
					// avoid lagging when passing by the intersection of two slopes
					if (slopeAngle != collisions.slopeAngle) {
						moveAmount.x = (hit.distance - skinWidth) * directionX;
						collisions.slopeAngle = slopeAngle;
						collisions.slopeNormal = hit.normal;
					}
					

				}
			}			
			// you can't fall off the platform when you are facing up slope (no downward ray casting)
			for (int i = 0; i < verticalRayCount; i++)
			{
				rayOrigin = raycastOrigins.bottomLeft + Vector2.right * (verticalRaySpacing * i + moveAmount.x);
				hits = Physics2D.RaycastAll(rayOrigin, -Vector2.up, originalRayLength * 5f, collisionMask);

				foreach (RaycastHit2D hit in hits)
				{
					if (hit.collider.tag == "Through" && hit.distance != 0 && hit.collider == collisions.curGroundCollider)
					{
						Debug.DrawRay(rayOrigin, -Vector2.up, Color.red);
						if (collisions.fallingThroughPlatform)
						{
							moveAmount.y = collisions.moveAmountOld.y;
						}
						if (playerInput.y == -1)
						{
							collisions.fallingThroughPlatform = true;
							moveAmount.y = collisions.moveAmountOld.y;
						}
					}
				}
			}
		}
	}
	#endregion

	#region Slope Handling

	void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal) {
		float moveDistance = Mathf.Abs (moveAmount.x);
		float climbmoveAmountY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (moveAmount.y <= climbmoveAmountY) {
			moveAmount.y = climbmoveAmountY;
			moveAmount.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (moveAmount.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
			collisions.slopeNormal = slopeNormal;
		}
	}

	void DescendSlope(ref Vector2 moveAmount) {

		// TODO: make it so DescendSlope only moves downward when climbing steep walls (stick to walls in climbing mode)
		
		RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast (raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs (moveAmount.y) + skinWidth, collisionMask);
		RaycastHit2D maxSlopeHitRight = Physics2D.Raycast (raycastOrigins.bottomRight, Vector2.down, Mathf.Abs (moveAmount.y) + skinWidth, collisionMask);
		
		// XOR, to avoid player slides when most of its body is still on flat ground
		if (maxSlopeHitLeft ^ maxSlopeHitRight) {
			SlideDownMaxSlope (maxSlopeHitLeft, ref moveAmount);
			SlideDownMaxSlope (maxSlopeHitRight, ref moveAmount);
		}

		if (!collisions.slidingDownMaxSlope) {
			// directionX should correspond to whatever direction we're facing, not just our x movement amount
			float directionX = collisions.faceDir;
			
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
			RaycastHit2D[] hits = Physics2D.RaycastAll (rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

			foreach (RaycastHit2D hit in hits)
			{
				if(hit.collider.CompareTag("Through") && playerInput.y == -1)
					continue;
				
				if (hit.distance > 0f) // ignore rays that are cast inside a collider
				{
					float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
					if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle) {
						if (Mathf.Sign (hit.normal.x) == directionX) {
							if (hit.distance - skinWidth <= Mathf.Tan (slopeAngle * Mathf.Deg2Rad) * Mathf.Max(Mathf.Abs (moveAmount.x), skinWidth)) {
								float moveDistance = Mathf.Abs (moveAmount.x);
								float descendmoveAmountY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
								moveAmount.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (moveAmount.x);
								moveAmount.y -= descendmoveAmountY;

								collisions.slopeAngle = slopeAngle;
								collisions.descendingSlope = true;
								collisions.below = true;
								collisions.slopeNormal = hit.normal;
							}
						}
					}
				}
			}
		}
	}

	void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount) {

		if (hit) {
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if (slopeAngle > maxSlopeAngle) {
				moveAmount.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs (moveAmount.y) - hit.distance) / Mathf.Tan (slopeAngle * Mathf.Deg2Rad);

				collisions.slopeAngle = slopeAngle;
				collisions.slidingDownMaxSlope = true;
				collisions.slopeNormal = hit.normal;
			}
		}
	}
	#endregion



}
