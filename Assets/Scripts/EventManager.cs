using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    [SerializeField] private float spawningDelay;
    [SerializeField] private List<SpawnedEvent> spawnEvents;

    [SerializeField] private SpriteRenderer zoneRecord;
    [SerializeField] private BoxCollider2D zoneRecordCollider;

    [SerializeField] private ParticleSystem particleSystemWin1;
    [SerializeField] private ParticleSystem particleSystemWin2;

    [SerializeField] private PlayerManager _playerManager;

    [SerializeField] private ParticleSystem particleSystemWin1WINWIN;
    [SerializeField] private ParticleSystem particleSystemWin2WINWIN;

    [SerializeField] private Image _finishScore;
    [SerializeField] private TextMeshProUGUI _finishScoreText;

    [SerializeField] private Image _fadePart;

    [SerializeField] private Sprite _spriteRED;
    [SerializeField] private Sprite _spriteGREEN;

    private int eventIndex;
    private float spawningTimer;
    private State spawnState;

    private float player1Score;
    private float player2Score;

    private enum State
    {
        SPAWN,
        EVENT
    }

    public void ReportScore (float score, bool isPlayer1)
    {
        if (isPlayer1)
        {
            spawnEvents[eventIndex].Player1Score += score;
            player1Score += score;
        }
        else
        {
            spawnEvents[eventIndex].Player2Score += score;
            player2Score += score;
        }
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
                if (spawnEvents[eventIndex].Player1Score > spawnEvents[eventIndex].Player2Score)
                {
                    particleSystemWin1.Play();
                }
                else if (spawnEvents[eventIndex].Player2Score > spawnEvents[eventIndex].Player1Score)
                {
                    particleSystemWin2.Play();
                }

                spawnEvents[eventIndex].HasFinished = false;
                spawnState = State.SPAWN;
                eventIndex++;

                if (eventIndex >= spawnEvents.Count)
                {
                    //end game
                    _playerManager.StopScripts(false);
                    if (player1Score > player2Score)
                    {
                        particleSystemWin1WINWIN.Play();
                        _finishScore.sprite = _spriteGREEN;
                        _finishScoreText.text = player1Score.ToString("F2");
                    }
                    else
                    {
                        particleSystemWin2WINWIN.Play();
                        _finishScore.sprite = _spriteRED;
                        _finishScoreText.text = player2Score.ToString("F2");
                    }

                    _finishScore.gameObject.SetActive(true);

                    _fadePart.gameObject.SetActive(true);
                    _fadePart.DOColor(new Color(0, 0, 0, 0.9f), 1);
                    StartCoroutine(CooldownCoroutine(10f, LoadBaseScene));
                }
            }
        }
    }

    private void LoadBaseScene ()
    {
        SceneManager.LoadScene(0);
    }

    private IEnumerator CooldownCoroutine(float cooldownTime, Action callBack)
    {
        yield return new WaitForSeconds(cooldownTime);
        callBack?.Invoke();
    }
}
