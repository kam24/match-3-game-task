using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TilesRow : MonoBehaviour, IAddTileButton
{
    [SerializeField] private Image _tilePrefab;

    private List<TileInRow> _tiles;
    private List<Transform> _tilesRowView;
    private bool _isAllTilesWithdrawn = false;

    public event Action AllTilesDestroyed;
    public event Action BecameNotAvailableForNewTiles;

    public TileInRow TryAdd(TileData tile)
    {
        if (_tiles.Count + 1 <= _tilesRowView.Count)
        {
            var newTileInRow = new TileInRow(tile) { isTaken = true };

            TileInRow sameTile = _tiles.LastOrDefault(tile => tile.ID == newTileInRow.ID);
            if (sameTile != null)
            {
                int vacancyIndex = _tiles.IndexOf(sameTile) + 1;
                newTileInRow.Transform = GetTile(vacancyIndex);
                InsertInList(newTileInRow, vacancyIndex);
            }
            else
            {
                newTileInRow.Transform = _tilesRowView[_tiles.Count];
                AddToList(newTileInRow);
            }

            return newTileInRow;
        }
        return null;
    }

    public void Remove(IReadOnlyCollection<TileInRow> removableTiles)
    {
        int index = _tiles.IndexOf(removableTiles.First());
        int count = removableTiles.Count;
        int shiftingTilesCount = _tiles.Count - (index + count);
        if (shiftingTilesCount > 0)
        {
            List<TileInRow> shiftingTiles = _tiles.GetRange(index + count, shiftingTilesCount);
            _tiles.RemoveRange(index, count);
            shiftingTiles.ForEach(tile => _tiles.Remove(tile));
            _tiles.InsertRange(index, shiftingTiles);
            ShiftTiles(shiftingTiles.FindAll(tile => tile.IsDestroying == false));
        }
        else
        {
            _tiles.RemoveRange(index, count);
        }

    }

    public void OnAllTilesWithdrawn()
    {
        _isAllTilesWithdrawn = true;
    }

    public void AddTile()
    {
        var tile = Instantiate(_tilePrefab, gameObject.transform);
        _tilesRowView.Add(tile.transform);
        //ShiftTiles(_tiles);
    }

    private void OnTilePuttedInRow(uint id)
    {
        List<TileInRow> destroyableTiles = _tiles.FindAll(tile => tile.ID == id && tile.IsInPlace);
        int index = _tiles.IndexOf(destroyableTiles[0]);
        if (destroyableTiles.Count() == 3)
        {
            var sequence = new DestroyableSequence(destroyableTiles);
            sequence.TilesDestroyed += Remove;
            if (_isAllTilesWithdrawn && AllTilesPutInRow())
            {
                int i = 0;
                sequence.TilesDestroyed += OnLastTilesDestroyed;
                destroyableTiles.ForEach(tile =>
                {
                    tile.DestroyAsLast(i++, GetTile(index).position);
                    tile.Destroyed += sequence.OnDestroy;
                });
            }
            else
            {
                destroyableTiles.ForEach(tile =>
                {
                    tile.Destroy();
                    tile.Destroyed += sequence.OnDestroy;
                });
            }
        }
        else if (_tiles.FindAll(tile => tile.IsInPlace && tile.IsDestroying == false).Count == _tilesRowView.Count)
        {
            BecameNotAvailableForNewTiles?.Invoke();
        }
    }

    private void OnLastTilesDestroyed(IReadOnlyCollection<TileInRow> obj)
    {
        AllTilesDestroyed?.Invoke();
    }

    private bool AllTilesPutInRow()
    {
        int count = _tiles.FindAll(tile => tile.IsInPlace).Count();
        return count == _tiles.Count();
    }

    private void Awake()
    {
        _tiles = new List<TileInRow>(transform.childCount);
        _tilesRowView = transform.Cast<Transform>().ToList();
        _tilesRowView.Sort((x, y) => x.position.x.CompareTo(y.position.x));
    }

    private Transform GetTile(int index)
    {
        return _tilesRowView[index];
    }

    private void AddToList(TileInRow newTile)
    {
        _tiles.Add(newTile);
        newTile.PuttedInRow += OnTilePuttedInRow;
    }

    private void InsertInList(TileInRow newTile, int index)
    {
        _tiles.Insert(index, newTile);
        newTile.PuttedInRow += OnTilePuttedInRow;
        ShiftTiles(_tiles.GetRange(index + 1, _tiles.Count - index - 1));
    }

    private void ShiftTiles(List<TileInRow> tiles)
    {
        tiles.ForEach(tile =>
        {
            Transform tileTransform = GetTile(_tiles.IndexOf(tile));
            tile.Shift(tileTransform);
        });
    }

}

public class DestroyableSequence
{
    public IReadOnlyCollection<TileInRow> Tiles { get; private set; }

    private int _destroyedTiles;

    public event Action<IReadOnlyCollection<TileInRow>> TilesDestroyed;

    public DestroyableSequence(List<TileInRow> tiles)
    {
        Tiles = tiles;
    }

    public void OnDestroy()
    {
        _destroyedTiles++;
        if (_destroyedTiles == Tiles.Count)
            TilesDestroyed?.Invoke(Tiles);
    }
}