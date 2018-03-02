using UnityEngine;


namespace Steerings
{

	public class LiderFollowing : SteeringBehaviour
	{

		public RotationalPolicy rotationalPolicy = RotationalPolicy.LWYGI;

		public GameObject leader;
		public float behindDistance;
		public float aheadDistance;
		public float sightRadius;
		public string idTag = "FOLLOWER";
		public float repulsionsThreshold;


		public override SteeringOutput GetSteering () {
			// no KS? get it
			if (this.ownKS==null) this.ownKS = GetComponent<KinematicState>();

			SteeringOutput result = LiderFollowing.GetSteering (ownKS, leader, behindDistance, aheadDistance, sightRadius, idTag, repulsionsThreshold);
			base.applyRotationalPolicy(rotationalPolicy, result, this.leader);
			return result;

		}

		public static SteeringOutput GetSteering (KinematicState ownKS, GameObject leader, float behindDistance, float aheadDistance, float sightRadius, string tag, float repulsionThreshold) {

			SteeringOutput arrive, repulsion, result;

			result = new SteeringOutput ();

			// we need the kinematic state of the leader
			KinematicState leaderKS = leader.GetComponent<KinematicState>();
			if (leaderKS == null) {
				Debug.Log ("Leader has no kinematic state attached.");
				result.linearActive = false;
				return result;
			}

			Vector3 behindPoint = leaderKS.position - leaderKS.linearVelocity.normalized * behindDistance;
			Vector3 aheadPoint  = leaderKS.position + leaderKS.linearVelocity.normalized * aheadDistance;

			SURROGATE_TARGET.transform.position = behindPoint;
			arrive = Arrive.GetSteering (ownKS, leader, 1); // using default parameters

			repulsion = LinearRepulsion.GetSteering (ownKS, tag, repulsionThreshold);


			result.linearActive = arrive.linearActive || repulsion.linearActive;

			if (arrive.linearActive)
				result.linearAcceleration += arrive.linearAcceleration;

			if (repulsion.linearActive)
				result.linearAcceleration += repulsion.linearAcceleration;


			// get out of the way of the leader
			if ((ownKS.position - aheadPoint).magnitude < sightRadius ) {
				SteeringOutput evade = Evade.GetSteering (ownKS, leader); // default maxprediction time
				result.linearAcceleration += evade.linearAcceleration;
				result.linearActive = true;
			}


			// if required acceleration is too high, clip it
			if (result.linearAcceleration.magnitude > ownKS.maxAcceleration) {
				result.linearAcceleration = result.linearAcceleration.normalized * ownKS.maxAcceleration;
			}

			return result;
		}
	}
}
