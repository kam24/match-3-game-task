using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridLayers : MonoBehaviour, ILayerButton
{
    [SerializeField] private TilemapRenderer _layerPrefab;
    [SerializeField] private TilemapRenderer _layerWithOffsetPrefab;
    private int offset = 5;
    public void AddLayer()
    {
        List<TilemapRenderer> layers = GetComponentsInChildren<TilemapRenderer>().ToList();
        layers.Sort((x,y)=> x.sortingOrder.CompareTo(y.sortingOrder));
        int gridCount = layers.Count();
        TilemapRenderer newGrid;
        if (gridCount % 2 == 0)
            newGrid = Instantiate(_layerWithOffsetPrefab, gameObject.transform);
        else
            newGrid = Instantiate(_layerPrefab, gameObject.transform);
        newGrid.transform.position += Vector3.back * (layers.Last().sortingOrder + offset);
        newGrid.gameObject.name = $"LAYER {gridCount}";
        newGrid.sortingOrder = layers.Last().sortingOrder + offset;
    }

    public void RemoveLayer()
    {
        var lastGrid = GetComponentsInChildren<Grid>().LastOrDefault();
        if (lastGrid != null)
            DestroyImmediate(lastGrid.gameObject);
        else
            Debug.LogWarning("No more layers");
    }
}

