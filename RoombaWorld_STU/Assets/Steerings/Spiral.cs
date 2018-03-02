using UnityEngine;

namespace Steerings
{

	public class Spiral : SteeringBehaviour
	{

		private static GameObject surrogateTarget;

		public override SteeringOutput GetSteering () {
			// no KS? get it
			if (this.ownKS==null) this.ownKS = GetComponent<KinematicState>();


			return Spiral.GetSteering (this.ownKS);
		}

		public static SteeringOutput GetSteering (KinematicState ownKS) {

			SteeringOutput result = new SteeringOutput ();
			result.angularAcceleration = ownKS.maxAngularAcceleration;
			result.angularActive = true;
			result.linearAcceleration = GoWhereYouLook.GetSteering (ownKS).linearAcceleration;

			return result;

		}
	}
}

