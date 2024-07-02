using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class FSurfaceEditor : ShaderGUI
{

    bool ambientOcclFold, normalFold, rimFold, propertiesFold, reflectFold, checkReflectRim, checkReflectRimInvert, emissionFold, translucencyFold, TransmissionFold;
    //bool _aoFold;

    int tempVar;

    void loadMatVariables(Material targetMat)
    {
        tempVar = targetMat.GetInt("_AmbientOcclusion");
        ambientOcclFold = tempVar == 1 ? true : false;

        tempVar = targetMat.GetInt("_Normal");
        normalFold = tempVar == 1 ? true : false;

        tempVar = targetMat.GetInt("_RimLight");
        rimFold = tempVar == 1 ? true : false;

        tempVar = targetMat.GetInt("_Properties");
        propertiesFold = tempVar == 1 ? true : false;

        tempVar = targetMat.GetInt("_Reflect");
        reflectFold = tempVar == 1 ? true : false;

        tempVar = targetMat.GetInt("_Emission");
        emissionFold = tempVar == 1 ? true : false;

        tempVar = targetMat.GetInt("_TranslucencyOnOff");
        translucencyFold = tempVar == 1 ? true : false;

        tempVar = targetMat.GetInt("_TransmissionOnOff");
        TransmissionFold = tempVar == 1 ? true : false;
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        Material targetMat = materialEditor.target as Material;
        loadMatVariables(targetMat);

        Color[] BDColors = {
            new Color(0, 0 ,0 ,0),
            new Color(0.6f, 0.6f, 0.6f, 0.3f),
            new Color(0.4f, 0.0f, 0.0f, 0.3f)
        };

        GUIStyle style = new GUIStyle();

        #region Fiber Surface Shader Title
        Texture banner = (Texture)AssetDatabase.LoadAssetAtPath("Assets/_Main/Art/Shaders/Editor/GUI/FiberSurfaceShader.png", typeof(Texture));

        GUILayout.BeginArea(new Rect(0, 0, 512, 32));
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Box(banner, GUILayout.MinHeight(32));
            }
            EditorGUILayout.EndHorizontal();
        }
        GUILayout.EndArea();
        GUILayout.Space(32);
        #endregion

        #region Main Group
        MaterialProperty mc = ShaderGUI.FindProperty("_MainColor1", properties);
        MaterialProperty mt = ShaderGUI.FindProperty("_MainTexture", properties);
        EditorGUILayout.BeginVertical();
        {
            materialEditor.ColorProperty(mc, "Main Color");
            materialEditor.TextureProperty(mt, "Main Texture");
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(5);
        #endregion

        #region AO Group
        style.normal.background = MakeBackground(1,1, BDColors[1]);
        EditorGUILayout.BeginVertical(style);
        EditorGUI.BeginChangeCheck();
        {
            ambientOcclFold = EditorGUILayout.Toggle("Ambient Occlusion", ambientOcclFold);
        }
        if(EditorGUI.EndChangeCheck())
        {
            if (ambientOcclFold)
            {
                targetMat.SetInt("_AmbientOcclusion", 1);
            }
            else
            {
                targetMat.SetInt("_AmbientOcclusion", 0);
            }
        }
        EditorGUILayout.EndVertical();

        if (ambientOcclFold)
        {
            MaterialProperty aoc = ShaderGUI.FindProperty("_AmbientOcclusionColor", properties);
            MaterialProperty aoi = ShaderGUI.FindProperty("_AOIntensity", properties);
            MaterialProperty aot = ShaderGUI.FindProperty("_AmbientOcclusionTexture", properties);

            EditorGUILayout.BeginVertical();
            {
                materialEditor.ColorProperty(aoc, "Ambient Occlusion Color");
                materialEditor.RangeProperty(aoi, "Ambient Occlusion Intensity");
                materialEditor.TextureProperty(aot, "Ambient Occlusion Texture");
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.Space(5);
        #endregion

        #region Normal Group
        style.normal.background = MakeBackground(1, 1, BDColors[1]);
        EditorGUILayout.BeginVertical(style);
        EditorGUI.BeginChangeCheck();
        {
            normalFold = EditorGUILayout.Toggle("Normal", normalFold);
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (normalFold)
            {
                targetMat.SetInt("_Normal", 1);
            }
            else
            {
                targetMat.SetInt("_Normal", 0);
            }
        }
        EditorGUILayout.EndVertical();

        style.normal.background = MakeBackground(1, 1, BDColors[0]);
        if (normalFold)
        {
            MaterialProperty nm = ShaderGUI.FindProperty("_NormalMap", properties);
            MaterialProperty ns = ShaderGUI.FindProperty("_NormalScale", properties);

            EditorGUILayout.BeginVertical();
            {
                materialEditor.TextureProperty(nm, "Normal Map");
                materialEditor.RangeProperty(ns, "Normal Scale");
            }
            EditorGUILayout.EndVertical();

        }
        EditorGUILayout.Space(5);
        #endregion

        #region Rim Group
        style.normal.background = MakeBackground(1, 1, BDColors[1]);
        EditorGUILayout.BeginVertical(style);
        EditorGUI.BeginChangeCheck();
        {
            rimFold = EditorGUILayout.Toggle("Rim Light", rimFold);
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (rimFold)
            {
                targetMat.SetInt("_RimLight", 1);
            }
            else
            {
                targetMat.SetInt("_RimLight", 0);
            }
        }
        EditorGUILayout.EndVertical();
        style.normal.background = MakeBackground(1, 1, BDColors[0]);
        if (rimFold)
        {
            MaterialProperty rimc = ShaderGUI.FindProperty("_RimColor", properties);
            MaterialProperty rimIntensity = ShaderGUI.FindProperty("_RimIntensity", properties);
            MaterialProperty rimBias = ShaderGUI.FindProperty("_RimBias", properties);
            MaterialProperty rimScale = ShaderGUI.FindProperty("_RimScale", properties);
            MaterialProperty rimPower = ShaderGUI.FindProperty("_RimPower", properties);

            EditorGUILayout.BeginVertical();
            {
                materialEditor.ColorProperty(rimc, "Rim Color");
                materialEditor.RangeProperty(rimIntensity, "Rim Intensity");
                materialEditor.RangeProperty(rimBias, "Rim Bias");
                materialEditor.RangeProperty(rimScale, "Rim Scale");
                materialEditor.RangeProperty(rimPower, "Rim Power");
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.Space(5);
        #endregion

        #region Properties Group
        style.normal.background = MakeBackground(1, 1, BDColors[1]);
        EditorGUILayout.BeginVertical(style);
        EditorGUI.BeginChangeCheck();
        {
            propertiesFold = EditorGUILayout.Toggle("Properties", propertiesFold);
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (propertiesFold)
            {
                targetMat.SetInt("_Properties", 1);
            }
            else
            {
                targetMat.SetInt("_Properties", 0);
            }
        }
        EditorGUILayout.EndVertical();

        if (propertiesFold)
        {
            MaterialProperty metalIntensity = ShaderGUI.FindProperty("_Metallic", properties);
            MaterialProperty metalTexture = ShaderGUI.FindProperty("_MetallicTexture", properties);
            MaterialProperty smoothness = ShaderGUI.FindProperty("_Smoothness", properties);

            EditorGUILayout.BeginVertical();
            {
                materialEditor.RangeProperty(metalIntensity, "Metallic Intensity");
                materialEditor.TextureProperty(metalTexture, "Metallic Texture");
                materialEditor.RangeProperty(smoothness, " Smoothness");
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.Space(5);
        #endregion

        #region Reflection
        style.normal.background = MakeBackground(1, 1, BDColors[1]);
        EditorGUILayout.BeginVertical(style);
        EditorGUI.BeginChangeCheck();
        {
            reflectFold = EditorGUILayout.Toggle("Reflection", reflectFold);
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (reflectFold)
            {
                targetMat.SetInt("_Reflect", 1);
            }
            else
            {
                targetMat.SetInt("_Reflect", 0);
            }
        }
        EditorGUILayout.EndVertical();

        style.normal.background = MakeBackground(1, 1, BDColors[0]);

        if (reflectFold)
        {
            MaterialProperty refC = ShaderGUI.FindProperty("_ReflectColor", properties);
            MaterialProperty refS = ShaderGUI.FindProperty("_ReflectionStrength", properties);
            MaterialProperty refMap = ShaderGUI.FindProperty("_ReflectMap1", properties);
            MaterialProperty refMapRot = ShaderGUI.FindProperty("_CubeMapRotate", properties);
            MaterialProperty refMapY = ShaderGUI.FindProperty("_CubemapYPosition", properties);

            EditorGUILayout.BeginVertical();
            {
                materialEditor.ColorProperty(refC, "Reflect Color");
                materialEditor.RangeProperty(refS, "Reflect Intensity");
                materialEditor.TextureProperty(refMap, "Reflect Map");
                materialEditor.RangeProperty(refMapRot, "Reflect Rotate");
                materialEditor.RangeProperty(refMapY, "Reflect Y Position");
            }
            EditorGUILayout.EndVertical();

            #region Check Reflect Rim
            EditorGUI.BeginChangeCheck();
            checkReflectRim = EditorGUILayout.Toggle("Reflect Fresnel", checkReflectRim);
            if (EditorGUI.EndChangeCheck())
            {
                if (checkReflectRim)
                {
                    targetMat.SetInt("_ReflectionFresnel", 1);
                }
                else
                {
                    targetMat.SetInt("_ReflectionFresnel", 0);
                }
            }
            if (checkReflectRim)
            {
                #region Check Reflection Rim Invert
                EditorGUI.BeginChangeCheck();
                checkReflectRimInvert = EditorGUILayout.Toggle("Reflect Fresnel Invert", checkReflectRimInvert);
                if (EditorGUI.EndChangeCheck())
                {
                    if (checkReflectRimInvert)
                    {
                        targetMat.SetInt("_RefFresnelInvert", 1);
                    }
                    else
                    {
                        targetMat.SetInt("_RefFresnelInvert", 0);
                    }
                }
                #endregion
                EditorGUILayout.BeginVertical();
                {
                    MaterialProperty refFresBias = ShaderGUI.FindProperty("_ReflectFresnelBias", properties);
                    MaterialProperty refFresScale = ShaderGUI.FindProperty("_ReflectFresnelScale", properties);
                    MaterialProperty refFresPower = ShaderGUI.FindProperty("_ReflectFresnelPower", properties);

                    materialEditor.RangeProperty(refFresBias, "Reflect Fresnel Bias");
                    materialEditor.RangeProperty(refFresScale, "Reflect Fresnel Scale");
                    materialEditor.RangeProperty(refFresPower, "Reflect Fresnel Power");
                }
                EditorGUILayout.EndVertical();
            }
            #endregion

        }
        EditorGUILayout.Space(5);
        #endregion

        #region Emission
        style.normal.background = MakeBackground(1, 1, BDColors[1]);
        EditorGUILayout.BeginVertical(style);
        EditorGUI.BeginChangeCheck();
        {
            emissionFold = EditorGUILayout.Toggle("Emission", emissionFold);
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (emissionFold)
            {
                targetMat.SetInt("_Emission", 1);
            }
            else
            {
                targetMat.SetInt("_Emission", 0);
            }
        }
        EditorGUILayout.EndVertical();

        style.normal.background = MakeBackground(1, 1, BDColors[0]);
        if (emissionFold)
        {
            MaterialProperty emi = ShaderGUI.FindProperty("_EmissionIntensity", properties);
            MaterialProperty emc = ShaderGUI.FindProperty("_EmissionColor", properties);
            MaterialProperty emt = ShaderGUI.FindProperty("_EmissionTexture", properties);

            EditorGUILayout.BeginVertical();
            {
                materialEditor.RangeProperty(emi, "Emission Intensity");
                materialEditor.ColorProperty(emc, "Emission Color");
                materialEditor.TextureProperty(emt, "Emission Texture");
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.Space(5);
        #endregion

        #region Translucency
        style.normal.background = MakeBackground(1, 1, BDColors[1]);
        EditorGUILayout.BeginVertical(style);
        EditorGUI.BeginChangeCheck();
        {
            translucencyFold = EditorGUILayout.Toggle("Trancslucency", translucencyFold);
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (translucencyFold)
            {
                targetMat.SetInt("_TranslucencyOnOff", 1);
            }
            else
            {
                targetMat.SetInt("_TranslucencyOnOff", 0);
            }
        }
        EditorGUILayout.EndVertical();

        style.normal.background = MakeBackground(1, 1, BDColors[0]);
        if (translucencyFold)
        {
            MaterialProperty tc = ShaderGUI.FindProperty("_TranslucencyColor", properties);
            MaterialProperty ts = ShaderGUI.FindProperty("_Translucency", properties);
            MaterialProperty tnd = ShaderGUI.FindProperty("_TransNormalDistortion", properties);
            MaterialProperty tsc = ShaderGUI.FindProperty("_TransScattering", properties);
            MaterialProperty td = ShaderGUI.FindProperty("_TransDirect", properties);
            MaterialProperty ta = ShaderGUI.FindProperty("_TransAmbient", properties);
            MaterialProperty tsh = ShaderGUI.FindProperty("_TransShadow", properties);

            EditorGUILayout.BeginVertical();
            {
                materialEditor.ColorProperty(tc, "Color");
                materialEditor.RangeProperty(ts, "Strength");
                materialEditor.RangeProperty(tnd, "Normal Distortion");
                materialEditor.RangeProperty(tsc, "Scattering");
                materialEditor.RangeProperty(td, "Direct");
                materialEditor.RangeProperty(ta, "Ambient");
                materialEditor.RangeProperty(tsh, "Shadow");
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.Space(5);

        #endregion

        #region Transmission
        style.normal.background = MakeBackground(1, 1, BDColors[1]);
        EditorGUILayout.BeginVertical(style);
        EditorGUI.BeginChangeCheck();
        {
            TransmissionFold = EditorGUILayout.Toggle("Transmission", TransmissionFold);
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (TransmissionFold)
            {
                targetMat.SetInt("_TransmissionOnOff", 1);
            }
            else
            {
                targetMat.SetInt("_TransmissionOnOff", 0);
            }
        }
        EditorGUILayout.EndVertical();

        style.normal.background = MakeBackground(1, 1, BDColors[0]);
        if (TransmissionFold)
        {
            MaterialProperty transm = ShaderGUI.FindProperty("_TransmissionColor", properties);

            EditorGUILayout.BeginVertical();
            {
                materialEditor.ColorProperty(transm, "Transmission Color");
            }
            EditorGUILayout.EndVertical();

        }
        EditorGUILayout.Space(5);

        #endregion

        #region Shader Defaults
        materialEditor.RenderQueueField();
        materialEditor.EnableInstancingField();
        #endregion
    }

    private Texture2D MakeBackground(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}

