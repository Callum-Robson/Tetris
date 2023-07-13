using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MPB_Manager : MonoBehaviour
{
    private Color defaultColor;
    public Color highlightColor = Color.cyan;
    public Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();
        defaultColor = _renderer.sharedMaterial.color;
    }

    public void ActivateHighlight()
    {
        _renderer.GetPropertyBlock(_propBlock);
        // Assign the new value.
        _propBlock.SetColor("_Color", highlightColor);
        //_propBlock.SetColor("_EmissionColor", new Color(0.25f, 0f, 0f));
        _renderer.SetPropertyBlock(_propBlock);
    }

    public void DeactivateHighlight()
    {
        _renderer.GetPropertyBlock(_propBlock);
        // Assign the new value.
        _propBlock.SetColor("_Color", defaultColor);
        _renderer.SetPropertyBlock(_propBlock);
    }
}
