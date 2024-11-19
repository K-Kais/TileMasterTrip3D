using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Sound[] _musicSound, _sfxSounds;
    [SerializeField] private AudioSource _musicSource, _sfxSource;
    [SerializeField] private SpawnTiles _spawnTiles;
    [SerializeField] private GameObject _gameScenePrefab;
    private void Start()
    {
        SetAudio();
    }
    private void SetAudio()
    {
        GameObject newGameScene = Instantiate(_gameScenePrefab);
        _spawnTiles = newGameScene.GetComponentInChildren<SpawnTiles>();
        _spawnTiles.SetLevelDataList();
        _musicSource.mute = !_spawnTiles.GetLevelDataList().Music;
        _sfxSource.mute = !_spawnTiles.GetLevelDataList().Sound;
        Destroy(newGameScene);
    }
    public void PlayMusic(string name)
    {
        Sound s = Array.Find(_musicSound, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Music not found");
        }
        else
        {
            _musicSource.clip = s.clip;
            _musicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(_sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("SFX not found");
        }
        else
        {
            _sfxSource.PlayOneShot(s.clip);
        }
    }

    public void ToggleSFX()
    {
        _sfxSource.mute = !_sfxSource.mute;
    }

    public void ToggleMusic()
    {
        _musicSource.mute = !_musicSource.mute;
    }
}
[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}