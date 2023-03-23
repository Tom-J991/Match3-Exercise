using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int _gridWidth;
    [SerializeField] private int _gridHeight;
    [SerializeField] private GameObject gemPrefab;

    public Sprite[] sprites;

    private GameObject _gridObject;
    private GameObject[,] _gems;

    [HideInInspector] public bool _shifting { get; set; }

    void Start()
    {
        GameManager instance = transform.GetComponent<GameManager>();

        _gems = new GameObject[_gridWidth, _gridHeight];

        _gridObject = new GameObject("Grid"); // Grid Object
        _gridObject.transform.position = new Vector3(-((_gridWidth-1.0f)/2.0f), -((_gridHeight-1.0f)/2.0f), 0.0f); // Offset to center.

        // Create grid of gems.
        int type = 0;
        int[] lType = new int[_gridHeight]; // to left
        int dType = 0; // below
        for (int i = 0; i < _gridWidth; ++i)
        {
            for (int j = 0; j < _gridHeight; ++j)
            {
                // Get surrounding gems and find non-repeating type.
                List<int> possibleGems = new List<int>();
                possibleGems.AddRange(Enum.GetValues(typeof(GemType)).Cast<int>());
                possibleGems.Remove(lType[j]);
                possibleGems.Remove(dType);
                type = possibleGems[UnityEngine.Random.Range(0, possibleGems.Count-1)];
                // Add Gem
                _gems[i, j] = CreateGem(_gridObject.transform, (GemType)type, new Vector2(i, j));
                //
                lType[j] = type;
                dType = type;
            }
        }
    }
    void Update()
    {
        
    }

    GameObject CreateGem(Transform parent, GemType type, Vector2 position)
    {
        GameObject gemObject = Instantiate(gemPrefab, parent);
        gemObject.transform.localScale = Vector3.one / 6.0f;
        gemObject.transform.position += new Vector3(position.x, position.y, 0.0f);
        gemObject.name = "Gem { " + position.x + ", " + position.y + " }";

        Gem gem = gemObject.GetComponent<Gem>();
        gem.gameInstance = transform.GetComponent<GameManager>();
        gem.type = type;

        gemObject.GetComponent<SpriteRenderer>().sprite = sprites[(int)type];

        return gemObject;
    }

    public IEnumerator FindNullTiles()
    {
        for (int x = 0; x < _gridWidth; x++)
        {
            for (int y = 0; y < _gridHeight; y++)
            {
                if (_gems[x, y].GetComponent<SpriteRenderer>().sprite == null || _gems[x, y].GetComponent<Gem>().type == GemType.NONE)
                {
                    yield return StartCoroutine(ShiftTilesDown(x, y));
                    break;
                }
            }
        }

        for (int x = 0; x < _gridWidth; x++)
        {
            for (int y = 0; y < _gridHeight; y++)
            {
                _gems[x, y].GetComponent<Gem>().ClearAllMatches();
            }
        }
    }
    private IEnumerator ShiftTilesDown(int _x, int _y, float shiftDelay = 0.03f)
    {
        _shifting = true;
        List<GameObject> gemsToShift = new List<GameObject>();
        int nullCount = 0;

        for (int y = _y; y < _gridHeight; y++)
        {
            if (_gems[_x, y].GetComponent<SpriteRenderer>().sprite == null || _gems[_x, y].GetComponent<Gem>().type == GemType.NONE)
                nullCount++;
            gemsToShift.Add(_gems[_x, y]);
        }

        for (int i = 0; i < nullCount; i++)
        {
            yield return new WaitForSeconds(shiftDelay);
            for (int j = 0; j < gemsToShift.Count-1; j++)
            {
                gemsToShift[j].GetComponent<SpriteRenderer>().sprite = gemsToShift[j+1].GetComponent<SpriteRenderer>().sprite;
                gemsToShift[j].GetComponent<Gem>().type = gemsToShift[j+1].GetComponent<Gem>().type;

                int newType = GetNewGem(_x, _gridHeight-1);
                gemsToShift[j+1].GetComponent<Gem>().type = (GemType)newType;
                gemsToShift[j+1].GetComponent<SpriteRenderer>().sprite = sprites[(int)newType];
            }
        }

        _shifting = false;
    }
    private int GetNewGem(int x, int y)
    {
        List<int> possibleGems = new List<int>();
        possibleGems.AddRange(Enum.GetValues(typeof(GemType)).Cast<int>());
        if (x > 0)
            possibleGems.Remove(_gems[x-1, y].GetComponent<Gem>().IType);
        if (x < _gridWidth - 1)
            possibleGems.Remove(_gems[x+1, y].GetComponent<Gem>().IType);
        if (y > 0)
            possibleGems.Remove(_gems[x, y-1].GetComponent<Gem>().IType);

        return possibleGems[UnityEngine.Random.Range(0, possibleGems.Count-1)];
    }
}
