using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StableEvent : SpawnedEvent
{
    [SerializeField] protected float spawnOffset = 5;

    public override void StartEvent(BoxCollider2D zoneRecordCollider)
    {
        base.StartEvent(zoneRecordCollider);
        transform.position = new Vector3(spawnOffset, transform.position.y, transform.position.z);
        zoneRecordCollider.transform.position = new Vector3(transform.position.x, zoneRecordCollider.transform.position.y, zoneRecordCollider.transform.position.z);
    }

    public override void UpdateEvent(BoxCollider2D zoneRecordCollider)
    {
        eventTimer += Time.deltaTime;

        if (eventTimer >= eventDuration)
        {
            ExitEvent(zoneRecordCollider);
        }
    }

    public override void ExitEvent(BoxCollider2D zoneRecordCollider)
    {
        gameObject.SetActive(false);
        base.ExitEvent(zoneRecordCollider);
    }
}
