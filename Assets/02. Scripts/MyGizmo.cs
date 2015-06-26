using UnityEngine;
using System.Collections;

public class MyGizmo : MonoBehaviour {
    public Color _color  = Color.yellow;
    public float _radius = 0.1f;

    void OnDrawGizmos (){
        //ギズモのカラー設定
        Gizmos.color = _color;
        //具体的な形のギズモを作成する。引数は(作成位置, 半径)
        Gizmos.DrawSphere ( transform.position, _radius );
    }
} 
