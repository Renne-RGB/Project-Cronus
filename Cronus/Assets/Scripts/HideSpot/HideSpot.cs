using UnityEngine;

public class HideSpot : MonoBehaviour
{
    public InteractionIcon interactionIcon; //Icon
    public Vector3 GetHidePosition()
    {
        return transform.position;
    }

    public void ToggleInteractionIcon(bool show)
    {
        if (interactionIcon == null)
            return;

        if (show)
        {
            interactionIcon.Show();
        }
        else
        {
            interactionIcon.Hide();
        }
    }
}