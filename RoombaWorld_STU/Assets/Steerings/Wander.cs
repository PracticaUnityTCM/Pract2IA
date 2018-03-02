/* **************** VERSION 2 ****************** */
using UnityEngine;

namespace Steerings
{
	public class Wander : SteeringBehaviour
	{

		public RotationalPolicy rotationalPolicy = RotationalPolicy.LWYGI;

		public float wanderRate = 30f;
		public float wanderRadius = 10f;
		public float wanderOffset = 20f;

		protected float targetOrientation = 0f;

		public override SteeringOutput GetSteering ()
		{ 
			// no KS? get it
			if (this.ownKS==null) this.ownKS = GetComponent<KinematicState>();

			SteeringOutput result = Wander.GetSteering (ownKS, ref targetOrientation, wanderRate, wanderRadius, wanderOffset);
			base.applyRotationalPolicy (rotationalPolicy, result, SURROGATE_TARGET);
			return result;
		}

		public static SteeringOutput GetSteering (KinematicState ownKS, 
			ref float targetOrientation, float wanderRate = 30f, 
			float wanderRadius = 10f, float wanderOffset = 20f) {


			// change target orientation (change location of surrogate target on unit circle)
			targetOrientation += wanderRate * Utils.binomial ();

			// place surrogate target on circle of wanderRadius
			SURROGATE_TARGET.transform.position = Utils.OrientationToVector(targetOrientation)*wanderRadius;

			// place circle  "in front"
			SURROGATE_TARGET.transform.position += ownKS.position + Utils.OrientationToVector(ownKS.orientation)*wanderOffset;

			// Seek the surrogate target
			return Seek.GetSteering(ownKS, SURROGATE_TARGET);
		}
	}
}
