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
        Move,
        Generator,
        Clear
    }

    public enum Foot
    {
        Left,
        Right
    }

    /// <summary>
    /// 脚印法线贴图，A通道是影响范围，0为不影响
    /// </summary>
    [SerializeField] private Texture _footPrintBump;
    // [SerializeField] private Texture _rightFootPrintBump;
    [SerializeField] private bool _isLeftFootPrintBump = true;
    [SerializeField] private float _footPrintBumpScale = 8; 
    
    [SerializeField] private float _footPrintSize = 1;
    [SerializeField] private int _worldSize = 20;
    [SerializeField] private int _textureSize = 256;
    [SerializeField] private bool _filterPoint;

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

    private float _rootScale;

    private bool _isUseScrTexture = true;

    private void OnEnable()
    {
        _footPrintMaterial = new Material(Shader.Find(FootPrintShaderName));

        _srcRenderTexture = RenderTexture.GetTemporary(_textureSize, _textureSize, 0, RenderTextureFormat.ARGB32,
            RenderTextureReadWrite.Linear, 1);
        _srcRenderTexture.wrapMode = TextureWrapMode.Clamp;
        if (_filterPoint)
        {
            _srcRenderTexture.filterMode = FilterMode.Point;
        }
        Graphics.Blit(null, _srcRenderTexture, _footPrintMaterial, (int)FootPrintPass.Clear);
        
        _dstRenderTexture = RenderTexture.GetTemporary(_srcRenderTexture.descriptor);
        _dstRenderTexture.wrapMode = TextureWrapMode.Clamp;
        if (_filterPoint)
        {
            _dstRenderTexture.filterMode = FilterMode.Point;
        }
        // Shader.SetGlobalTexture("_FootPrintTrace", _dstRenderTexture);
        Shader.SetGlobalTexture("_FootPrintBump", _footPrintBump);

        _cacheTransform = transform;
        _lastWorldPosition = _cacheTransform.position;
        _rootScale = _cacheTransform.lossyScale.x;
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

        Shader.SetGlobalInt("_WorldSize", _worldSize);
        Shader.SetGlobalFloat("_FootPrintAtten", _footPrintAtten * _curDuration);
        Shader.SetGlobalVector("_WorldPosition", curPos);
        Shader.SetGlobalVector("_DeltaWorldPosition", _lastWorldPosition - curPos);

        // Graphics.Blit(_srcRenderTexture, _dstRenderTexture, _footPrintMaterial, (int)FootPrintPass.Move);
        // Graphics.Blit(_dstRenderTexture, _srcRenderTexture);

        SwitchRenderTexture(out var srcRT, out var dstRT);
        Graphics.Blit(srcRT, dstRT, _footPrintMaterial, (int)FootPrintPass.Move);
        
        // FootPrintActive(curPos);
        
        _curDuration = 0;
        _lastWorldPosition = curPos;
    }

    public void FootPrintActive(Vector3 worldPos,  float yDegress, Foot foot)
    {
        
        Shader.SetGlobalFloat("_FootPrintBumpScale", _footPrintBumpScale);
        Shader.SetGlobalFloat("_FootPrintSize", _footPrintSize * _rootScale);
        Shader.SetGlobalVector("_DeltaFootPosition", worldPos - worldPos);
        Shader.SetGlobalFloat("_YDegress", yDegress);
        
        // Debug.Log(yDegress);
        
        var alpha = yDegress * Mathf.PI / 180.0f;
        var sina = Mathf.Sin(alpha);
        var cosa = Mathf.Cos(alpha);
        Matrix4x4 matrix = new Matrix4x4()
        {
            m00 = cosa,
            m01 = -sina,
            m10 = sina,
            m11 = cosa,
        };
        Shader.SetGlobalMatrix("_RotationFootPrint", matrix);

        var noFlipPrint = 1;
        if ((foot == Foot.Left && !_isLeftFootPrintBump) || (foot == Foot.Right && _isLeftFootPrintBump) )
        {
            noFlipPrint = 0;
        }
        Shader.SetGlobalInt("_IsLeftFoot", noFlipPrint);
        // Shader.SetGlobalTexture("_FootPrintBump", foot == Foot.Left ? _leftFootPrintBump : _rightFootPrintBump);
        
        // Graphics.Blit(_srcRenderTexture, _dstRenderTexture, _footPrintMaterial, (int)FootPrintPass.Generator);
        // Graphics.Blit(_dstRenderTexture, _srcRenderTexture);
        
        SwitchRenderTexture(out var srcRT, out var dstRT);
        Graphics.Blit(srcRT, dstRT, _footPrintMaterial, (int)FootPrintPass.Generator);
    }

    /// <summary>
    /// 优化切换贴图性能
    /// </summary>
    /// <param name="srcRT"></param>
    /// <param name="dstRT"></param>
    private void SwitchRenderTexture(out RenderTexture srcRT, out RenderTexture dstRT)
    {
        _isUseScrTexture = !_isUseScrTexture;
        srcRT = _isUseScrTexture ? _dstRenderTexture : _srcRenderTexture;
        dstRT = _isUseScrTexture ? _srcRenderTexture : _dstRenderTexture;
        
        Shader.SetGlobalTexture("_FootPrintTrace", dstRT);
    }
}