using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private SpawnTiles _spawnTiles;
    [SerializeField] private CollectTiles _collectTiles;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private TMP_Text _txtStar;
    [SerializeField] private TMP_Text _txtStarWin;
    [SerializeField] private TMP_Text _txtClock;
    [SerializeField] private TMP_Text _txtLevel;
    [SerializeField] private TMP_Text _txtHomeLevel;
    [SerializeField] private TMP_Text _txtHomeCoin;
    [SerializeField] private TMP_Text _txtHomeStar;
    [SerializeField] private TMP_Text _txtLevelLose;
    [SerializeField] private TMP_Text _txtNumberBack;
    [SerializeField] private TMP_Text _txtCombo;
    [SerializeField] private GameObject _gameScenePrefab;
    [SerializeField] private GameObject _backgroundPause;
    [SerializeField] private GameObject _backgroundLose;
    [SerializeField] private GameObject _backgroundWin;
    [SerializeField] private GameObject _backgroundSeting;
    [SerializeField] private GameObject _backgroundGamePlay;
    [SerializeField] private GameObject _backgroundGameHome;
    [SerializeField] private GameObject _backgroundPurchaseItems;
    [SerializeField] private GameObject _backgroundLoading;
    [SerializeField] private GameObject _backgroundCombo;
    [SerializeField] private GameObject _loading;
    [SerializeField] private GameObject _gameScene;
    [SerializeField] private GameObject _star;
    [SerializeField] private GameObject _clock;
    [SerializeField] private GameObject _buttonPlus;
    [SerializeField] private Sprite[] _backgroundSound;
    [SerializeField] private Sprite[] _backgroundMusic;
    [SerializeField] private Image _backgroundSoundButton;
    [SerializeField] private Image _backgroundMusicButton;
    [SerializeField] private Image _backgroundSetingSoundButton;
    [SerializeField] private Image _backgroundSetingMusicButton;
    [SerializeField] private float _countdownTime = 600f;
    private bool _isPaused = false;
    private bool _canScale = true;
    private void Awake()
    {
        StartCoroutine(Load());
        SpawnGameScene(false);
        _spawnTiles.SetLevelDataList();
    }
    private void Start()
    {
        _txtHomeStar.text = $"{_spawnTiles.GetLevelDataList().Star}";
        _txtHomeCoin.text = $"{_spawnTiles.GetLevelDataList().Coin}";
        _txtHomeLevel.text = $"{_spawnTiles.GetLevelDataList().Level}";
        _txtLevel.text = $"Lv.{_spawnTiles.GetLevelDataList().Level}";
        _txtNumberBack.text = $"{_spawnTiles.GetLevelDataList().BackTile}";
        _txtStar.text = "0";
        _backgroundSoundButton.sprite = _backgroundSound[_spawnTiles.GetLevelDataList().Sound ? 1 : 0];
        _backgroundMusicButton.sprite = _backgroundMusic[_spawnTiles.GetLevelDataList().Music ? 1 : 0];
        _backgroundSetingSoundButton.sprite = _backgroundSound[_spawnTiles.GetLevelDataList().Sound ? 1 : 0];
        _backgroundSetingMusicButton.sprite = _backgroundMusic[_spawnTiles.GetLevelDataList().Music ? 1 : 0];
        if (_spawnTiles.GetLevelDataList().BackTile == 0) _buttonPlus.SetActive(true);
        else _buttonPlus.SetActive(false);
    }
    private void Update()
    {
        if (_backgroundGamePlay.activeInHierarchy) UpdateTimerUI();
        else if (_backgroundLoading.activeInHierarchy) _loading.transform.Rotate(0, 0, Time.deltaTime * 50f);
    }
    private void UpdateTimerUI()
    {
        if (_isPaused) return;
        _countdownTime -= Time.deltaTime;
        if (_countdownTime < 0)
        {
            _countdownTime = 0;
            SetActiveLose(true);
        }
        int minutes = Mathf.FloorToInt(_countdownTime / 60);
        int seconds = Mathf.FloorToInt(_countdownTime % 60);
        _txtClock.text = string.Format("{0:00}:{1:00}", minutes, seconds); ;
        if (_countdownTime <= 30 && _countdownTime > 0 && _canScale)
        {
            _canScale = false;
            AnimClock();
            Invoke("EnableScale", 1f);
        }
    }

    private IEnumerator Load()
    {
        yield return new WaitForSeconds(3f);
        _backgroundGameHome.SetActive(true);
        _backgroundLoading.SetActive(false);
    }
    public void SetStar(int star)
    {
        _txtStar.text = $"{star}";
    }
    public void UICombo(float second, int number)
    {
        if (second > 0)
        {
            _backgroundCombo.SetActive(true);
            _backgroundCombo.GetComponent<Image>().fillAmount = second / 10f;
            _txtCombo.text = $"combo x{number - 1}";
        } else _backgroundCombo.SetActive(false);
    }
    public void AnimStar()
    {
        _star.transform.DOScale(2f, 1f).SetDelay(0f).SetEase(Ease.OutBack);
        _star.transform.DOScale(1f, 2f).SetDelay(0.1f).SetEase(Ease.OutBack);
    }
    public void AnimClock()
    {
        _clock.transform.DOScale(1.5f, 1f).SetDelay(0f).SetEase(Ease.OutBack);
        _clock.transform.DOScale(1f, 1.5f).SetDelay(0.3f).SetEase(Ease.OutBack);
    }
    public void AnimOpen(GameObject bg)
    {
        bg.transform.DOScale(1.1f, 1f).SetDelay(0f).SetEase(Ease.OutBack);
        bg.transform.DOScale(1f, 1.1f).SetDelay(0.1f).SetEase(Ease.OutBack);
    }
    public void AnimClose(GameObject bg)
    {
        if (bg.transform.localScale == Vector3.zero) return;
        bg.transform.DOScale(1.1f, 1f).SetDelay(0f).SetEase(Ease.OutBack);
        bg.transform.DOScale(0f, 1.1f).SetDelay(0.1f);
    }

    private void EnableScale()
    {
        _canScale = true;
    }
    public void SetActiveWin(bool active, int star)
    {
        if (active) AnimOpen(_backgroundWin);
        else AnimClose(_backgroundWin);
        _backgroundCombo.SetActive(false);
        _txtStarWin.text = $"+{star}";
        _isPaused = true;
    }
    private void SpawnGameScene(bool active)
    {
        if (_gameScene != null) Destroy(_gameScene);
        GameObject newGameScene = Instantiate(_gameScenePrefab);
        newGameScene.name = _gameScenePrefab.name;
        _spawnTiles = newGameScene.GetComponentInChildren<SpawnTiles>();
        _collectTiles = newGameScene.GetComponentInChildren<CollectTiles>();
        _gameScene = newGameScene;
        newGameScene.SetActive(active);
    }
    public void SetActiveLose(bool active)
    {
        if (active) AnimOpen(_backgroundLose);
        else AnimClose(_backgroundLose);
        _backgroundCombo.SetActive(false);
        _isPaused = active;
        _txtLevelLose.text = $"LEVEL {_spawnTiles.GetLevelDataList().Level}";
        _countdownTime = 0;
        _audioManager.PlaySFX("Lose");
    }

    public bool GetPause()
    {
        return _isPaused;
    }
    public void Pause()
    {
        _isPaused = true;
        AnimOpen(_backgroundPause);
        AnimClose(_backgroundWin);
        AnimClose(_backgroundLose);
        _audioManager.PlaySFX("Button");
    }
    public void Continute()
    {
        _isPaused = false;
        AnimClose(_backgroundPause);
        AnimClose(_backgroundPurchaseItems);
        if ($"LV.{_spawnTiles.GetLevelDataList().Level}" != _txtLevel.text) PlayGame();
        _audioManager.PlaySFX("Button");
    }
    public void BackTile()
    {
        _collectTiles.BackBox();
        _txtNumberBack.text = $"{_spawnTiles.GetLevelDataList().BackTile}";
        if (_buttonPlus.activeInHierarchy)
        {
            AnimOpen(_backgroundPurchaseItems);
            _isPaused = true;
        }
        else if (_spawnTiles.GetLevelDataList().BackTile == 0) _buttonPlus.SetActive(true);
        _audioManager.PlaySFX("Button");
    }
    public void Purchase()
    {
        _audioManager.PlaySFX("Button");
        if (_spawnTiles.GetLevelDataList().Coin < 500) return;
        AnimClose(_backgroundPurchaseItems);
        _isPaused = false;
        _spawnTiles.GetLevelDataList().Coin -= 500;
        ++_spawnTiles.GetLevelDataList().BackTile;
        string updatedJson = JsonUtility.ToJson(_spawnTiles.GetLevelDataList());
        _spawnTiles.SetLevelDataJson(updatedJson);
        _txtNumberBack.text = $"{_spawnTiles.GetLevelDataList().BackTile}";
        _buttonPlus.SetActive(false);
    }
    public void OpenSeting()
    {
        AnimOpen(_backgroundSeting);
        _audioManager.PlaySFX("Button");
    }
    public void CloseSeting()
    {
        AnimClose(_backgroundSeting);
        _audioManager.PlaySFX("Button");
    }
    public void PlayGame()
    {
        _txtStar.text = "0";
        _txtLevel.text = $"LV.{_spawnTiles.GetLevelDataList().Level}";
        _countdownTime = 240f + (_spawnTiles.GetLevelDataList().Level * 60);
        _isPaused = false;
        _backgroundGamePlay.SetActive(true);
        _backgroundGameHome.SetActive(false);
        AnimClose(_backgroundWin);
        AnimClose(_backgroundLose);
        AnimClose(_backgroundSeting);
        SpawnGameScene(true);
        _audioManager.PlaySFX("Button");
    }
    public void GoHome()
    {
        _countdownTime = 120f;
        _isPaused = false;
        _backgroundGameHome.SetActive(true);
        _backgroundGamePlay.SetActive(false);
        _backgroundCombo.SetActive(false);
        AnimClose(_backgroundLose);
        AnimClose(_backgroundPause);
        Start();
        Destroy(_gameScene);
        _audioManager.PlaySFX("Button");
    }
    public void Sound()
    {
        _spawnTiles.GetLevelDataList().Sound = !_spawnTiles.GetLevelDataList().Sound;
        _backgroundSoundButton.sprite = _backgroundSound[_spawnTiles.GetLevelDataList().Sound ? 1 : 0];
        _backgroundSetingSoundButton.sprite = _backgroundSound[_spawnTiles.GetLevelDataList().Sound ? 1 : 0];
        string updatedJson = JsonUtility.ToJson(_spawnTiles.GetLevelDataList());
        _spawnTiles.SetLevelDataJson(updatedJson);
        _audioManager.PlaySFX("Button");
    }

    public void Music()
    {
        _spawnTiles.GetLevelDataList().Music = !_spawnTiles.GetLevelDataList().Music;
        _backgroundMusicButton.sprite = _backgroundMusic[_spawnTiles.GetLevelDataList().Music ? 1 : 0];
        _backgroundSetingMusicButton.sprite = _backgroundMusic[_spawnTiles.GetLevelDataList().Music ? 1 : 0];
        string updatedJson = JsonUtility.ToJson(_spawnTiles.GetLevelDataList());
        _spawnTiles.SetLevelDataJson(updatedJson);
        _audioManager.PlaySFX("Button");
    }
}
