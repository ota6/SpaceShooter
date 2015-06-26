using UnityEngine;
using System.Collections;
using UnityEngine.UI; // ←※UIはこれを忘れずに入れる

public class PlayerCtrl : MonoBehaviour {
    Slider _slider;
    //クラスを Inspectorビューに表示するためには、
    //System.Serializableというアトリビュート(Attribute)を明示する必要がある
    [System.Serializable]
    

    //アニメーションクリップを保存するクラス
    public class Anim {
        public AnimationClip idle;
        public AnimationClip runForward;
        public AnimationClip runBackward;
        public AnimationClip runRight;
        public AnimationClip runLeft;
    }

    //Inspectorビューに表示するアニメーションクラス変数
    public Anim anim;

    //下位にある3DモデルのAnimationコンポネントにアクセスするための変数
    public Animation aniBody;
    
    private float h = 0.0f;
    private float v = 0.0f;

    //頻繁に使用するコンポネントは必ず変数に代入してから使用する
    private Transform tr;
    //移動速度変数
    public float moveSpeed = 10.0f;
    //回転速度変数
    public float rotSpeed = 100.0f;

    //Playerの生命変数
    public float _hp = 1.0f;

    //デリゲートおよびイベント宣言
    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;

    //ゲームマネージャーにアクセスするための変数
    private GameMgr _gameMgr;

    void Start () {
		//
        _slider = GameObject.Find("Slider").GetComponent<Slider>();
		Cursor.visible = false;

        //スクリプトの初めにTransformコンポネントを割り当てる
        tr = GetComponent<Transform>();

        //GameMgrスクリプトを割り当てる
        _gameMgr = GameObject.Find ("GameManager").GetComponent<GameMgr>();

        //Animationコンポーネントのclipプロパティにidle Animation Clipを指定
        aniBody.clip = anim.idle;
        //指定されたAnimation Clipを実行
        aniBody.Play();
    }

    void Update () {
        _slider.value = _hp;
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        /*
        Debug.Log ("H = " + h.ToString());
        Debug.Log ("V = " + v.ToString());
        */

        //前後左右の移動方向ベクトルを計算する
        Vector3 moveDir = (Vector3.forward * v)  + (Vector3.right * h );

        //Transformクラスに定義された移動関連Translateメソッド
        //Translate（移動方向*Time.deltaTime*速度, 基準座標）
        tr.Translate( moveDir * Time.deltaTime * moveSpeed, Space.Self );

        //Vector3.up軸を中心にrotSpeedの速度で回転させる
        tr.Rotate ( Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X"), Space.Self);

        //キーボードからの入力値を基に動作するアニメーションを実行
        if (v >= 0.1f){
            //前進アニメーション
            aniBody.CrossFade(anim.runForward.name, 0.3f);
        }
        else if (v <= -0.1f){
            //後退アニメーション
            aniBody.CrossFade(anim.runBackward.name, 0.3f);
        }
        else if (h >= 0.1f){
            //右に移動するアニメーション
            aniBody.CrossFade(anim.runRight.name, 0.3f);
        }
        else if (h <= -0.1f){
            //左に移動するアニメーション
            aniBody.CrossFade(anim.runLeft.name, 0.3f);
        }
        else {
            //左に移動するアニメーション
            aniBody.CrossFade(anim.idle.name, 0.3f);
		}

    }

    //衝突したColliderのIsTriggerオプションがチェックされた時に発生
    void OnTriggerEnter ( Collider coll )
    {
        //衝突したColliderがモンスターのPUNCHだったら、 PlayerのHPを減少させる
        if ( coll.gameObject.tag == "PUNCH" )
        {
            _hp -= 0.1f;
            //Debug.Log ("Player HP = " + hp.ToString() );

            //Playerの命がゼロ以下だったら、死亡処理
            if ( _hp <= 0.0f )
            {
                //PlayerDie();

                //イベントを発生させる
                OnPlayerDie();
                //ゲームマネージャーのisGameOver変数値を変更してモンスターの出現を停止させる
                _gameMgr.isGameOver = true;
            }
        }
    }
     

    //Playerの死亡処理ルーチン
    void PlayerDie()
    {
        Debug.Log ("Player Die !!");

        //MONSTER Tagが付いているすべてのゲームオブジェクトを見つける
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER");

        //全てのモンスターのOnPlayerDie関数を順次に呼び出す
        foreach( GameObject monster in monsters )
        {
            monster.SendMessage ( "OnPlayerDie" , SendMessageOptions.DontRequireReceiver ); 
        }
    }

}
