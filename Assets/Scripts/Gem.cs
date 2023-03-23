using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum GemType
{
    Purple = 0,
    Blue,
    Green,
    Pink,
    Red,
    NONE = -1
}

public class Gem : MonoBehaviour, IPointerDownHandler
{
    [HideInInspector] public GameManager gameInstance;
    [HideInInspector] public GemType type;

    static private Gem _prevSelected;
    private bool _isSelected;

    private bool _matchFound;

    private Vector2[] _adjacentDirs = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    public void Start()
    {

    }
    public void Update()
    {
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (transform.GetComponent<SpriteRenderer>().sprite == null || type == GemType.NONE || gameInstance._shifting)
            return;

        if (_isSelected)
            Deselect();
        else
        {
            if (_prevSelected == null)
                Select();
            else
            {
                if (GetAllAdjacentGems().Contains(_prevSelected.gameObject))
                {
                    SwapSprite(_prevSelected);
                    _prevSelected.ClearAllMatches();
                    ClearAllMatches();
                    _prevSelected.Deselect();
                }
                else
                {
                    _prevSelected.Deselect();
                    Select();
                }
            }
        }
    }

    private void Select()
    {
        transform.GetComponent<SpriteRenderer>().color = Color.gray;
        _isSelected = true;
        _prevSelected = transform.GetComponent<Gem>();
    }
    private void Deselect()
    {
        transform.GetComponent<SpriteRenderer>().color = Color.white;
        _isSelected = false;
        _prevSelected = null;
    }

    private void SwapSprite(Gem other)
    {
        if (type == other.Type)
            return;

        GemType tmpType = other.Type;
        other.Type = type;
        type = tmpType;

        transform.GetComponent<SpriteRenderer>().sprite = gameInstance.sprites[(int)type];
        other.GetComponent<SpriteRenderer>().sprite = gameInstance.sprites[(int)other.Type];
    }

    private GameObject GetAdjacent(Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir);
        if (hit.collider != null)
        {
            Vector2 len = new Vector2(hit.point.x, hit.point.y);
            Debug.DrawRay(transform.position, len, Color.magenta, 3.0f);
            Debug.Log(hit.collider.gameObject);
            return hit.collider.gameObject;
        }
        return null;
    }
    private List<GameObject> GetAllAdjacentGems()
    {
        List<GameObject> adjacent = new List<GameObject>();
        for (int i = 0; i < _adjacentDirs.Length; i++)
        {
            adjacent.Add(GetAdjacent(_adjacentDirs[i]));
        }
        return adjacent;
    }

    private List<GameObject> FindMatch(Vector2 dir)
    {
        List<GameObject> matching = new List<GameObject>();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir);
        while (hit.collider != null && hit.collider.GetComponent<Gem>().type == type)
        {
            matching.Add(hit.collider.gameObject);
            hit = Physics2D.Raycast(hit.collider.transform.position, dir);
        }
        return matching;
    }
    private void ClearMatch(Vector2[] paths)
    {
        List<GameObject> matching = new List<GameObject>();
        for (int i = 0; i < paths.Length; i++)
        {
            matching.AddRange(FindMatch(paths[i]));
        }
        if (matching.Count >= 2)
        {
            for (int i = 0; i < matching.Count; i++)
            {
                matching[i].GetComponent<Gem>().Type = GemType.NONE;
                matching[i].GetComponent<SpriteRenderer>().sprite = null;
            }
            _matchFound = true;
        }
    }
    public void ClearAllMatches()
    {
        if (transform.GetComponent<SpriteRenderer>().sprite == null || type == GemType.NONE)
            return;

        ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
        ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });
        if (_matchFound)
        {
            transform.GetComponent<SpriteRenderer>().sprite = null;
            type = GemType.NONE;
            StopCoroutine(gameInstance.FindNullTiles());
            StartCoroutine(gameInstance.FindNullTiles());
            _matchFound = false;
        }
        else
        {
            if (_prevSelected != null)
                SwapSprite(_prevSelected);
        }
    }

    // Setters and Getters
    public GemType Type
    {
        get
        {
            return type;
        }
        set
        {
            type = value;
        }
    }
    public int IType
    {
        get
        {
            return (int)type;
        }
    }
}
