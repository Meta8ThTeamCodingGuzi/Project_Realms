using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterTargetUI : MonoBehaviour
{
    private Slider monsterHPSlider;
    private TextMeshProUGUI monsterName;
    private Monster targetMonster;

    // UI 오브젝트를 저장할 변수 추가
    [SerializeField] private GameObject targetUI;

    private void Start()
    {
        Initialize();
        // 시작할 때는 UI를 숨김
        if (targetUI != null)
            targetUI.SetActive(false);
    }

    public void Initialize()
    {
        monsterHPSlider = GetComponentInChildren<Slider>();
        monsterName = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        SelectTarget();
        
        // 타겟이 있다면 정보 업데이트
        if (targetMonster != null)
        {
            UpdateTargetInfo();
        }
    }

    private void SelectTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red, 1f);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerMask.GetMask("Monster")))
        {
            Debug.Log($"Hit object: {hit.collider.gameObject.name}");

            targetMonster = hit.collider.GetComponent<Monster>();

            if (targetUI != null)
                targetUI.SetActive(true);

            UpdateTargetInfo();
        }
        else
        {
            targetMonster = null;
            if (targetUI != null)
                targetUI.SetActive(false);
        }
    }

    private void UpdateTargetInfo()
    {
        if (targetMonster == null)
            return;

        if (targetMonster.CharacterStats.GetStatValue(StatType.Health) <= 0)
        {
            targetMonster = null;
            if (targetUI != null)
                targetUI.SetActive(false);
            return;
        }

        float currentHealth = targetMonster.CharacterStats.GetStatValue(StatType.Health);
        float maxHealth = targetMonster.CharacterStats.GetStatValue(StatType.MaxHealth);
        monsterHPSlider.value = currentHealth / maxHealth;

        monsterName.text = targetMonster.name;
    }
}
