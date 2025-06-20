using System;
using UnityEngine;

public class Interactable: MonoBehaviour
{
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material highlightMaterial;
    
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    protected void UpdateMeshRendererAndMaterial(MeshRenderer meshRenderer)
    {
        _meshRenderer = meshRenderer;
        // defaultMaterial = _meshRenderer.sharedMaterial;
    }
    
    public void HighlightActive(bool active)
    {
        if (active)
        {
            _meshRenderer.material = highlightMaterial;
        }
        else
        {
            _meshRenderer.material = defaultMaterial;
        }
    }

    public virtual void Interact()
    {
        Debug.Log($"Interact with {gameObject.name}");
    }
    
    protected virtual void OnTriggerEnter(Collider other)
    {
        PlayerInteraction interaction = other.transform.GetComponent<PlayerInteraction>();
        if (interaction is null) return;
        
        interaction.InteractableList.Add(this);
        interaction.UpdateClosestInteractable();
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        PlayerInteraction interaction = other.transform.GetComponent<PlayerInteraction>();
        if (interaction is null) return;
        
        interaction.InteractableList.Remove(this);
        interaction.UpdateClosestInteractable();
    }
}
