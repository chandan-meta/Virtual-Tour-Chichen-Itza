using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightObject : MonoBehaviour
{
    private List<Material> mats = new List<Material>();
    private List<Color> originalEmissionColors = new List<Color>();

    [Header("Highlight Settings")]
    public bool isHighlighting = false;

    public Color emissionColor = Color.cyan;
    public float emissionIntensity = 1.5f;

    void Start()
    {
        GetAllChildRenderers();
        StartCoroutine(CheckHighlightObject()); // Start ONLY once
    }

    void GetAllChildRenderers()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.materials)
            {
                mats.Add(mat);

                if (mat.HasProperty("_EmissionColor"))
                    originalEmissionColors.Add(mat.GetColor("_EmissionColor"));
                else
                    originalEmissionColors.Add(Color.black);
            }
        }
    }

    public void HighlightObjectActive()
    {
        isHighlighting = true;
    }

    public void StopHighlight()
    {
        isHighlighting = false;

        // Force immediate OFF (important)
        HighlightOff();
    }

    private IEnumerator CheckHighlightObject()
    {
        while (true)
        {
            if (isHighlighting)
            {
                HighlightOn();
                yield return new WaitForSeconds(0.5f);

                HighlightOff();
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                yield return null;
            }
        }
    }

    void HighlightOn()
    {
        foreach (var mat in mats)
        {
            if (mat.HasProperty("_EmissionColor"))
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", emissionColor * emissionIntensity);
            }
        }
    }

    void HighlightOff()
    {
        for (int i = 0; i < mats.Count; i++)
        {
            if (mats[i].HasProperty("_EmissionColor"))
            {
                mats[i].SetColor("_EmissionColor", originalEmissionColors[i]);
            }
        }
    }
}