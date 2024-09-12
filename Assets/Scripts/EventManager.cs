using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    [SerializeField] private float spawningDelay;
    [SerializeField] private List<SpawnedEvent> spawnEvents;

    [SerializeField] private SpriteRenderer zoneRecord;
    [SerializeField] private BoxCollider2D zoneRecordCollider;

    private int eventIndex;
    private float spawningTimer;
    private State spawnState;

    private enum State
    {
        SPAWN,
        EVENT
    }

    private void Update()
    {
        spawningTimer += Time.deltaTime;

        if (spawnState == State.SPAWN && spawningTimer >= spawningDelay)
        {
            spawnState = State.EVENT;
            spawningTimer = 0;
            spawnEvents[eventIndex].StartEvent(zoneRecordCollider);
        }

        else if (spawnState == State.EVENT)
        {
            spawnEvents[eventIndex].UpdateEvent(zoneRecordCollider);

            if (spawnEvents[eventIndex].HasFinished)
            {
                spawnEvents[eventIndex].HasFinished = false;
                spawnState = State.SPAWN;
                eventIndex++;

                if (eventIndex >= spawnEvents.Count)
                {
                    //end game
                }
            }
        }
    }
}
