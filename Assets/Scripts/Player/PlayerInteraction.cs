using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public List<Interactable> InteractableList { get; private set; } = new();

    private Interactable _closestInteractable;
    
    private void Start()
    {
        Player player = GetComponent<Player>();
        player.Controls.Character.Interaction.performed += _ => InteractClosest();
    }

    private void InteractClosest()
    {
        _closestInteractable?.Interact();
        if (_closestInteractable is IPickup)
        {
            InteractableList.Remove(_closestInteractable);
            _closestInteractable = null;
        }
    }

    public void UpdateClosestInteractable()
    {
        _closestInteractable?.HighlightActive(false);
        _closestInteractable = null;
        
        float closestDistance = float.MaxValue;
        foreach (var interactable in InteractableList)
        {
            float distance = Vector3.Distance(interactable.transform.position, transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                _closestInteractable = interactable;
            }
        }
        
        _closestInteractable?.HighlightActive(true);
    }
}
