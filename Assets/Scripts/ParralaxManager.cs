using System.Collections.Generic;
using UnityEngine;

public class ParralaxManager : MonoBehaviour
{
    [SerializeField] List<Sprite> levelBlocks;
    [SerializeField] private Transform blocksParent;

    [SerializeField] private float scrollingSpeed;

    [SerializeField] private SpriteRenderer block1;
    [SerializeField] private SpriteRenderer block2;

    private float scrollingDistance;

    void Update()
    {
        blocksParent.transform.position += Vector3.left * scrollingSpeed * Time.deltaTime;
        scrollingDistance += scrollingSpeed * Time.deltaTime;

        if (scrollingDistance >= block1.bounds.size.x)
        {
            blocksParent.transform.position += Vector3.right * scrollingDistance;
            scrollingDistance = 0;

            block1.sprite = block2.sprite;
            block2.sprite = levelBlocks[Random.Range(0, levelBlocks.Count)];
        }
    }
}
