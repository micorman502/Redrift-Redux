using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bucket : MonoBehaviour
{

    [SerializeField] bool waterBucket;

    [SerializeField] ItemInfo bucketItem;
    [SerializeField] ItemInfo waterBucketItem;

    [SerializeField] Transform waterLevel;

    WorldManager.WorldType worldType;

    [SerializeField] float fillMLs;
    [SerializeField] float capacityMLs = 1000f;
    float rainFillMLs = 50f;

    float nextTimeToWorldCheck;
    float worldCheckInterval = 1f;

    void Start ()
    {
        if (waterBucket)
        {
            fillMLs = capacityMLs;
        }

        ChangeFill(0);
    }

    void Update ()
    {
        if (worldType == WorldManager.WorldType.Dark || WeatherManager.CurrentIntensity >= 0.45f)
        {
            ChangeFill(Time.deltaTime * Mathf.Max(0f, transform.up.y) * rainFillMLs);
        }

        if (Vector3.Dot(transform.up, Vector3.down) < 0)
        {
            Spill();
        }

        if (Time.time >= nextTimeToWorldCheck)
        {
            CheckWorldType();
            nextTimeToWorldCheck = Time.time + worldCheckInterval;
        }
    }

    void CheckWorldType ()
    {
        if (transform.position.y > VoidOcean.startThreshold)
        {
            worldType = WorldManager.WorldType.Light;
        }
        else
        {
            worldType = WorldManager.WorldType.Dark;
        }
    }

    void ChangeFill (float fillChange)
    {
        if (waterBucket)
            return;

        fillMLs += fillChange;

        if (fillMLs < 0)
        {
            fillMLs = 0;
        }

        if (fillMLs >= capacityMLs)
        {
            fillMLs = capacityMLs;

            if (!waterBucket)
            {
                ReachCap();
            }
        }

        waterLevel.transform.localPosition = Vector3.Lerp(Vector3.up * -0.49f, Vector3.zero, fillMLs / capacityMLs);
        waterLevel.transform.localScale = Vector3.Lerp(Vector3.one * 0.82f, Vector3.one, fillMLs / capacityMLs);
    }

    void Spill ()
    {
        if (!waterBucket)
            return;

        Instantiate(bucketItem.droppedPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    void ReachCap ()
    {
        if (waterBucket)
            return;

        Instantiate(waterBucketItem.droppedPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
