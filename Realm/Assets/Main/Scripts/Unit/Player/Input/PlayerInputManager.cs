using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PlayerInputManager : MonoBehaviour
{
    private Player player;
    private Camera mainCamera;
    private Dictionary<KeyCode, System.Action> skillKeyBindings;
    private bool isProcessingBasicAttack = false;
    private bool isHandlingInput = false;

    public void Initialize(Player player)
    {
        this.player = player;
        mainCamera = Camera.main;
        InitializeKeyBindings();
    }

    private void InitializeKeyBindings()
    {
        skillKeyBindings = new Dictionary<KeyCode, System.Action>
        {
            { KeyCode.Q, () => TryUseSkill(KeyCode.Q) },
            { KeyCode.W, () => TryUseSkill(KeyCode.W) },
            { KeyCode.E, () => TryUseSkill(KeyCode.E) },
            { KeyCode.R, () => TryUseSkill(KeyCode.R) },
            { KeyCode.Space, () => TryUseSkill(KeyCode.Space) }
        };
    }

    public void HandleInput()
    {
        if (!player.IsAlive || isHandlingInput) return;

        if (player.skillController.CurrentSkill != null &&
            player.skillController.CurrentSkill.IsSkillInProgress)
        {
            return;
        }

        isHandlingInput = true;
        try
        {
            if (!isProcessingBasicAttack)
            {
                HandleMouseInput();
            }

            CheckSkillInputs();
            CheckItemHover();
        }
        finally
        {
            isHandlingInput = false;
        }
    }

    private void CheckSkillInputs()
    {
        foreach (var binding in skillKeyBindings)
        {
            if (Input.GetKey(binding.Key))
            {
                binding.Value.Invoke();
            }
        }
    }

    private void TryUseSkill(KeyCode keyCode)
    {
        if (player.skillController.TryUseSkillByKey(keyCode))
        {
            if (player.PlayerHandler.CurrentState is not PlayerSkillState)
            {
                player.PlayerHandler.TransitionTo(new PlayerSkillState(player));
            }
        }
    }

    private void HandleMouseInput()
    {
        if (!Input.GetMouseButton(0)) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        float sphereRadius = 1f;
        RaycastHit[] hits = Physics.SphereCastAll(ray, sphereRadius, Mathf.Infinity);

        WorldDropItem closestItem = null;
        Monster closestMonster = null;
        float closestItemDistance = float.MaxValue;
        float closestMonsterDistance = float.MaxValue;
        float maxScreenDistance = 100f;

        foreach (RaycastHit hit in hits)
        {
            Vector3 screenPoint = mainCamera.WorldToScreenPoint(hit.transform.position);
            if (screenPoint.z < 0) continue;

            float screenDistance = Vector2.Distance(
                new Vector2(Input.mousePosition.x, Input.mousePosition.y),
                new Vector2(screenPoint.x, screenPoint.y)
            );

            if (screenDistance > maxScreenDistance) continue;

            if (hit.collider.TryGetComponent<WorldDropItem>(out WorldDropItem dropItem))
            {
                if (screenDistance < closestItemDistance)
                {
                    closestItemDistance = screenDistance;
                    closestItem = dropItem;
                }
            }
            else if (hit.collider.TryGetComponent<Monster>(out Monster monster))
            {
                if (screenDistance < closestMonsterDistance)
                {
                    closestMonsterDistance = screenDistance;
                    closestMonster = monster;
                }
            }
        }

        if (closestItem != null)
        {
            HandleDropItemClick(closestItem);
            return;
        }

        // 몬스터 처리
        if (closestMonster != null)
        {
            HandleMonsterClick(closestMonster);
            return;
        }

        // 지형 처리
        if (Physics.Raycast(ray, out RaycastHit groundHit, Mathf.Infinity, player.GroundLayer))
        {
            HandleGroundClick(groundHit.point);
        }
    }

    private void HandleMonsterClick(Monster monster)
    {
        if (isProcessingBasicAttack) return;

        try
        {
            isProcessingBasicAttack = true;
            float distanceToTarget = Vector3.Distance(player.transform.position, monster.transform.position);
            float attackRange = player.CharacterStats.GetStatValue(StatType.AttackRange);

            player.SetTarget(monster);

            if (distanceToTarget <= attackRange)
            {
                player.PlayerHandler.TransitionTo(new PlayerSkillState(player));
            }
            else
            {
                player.PlayerHandler.TransitionTo(new PlayerMoveState(player));
            }
        }
        finally
        {
            isProcessingBasicAttack = false;
        }
    }

    private void HandleGroundClick(Vector3 position)
    {
        player.SetDestination(position);
        player.PlayerHandler.TransitionTo(new PlayerMoveState(player));
    }

    private void HandleDropItemClick(WorldDropItem dropItem)
    {
        float distanceToItem = Vector3.Distance(player.transform.position, dropItem.transform.position);
        if (distanceToItem <= dropItem.InteractionRadius)
        {
            dropItem.TryPickupItem();
        }
        else
        {
            player.SetDestination(dropItem.transform.position);
            dropItem.TryPickupItem();
        }
    }

    public void CheckItemHover()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        float sphereRadius = 1f;
        RaycastHit[] hits = Physics.SphereCastAll(ray, sphereRadius, Mathf.Infinity);

        WorldDropItem closestItem = null;
        float closestDistance = float.MaxValue;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.TryGetComponent<WorldDropItem>(out WorldDropItem dropItem))
            {
                Vector3 screenPoint = mainCamera.WorldToScreenPoint(dropItem.transform.position);
                if (screenPoint.z < 0) continue;

                float screenDistance = Vector2.Distance(
                    new Vector2(Input.mousePosition.x, Input.mousePosition.y),
                    new Vector2(screenPoint.x, screenPoint.y)
                );

                float maxScreenDistance = 100f;
                if (screenDistance < maxScreenDistance && screenDistance < closestDistance)
                {
                    closestDistance = screenDistance;
                    closestItem = dropItem;
                }
            }
        }

        WorldDropItem.UpdateHoveredItem(closestItem);
    }
}