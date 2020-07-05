using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class Game : MonoBehaviour
{
    public Image gameOverImage;
    public GameObject gameOverUI;
    public GameObject healthBar;
    public GameObject teleportBar;

    public void GameOver()
    {
        Cursor.visible = true;
        StartCoroutine("GameOverFade");
        gameOverUI.SetActive(true);
        healthBar.SetActive(false);
        teleportBar.SetActive(false);
    }

    IEnumerator GameOverFade()
    {
        float precent = 0.0f;

        while (precent <= 1.0f)
        {
            precent += Time.deltaTime;
            gameOverImage.color = Color.Lerp(Color.clear, Color.black, precent);
            yield return null;
        }
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene("MainScene");
    }
}
