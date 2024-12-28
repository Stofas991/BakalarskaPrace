/*
 * File: HealthBarScript.cs
 * Description: Manages the health bar UI element.
 * Author: Kryštof Glos
 * Date: 25.3.2024
 */
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{

    public Slider slider;
    public Image fill;

    ///<summary>
    /// Sets the maximum health value and initializes the health bar.
    ///</summary>
    ///<param name="health">The maximum health value.</param>
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    ///<summary>
    /// Updates the current health value and adjusts the health bar accordingly.
    ///</summary>
    ///<param name="health">The current health value.</param>
    public void SetHealth(int health)
    {
        slider.value = health;
    }
}
