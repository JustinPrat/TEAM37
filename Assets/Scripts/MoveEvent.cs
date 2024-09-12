using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEvent : SpawnedEvent
{
    [SerializeField] protected float spawnOffset = 15;
    private Vector3 basePos;

    public override void StartEvent(BoxCollider2D zoneRecordCollider)
    {
        base.StartEvent(zoneRecordCollider);
        transform.position = new Vector3(spawnOffset, transform.position.y, transform.position.z);
        zoneRecordCollider.transform.SetParent(transform);
        zoneRecordCollider.transform.localPosition = new Vector3(0, zoneRecordCollider.transform.localPosition.y, zoneRecordCollider.transform.localPosition.z);
        basePos = transform.position;
    }

    public override void UpdateEvent(BoxCollider2D zoneRecordCollider)
    {
        eventTimer += Time.deltaTime;
        transform.position = Vector3.Lerp(basePos, (-spawnOffset * 2 * Vector3.right) + basePos, eventTimer/eventDuration);

        if (eventTimer >= eventDuration)
        {
            ExitEvent(zoneRecordCollider);
        }
    }

    public override void ExitEvent(BoxCollider2D zoneRecordCollider)
    {
        zoneRecordCollider.transform.parent = null;
        gameObject.SetActive(false);
        base.ExitEvent(zoneRecordCollider);
    }
}
