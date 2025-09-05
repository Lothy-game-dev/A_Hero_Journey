using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    #region Public
    public int Score;
    public TextMeshPro ScoreText;
    #endregion
    #region Private
    private bool AlreadyDoneSpawningChest;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScoreText();
        // If condition reaced -> spawn chest
        if (!AlreadyDoneSpawningChest && Score>=100)
        {
            AlreadyDoneSpawningChest = true;
            GetComponent<ItemSpawner>().SpawnChest();
        }
    }

    private void UpdateScoreText()
    {
        ScoreText.text = "Score:" + Score;
    }

    public void AddScore()
    {
        Score++;
    }
}
