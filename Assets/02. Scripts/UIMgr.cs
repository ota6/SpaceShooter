using UnityEngine;
using System.Collections;

public class UIMgr : MonoBehaviour {

    //AssetBundleをダウンロードするアドレス
    string url = "http://www.Unity3dStudy.com/data/Spaceshooter.unity3d";
    //AssetBundleのバージョン
    int version = 1;

    //Webにアクセスするための変数
    WWW www;

    IEnumerator Start () 
    {
        //指定されたurlアドレスにアクセスして、関連ファイルをダウンロードする
        www = WWW.LoadFromCacheOrDownload(url, version);

        yield return www;

        //エラーが発生したら、メッセージを出力
        if ( !string.IsNullOrEmpty(www.error) )
        {
            Debug.Log (www.error.ToString());
        }else{
            //ダウンロードしたAssetBundleをメモリにロード
            AssetBundle assetBundle = www.assetBundle;
        }
    }
    
    void OnGUI()
    {
        //AssetBundleをすべてダウンロードして取得すると、 GUIボタンを作成
        if (www.isDone && GUI.Button ( new Rect(20,50,100,30), "Start Game") )
        {
            LoadScene();
        }

        //ダウンロードする進行状況を表示
        GUI.Label ( new Rect(20, 20, 200, 30)
                   , "Downloading ..." + (www.progress * 100.0f).ToString() +"%");
    }

    void LoadScene()
    {
        Application.LoadLevel("scLevel01");
        Application.LoadLevelAdditive("scPlay");
    }
}
