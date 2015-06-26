using UnityEngine;
using System.Collections;

public class GameMgr : MonoBehaviour {
    //モンスターが出現する位置を保持する配列
    public Transform[] points;
    //モンスタープレハブを割り当てる変数
    public GameObject monsterPrefab;

    //モンスターを発生させる周期
    public float createTime = 2.0f;
    //モンスターの最大発生数
    public int maxMonster = 10;
    //ゲーム終了であるかを示す変数
    public bool isGameOver = false;

    //サウンドの音量設定変数
    public float sfxVolumn = 1.0f;
    //サウンドのミュート機能 
    public bool isSfxMute = false;

    void Start () {
        //HierarchyビューのSpawnPointを見つけるの下位にあるすべてのTransformコンポーネントを検索 
        points = GameObject.Find ("SpawnPoint").GetComponentsInChildren<Transform>();

        if (points.Length > 0){
            //モンスター作成コルーチン関数を呼びだす
            StartCoroutine( this.CreateMonster() );
        }
    }

    //モンスター作成コルーチン関数
    IEnumerator CreateMonster()
    {
        //ゲーム終了時まで無限ループ
        while ( !isGameOver )
        {
            //現在作成されたモンスター数を計算
            int monsterCount = (int) GameObject.FindGameObjectsWithTag("MONSTER").Length;

            //モンスターの最大作成数よりも小さい時にのみモンスター作成
            if (monsterCount < maxMonster)
            {
                //モンスターの作成サイクル時間だけ待機
                yield return new WaitForSeconds( createTime );

                //ランダムな位置を計算
                int idx = Random.Range(1, points.Length);
                //モンスターの動的作成
                Instantiate( monsterPrefab, points[idx].position, points[idx].rotation );
            }else{
                yield return null;
            }

        }
    }

    //サウンドのパブリック関数
    public void PlaySfx ( Vector3 pos, AudioClip sfx )
    {
        //ミュートオプションが設定されると、すぐにリターンする
        if (isSfxMute) return;

        //ゲームオブジェクトを動的に作成
        GameObject soundObj = new GameObject("Sfx");
        //サウンド発生位置を指定
        soundObj.transform.position = pos;

        //作成されたゲームオブジェクトにAudioSourceコンポーネントを追加
        AudioSource _audioSource = soundObj.AddComponent<AudioSource>();
        //AudioSource プロパティ設定
        _audioSource.clip = sfx;
        _audioSource.minDistance = 10.0f;
        _audioSource.maxDistance = 30.0f;
        //sfxVolumn変数でゲーム全体の音量設定が可能
        _audioSource.volume = sfxVolumn;
        //サウンド実行
        _audioSource.Play();

        //サウンドの再生が終了すると、動的に作成したゲームオブジェクトを削除
        Destroy(soundObj, sfx.length);
    }

}
