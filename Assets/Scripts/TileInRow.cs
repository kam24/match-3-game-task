using System;
using UnityEngine;

public class TileInRow : IDisposable
{
    public uint ID { get; private set; }
    public bool IsInPlace { get; private set; }
    public bool IsDestroying { get; private set; }

    public Transform Transform;
    public bool isTaken;

    public event Action<Vector2> Shifted;
    public event Action<uint> PuttedInRow;
    public event Action Destroying;
    public event Action<int, Vector2> DestroyingAsLast;
    public event Action Destroyed;

    public TileInRow(TileData tile)
    {
        ID = tile.ID;
        isTaken = false;
        IsInPlace = false;
        IsDestroying = false;
    }

    public void Shift(Transform transform)
    {
        Transform = transform;
        Shifted?.Invoke(transform.position);
    }

    public void Destroy()
    {
        IsDestroying = true;
        Destroying?.Invoke();
    }

    public void DestroyAsLast(int index, Vector3 firstTile)
    {
        IsDestroying = true;
        DestroyingAsLast?.Invoke(index, firstTile);
    }

    public void Dispose()
    {
        Destroyed?.Invoke();
        Shifted = null;
        PuttedInRow = null;
        Destroying = null;
        Destroyed = null;
    }

    public void PutInRow()
    {
        IsInPlace = true;
        PuttedInRow?.Invoke(ID);
    }
}

