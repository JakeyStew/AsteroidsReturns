using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game_Manager : MonoBehaviour
{
    [SerializeField]
    private bool _isGameOver = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _isGameOver == true)
        {
            int scene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(scene, LoadSceneMode.Single);
        }
    }

    public void GameOver()
    {
        _isGameOver = true;
    }
}
