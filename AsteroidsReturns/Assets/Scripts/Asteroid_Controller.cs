using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid_Controller : MonoBehaviour
{
    private float _asteroidRotateSpeed = 10.0f;

    private float _delayTime = 10.0f;

    private Rigidbody2D _asteroidRb;
    private Player_Controller _player;

    private float _screenEdgeRight;
    private float _screenEdgeLeft;
    private float _screenEdgeTop;
    private float _screenEdgeBottom;

    [SerializeField]
    private AudioClip _audioClipExplosion;
    [SerializeField]
    private AudioClip _audioClipPlayerHit;

    [SerializeField]
    private GameObject _particleEffect;

    void Start()
    {
        _asteroidRb = GetComponent<Rigidbody2D>();
        _player = GameObject.Find("Player").GetComponent<Player_Controller>();
        if (_player == null)
        {
            Debug.Log("Player is NULL");
        }

        _screenEdgeRight = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 0, 0.0f)).x;
        _screenEdgeLeft = -Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 0, 0.0f)).x;

        _screenEdgeTop = -Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 0, 0.0f)).y;
        _screenEdgeBottom = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 0, 0.0f)).y;

        _asteroidRotateSpeed = Random.Range(-50.0f, 50.0f);

        PositionCheck();
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * _asteroidRotateSpeed * Time.deltaTime);
        StartCoroutine(WaitToDestroyAsteroid());
    }


    private void PositionCheck()
    {
        float XDir = 0.1f;
        float YDir = 0.1f;

        float RandomNegative = Random.Range(-1.0f, -7.5f);
        float RandomPositive = Random.Range(1.0f, 7.5f);

        float asteroidPosX = _asteroidRb.position.x;
        float asteroidPosY = _asteroidRb.position.y;
        
        //Right && Top/Bottom
        if (asteroidPosX > _screenEdgeRight)
        {
            XDir = RandomNegative;
            if(asteroidPosY > _screenEdgeTop)
            {
                YDir = RandomNegative;
            } 
            else if (asteroidPosY < _screenEdgeBottom)
            {
                YDir = RandomPositive;
            }
        } 
        //Left && Top/Bottom
        else if (asteroidPosX < _screenEdgeLeft)
        {
            XDir = RandomPositive;
            if (asteroidPosY > _screenEdgeTop)
            {
                YDir = RandomNegative;
            }
            else if (asteroidPosY < _screenEdgeBottom)
            {
                YDir = RandomPositive;
            }
        }
        //Top && Right/Left
        else if (asteroidPosY > _screenEdgeTop)
        {
            YDir = RandomNegative;
            if(asteroidPosX > _screenEdgeRight)
            {
                XDir = RandomNegative;
            } 
            else if(asteroidPosX < _screenEdgeLeft)
            {
                XDir = RandomPositive;
            }
        }
        //Bottom && Right/Left
        else if (asteroidPosY < _screenEdgeBottom)
        {
            YDir = RandomPositive;
            if (asteroidPosX > _screenEdgeRight)
            {
                XDir = RandomNegative;
            }
            else if (asteroidPosX < _screenEdgeLeft)
            {
                XDir = RandomPositive;
            }
        }

        _asteroidRb.velocity = new Vector2(XDir, YDir);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player_Controller player = other.GetComponent<Player_Controller>();
            if (player != null)
            {
                player.Damage();
            }
            AudioSource.PlayClipAtPoint(_audioClipPlayerHit, transform.position);
            GameObject Effect = Instantiate(_particleEffect, transform.position, Quaternion.identity);
            Destroy(Effect, 0.5f);
            Destroy(this.gameObject);
        }

        if (other.tag == "Bullet")
        {
            other.gameObject.SetActive(false);
            if (_player != null)
            {
                _player.AddScore(10);
            }
            AudioSource.PlayClipAtPoint(_audioClipExplosion, transform.position);
            GameObject Effect = Instantiate(_particleEffect, transform.position, Quaternion.identity);
            Destroy(Effect, 0.5f);
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject);
        }
    }

    IEnumerator WaitToDestroyAsteroid()
    {
        yield return new WaitForSeconds(_delayTime);
        Destroy(gameObject);
    }
}
