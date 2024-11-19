using DG.Tweening;
using System.Collections;
using UnityEngine;

public class CollectTiles : MonoBehaviour
{
    [SerializeField] private GameObject _currentHoveredTile = null;
    [SerializeField] private Transform[] _boxTransforms;
    [SerializeField] private Vector3[] _backTransformsPositon;
    [SerializeField] private Vector3 _backTransformsScale;
    [SerializeField] private Quaternion[] _backTransformsRotation;
    [SerializeField] private GameObject[] _box;
    [SerializeField] private SpawnTiles _spawnTiles;
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private AudioManager _audioManager;
    private Vector3 _originalScale;
    private bool _checkWin = false;
    private bool _canClick = true;
    private int _star = 0;
    private int _multiCombo = 1;
    private float _secondCombo = 0f;

    private void Start()
    {
        _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        _uiManager = GameObject.Find("UICode").GetComponent<UIManager>();
        _box = new GameObject[_boxTransforms.Length + 2];
        _backTransformsPositon = new Vector3[_boxTransforms.Length + 2];
        _backTransformsScale = _spawnTiles.GetTilePrefab().transform.localScale;
        _backTransformsRotation = new Quaternion[_boxTransforms.Length + 2];
    }
    private void Update()
    {
        HoverScale();
        MoveToBoxAndCheckCollect();
        Combo();
    }

