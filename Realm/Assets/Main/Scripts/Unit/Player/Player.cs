using UnityEngine;

public class Player : Unit
{
    protected override void Awake()
    {
        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize();
        if (characterStats == null)
        {
            UnitStats unitStats = gameObject.AddComponent<UnitStats>();
            characterStats = unitStats;
        }
    }
    private void Update()
    {
        MovetoCursor();
    }

    private void MovetoCursor()
    {
        if (Input.GetMouseButtonDown(0)) 
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray,out RaycastHit hit ,1000f))
            {
                if (hit.collider.TryGetComponent<Monster>(out Monster monster))
                {
                    Attack(monster);
                }
                else 
                {
                    MoveTo(hit.point);
                }
                   
            }         
        }    
    }

    private void OnDrawGizmos()
    {
           
    }


    public float GetAttack()
    {
        return characterStats.GetStatValue(StatType.Attack);
    }

    public float GetDefense()
    {
        return characterStats.GetStatValue(StatType.Defense);
    }
}
