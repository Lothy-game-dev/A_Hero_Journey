using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Chest : MonoBehaviour
{
    #region Public
    public ParticleSystem[] EnterVFX;
    public ParticleSystem[] ExitVFX;
    public Sprite OpenSprite;
    public TextMeshPro ChestText;
    #endregion
    #region Private
    private bool AllowCheckEnd;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StopAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator StopAnimation()
    {
        yield return new WaitForSeconds(1f);
        foreach (var vfx in EnterVFX)
        {
            vfx.Stop();
        }
        ChestText.gameObject.SetActive(true);
        AllowCheckEnd = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (AllowCheckEnd)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                GetComponent<SpriteRenderer>().sprite = OpenSprite;
                foreach (var vfx in ExitVFX)
                {
                    vfx.Play();
                }
                StartCoroutine(EndGame());
            }
        }
    }

    private IEnumerator EndGame()
    {
        for (int i=0;i<10;i++)
        {
            // Make VFX bigger 
            var m1 = ExitVFX[0].emission;
            m1.rateOverTimeMultiplier += 10;
            var m = ExitVFX[1].main;
            m.startSizeMultiplier += 1;
            yield return new WaitForSeconds(0.5f);
        }
        PlayerPrefs.SetInt("Score", Camera.main.GetComponent<ScoreController>().Score);
        PlayerPrefs.SetString("Result", "VICTORY!");
        SceneManager.LoadSceneAsync(2);
        SceneManager.UnloadSceneAsync(1);
    }
}
