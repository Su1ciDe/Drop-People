using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class FSkyboxEditor : ShaderGUI
{
    bool gradientFold, cubeMapFold, rotationFold, positionFold, hazeFold, fogFold, customFogColor;
    bool Additive,blendSky;
    int tempVar;

    void loadMatVariables(Material targetMat)
    {
        tempVar = targetMat.GetInt("_Gradient");
        gradientFold = tempVar == 1 ? true : false;

        tempVar = targetMat.GetInt("_CubemapSky");
        cubeMapFold = tempVar == 1 ? true : false;

        tempVar = targetMat.GetInt("_EnableRotation");
        rotationFold = tempVar == 1 ? true : false;

        tempVar = targetMat.GetInt("_EnableHaze");
        hazeFold = tempVar == 1 ? true : false;

        tempVar = targetMat.GetInt("_EnableFog");
        fogFold = tempVar == 1 ? true : false;

        tempVar = targetMat.GetInt("_CustomFogColor");
        customFogColor = tempVar == 1 ? true : false;

        tempVar = targetMat.GetInt("_BlendSky");
        blendSky = tempVar == 1 ? true : false;

        tempVar = targetMat.GetInt("_AdditiveGradient");
        Additive = tempVar == 1 ? true : false;
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

        #region Fiber Skybox Shader Title
        Texture banner = (Texture)AssetDatabase.LoadAssetAtPath("Assets/_Main/Art/Shaders/Editor/GUI/FiberSkyboxShader.png", typeof(Texture));

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

        style.normal.background = MakeBackground(1, 1, BDColors[1]);

        #region Additive
        EditorGUI.BeginChangeCheck();
        {
            Additive = EditorGUILayout.Toggle("Additive", Additive);
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (Additive)
            {
                targetMat.SetInt("_AdditiveGradient", 1);
            }
            else
            {
                targetMat.SetInt("_AdditiveGradient", 0);
            }
        }
        MaterialProperty solidBaseColor = ShaderGUI.FindProperty("_BaseColor", properties);
        materialEditor.ColorProperty(solidBaseColor, "Base Solid Color");

        #endregion

        #region Gradient Group
        EditorGUILayout.BeginVertical(style);
        EditorGUI.BeginChangeCheck();
        {
            gradientFold = EditorGUILayout.Toggle("Gradient", gradientFold);
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (gradientFold)
            {
                targetMat.SetInt("_Gradient", 1);
            }
            else
            {
                targetMat.SetInt("_Gradient", 0);
            }
        }
        EditorGUILayout.EndVertical();
        if (gradientFold)
        {
            MaterialProperty topc = ShaderGUI.FindProperty("_TopColor", properties);
            MaterialProperty botc = ShaderGUI.FindProperty("_BottomColor", properties);
            MaterialProperty HorH = ShaderGUI.FindProperty("_HorizonHeight", properties);
            MaterialProperty Power = ShaderGUI.FindProperty("_Power", properties);
            MaterialProperty Falloff = ShaderGUI.FindProperty("_Falloff", properties);

            EditorGUILayout.BeginVertical();
            {
                materialEditor.ColorProperty(topc, "Top Color");
                materialEditor.ColorProperty(botc, "Bottom Color");
                materialEditor.RangeProperty(HorH, "Horizontal Height");
                materialEditor.RangeProperty(Power, "Power");
                materialEditor.RangeProperty(Falloff, "Falloff");
            }
            EditorGUILayout.EndVertical();
        }
        #endregion

        #region Cubemap Sky
        EditorGUILayout.BeginVertical(style);
        EditorGUI.BeginChangeCheck();
        {
            cubeMapFold = EditorGUILayout.Toggle("Cubemap", cubeMapFold);
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (cubeMapFold)
            {
                targetMat.SetInt("_CubemapSky", 1);
            }
            else
            {
                targetMat.SetInt("_CubemapSky", 0);
            }
        }
        EditorGUILayout.EndVertical();

        if (cubeMapFold)
        {
            MaterialProperty cmTint = ShaderGUI.FindProperty("_TintColor", properties);
            MaterialProperty cmTxt = ShaderGUI.FindProperty("_Tex", properties);
            MaterialProperty cmTxt2 = ShaderGUI.FindProperty("_Tex1", properties);
            MaterialProperty cmblend = ShaderGUI.FindProperty("_CubemapTransition", properties);
            MaterialProperty cmexpo = ShaderGUI.FindProperty("_Exposure", properties);
            MaterialProperty cmRot = ShaderGUI.FindProperty("_Rotation", properties);
            MaterialProperty cmRotSpd = ShaderGUI.FindProperty("_RotationSpeed", properties);

            MaterialProperty cmPosX = ShaderGUI.FindProperty("_PositionX", properties);
            MaterialProperty cmPosY = ShaderGUI.FindProperty("_PositionY", properties);
            MaterialProperty cmPosZ = ShaderGUI.FindProperty("_PositionZ", properties);

            EditorGUILayout.BeginVertical();
            {
                materialEditor.ColorProperty(cmTint, "Tint Color");
                materialEditor.RangeProperty(cmexpo, "Cubemap Exposure");
                materialEditor.TextureProperty(cmTxt, "Cubemap");
                EditorGUI.BeginChangeCheck();
                {
                    blendSky = EditorGUILayout.Toggle("Layered Cubemap", blendSky);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    if (blendSky)
                    {
                        targetMat.SetInt("_BlendSky", 1);
                    }
                    else
                    {
                        targetMat.SetInt("_BlendSky", 0);
                    }
                }
                if (blendSky)
                {
                    materialEditor.RangeProperty(cmblend, "Cubemap Blend");
                    materialEditor.TextureProperty(cmTxt2, "Cubemap Layer");
                }

                EditorGUI.BeginChangeCheck();
                {
                    positionFold = EditorGUILayout.Toggle("Position", positionFold);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    if (positionFold)
                    {
                        targetMat.SetInt("_EnablePosition", 1);
                    }
                    else
                    {
                        targetMat.SetInt("_EnablePosition", 0);
                    }
                }

                if (positionFold)
                {
                    materialEditor.FloatProperty(cmPosX, "Position X");
                    materialEditor.FloatProperty(cmPosY, "Position Y");
                    materialEditor.FloatProperty(cmPosZ, "Position Z");
                }

                EditorGUI.BeginChangeCheck();
                {
                    rotationFold = EditorGUILayout.Toggle("Rotation", rotationFold);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    if (rotationFold)
                    {
                        targetMat.SetInt("_EnableRotation", 1);
                    }
                    else
                    {
                        targetMat.SetInt("_EnableRotation", 0);
                    }
                }
                if (rotationFold)
                {
                    materialEditor.RangeProperty(cmRot, "Rotate");
                    materialEditor.FloatProperty(cmRotSpd, "Rotation Speed");
                }



            }
            EditorGUILayout.EndVertical();
        }
        #endregion

        #region Haze Group
        EditorGUILayout.BeginVertical(style);
        EditorGUI.BeginChangeCheck();
        {
            hazeFold = EditorGUILayout.Toggle("Haze", hazeFold);
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (hazeFold)
            {
                targetMat.SetInt("_EnableHaze", 1);
            }
            else
            {
                targetMat.SetInt("_EnableHaze", 0);
            }
        }
        EditorGUILayout.EndVertical();

        if (hazeFold)
        {
            MaterialProperty hazeCol = ShaderGUI.FindProperty("_HazeColor", properties);
            MaterialProperty hazeInten = ShaderGUI.FindProperty("_HazeIntensity", properties);
            MaterialProperty hazeFill = ShaderGUI.FindProperty("_HazeFill", properties);
            MaterialProperty hazeSmooth = ShaderGUI.FindProperty("_HazeSmoothness", properties);
            MaterialProperty hazeHeight = ShaderGUI.FindProperty("_HazeHeight", properties);
            MaterialProperty hazePosY = ShaderGUI.FindProperty("_HazePosition", properties);

            EditorGUILayout.BeginVertical();
            {
                materialEditor.ColorProperty(hazeCol, "Haze Color");
                materialEditor.RangeProperty(hazeInten, "Haze Intensity");
                materialEditor.RangeProperty(hazeFill, "Haze Fill");
                materialEditor.RangeProperty(hazeSmooth, "Haze Smoothness");
                materialEditor.RangeProperty(hazeHeight, "Haze Height");
                materialEditor.FloatProperty(hazePosY, "Haze Position Vertical");
            }
            EditorGUILayout.EndVertical();
        }
        #endregion

        #region Fog Group
        EditorGUILayout.BeginVertical(style);
        EditorGUI.BeginChangeCheck();
        {
            fogFold = EditorGUILayout.Toggle("Fog", fogFold);
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (fogFold)
            {
                targetMat.SetInt("_EnableFog", 1);
            }
            else
            {
                targetMat.SetInt("_EnableFog", 0);
            }
        }
        EditorGUILayout.EndVertical();

        if (fogFold)
        {
            MaterialProperty fogCol = ShaderGUI.FindProperty("_FogColor", properties);
            MaterialProperty fogInten = ShaderGUI.FindProperty("_FogIntensity", properties);
            MaterialProperty fogFill = ShaderGUI.FindProperty("_FogFill", properties);
            MaterialProperty fogSmooth = ShaderGUI.FindProperty("_FogSmoothness", properties);
            MaterialProperty fogHeight = ShaderGUI.FindProperty("_FogHeight", properties);
            MaterialProperty fogPosY = ShaderGUI.FindProperty("_FogPosition", properties);

            EditorGUILayout.BeginVertical();
            {
                EditorGUI.BeginChangeCheck();
                {
                    customFogColor = EditorGUILayout.Toggle("Custom Fog Color", customFogColor);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    if (customFogColor)
                    {
                        targetMat.SetInt("_CustomFogColor", 1);
                    }
                    else
                    {
                        targetMat.SetInt("_CustomFogColor", 0);
                    }
                }

                if (customFogColor)
                {
                    materialEditor.ColorProperty(fogCol, "Fog Color");
                }
                materialEditor.RangeProperty(fogInten, "Fog Intensity");
                materialEditor.RangeProperty(fogFill, "Fog Fill");
                materialEditor.RangeProperty(fogSmooth, "Fog Smoothness");
                materialEditor.RangeProperty(fogHeight, "Fog Height");
                materialEditor.FloatProperty(fogPosY, "Fog Position Vertical");
            }
            EditorGUILayout.EndVertical();
        }
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
