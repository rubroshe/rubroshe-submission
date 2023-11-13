using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager Instance;

    public delegate void ExperienceChangeHandler(int amount);
    public event ExperienceChangeHandler OnExperienceChange;


    // Singleton check
    private void Awake()
    {
        Debug.Log("ExperienceManager Awake is being called.");
        if (Instance != null && Instance != this)
        {
            Debug.Log("Duplicate instance of ExperienceManager detected and destroyed.");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            Debug.Log("ExperienceManager instance assigned.");
        }
    }

    public void AddExperience(int amount)
    {
        OnExperienceChange?.Invoke(amount);
    }
}
