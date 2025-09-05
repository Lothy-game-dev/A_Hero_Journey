using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    #region Public
    public TextMeshPro ScoreText;
    public TextMeshPro ResultText;
    public LayerMask UILayer;
    public bool isStart;
    #endregion
    #region Private
    private bool AlreadyClick;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        // Set Data for restart button
        if (!isStart)
        {
            ScoreText.text = "SCORE:<br>" + PlayerPrefs.GetInt("Score");
            ResultText.text = PlayerPrefs.GetString("Result");
            PlayerPrefs.DeleteAll();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check click
        if (!AlreadyClick)
        {
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);
                    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.position), new Vector3(0, 0, 1), Mathf.Infinity, UILayer);
                    if (hit.collider != null)
                    {
                        if (hit.collider.gameObject == gameObject)
                        {
                            AlreadyClick = true;
                            ClickAction();
                        }
                    }
                }
            }
        }
    }

    private void ClickAction()
    {
        // Move Scene
        if (isStart)
        {
            SceneManager.LoadSceneAsync(1);
            SceneManager.UnloadSceneAsync(2);
        } else
        {
            SceneManager.LoadSceneAsync(1);
            SceneManager.UnloadSceneAsync(0);
        }
    }
}
