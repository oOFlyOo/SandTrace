using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FootPrint : MonoBehaviour
{
    private const string FootPrintShaderName = "WWFramework/FootPrintGenerator";
    private enum FootPrintPass
    {
        Generator = 0,
        Clear = 1,
    }

    /// <summary>
    /// 脚印法线贴图，A通道是影响范围，0为不影响
    /// </summary>
    [SerializeField] private Texture _footPrintBump;
    [SerializeField] private float _footPrintBumpScale = 8; 
    
    [SerializeField] private float _footPrintSize = 1;
    [SerializeField] private int _worldSize = 20;
    [SerializeField] private int _textureSize = 256;

    [SerializeField] private float _footPrintAtten = 0.1f;
    [SerializeField] private float _duration = 0.1f;
    private float _curDuration = 0;

    /// <summary>
    /// 上一帧的结果
    /// RGB通道是法线
    /// </summary>
    [SerializeField] private RenderTexture _srcRenderTexture;
    [SerializeField] private RenderTexture _dstRenderTexture;
    
    private Material _footPrintMaterial;
    private Transform _cacheTransform;
    private Vector3 _lastWorldPosition;

    private void OnEnable()
    {
        _footPrintMaterial = new Material(Shader.Find(FootPrintShaderName));

        _srcRenderTexture = RenderTexture.GetTemporary(_textureSize, _textureSize, 0, RenderTextureFormat.ARGB32,
            RenderTextureReadWrite.Linear, 1);
        _srcRenderTexture.wrapMode = TextureWrapMode.Clamp;
        _srcRenderTexture.filterMode = FilterMode.Point;
        Graphics.Blit(null, _srcRenderTexture, _footPrintMaterial, (int)FootPrintPass.Clear);
        
        _dstRenderTexture = RenderTexture.GetTemporary(_srcRenderTexture.descriptor);
        Shader.SetGlobalTexture("_FootPrintTrace", _dstRenderTexture);
        
        Shader.SetGlobalTexture("_FootPrintBump", _footPrintBump);

        _cacheTransform = transform;
        _lastWorldPosition = transform.position;
        _curDuration = 0;
    }

    private void OnDisable()
    {
        RenderTexture.ReleaseTemporary(_srcRenderTexture);
        _srcRenderTexture = null;
        
        RenderTexture.ReleaseTemporary(_dstRenderTexture);
        _dstRenderTexture = null;
        
        _footPrintMaterial = null;
    }

    private void LateUpdate()
    {
        if (_footPrintMaterial == null)
        {
            return;
        }

        _curDuration += Time.deltaTime;
        if (_curDuration < _duration)
        {
            return;
        }
        var curPos = _cacheTransform.position;

        Shader.SetGlobalFloat("_FootPrintBumpScale", _footPrintBumpScale);
        Shader.SetGlobalFloat("_FootPrintSize", _footPrintSize);
        Shader.SetGlobalInt("_WorldSize", _worldSize);
        Shader.SetGlobalFloat("_FootPrintAtten", _footPrintAtten * _curDuration);
        Shader.SetGlobalVector("_WorldPosition", curPos);
        Shader.SetGlobalVector("_DeltaWorldPosition", _lastWorldPosition - curPos);
        Shader.SetGlobalTexture("_FootPrintTex", _dstRenderTexture);

        Graphics.Blit(_srcRenderTexture, _dstRenderTexture, _footPrintMaterial, (int)FootPrintPass.Generator);
        Graphics.Blit(_dstRenderTexture, _srcRenderTexture);
        
        _curDuration = 0;
        _lastWorldPosition = curPos;
    }
}