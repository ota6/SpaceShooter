using UnityEngine;
using System.Collections;

//必ず必要なコンポネントを明示して、そのコンポネントが削除されることを防止するAttribute
[RequireComponent(typeof(AudioSource))]
public class FireCtrl : MonoBehaviour {

    //弾丸プレハブ 
    public GameObject bullet;
    //弾丸発射座標
    public Transform firePos;
    //弾丸発射音 
    public AudioClip fireSfx;
    //MuzzleFlashのMeshRendererコンポネント接続変数
    public MeshRenderer _renderer;

    //ゲームマネージャーにアクセスするための変数
    private GameMgr _gameMgr;
    
    void Start (){
        //最初にMuzzleFlash MeshRendererを無効にする
        _renderer.enabled = false;

        //ゲームマネージャーを探す
        _gameMgr = GameObject.Find ("GameManager").GetComponent<GameMgr>();
    }

    void Update () {
        //Rayを視覚的に表示するために使用
        //Debug.DrawRay(firePos.position, firePos.forward * 10.0f, Color.green);

        //マウス左クリックした時にFire関数を呼びだす
        if ( Input.GetMouseButtonDown(0)){
            Fire();

            //Rayに当たったゲームオブジェクトの情報を受け取る変数
            RaycastHit hit;

            //Raycast関数でRayを発射してそれに当たったゲームオブジェクトがある時にはTrueを返す
            if ( Physics.Raycast ( firePos.position, firePos.forward, out hit, 10.0f) )
            {
                //Rayに当たったゲームオブジェクトのTag値を比較し、それがモンスターであるかをチェック
                if ( hit.collider.tag == "MONSTER")
                {
                    //SendMessageを通じて渡す引数を配列に入れる
                    object[] _params = new object[2];
                    _params[0] = hit.point;  //Rayに当たった正確な位置値 (Vector3)
                    _params[1] = 20;         //モンスターに与えるダメージ値

                    //モンスターにダメージを与える関数を呼びだす
                    hit.collider.gameObject.SendMessage("OnDamage"
                                                        , _params
                                                        , SendMessageOptions.DontRequireReceiver);
                }

                //Rayに当たったゲームオブジェクトがBarrelであるかを確認
				if ( hit.collider.tag == "BARREL")
				{
                    //ドラム缶に当たったRayの入射角を計算するために発射原点と光線が当たったポイントを渡す
					object[] _params = new object[2];
					_params[0] = firePos.position;
					_params[1] = hit.point;
					hit.collider.gameObject.SendMessage("OnDamage"
					                                    , _params
					                                    , SendMessageOptions.DontRequireReceiver);
				}
            }
        }   
    }

    void Fire(){
        //並列処理のためのコルーチン関数の呼び出し
        StartCoroutine( this.ShowMuzzleFlash() );

        //Raycast方式に変更されるので、弾丸を作成するルーチンはコメントアウト
        //StartCoroutine( this.CreateBullet() );

        StartCoroutine( this.PlaySfx(fireSfx) );
    }

    // MuzzleFlashの有効化・無効化を短時間の間に繰り返す
    IEnumerator ShowMuzzleFlash(){
        _renderer.enabled = true;

        //不規則な間隔の時間の間にDelayした後、MeshRendererを無効にする
        yield return new WaitForSeconds(Random.Range(0.01f, 0.2f));

        _renderer.enabled = false;
    }

    // コルーチン関数
    IEnumerator CreateBullet(){
        //Bulletプレハブを動的に作成
        Instantiate ( bullet, firePos.position, firePos.rotation);
        yield return null;
    }

    // 弾丸発射音をコルーチンで作成
    IEnumerator PlaySfx( AudioClip _clip ){
        //既存のサウンド発生関数はコメントアウト
        //audio.PlayOneShot(_clip, 0.9f);

        //パブリックのサウンド関数を呼びだす
        _gameMgr.PlaySfx( firePos.position , _clip);
        yield return null;
    }
}
