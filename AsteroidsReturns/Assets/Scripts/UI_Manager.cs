using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _scoreText;
    [SerializeField]
    private TextMeshProUGUI _spaceToStartText;
    [SerializeField]
    private Image _LivesImg;
    [SerializeField]
    private Sprite[] _liveSprites;

    [SerializeField]
    private TextMeshProUGUI _gameOverText;
    [SerializeField]
    private TextMeshProUGUI _restartText;

    private Game_Manager _gameManager;
    void Start()
    {
        _scoreText.text = "00000";
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game Manager").GetComponent<Game_Manager>();
        if (_gameManager == null)
        {
            Debug.Log("Game Manager is NULL");
        }
        StartCoroutine(StartFlickerRoutine());
    }

    public void UpdateScore(int score)
    {
        _scoreText.text = score.ToString("D5");
    }

    public void UpdateLives(int currentLives)
    {
        //give it a new one based on currentLives index
        _LivesImg.sprite = _liveSprites[currentLives];
        if (currentLives == 0)
        {
            GameOverSequence();
        }
    }

    public void StartGame()
    {
        _spaceToStartText.gameObject.SetActive(false);
    }
    private void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
    }

    IEnumerator StartFlickerRoutine()
    {
        while (true)
        {
            _spaceToStartText.text = "Press Space To Start";
            yield return new WaitForSeconds(0.75f);
            _spaceToStartText.text = "";
            yield return new WaitForSeconds(0.75f);
        }
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
