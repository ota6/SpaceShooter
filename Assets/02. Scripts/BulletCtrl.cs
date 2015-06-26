using UnityEngine;
using System.Collections;

public class BulletCtrl : MonoBehaviour {
    //弾丸の破壊力
    public int damage = 20;
    //弾丸の発射速度
    public float speed  = 1000.0f;

    //発射原点
    public Vector3 firePos = Vector3.zero;

    void Start () {
        GetComponent<Rigidbody>().AddForce( transform.forward * speed);
        //弾丸が作成された位置がFirePosの位置と同じ
        firePos = transform.position;
    }
}
