using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using TMPro;

public class VideoSceneManager : MonoBehaviour
{

    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private TextMeshProUGUI _textBox;
    [SerializeField] private UnityEngine.UI.Image _panel; 

    private bool loadSceneTrigger;

    // Start is called before the first frame update
    void Start()
    {
        loadSceneTrigger = false;
        _videoPlayer.loopPointReached += BlueOut;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void BlueOut(VideoPlayer vp)
    {
        _videoPlayer.gameObject.SetActive(false);
        StartCoroutine(Sequence());
        StartCoroutine(LoadMovieScene());
    }

    private IEnumerator Sequence()
    {
        _panel.enabled = true;
        yield return new WaitForSeconds(3f);
        _textBox.enabled = true;
        _textBox.text = "IF YOU'RE WATCHING THIS IT MEANS YOU REWOUND BACK BEFORE TRACK 1";
        yield return new WaitForSeconds(3f);
        _textBox.text = "WHY?";
        yield return new WaitForSeconds(3f);
        _textBox.text += "DID I NOT TELL YOU NOT TO REWIND BEYOND TRACK 1?";
        yield return new WaitForSeconds(3f);
        _textBox.text = "I ASSUME YOU WATCHED THAT CHEAP DOCUMENTARY.";
        yield return new WaitForSeconds(3f);
        _textBox.text += "THEY WEREN'T BULLSHITTING. THEY'RE REAL.";
        yield return new WaitForSeconds(3f);
        _textBox.text = "I'M SORRY STRANGER. BUT YOUR FATE IS WRITTEN NOW.";
        yield return new WaitForSeconds(3f);
        _textBox.text = "ONE THAT IS WORSE THAN DEATH";
        yield return new WaitForSeconds(3f);
        _textBox.text = "PLAY THE SCREENSAVER. HIT THE CORNERS.";
        yield return new WaitForSeconds(3f);
        _textBox.text = "IF YOU DON'T, IT'LL COME OUT.";
        yield return new WaitForSeconds(3f);
        _textBox.text = "THAT'S YOUR ONLY WAY OF STAYING ALIVE NOW.";
        yield return new WaitForSeconds(3f);
        _textBox.text = "WHEN YOU CAN'T SEE ANYTHING ANYMORE";
        yield return new WaitForSeconds(3f);
        _textBox.text = "IT'S YOUR TIME TO DIE";
        yield return new WaitForSeconds(5f);
        _textBox.enabled = false;
        _panel.color = Color.black;
        loadSceneTrigger = true;
    }


    private IEnumerator LoadMovieScene()
    {
        yield return null;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(0);
        asyncOperation.allowSceneActivation = false;

        while(!asyncOperation.isDone)
        {
            if(asyncOperation.progress >= 0.9f)
            {
                if(loadSceneTrigger == true)
                {
                    ModeDecider.Devil = true;
                    asyncOperation.allowSceneActivation = true;
                }
            }
            yield return null;
        }
    }
}
