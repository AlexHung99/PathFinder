using PathFinder.Combat;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Attact(CombatTarget target)
    {
        print("Take that you short, squat peasant! "+ target.ToString());
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
