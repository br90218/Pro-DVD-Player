using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _background;
    [SerializeField] private AudioSource _noise;
    [SerializeField] private AudioSource _devil;
    [SerializeField] private AudioClip _deathclip;



    private void Start() {
        if(GameMaster.Instance.gameMode == GameMaster.GameMode.DEVIL)
        {
            _noise.enabled = true;
            _devil.enabled = true;
            StartCoroutine(NoiseDevilRoutine());
        }
    }


    private IEnumerator NoiseDevilRoutine()
    {
        
        while(true)
        {
            var value = (1 - GameMaster.Instance.remainingTime  / 15f);

            _noise.volume = (1 - GameMaster.Instance.remainingTime  / 15f);
            _devil.volume = (1 - GameMaster.Instance.remainingTime  / 10f);

            yield return null;

        }

    }

    internal void PlayDeath()
    {
        _devil.PlayOneShot(_deathclip, 1.0f);
    }


    
}
