using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class CRTEffect : MonoBehaviour
{
    public Material material;

    [Header("Low Resolution")]
    public bool isLowResolution;
    public int lowResolutionMultiple = 2;
    [Header("Screen Jump")]
    public bool isScreenJump;
    public int screenJumpFrequency = 1;
    public float screenJumpMinLength = 0.01f;
    public float screenJumpMaxLength = 0.2f;
    public float screenJumpMinOffset = 0.1f;
    public float screenJumpMaxOffset = 0.9f;
    private float screenJumpTimer;
    [Header("Frickering")]
    public bool isFrickering;
    public float flickeringStrength = 0.002f;
    public float flickeringCycle = 50f;
    [Header("Slippage")]
    public bool isSlippage;
    public bool isSlippageNoise;
    public float slippageStrength = 0.005f;
    public float slippageSize = 11f;
    public float slippageInterval = 1f;
    public float slippageScrollSpeed = 33f;
    [Header("Chromatic Aberration")]
    public bool isChromaticAberration;
    public float chromaticAberrationStrength = 0.005f;
    [Header("Multiple Ghost")]
    public bool isMultipleGhostStrength;
    public float multipleGhostStrength = 0.01f;
    [Header("Scanline")]
    public bool isScanline;
    public bool isScanlineNoise;
    public float scanlineFrequency = 700.0f;
    [Header("Monochorme")]
    public bool isMonochorme;
    [Header("Film Dirt")]
    public bool isFilmDirt;
    public Texture2D filmDirtTex;
    [Header("Decal")]
    public bool isDecal;
    public Vector2 decalTexPos;
    public Vector2 decalTexScale;
    public Texture2D decalTex;

    #region Shader Properties
    // Screen Jump
    private int _ScreenJumpOnOff;
    private int _ScreenJumpOffset;
    // Frickering
    private int _FlickeringOnOff;
    private int _FlickeringStrength;
    private int _FlickeringCycle;
    // Slippage
    private int _SlippageOnOff;
    private int _SlippageNoiseOnOff;
    private int _SlippageStrength;
    private int _SlippageSize;
    private int _SlippageInterval;
    private int _SlippageScrollSpeed;
    // Chromatic Aberration
    private int _ChromaticAberrationOnOff;
    private int _ChromaticAberrationStrength;
    // Multiple Ghost
    private int _MultipleGhostOnOff;
    private int _MultipleGhostStrength;
    // Scanline
    private int _ScanlineOnOff;
    private int _ScanlineNoiseOnOff;
    private int _ScanlineFrequency;
    // Monochorme
    private int _MonochromeOnOff;
    // Film Dirt
    private int _FilmDirtOnOff;
    private int _FilmDirtTex;
    // Decal
    private int _DecalTexOnOff;
    private int _DecalTexPos;
    private int _DecalTexScale;
    private int _DecalTex;
    #endregion

    void Start()
    {
        // Screen Jump
        _ScreenJumpOnOff = Shader.PropertyToID("_ScreenJumpOnOff");
        _ScreenJumpOffset = Shader.PropertyToID("_ScreenJumpOffset");
        // Frickering
        _FlickeringOnOff = Shader.PropertyToID("_FlickeringOnOff");
        _FlickeringStrength = Shader.PropertyToID("_FlickeringStrength");
        _FlickeringCycle = Shader.PropertyToID("_FlickeringCycle");
        // Slippage
        _SlippageOnOff = Shader.PropertyToID("_SlippageOnOff");
        _SlippageNoiseOnOff = Shader.PropertyToID("_SlippageNoiseOnOff");
        _SlippageStrength = Shader.PropertyToID("_SlippageStrength");
        _SlippageSize = Shader.PropertyToID("_SlippageSize");
        _SlippageInterval = Shader.PropertyToID("_SlippageInterval");
        _SlippageScrollSpeed = Shader.PropertyToID("_SlippageScrollSpeed");
        // Chromatic Aberration
        _ChromaticAberrationOnOff = Shader.PropertyToID("_ChromaticAberrationOnOff");
        _ChromaticAberrationStrength = Shader.PropertyToID("_ChromaticAberrationStrength");
        // Multiple Ghost
        _MultipleGhostOnOff = Shader.PropertyToID("_MultipleGhostOnOff");
        _MultipleGhostStrength = Shader.PropertyToID("_MultipleGhostStrength");
        // Scanline
        _ScanlineOnOff = Shader.PropertyToID("_ScanlineOnOff");
        _ScanlineNoiseOnOff = Shader.PropertyToID("_ScanlineNoiseOnOff");
        _ScanlineFrequency = Shader.PropertyToID("_ScanlineFrequency");
        // Monochorme
        _MonochromeOnOff = Shader.PropertyToID("_MonochromeOnOff");
        // Film Dirt
        _FilmDirtOnOff = Shader.PropertyToID("_FilmDirtOnOff");
        _FilmDirtTex = Shader.PropertyToID("_FilmDirtTex");
        // Decal
        _DecalTexOnOff = Shader.PropertyToID("_DecalTexOnOff");
        _DecalTexPos = Shader.PropertyToID("_DecalTexPos");
        _DecalTexScale = Shader.PropertyToID("_DecalTexScale");
        _DecalTex = Shader.PropertyToID("_DecalTex");
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // Screen Jump
        material.SetInteger(_ScreenJumpOnOff, isScreenJump ? 1 : 0);
        screenJumpTimer -= 0.01f;
        if (screenJumpTimer <= 0)
        {
            if (Random.Range(0, 5000) < screenJumpFrequency)
            {
                material.SetFloat(_ScreenJumpOffset, Random.Range(screenJumpMinOffset, screenJumpMaxOffset));
                screenJumpTimer = Random.Range(screenJumpMinLength, screenJumpMaxLength);
            }
            else
            {
                material.SetFloat(_ScreenJumpOffset, 0);
            }
        }

        // Frickering
        material.SetInteger(_FlickeringOnOff, isFrickering ? 1 : 0);
        material.SetFloat(_FlickeringStrength, flickeringStrength);
        material.SetFloat(_FlickeringCycle, flickeringCycle);

        // Slippage
        material.SetInteger(_SlippageOnOff, isSlippage ? 1 : 0);
        material.SetFloat(_SlippageNoiseOnOff, isSlippageNoise ? Random.Range(0f, 1f) : 1f);
        material.SetFloat(_SlippageStrength, slippageStrength);
        material.SetFloat(_SlippageSize, slippageSize);
        material.SetFloat(_SlippageInterval, slippageInterval);
        material.SetFloat(_SlippageScrollSpeed, slippageScrollSpeed);

        // Chromatic Aberration
        material.SetInteger(_ChromaticAberrationOnOff, isChromaticAberration ? 1 : 0);
        material.SetFloat(_ChromaticAberrationStrength, chromaticAberrationStrength);

        // Multiple Ghost
        material.SetInteger(_MultipleGhostOnOff, isMultipleGhostStrength ? 1 : 0);
        material.SetFloat(_MultipleGhostStrength, multipleGhostStrength);

        // Scanline
        material.SetInteger(_ScanlineOnOff, isScanline ? 1 : 0);
        material.SetInteger(_ScanlineNoiseOnOff, isScanlineNoise ? 1 : 0);
        material.SetFloat(_ScanlineFrequency, scanlineFrequency);

        // Monochorme
        material.SetInteger(_MonochromeOnOff, isMonochorme ? 1 : 0);

        // Film Dirt
        material.SetInteger(_FilmDirtOnOff, isFilmDirt ? 1 : 0);
        material.SetTexture(_FilmDirtTex, filmDirtTex);

        // Decal
        material.SetInteger(_DecalTexOnOff, isDecal ? 1 : 0);
        material.SetVector(_DecalTexPos, decalTexPos);
        material.SetVector(_DecalTexScale, decalTexScale);
        material.SetTexture(_DecalTex, decalTex);

        if (isLowResolution)
        {
            RenderTexture target = RenderTexture.GetTemporary(source.width / lowResolutionMultiple, source.height / lowResolutionMultiple);
            Graphics.Blit(source, target);
            Graphics.Blit(target, destination, material);
            RenderTexture.ReleaseTemporary(target);
        }
        else
        {
            Graphics.Blit(source, destination, material);
        }
    }
}