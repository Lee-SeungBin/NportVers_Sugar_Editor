using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }


    [SerializeField]
    private AudioSource bgmAudio;

    [SerializeField]
    private AudioClip[] bgms;

    private int selectBGM;



    private void Awake()
    {
        Instance = this;

        SetBGM(0);
    }


    public void SetBGM(int bgmNumber)
    {
        selectBGM = bgmNumber;
        bgmAudio.clip = bgms[selectBGM];
    }

    public void BGMPlay()
    {
        bgmAudio.Play();
    }

    public void BGMStop()
    {
        bgmAudio.Stop();
    }
}
