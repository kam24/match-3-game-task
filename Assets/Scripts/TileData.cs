using UnityEngine;

[CreateAssetMenu(menuName = "Tile")]
public class TileData : ScriptableObject
{
    [SerializeField] private uint _id;
    [SerializeField] private Sprite _icon;

    public uint ID => _id;
    public Sprite Icon => _icon;
}
