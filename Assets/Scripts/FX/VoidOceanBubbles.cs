using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidOceanBubbles : MonoBehaviour
{
    [SerializeField] ParticleSystem particles;
    [SerializeField] float emissionStartDistance;
    [SerializeField] float baseParticleEmissions;
    [SerializeField] string targetRealm;
    [SerializeField] float baseYPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = PlayerController.currentPlayer ? PlayerController.currentPlayer.transform.position : Vector3.zero;
        if (RealmTeleportManager.Instance.GetCurrentRealmName() == targetRealm)
        {
            transform.position = new Vector3(playerPos.x, baseYPos, playerPos.z);
            ParticleSystem.EmissionModule emission = particles.emission;
            emission.rateOverTime = Mathf.Clamp(emissionStartDistance - (playerPos.y - transform.position.y), 0, 10) * baseParticleEmissions;
        } else
        {
            transform.position = new Vector3(0, baseYPos, 0);
        }
    }
}
