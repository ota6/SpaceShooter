using UnityEngine;
using System.Collections;

public class LaserBeam : MonoBehaviour {
    private Transform tr;
    private LineRenderer _line;

    void Start () {
        tr =  GetComponent<Transform>();
        _line = GetComponent<LineRenderer>();

        //Line Rendererを無効にしてから開始
        _line.enabled = false;
        //Line Rendererの始点と終点の幅を設定
        _line.SetWidth ( 0.3f, 0.05f);
    }

    void Update () 
    {
        //光線を予め作成
        Ray ray = new Ray ( tr.position, tr.forward);

        //光線が目に見えるように設定する
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.blue);

        //ビームに衝突したゲームオブジェクトの情報を受け取る変数 
        RaycastHit hit;

        if ( Input.GetMouseButtonDown(0) )
        {
            //Line Rendererの最初の点の位置を設定する
            _line.SetPosition(0, ray.origin);

            //ある物体に光線が当たった時の位置をLine Rendererの終点に設定
            if (Physics.Raycast(ray, out hit, 100.0f)){
                _line.SetPosition(1, hit.point);
            }else{
                _line.SetPosition(1, ray.GetPoint(100.0f));
            }

            //光線を表示するコルーチン関数を呼びだす
            StartCoroutine(this.ShowLaserBeam());
        }
    }

    IEnumerator ShowLaserBeam()
    {
        _line.enabled = true;
        yield return new WaitForSeconds( Random.Range(0.01f, 0.2f) );
        _line.enabled = false;
    }
}
