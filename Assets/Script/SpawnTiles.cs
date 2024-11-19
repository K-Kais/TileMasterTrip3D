using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SpawnTiles : MonoBehaviour
{
    [SerializeField] private CollectTiles _collectTiles;
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private List<GameObject> _tileList;
    [SerializeField] private TextAsset _levelDataJSON;
    [SerializeField] private LevelData _levelDataList;
    private List<string> _colorOptions = new List<string>();
    private string[] _curentColorOptions = new string[] { "TextureBlue", "TexturePink", "TexturePurple", "TextureRed", "TextureWhite", "TextureYellow" };
    //private string filePath = "/TextAsset/";
    private void Start()
    {
        SetLevelDataList();
        SpawnTilesPrefab();
    }

    public GameObject GetTilePrefab()
    { return _tilePrefab; }
    public List<GameObject> GetTileList()
    {
        return _tileList;
    }
    public LevelData SetLevelDataList()
    {
        string filePath = Path.Combine(Application.persistentDataPath, _levelDataJSON.name + ".json");
        if (!File.Exists(filePath)) File.WriteAllText(filePath, _levelDataJSON.text);
        string read = File.ReadAllText(filePath);
        return _levelDataList = JsonUtility.FromJson<LevelData>(read);
    }
    public LevelData GetLevelDataList()
    {
        return _levelDataList;
    }
    public void SetLevelDataJson(string updatedJson)
    {
        string filePath = Path.Combine(Application.persistentDataPath, _levelDataJSON.name + ".json");
        File.WriteAllText(filePath, updatedJson);
    }

    private string GetRandomLevelTexture(List<string> textureList)
    {
        if (textureList == null || textureList.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, _levelDataList.Level);
        return textureList[randomIndex];
    }
    private string GetRandomColorTexture()
    {
        if (_colorOptions.Count == 0)
        {
            _colorOptions = _curentColorOptions.ToList();
            GetRandomColorTexture();
        }
        string randomColor = _colorOptions[Random.Range(0, _colorOptions.Count)];
        switch (randomColor)
        {
            case "TextureBlue":
                _colorOptions.Remove(randomColor);
                return GetRandomLevelTexture(_levelDataList.TextureBlue);
            case "TexturePink":
                _colorOptions.Remove(randomColor);
                return GetRandomLevelTexture(_levelDataList.TexturePink);
            case "TexturePurple":
                _colorOptions.Remove(randomColor);
                return GetRandomLevelTexture(_levelDataList.TexturePurple);
            case "TextureRed":
                _colorOptions.Remove(randomColor);
                return GetRandomLevelTexture(_levelDataList.TextureRed);
            case "TextureWhite":
                _colorOptions.Remove(randomColor);
                return GetRandomLevelTexture(_levelDataList.TextureWhite);
            case "TextureYellow":
                _colorOptions.Remove(randomColor);
                return GetRandomLevelTexture(_levelDataList.TextureYellow);
            default:
                return GetRandomColorTexture();
        }
    }
    private void SpawnTilesPrefab()
    {
        float x = -3f;
        float z = 5f;
        float size = _tilePrefab.transform.position.x + 1f;
        string nameTexture = GetRandomColorTexture();
        for (int i = 1; i <= _levelDataList.Level * 9; i++)
        {
            Vector3 spawnPosition = FindValidSpawnPosition(new Vector3(size, size, size));
            GameObject newTile = Instantiate(_tilePrefab, spawnPosition, Quaternion.Euler(0, Random.Range(0, 360), 0), transform);
            newTile.name = nameTexture;
            _tileList.Add(newTile);
            newTile.GetComponent<Renderer>().material.mainTexture = Resources.Load<Texture>("tiles/" + nameTexture);
            if (i % 3 == 0) nameTexture = GetRandomColorTexture();
            if (_levelDataList.Level != 1) continue;
            SpawnPositonTilesPrefabLevel1(newTile, ref x, ref z);
        }
    }
    private Vector3 FindValidSpawnPosition(Vector3 collisionSize)
    {
        Vector3 randomPosition;
        int maxAttempts = 100;
        int attempts = 0;
        while (attempts < maxAttempts)
        {
            randomPosition = new Vector3(
                Random.Range(-4f, 4f),
                Random.Range(1f, 3f),
                Random.Range(-2.5f, 7f)
            );
            Collider[] colliders = Physics.OverlapBox(randomPosition, collisionSize / 2);

            if (colliders.Length == 0) return randomPosition;
            attempts++;
        }

        return Vector3.zero;
    }
    private void SpawnPositonTilesPrefabLevel1(GameObject tile, ref float x, ref float z)
    {
        tile.transform.localPosition = new Vector3(x, 2f, z);
        tile.transform.rotation = Quaternion.identity;
        if (x > 2f)
        {
            z -= 3f;
            x = -3f;
        }
        else x += 3f;
    }
}


[System.Serializable]
public class LevelData
{
    public int Level;
    public int Coin;
    public int Star;
    public int BackTile;
    public bool Sound;
    public bool Music;
    public List<string> TextureBlue;
    public List<string> TexturePink;
    public List<string> TexturePurple;
    public List<string> TextureRed;
    public List<string> TextureWhite;
    public List<string> TextureYellow;
}