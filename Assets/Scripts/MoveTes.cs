using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTes : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 2.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // aキーで左旋回 
        if (Input.GetKey("a")) 
        { 
            rb.angularVelocity = -Vector3.up * Mathf.PI/2;
        } 
        // sキーで右旋回 
        else if (Input.GetKey("s")) 
        { 
            rb.angularVelocity = Vector3.up * Mathf.PI/2;
        } 
        else 
        {
            rb.angularVelocity = Vector3.zero; // 回転をリセット
        }

        // 前後左右の移動ベクトル
        Vector3 moveDirection = Vector3.zero;

        // upキーで前進
        if (Input.GetKey("up")) 
        { 
            moveDirection += transform.forward * speed; 
        } 
        // downキーで後退
        if (Input.GetKey("down")) 
        { 
            moveDirection -= transform.forward * speed; 
        } 
        // rightキーで右に進む 
        if (Input.GetKey("right")) 
        { 
            moveDirection += transform.right * speed; 
        } 
        // leftキーで左に進む 
        if (Input.GetKey("left")) 
        { 
            moveDirection -= transform.right * speed; 
        }

        // オブジェクトに移動速度を設定
        rb.velocity = moveDirection;
    }
}
