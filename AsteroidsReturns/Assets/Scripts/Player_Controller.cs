using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    private Rigidbody2D _playerRb;
    [SerializeField]
    private float _playerSpeed = 2.5f;
    [SerializeField]
    private float _playerMaxSpeed = 5.0f;
    [SerializeField]
    private float _playerRotateSpeed = 5.0f;
    private SpriteRenderer _playerSpriteRenderer;
    [SerializeField]
    private float _playerFlashTimer = 0.002f;

    private Camera _mainCam;

    private float _horizontalInput;
    private float _verticalInput;

    private Vector3 _viewPosition;
    private Renderer[] _renderers;
    private bool _isWrappingX = false;
    private bool _isWrappingY = false;

    [SerializeField]
    private Transform _firePoint;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1f;

    private int _score = 0;
    private UI_Manager _uiManager;
    private Spawn_Manager _spawnManager;

    private int _lives = 3;

    [SerializeField]
    private AudioClip _bulletSoundClip;
    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _playerRb = GetComponent<Rigidbody2D>();
        _playerSpriteRenderer = GetComponent<SpriteRenderer>();
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<Spawn_Manager>();
        _mainCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        _renderers = GetComponentsInChildren<Renderer>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UI_Manager>();
        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL!");
        }
        if (_audioSource == null)
        {
            Debug.LogError("The AudioSource is NULL!");
        }
        else
        {
            _audioSource.clip = _bulletSoundClip; 
        }
        if (_spawnManager == null)
        {
            Debug.Log("Spawn Manager is NULL - Asteroid.cs");
        }
    }

    void Update()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
        ScreenWrapping();
        //Disable reverse input
        if(_verticalInput < 0)
        {
            _verticalInput = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            _spawnManager.StartSpawning();
            _uiManager.StartGame();
            FireBullet();
            _audioSource.Play();
        }
    }

    private void FixedUpdate()
    {
        transform.Rotate(0, 0, -_horizontalInput * _playerRotateSpeed);
        _playerRb.AddForce(transform.up * _playerSpeed * _verticalInput);
        if (_playerRb.velocity.magnitude > _playerMaxSpeed)
        {
            _playerRb.velocity = _playerRb.velocity.normalized * _playerMaxSpeed;
        }
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

    private void FireBullet()
    {
        _canFire = Time.time + _fireRate;
        GameObject bullet = Game_Controller.SharedInstance.GetPooledObject("Bullet");
        if (bullet != null)
        {
            bullet.transform.position = _firePoint.transform.position;
            bullet.transform.rotation = _firePoint.transform.rotation;
            bullet.SetActive(true);
        }
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

    public void Damage()
    {
        StartCoroutine(PlayerHitFlash());
        //Equal to _lives = _lives - 1;
        _lives -= 1;
        _uiManager.UpdateLives(_lives);
        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    private IEnumerator PlayerHitFlash()
    {
        _playerSpriteRenderer.enabled = false;
        yield return new WaitForSeconds(_playerFlashTimer);
        _playerSpriteRenderer.enabled = true;
        yield return new WaitForSeconds(_playerFlashTimer);
        _playerSpriteRenderer.enabled = false;
        yield return new WaitForSeconds(_playerFlashTimer);
        _playerSpriteRenderer.enabled = true;
        yield return new WaitForSeconds(_playerFlashTimer);
    }
}
