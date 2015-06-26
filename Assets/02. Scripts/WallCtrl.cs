using UnityEngine;
using System.Collections;

public class WallCtrl : MonoBehaviour {
    //スパーク・パーティクル・プレハブを接続するパラメータ
    public GameObject sparkEffect;

    //衝突が始まる時に発生するイベント
    void OnCollisionEnter ( Collision coll)
    {
        //衝突したゲームオブジェクトのタグ値の比較
        if (coll.collider.tag == "BULLET")
        {
            //衝突した弾丸に保存された発射原点を抽出
            Vector3 firePos = coll.gameObject.GetComponent<BulletCtrl>().firePos;
            //入射角の逆ベクトル = 発射原点 - 衝突点
			Vector3 relativePos = firePos - coll.transform.position;

            //スパーク・パーティクルを動的に作成 
            Object obj = Instantiate ( sparkEffect, coll.transform.position, Quaternion.LookRotation(relativePos));
            //Destroy ( obj , 3f);

            //衝突したゲームオブジェクトを削除
            Destroy ( coll.gameObject );
        }
    }
}