    public void BackBox()
    {
        if (_spawnTiles.GetLevelDataList().BackTile == 0) return;
        bool back = false;
        for (int i = _box.Length - 1; i >= 0; i--)
        {
            if (_box[i] == null) continue;
            else
            {
                _box[i].transform.position = _backTransformsPositon[i];
                _box[i].transform.rotation = _backTransformsRotation[i];
                _box[i].transform.localScale = _backTransformsScale;
                _box[i].tag = "Tile";
                _box[i] = null;
                back = true;
                break;
            }
        }
        if (!back) return;
        --_spawnTiles.GetLevelDataList().BackTile;
        string updatedJson = JsonUtility.ToJson(_spawnTiles.GetLevelDataList());
        _spawnTiles.SetLevelDataJson(updatedJson);
        _audioManager.PlaySFX("Back");
    }
    private void HoverScale()
    {
        if (_uiManager.GetPause()) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject.tag == "Tile" && hit.collider.gameObject != _currentHoveredTile)
            {
                if (_currentHoveredTile != null)
                {
                    if (_currentHoveredTile.transform.localScale != _originalScale)
                    {
                        _currentHoveredTile.transform.localScale = _originalScale;
                    }
                }
                _originalScale = hit.collider.gameObject.transform.localScale;
                _currentHoveredTile = hit.collider.gameObject;
                _currentHoveredTile.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
                _audioManager.PlaySFX("Pick");
            }
            else if (hit.collider.gameObject != _currentHoveredTile && _currentHoveredTile != null)
            {
                _currentHoveredTile.transform.localScale = _originalScale;
                _currentHoveredTile = null;
            }
        }
    }
    private void MoveToBoxAndCheckCollect()
    {
        if (Input.GetMouseButtonUp(0) && _currentHoveredTile != null && _canClick)
        {
            _audioManager.PlaySFX("Click");
            _canClick = false;
            Invoke("EnableClick", 0.2f);
            for (int i = 0; i < _box.Length; i++)
            {
                if (_box[i] != null && _box[i].name != _currentHoveredTile.name) continue;
                else if (_box[i] != null && _box[i + 1] != null && _box[i].name == _currentHoveredTile.name && _box[i + 1].name != _currentHoveredTile.name)
                {
                    BackTileFromBox(i + 1, _box.Length);
                    _box[i + 1] = _currentHoveredTile;
                    _backTransformsPositon[i + 1] = _currentHoveredTile.transform.position;
                    _backTransformsRotation[i + 1] = _currentHoveredTile.transform.rotation;
                    _currentHoveredTile.transform.position = _boxTransforms[i + 1].position;
                    _currentHoveredTile.transform.rotation = Quaternion.identity;
                    CheckLose();
                    DontTouch();
                    break;
                }
                else if (_box[i + 2] != null && _box[i + 1].name == _currentHoveredTile.name)
                {
                    BackTileFromBox(i + 2, _box.Length);
                    _box[i + 2] = _currentHoveredTile;
                    _backTransformsPositon[i + 2] = _currentHoveredTile.transform.position;
                    _backTransformsRotation[i + 2] = _currentHoveredTile.transform.rotation;
                    _currentHoveredTile.transform.position = _boxTransforms[i + 2].position;
                    _currentHoveredTile.transform.rotation = Quaternion.identity;
                    StartCoroutine(CollectTileFromBox(i, i + 2));
                    if (_box[i + 3] != null) StartCoroutine(ForwardTileFromBox(i + 3, _box.Length));
                    DontTouch();
                    break;
                }
                else if (_box[i] == null)
                {
                    _box[i] = _currentHoveredTile;
                    _backTransformsPositon[i] = _currentHoveredTile.transform.position;
                    _backTransformsRotation[i] = _currentHoveredTile.transform.rotation;
                    _currentHoveredTile.transform.position = _boxTransforms[i].position;
                    _currentHoveredTile.transform.rotation = Quaternion.identity;
                    if (i >= 2)
                    {
                        if (_box[i - 2].name == _currentHoveredTile.name)
                        {
                            StartCoroutine(CollectTileFromBox(i - 2, i));
                        }
                        else CheckLose();
                    }
                    DontTouch();
                    break;
                }
            }

        }
    }

    private void EnableClick()
    {
        _canClick = true;
    }
    private void Combo()
    {
        if (_secondCombo > 0 && !_uiManager.GetPause())
        {
            _secondCombo -= Time.deltaTime;
            _uiManager.UICombo(_secondCombo, _multiCombo);
        }
        else if (_secondCombo <= 0) _multiCombo = 1;
    }
    private IEnumerator CollectTileFromBox(int start, int end)
    {
        for (int i = start; i <= end; i++)
        {
            yield return new WaitForSeconds(0.06f);
            _box[i].SetActive(false);
            _box[i] = null;
        }
        _uiManager.SetStar(_star += _multiCombo);
        ++_multiCombo;
        _secondCombo = 10f;
        _uiManager.AnimStar();
        _audioManager.PlaySFX("Collect");
        CheckWin();
    }
    private IEnumerator ForwardTileFromBox(int start, int end)
    {
        for (int i = start; i < end; i++)
        {
            yield return new WaitForSeconds(0.06f);
            if (_box[i] == null) continue;
            _box[i - 3] = _box[i];
            _backTransformsPositon[i - 3] = _backTransformsPositon[i];
            _backTransformsRotation[i - 3] = _backTransformsRotation[i];
            _box[i - 3].transform.position = _boxTransforms[i - 3].position;
            _box[i] = null;
        }
    }
    private void BackTileFromBox(int start, int end)
    {
        for (int i = end - 1; i >= start; i--)
        {
            if (_box[i] == null) continue;
            _box[i + 1] = _box[i];
            _backTransformsPositon[i + 1] = _backTransformsPositon[i];
            _backTransformsRotation[i + 1] = _backTransformsRotation[i];
            _box[i + 1].transform.position = _boxTransforms[i + 1].position;
        }
    }

    private void ShrinkTile(GameObject tile)
    {
        if (tile != null) tile.transform.DOScale(2f, 0.5f);
    }
    private void DontTouch()
    {
        foreach (var tile in _box)
        {
            if (tile != null && tile.tag != "Untagged")
            {
                tile.tag = "Untagged";
                ShrinkTile(tile);
            }
        }
    }
    private void CheckWin()
    {
        foreach (var tile in _spawnTiles.GetTileList())
        {
            if (tile.activeInHierarchy)
            {
                _checkWin = false;
                break;
            }
            else _checkWin = true;
        }
        if (_checkWin)
        {
            _uiManager.SetActiveWin(true, _star);
            _spawnTiles.GetLevelDataList().Star += _star;
            ++_spawnTiles.GetLevelDataList().Level;
            string updatedJson = JsonUtility.ToJson(_spawnTiles.GetLevelDataList());
            _spawnTiles.SetLevelDataJson(updatedJson);
            _audioManager.PlaySFX("Victory");
        }
    }

    private void CheckLose()
    {
        int index = 0;
        foreach (var tile in _box) if (tile != null) index++;
        if (index >= 7)
        {
            _uiManager.SetActiveLose(true);
            _currentHoveredTile = null;
            _audioManager.PlaySFX("Lose");
        }
    }
}
