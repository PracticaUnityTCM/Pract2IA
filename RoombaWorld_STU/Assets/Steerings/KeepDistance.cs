using UnityEngine;
using System.Collections;

namespace Steerings
{
	// SEEK-based KEEP DISTANCE

	public class KeepDistance : SteeringBehaviour
	{

		private static GameObject surrogateTarget = null;

		public float requiredDistance = 40;

		public GameObject target;


		public override  SteeringOutput GetSteering () {

			// no KS? get it
			if (this.ownKS==null) this.ownKS = GetComponent<KinematicState>();

			return KeepDistance.GetSteering (this.ownKS, this.target, this.requiredDistance);
		} 

		public static SteeringOutput GetSteering (KinematicState ownKS, GameObject target, float requiredDistance = 40) {

			Vector3 directionFromTarget;
			Vector3 desiredPosition;

			directionFromTarget = (ownKS.position - target.transform.position).normalized;
			desiredPosition = target.transform.position + directionFromTarget * requiredDistance;  

			/*
			 desiredPosition is a point at requiredDistance units of distance from the target, on the line from target to me
			 */


			if (KeepDistance.surrogateTarget == null) {
				surrogateTarget = new GameObject ("Surrogate for Keep distance");
				surrogateTarget.SetActive (false);
			}
			KeepDistance.surrogateTarget.transform.position = desiredPosition;

			return Seek.GetSteering (ownKS, KeepDistance.surrogateTarget);
		}


	}
}