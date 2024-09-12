using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnedEvent : MonoBehaviour
{
    [SerializeField] protected float zoneSize;
    [SerializeField] protected float eventDuration;
    protected float eventTimer;

    public bool HasFinished { get; set; }

    public virtual void StartEvent(BoxCollider2D zoneRecordCollider)
    {
        gameObject.SetActive(true);
        zoneRecordCollider.gameObject.SetActive(true);
        zoneRecordCollider.transform.localScale = new Vector2(zoneSize, zoneRecordCollider.transform.localScale.y);
        zoneRecordCollider.size = new Vector2(zoneSize, zoneRecordCollider.transform.localScale.y);
    }
    public abstract void UpdateEvent(BoxCollider2D zoneRecordCollider);
    public virtual void ExitEvent(BoxCollider2D zoneRecordCollider)
    {
        eventTimer = 0;
        zoneRecordCollider.gameObject.SetActive(false);
        HasFinished = true;
    }
}
