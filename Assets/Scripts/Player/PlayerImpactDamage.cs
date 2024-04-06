using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerImpactDamage : MonoBehaviour
{
	[SerializeField] PlayerVitals vitals;
	public float lowerVelocityThreshold = 1f;
	public float upperVelocityThreshold = 5f;
	public float minImpactDamage = 10f;
	public float maxImpactDamage = 25f;
	float lastImpact;

	void OnCollisionEnter (Collision col)
	{
		Vector3 hitDirection = (col.GetContact(0).point - transform.position).normalized;
		float velocityCoeff = Vector3.Dot(hitDirection, col.relativeVelocity.normalized); // Use the dot product to scale the effective velocity by how similar the velocity and the direction to the collision point is.
		float effectiveVelocity = col.relativeVelocity.magnitude * velocityCoeff;

		if (effectiveVelocity < lowerVelocityThreshold)
			return;
		if (Time.time < lastImpact + 0.3f)
			return;

		float lerpCoeff = Mathf.Clamp01((effectiveVelocity - lowerVelocityThreshold) / (upperVelocityThreshold - lowerVelocityThreshold));
		float damage = Mathf.Lerp(minImpactDamage, maxImpactDamage, lerpCoeff);

		vitals.RemoveHealth(damage);
		lastImpact = Time.time;

	}
}
