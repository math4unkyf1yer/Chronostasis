using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoolDownAbility : MonoBehaviour
{
    public Image abilityImage;
    private float fillSpeed;
    private float targetFill;

    public void SetMaxFill()
    {
        abilityImage.fillAmount = 1;
    }
    public IEnumerator CoolDownRoutine(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float fill = Mathf.Lerp(1f, 0f, timer / duration);
            if (abilityImage != null)
                abilityImage.fillAmount = fill;

            yield return null;
        }

        if (abilityImage != null)
            abilityImage.fillAmount = 0f;
    }
}
