﻿using UnityEngine;

public class MazeObjects : MonoBehaviour, IInteractable
{
    public bool IsInteractable { get => InputManager.Instance.Action.Interact; set { } }
    public bool IsInRange
    {
        get => _isInRange; set
        {
            if (_isInRange != value)
            {
                _isInRange = value;
                OnRangeChanged?.Invoke(_isInRange);
                UIHandler.Instance.RegisterTarget(this);
            }
        }
    }

    [SerializeField] private Transform _targetSprite;
    [SerializeField] private Vector3 followOffset = new Vector3(0, 0.5f, 0);
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private bool _isFollowing = false;
    private bool _isInRange;

    public event System.Action<bool> OnRangeChanged;

    private void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void Update()
    {
        LoadInteractImage();

        if (_isFollowing && _playerTransform != null)
        {
            FollowPlayer();
        }

        if (IsInteractable && IsInRange && InputManager.Instance.Action.Interact)
        {
            DoInteract();
        }
    }
    protected void LoadInteractImage()
    {
        if (_targetSprite.gameObject.activeSelf && !IsInRange)
        {
            _targetSprite.gameObject.SetActive(false);
        }
        else if (!_targetSprite.gameObject.activeSelf && IsInRange)
        {
            _targetSprite.gameObject.SetActive(true);
        }
    }
    private void StartFollow()
    {
        _isFollowing = true;
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
            transform.GetComponent<Collider>().isTrigger = true;
        }
    }

    private void StopFollow()
    {
        _isFollowing = false;
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
            transform.GetComponent<Collider>().isTrigger = false;

        }
    }
    private void FollowPlayer()
    {
        transform.position = _playerTransform.position + followOffset;
    }
    public void DisableInteract() { }

    public void DoInteract()
    {
        if (!_isFollowing)
        {
            StartFollow();
        }
        else
        {
            StopFollow();
        }
    }
}
