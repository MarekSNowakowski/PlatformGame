using System;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class MusicManager : MonoBehaviour
{
    private enum MusicGameState { Menu, Gameplay, Results }

    [Header("Audio Sources")]
    [SerializeField] private ManagedAudioSource[] menuMusic;
    [SerializeField] private ManagedAudioSource[] gameplayMusic;
    [SerializeField] private ManagedAudioSource[] resultsMusic;
    [SerializeField] private ManagedAudioSource gameOverSound;

    private ManagedAudioSource currentMenuAudioSource;
    private ManagedAudioSource currentGameplaySource;
    private ManagedAudioSource currentResultSource;

    private MusicGameState gameState = MusicGameState.Menu;
    private bool isLooping = false;

    private void Awake()
    {
        currentMenuAudioSource = menuMusic[Random.Range(0,menuMusic.Length)];
        currentGameplaySource = gameplayMusic[Random.Range(0,gameplayMusic.Length)];
        currentResultSource = resultsMusic[Random.Range(0,resultsMusic.Length)];
    }

    public void AdjustStateToScene(Scene newScene)
    {
        isLooping = false;
        gameState = newScene.buildIndex == 0 ? MusicGameState.Menu : MusicGameState.Gameplay;

        if (gameState == MusicGameState.Menu)
            PlayMenuMusic();
        // In gameplay scene we play music manually to match its start with the beginning of countdown
    }

    private void Update()
    {
        ManageLoopedMusic();
    }

    private void ManageLoopedMusic()
    {
        if (isLooping)
        {
            switch (gameState)
            {
                case MusicGameState.Menu:
                    if (!currentMenuAudioSource.AudioSource.isPlaying)
                    {
                        PlayNextTrack(menuMusic, ref currentMenuAudioSource);
                    }
                    break;
                case MusicGameState.Gameplay:
                    if (!currentGameplaySource.AudioSource.isPlaying)
                    {
                        PlayNextTrack(gameplayMusic, ref currentGameplaySource);
                    }
                    break;
                case MusicGameState.Results:
                    if (!currentResultSource.AudioSource.isPlaying)
                    {
                        PlayNextTrack(resultsMusic, ref currentResultSource);
                    }
                    break;
            }
        }
        else
        {
            if (gameState == MusicGameState.Results && currentResultSource.AudioSource.isPlaying)
            {
                isLooping = true;
            }
        }
    }

    private void PlayNextTrack(ManagedAudioSource[] sources, ref ManagedAudioSource currentSource)
    {
        // Track stopped, play next
        int index = Array.IndexOf(sources, currentSource);
        index = index == sources.Length - 1 ? 0 : index + 1;
        currentSource = sources[index];
        currentSource.AudioSource.Play();
    }

    private void PlayMenuMusic()
    {
        currentResultSource.AudioSource.Stop();
        currentGameplaySource.AudioSource.Stop();
        currentMenuAudioSource.AudioSource.Play();
        isLooping = true;
    }

    public void PlayGameplayMusic()
    {
        currentResultSource.AudioSource.Stop();
        currentMenuAudioSource.AudioSource.Stop();
        currentGameplaySource.AudioSource.Play();
        isLooping = true;
    }

    public void PlayGameOverSoundEffects()
    {
        isLooping = false;
        gameState = MusicGameState.Results;

        currentGameplaySource.AudioSource.Stop();

        gameOverSound.AudioSource.Play();
        var resultsMusicDelay = gameOverSound.AudioSource.clip.length;
        currentResultSource.AudioSource.PlayDelayed(resultsMusicDelay);
    }
}
