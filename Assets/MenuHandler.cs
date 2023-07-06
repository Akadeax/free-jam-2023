using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    public static MenuHandler instance;
    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        highscoreText.text = "Highscore: " + PlayerPrefs.GetInt("highscore");
    }

    public Button button;
    public float waitTime = 0.3f;

    public AudioSource src;

    public TextMeshProUGUI highscoreText;

    public void OnClick()
    {
        button.interactable = false;
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        src.Play();
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(1);
    }
}
