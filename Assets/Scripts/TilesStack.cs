using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Tile))]
public class TilesStack : MonoBehaviour, IApplyButton
{
    [SerializeField] private Tile _tileTemplate;
    [SerializeField] private float _offset;
    [SerializeField] private Direction _direction;
    [SerializeField] private List<Tile> _otherTilesPrefabs;

    public enum Direction
    {
        Up, Down, Left, Right
    }

    public void Apply()
    {
        var transforms = transform.Cast<Transform>().ToList();
        transforms.ForEach(t =>
        {
            if (t.TryGetComponent<Tile>(out Tile tile))
                DestroyImmediate(tile.gameObject);
        });

        Vector3 lastTilePosition = transform.position;
        _otherTilesPrefabs.ForEach(tile =>
        {
            Vector3 position = lastTilePosition + GetDirection() * _offset + Vector3.forward * _offset;
            Tile newTile = (Tile)PrefabUtility.InstantiatePrefab(tile, transform);
            newTile.transform.position = position;
            lastTilePosition = newTile.transform.position;
        });
    }

    private void Awake()
    {
        var transforms = transform.Cast<Transform>();
        int count = transforms.Count();
        transform.GetComponent<Tile>().SetLayer(count);
        int layer = count - 1;
        transforms.ToList().ForEach(t =>
        {            
            if (t.TryGetComponent<Tile>(out Tile tile))
            {
                t.transform.SetParent(transform.parent);
                tile.SetLayer(layer--);
            }
        });
    }

    private Vector3 GetDirection()
    {
        return _direction switch
        {
            Direction.Up => Vector3.up,
            Direction.Down => Vector3.down,
            Direction.Left => Vector3.left,
            Direction.Right => Vector3.right,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

