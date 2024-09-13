using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    [SerializeField] Vector2 delayBetweenObstaclesRange;
    [SerializeField] GameObject obstaclePrefab;
    [SerializeField] Transform scrollingParent;
    [SerializeField] SpriteRenderer spawnZone;
    [SerializeField] ParralaxManager parralaxManager;
    private float timer;

    private void Update()
    {
        if (timer > Random.Range(delayBetweenObstaclesRange.x, delayBetweenObstaclesRange.y))
        {
            timer = 0;
            SpawnObstacle();
        }

        timer += Time.deltaTime;
    }

    private void SpawnObstacle ()
    {
        GameObject obstacle = Instantiate(obstaclePrefab, scrollingParent);
        obstacle.transform.position = (spawnZone.bounds.center - Vector3.up * spawnZone.bounds.extents.y) + (Vector3.up * Random.Range(0, spawnZone.bounds.size.y));
        parralaxManager.AddNotRewind(obstacle);
    }
}
