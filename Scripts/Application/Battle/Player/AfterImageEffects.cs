using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 残影特效
/// </summary>
public class AfterImageEffects : MonoBehaviour {
 
    //开启残影
    public bool _OpenAfterImage;
 
    //残影颜色
    public Color _AfterImageColor = new Color(10, 10, 150, 255);
    //残影的生存时间
    public float _SurvivalTime = 1;
    //生成残影的间隔时间
    public float _IntervalTime = 0.2f;
    private float _Time = 0;
    //残影初始透明度
    [Range(0.1f, 1.0f)]
    public float _InitialAlpha = 1.0f;
 
    private List<AfterImage> _AfterImageList;
    private MeshRenderer _MeshRenderer;
    private MeshFilter _MeshFilter;

    public void SetEnable(bool enable)
    {
        if (enable)
        {
            for (int i = _AfterImageList.Count-1; i >= 0; i--)
            {
                AfterImage image = _AfterImageList[i];
                _AfterImageList.Remove(_AfterImageList[i]);
                Destroy(image);
            }
        }
        _OpenAfterImage = enable;
    }
    
    
    void Awake () {
        _AfterImageList = new List<AfterImage>();
        _MeshRenderer = GetComponent<MeshRenderer>();
        
        _MeshFilter = GetComponent<MeshFilter>();
    }
	void Update () {

        if (_OpenAfterImage && _AfterImageList != null)
        {
            if (_MeshRenderer == null)
            {
                _OpenAfterImage = false;
                return;
            }
 
            _Time += Time.deltaTime;
            //生成残影
            CreateAfterImage();
            //刷新残影
            UpdateAfterImage();
        }
	}
    /// <summary>
    /// 生成残影
    /// </summary>
    void CreateAfterImage()
    {
        //生成残影
        if (_Time >= _IntervalTime)
        {
            _Time = 0;
 
            Mesh mesh = new Mesh();
            mesh = _MeshFilter.mesh;

            Material material = new Material(_MeshRenderer.material);
            SetMaterialRenderingMode(material, RenderingMode.Fade);
 
            _AfterImageList.Add(new AfterImage(
                mesh,
                _MeshRenderer,
                material,
                transform.localToWorldMatrix,
                _InitialAlpha,
                Time.realtimeSinceStartup,
                _SurvivalTime));
        }
    }
    /// <summary>
    /// 刷新残影
    /// </summary>
    void UpdateAfterImage()
    {
        //刷新残影，根据生存时间销毁已过时的残影
        for (int i = _AfterImageList.Count-1; i >= 0; i--)
        {
            float _PassingTime = Time.realtimeSinceStartup - _AfterImageList[i]._StartTime;

            if (_PassingTime > _AfterImageList[i]._Duration)
            {
                AfterImage image = _AfterImageList[i];
                _AfterImageList.Remove(_AfterImageList[i]);
                Destroy(image);
                continue;
            }
            
            _AfterImageList[i]._Alpha *= (1 - _PassingTime / _AfterImageList[i]._Duration);
            _AfterImageColor.a = _AfterImageList[i]._Alpha;

            _AfterImageList[i]._Material.SetColor("_Color", _AfterImageColor);

            Graphics.DrawMesh(_AfterImageList[i]._Mesh, _AfterImageList[i]._Matrix, _AfterImageList[i]._Material, gameObject.layer);
        }
    }
    /// <summary>
    /// 设置纹理渲染模式
    /// </summary>
    void SetMaterialRenderingMode(Material material, RenderingMode renderingMode)
    {
        switch (renderingMode)
        {
            case RenderingMode.Opaque:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = -1;
                break;
            case RenderingMode.Cutout:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetInt("_ZWrite", 1);
                material.EnableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 2450;
                break;
            case RenderingMode.Fade:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.EnableKeyword("_ALPHABLEND_ON");
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
            case RenderingMode.Transparent:
                material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetInt("_ZWrite", 0);
                material.DisableKeyword("_ALPHATEST_ON");
                material.DisableKeyword("_ALPHABLEND_ON");
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.renderQueue = 3000;
                break;
        }
    }
}
public enum RenderingMode
{
    Opaque,
    Cutout,
    Fade,
    Transparent,
}
class AfterImage : Object
{
    //残影网格
    public Mesh _Mesh;

    public MeshRenderer _MeshRender;
    //残影纹理
    public Material _Material;
    //残影位置
    public Matrix4x4 _Matrix;
    //残影透明度
    public float _Alpha;
    //残影启动时间
    public float _StartTime;
    //残影保留时间
    public float _Duration;
 
    public AfterImage(Mesh mesh,MeshRenderer meshRenderer, Material material, Matrix4x4 matrix4x4, float alpha, float startTime, float duration)
    {
        _Mesh = mesh;
        _MeshRender = meshRenderer;
        _Material = material;
        _Matrix = matrix4x4;
        _Alpha = alpha;
        _StartTime = startTime;
        _Duration = duration;
    }
}