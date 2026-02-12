using UnityEngine;

public class GoalSpot : MonoBehaviour
{
    public string sceneName;
    public InteractionIcon interactionIcon; //Icon
    public string GetSceneName()
    {
        return sceneName;
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
