using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineHighlighter : MonoBehaviour
{
    private Renderer rend;
    private Material originalMaterial;
    public Material brightYellow;
    private Outline outlineScript;
  //  public Material highlightMaterial;

    void Start()
    {
        outlineScript = GetComponent<Outline>();
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            originalMaterial = rend.material;
        }
    }

    public void Highlight()
    {
        if (!outlineScript.enabled)
        {
            outlineScript.enabled = true;
        }
    }

    public void Unhighlight()
    {
        if (outlineScript.enabled)
        {
            outlineScript.enabled = false;
        }
    }
    public void selectedHighlight()
    {
        if (rend != null)
        {
            rend.material = brightYellow;
        }
    }
    public void UnselectedHighlight()
    {
        if (rend != null)
        {
            rend.material = originalMaterial;
        }
    }
}
