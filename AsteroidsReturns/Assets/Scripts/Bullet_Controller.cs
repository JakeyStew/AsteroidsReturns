using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Controller : MonoBehaviour
{
    [SerializeField]
    private float _bulletSpeed = 10.0f;
    private Rigidbody2D _bulletRb;

    private Camera _mainCam;

    private Vector3 _viewPosition;
    private Renderer[] _renderers;
    private bool _isWrappingX = false;
    private bool _isWrappingY = false;

    private float _delayTime = 5.0f;

    [SerializeField]
    private GameObject _particleEffect;

    private void Start()
    {
        _bulletRb = GetComponent<Rigidbody2D>();
        _mainCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        _renderers = GetComponentsInChildren<Renderer>();
    }

    private void Update()
    {
        ScreenWrapping();
        StartCoroutine(WaitToDeativateBullet());
    }

    private void FixedUpdate()
    {
        _bulletRb.velocity = transform.up * _bulletSpeed;
    }

    bool CheckRenderers()
    {
        foreach (var renderer in _renderers)
        {
            if (renderer.isVisible)
            {
                return true;
            }
        }
        return false;
    }

    private void ScreenWrapping()
    {
        var isVisible = CheckRenderers();

        if (isVisible)
        {
            _isWrappingX = false;
            _isWrappingY = false;
            return;
        }

        if (_isWrappingX && _isWrappingY)
        {
            return;
        }

        _viewPosition = _mainCam.WorldToViewportPoint(transform.position);
        var newPosition = transform.position;

        if (!_isWrappingX && (_viewPosition.x > 1 || _viewPosition.x < 0))
        {
            newPosition.x = -newPosition.x;

            _isWrappingX = true;
        }

        if (!_isWrappingY && (_viewPosition.y > 1 || _viewPosition.y < 0))
        {
            newPosition.y = -newPosition.y;

            _isWrappingY = true;
        }
        transform.position = newPosition;
    }

    IEnumerator WaitToDeativateBullet()
    {
        yield return new WaitForSeconds(_delayTime);
        gameObject.SetActive(false);
        GameObject Effect = Instantiate(_particleEffect, transform.position, Quaternion.identity);
        Destroy(Effect, 0.5f);
    }
}
