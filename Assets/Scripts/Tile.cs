using Assets.Scripts;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Sequence = DG.Tweening.Sequence;

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class Tile : MonoBehaviour, IApplyButton
{
    [SerializeField] private TileData _tileData;
    [SerializeField] private SpriteRenderer _bleach;
    [SerializeField] private Color _hiddenColor;
    [SerializeField] private int _layer = -1;

    public int Layer => _layer;
    public TileData Data => _tileData;

    private SpriteRenderer _spriteRenderer;

    private BoxCollider2D _collider;
    private Bounds _bounds;
    private float _boxCastSizePercent = 0.9f;

    private bool _isActive = true;

    private float _jumpPower = 2;
    private float _moveDuration = 0.5f;
    private float _jumpDuration = 1f;

    public event Action<Tile> Clicked;
    public event Action Destroyed;

    public void SetData(TileData data)
    {
        _tileData = data;
        OnValidate();
    }

    public void SetLayer(int layer)
    {
        if (layer >= 0)
            _layer = layer;
    }

    public void TryActivate()
    {
        if (!Utilities.HasUpperTile(_bounds, _boxCastSizePercent, Layer))
        {
            _isActive = true;
            _spriteRenderer.color = Color.white;
        }
    }

    public void Deactivate()
    {
        _isActive = false;
        _spriteRenderer.color = _hiddenColor;
    }

    public void Withdraw()
    {
        _collider.enabled = false;
        TryActivateLowerTiles();
    }

    public Sequence JumpTo(Vector2 position)
    {
        return transform.DOJump(position, _jumpPower, 1, _moveDuration).SetLink(gameObject);
    }

    public void Shift(Vector2 position)
    {
        transform.DOMove(position, _moveDuration).SetLink(gameObject);
    }

    public void Remove()
    {
        _bleach.DOFade(0.75f, 0.5f);
        transform.DOScale(transform.localScale * 1.25f, 0.5f)
            .SetLink(gameObject)
            .OnComplete(Destroy);
    }

    public void RemoveAsLast(int index, Vector2 firstTile)
    {
        if (index == 0)
            JumpAndRotate().OnComplete(Destroy);
        else
            MoveAsYoyo(firstTile).OnComplete(Destroy);
    }

    private void Destroy()
    {
        Destroy(gameObject);
        Destroyed?.Invoke();
    }

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _bounds = _collider.bounds;
        if (_layer < 0)
            SetLayer(GetComponentInParent<TilemapRenderer>().sortingOrder);
        OnValidate();
    }

    private void OnValidate()
    {
        GetComponent<SpriteRenderer>().sprite = _tileData != null ? _tileData.Icon : null;
    }

    private void OnMouseDown()
    {
        if (_isActive)
            Clicked?.Invoke(this);
    }

    private void Start()
    {
        DeactivateLowerTiles();
    }

    private void TryActivateLowerTiles()
    {
        var tiles = Utilities.FindLowerTiles(_bounds, _boxCastSizePercent, Layer);
        tiles.ForEach(tile => tile.TryActivate());
    }

    private void DeactivateLowerTiles()
    {
        var tiles = Utilities.FindLowerTiles(_bounds, _boxCastSizePercent, Layer);
        tiles.ForEach(tile => tile.Deactivate());
    }

    private Sequence JumpAndRotate()
    {
        transform.DORotate(new Vector3(0, 0, 360f), _jumpDuration / 2, RotateMode.FastBeyond360);
        return transform.DOJump(transform.position, _jumpPower, 1, _jumpDuration);
    }

    private Sequence MoveAsYoyo(Vector2 target)
    {
        _bleach.DOFade(0.3f, _jumpDuration / 2);
        transform.DOScaleX(transform.localScale.x * 1.25f, _jumpDuration / 2).SetLoops(2, LoopType.Yoyo);
        return DOTween.Sequence()
            .Append(transform.DOMoveX(transform.position.x + 0.5f, _jumpDuration / 2))
            .Append(transform.DOMove(target, _jumpDuration / 2));
    }

    public void Apply()
    {
        OnValidate();
    }
}
