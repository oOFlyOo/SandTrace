using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootTrigger : MonoBehaviour
{
    [SerializeField] private FootPrint.Foot _foot;
    [SerializeField] private FootPrint _footPrint;
    [SerializeField] private float _hitTerrainBias = 0.01f;

    private Transform _rootTrans;
    private Transform _cacheTrans;

    private bool _isHit;

    private void Start()
    {
        _rootTrans = _footPrint.transform;
        _cacheTrans = transform;
    }

    private void LateUpdate()
    {
        var deltaY = _cacheTrans.position.y - _rootTrans.position.y;

        if (!_isHit)
        {
            // 判断该帧是否踩在了地上
            _isHit = deltaY < _hitTerrainBias;
        }
        else
        {
            // 离开地板
            if (deltaY > _hitTerrainBias)
            {
                _isHit = false;
                _footPrint.FootPrintActive(transform.position, transform.eulerAngles.y + 90, _foot);
            }
        }
    }
    
    // void OnTriggerExit(Collider grounder)
    // {
    //     if (grounder.CompareTag("Terrain"))
    //     {
    //         _footPrint.FootPrintActive(transform.position, transform.eulerAngles.y + 90);
    //     }
    // }
}
