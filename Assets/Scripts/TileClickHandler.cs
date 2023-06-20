using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TileClickHandler : MonoBehaviour
{
    private List<Tile> tiles;
    private TilesRow _tilesRow;
    private int _tilesCount;

    public event Action AllTilesWithdrawn;

    public void Init(TilesRow tilesRow)
    {
        _tilesRow = tilesRow;
        tiles = GetComponentsInChildren<Tile>().ToList<Tile>();
        _tilesCount = tiles.Count;
        GetComponentsInChildren<Transform>().ToList().ForEach(t => t.AddComponent<RectTransform>());
        enabled = true;
    }

    private void OnEnable()
    {
        tiles.ForEach(tile => tile.Clicked += OnTileClicked);
    }

    private void OnDisable()
    {
        tiles.ForEach(tile => tile.Clicked -= OnTileClicked);
    }

    private void OnTileClicked(Tile tile)
    {
        TileInRow newTileInRow = _tilesRow.TryAdd(tile.Data);
        if (newTileInRow != null)
        {
            tile.Withdraw();
            tile.JumpTo(newTileInRow.Transform.position)
                .OnComplete(newTileInRow.PutInRow);
            tile.transform.SetParent(newTileInRow.Transform);
            newTileInRow.Shifted += tile.Shift;
            newTileInRow.Destroying += tile.Remove;
            newTileInRow.DestroyingAsLast += tile.RemoveAsLast;
            tile.Destroyed += newTileInRow.Dispose;
            _tilesCount--;
        }

        if (_tilesCount == 0)
            AllTilesWithdrawn?.Invoke();
    }
}

