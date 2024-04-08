using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidOcean : MonoBehaviour
{
    public const float startThreshold = -100f;
    [SerializeField] float dragMin;
    [SerializeField] float damageMin;
    [SerializeField] float yEnd;
    [SerializeField] float dragMax;
    [SerializeField] float damageMax;
    [SerializeField] float upForce;
    float dragDiff;

    void Awake ()
    {
        dragDiff = Mathf.Abs(yEnd - startThreshold);
    }

    void OnTriggerStay (Collider other)
    {
        if (!other.attachedRigidbody)
            return;
        if (other.isTrigger)
            return;

        ApplyDrag(other.attachedRigidbody, CalculateDrag(other.transform.position));
        ApplyUpForce(other.attachedRigidbody);

        if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
            return;

        other.gameObject.GetComponent<IDamageable>().RemoveHealth(CalculateDamage(other.transform.position) * Time.fixedDeltaTime);
    }

    void ApplyUpForce (Rigidbody rigidbody)
    {
        rigidbody.AddForce(Vector3.up * upForce, ForceMode.Acceleration);
    }

    float CalculateDamage (Vector3 bodyPos)
    {
        float distance = Mathf.Abs(bodyPos.y - startThreshold);
        return Mathf.Lerp(damageMin, damageMax, distance / dragDiff);
    }

    float CalculateDrag (Vector3 bodyPos)
    {
        float distance = Mathf.Abs(bodyPos.y - startThreshold);
        return Mathf.Lerp(dragMin, dragMax, distance / dragDiff);
    }

    void ApplyDrag (Rigidbody rigidbody, float drag)
    {
        float dragEffect = 1 + drag * Time.fixedDeltaTime;
        rigidbody.velocity /= dragEffect;
    }
}
