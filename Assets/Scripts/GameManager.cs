using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public enum GemType
{
    Purple = 0,
    Blue,
    Green,
    Pink,
    Red,
}

public class Gem
{
    private GameObject _gemObject;
    private GemType _type;

    public Gem(GemType type, Sprite[] spriteList, Transform parent, Vector2 position)
    {
        _type = type;
        _gemObject = new GameObject("Gem");
        _gemObject.transform.SetParent(parent); // Set parent to grid object.
        _gemObject.transform.localScale = Vector3.one / 8.0f;
        _gemObject.transform.position += parent.position; // Ignore parent offset.
        _gemObject.transform.position += new Vector3(position.x, position.y, 0.0f);

        _gemObject.AddComponent<SpriteRenderer>();
        _gemObject.GetComponent<SpriteRenderer>().sprite = spriteList[(int)_type];
    }

    // Setters and Getters
    public GemType Type
    {
        get
        {
            return _type;
        }
    }
    public int IType
    {
        get
        {
            return (int)_type;
        }
    }
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private int _gridWidth;
    [SerializeField] private int _gridHeight;
    [SerializeField] private Sprite[] _sprites;

    private GameObject _gridObject;
    private List<Gem> _grid;

    void Start()
    {
        _grid = new List<Gem>(); // Grid Array
        _gridObject = new GameObject("Grid"); // Grid Object
        _gridObject.transform.position = new Vector3(-((_gridWidth-1.0f)/2.0f), -((_gridHeight-1.0f)/2.0f), 0.0f); // Offset to center.

        // Create grid of gems.
        int type = 0;
        for (int i = 0; i < _gridWidth; ++i)
        {
            for (int j = 0; j < _gridHeight; ++j)
            {
                type = Random.Range(0, _sprites.Length);
                // Add Gem
                Gem gem = new Gem((GemType)type, _sprites, _gridObject.transform, new Vector2(i, j));
                _grid.Add(gem); // Add to array
            }
        }
    }
    void Update()
    {
        
    }
}
