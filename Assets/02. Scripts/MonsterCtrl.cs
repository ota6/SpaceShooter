using UnityEngine;
using System.Collections;

public class MonsterCtrl : MonoBehaviour {
    //モンスターの状態情報を列挙するEnumerable変数の宣言
    public enum MonsterState { idle, trace, attack, die };
    //モンスターの現在の状態情報を格納するEnum変数
    public MonsterState monsterState = MonsterState.idle;

    //速度向上のために様々なコンポネントを変数に割り当てる
    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent nvAgent;
    private Animator animator;

    //追跡射程距離
    public float traceDist = 10.0f;
    //攻撃射程距離
    public float attackDist = 2.0f;

    //モンスターは死亡したか
    private bool isDie = false;

    //血痕効果のプレハブ
    public GameObject bloodEffect;
    //血痕デカール効果のプレハブ
    public GameObject bloodDecal;
    //
    private int hp = 100;

    void Start () 
    {
        //モンスターのTransformを割り当てる
        monsterTr = this.gameObject.GetComponent<Transform>();
        //追跡対象であるPlayerのTransformを割り当てる
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        //NavMeshAgentコンポネントを割り当てる
        nvAgent = this.gameObject.GetComponent<NavMeshAgent>();
        //Animatorコンポネント割り当て
        animator = this.gameObject.GetComponent<Animator>();

        //一定の間隔でモンスターの行動状態をチェックするコルーチン関数の実行
        StartCoroutine( this.CheckMonsterState() );

        //モンスターの状態に応じて動作するルーチンを実行するコルーチン関数の実行
        StartCoroutine( this.MonsterAction() );
    }


    /*
        一定の間隔でモンスターの行動状態をチェックしてmonsterState値を変更
    */
    IEnumerator CheckMonsterState()
    {
        while(!isDie){
            yield return new WaitForSeconds(0.2f);

            //モンスターとプレーヤーとの間の距離を測定
            float dist = Vector3.Distance(playerTr.position , monsterTr.position);


            if (dist <= attackDist) //攻撃距離範囲内に入っているかを確認
            {
                monsterState = MonsterState.attack;
            }
            else if (dist <= traceDist) //追跡距離範囲内に入っているかを確認
            {
                monsterState = MonsterState.trace; //モンスターの状態を追跡モードに設定
            }
            else
            {
                monsterState = MonsterState.idle; //モンスターの状態をidleモードに設定
            }
        }
    }

    /*
        モンスターの状態値に応じて、適切なアクションを実行する関数
    */
    IEnumerator MonsterAction()
    {
        while (!isDie)
        {
            switch ( monsterState ){
                //idle状態
                case MonsterState.idle:
                    //追跡停止
                    nvAgent.Stop ();

                    //AnimatorのIsAttack変数値をfalseに設定する
                    animator.SetBool("IsAttack", false);

                    //AnimatorのIsTrace変数値をfalseに設定
                    animator.SetBool("IsTrace", false);
                    break;

                //追跡状態
                case MonsterState.trace:
                    //追跡対象の位置を渡す
                    nvAgent.Resume();
                    nvAgent.destination = playerTr.position;

                    //AnimatorのIsAttack変数値をfalseに設定する
                    animator.SetBool("IsAttack", false);

                    //AnimatorのIsTrace変数値をtrueに設定
                    animator.SetBool("IsTrace", true);
                    break;

                //攻撃状態
                case MonsterState.attack:
                    //追跡停止
                    nvAgent.Stop ();
                    //IsAttackをtrueに設定してattackStateに転移
                    animator.SetBool ("IsAttack", true);
                    nvAgent.Resume();
                    break;
            }
            yield return null;
        }
    }

    void Update ()
    {
        //現在実行中のアニメーションのStateがgothitかどうかをチェック
        if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash("Base Layer.gothit"))
        {
            animator.SetBool("IsHit", false);
        }
        //現在実行中のアニメーションStateがfallであるかをチェック
        if ( animator.GetCurrentAnimatorStateInfo(0).fullPathHash== Animator.StringToHash("Base Layer.fall") )
        {
            animator.SetBool("IsPlayerDie", false);
        }
        //現在実行中のアニメーションStateがdieであるかをチェック
        if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash == Animator.StringToHash("Base Layer.die"))
        {
            animator.SetBool("IsDie", false);
        }
    }

    //Bulletとの衝突チェック
    void OnCollisionEnter ( Collision coll )
    {
        if ( coll.gameObject.tag == "BULLET" )
        {
            //血痕エフェクトのコルーチン関数を呼びだす
            StartCoroutine ( this.CreateBloodEffect( coll.transform.position ) );

            //当たった弾丸のDamageを抽出して、モンスターhpを減少させる
            hp -= coll.gameObject.GetComponent<BulletCtrl>().damage;
            if ( hp <= 0 )
            {
                MonsterDie();
            }

            //Bullet 削除
            Destroy( coll.gameObject );

            //IsHitをtrueに変更すると、Any Stateからgothitに転移される
            animator.SetBool ("IsHit", true );
        }
    }

    //モンスターが Rayに当たった時に呼び出される関数
    void OnDamage( object[] _params )
    {
        //Debug.Log ( string.Format("Hit ray {0} : {1}",  _params[0], _params[1]) );
		//血痕エフェクトのコルーチン関数呼び出し。当たった位置は _params[0]を使用する。
		StartCoroutine ( this.CreateBloodEffect( (Vector3)_params[0] ) );
		
		//当たった弾丸の Damageは _params[1]を hpから減算する
		hp -= (int)_params[1];
		if ( hp <= 0 )
		{
			MonsterDie();
		}

		//IsHitを trueにすると、Any Stateから gothitに移る
		animator.SetBool ("IsHit", true );
    }

    //モンスター死亡時の処理ルーチン
    void MonsterDie()
    {
        //すべてのコルーチンを停止させる
        StopAllCoroutines();
        
        isDie = true;
        monsterState = MonsterState.die;
        nvAgent.Stop();
        animator.SetBool("IsDie", true);

        //モンスターに追加されたColliderを無効にする
        gameObject.GetComponentInChildren<CapsuleCollider>().enabled = false;

        foreach(Collider coll in gameObject.GetComponentsInChildren<SphereCollider>())
        {
            coll.enabled = false;
        }
    }

    IEnumerator CreateBloodEffect( Vector3 pos)
    {
        //血痕エフェクト作成
        GameObject _blood1 = (GameObject) Instantiate ( bloodEffect, pos, Quaternion.identity );
        Destroy ( _blood1, 2.0f );

        //デカール作成位置 - 床から少し上に上げた位置を計算
        Vector3 decalPos = monsterTr.position + (Vector3.up * 0.01f);
        //デカールの回転値をランダムに設定
        Quaternion decalRot = Quaternion.Euler ( 0, Random.Range(0,360), 0);
        //デカールプレハブ作成
        GameObject _blood2 = (GameObject) Instantiate ( bloodDecal, decalPos, decalRot );
        //デカールの大きさも不規則にスケール調整
        float _scale = Random.Range(1.5f, 3.5f);
        _blood2.transform.localScale = new Vector3(_scale, 1, _scale);

        Destroy ( _blood2, 5.0f );
        
        yield return null;
    }

    void OnEnable()
    {
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;
    }

    void OnDisable()
    {
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }


    //プレーヤーが死亡した時に実行される関数
    void OnPlayerDie()
    {
        //モンスターの状態をチェックするコルーチン関数をすべて停止させる
        StopAllCoroutines();
        //追跡を停止してアニメーションを実行
        nvAgent.Stop();
        animator.SetBool ("IsPlayerDie", true);
    }


}

