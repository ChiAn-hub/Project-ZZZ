using System.Collections.Generic;
using UnityEngine;

public class DamageNumberPool : Singleton<DamageNumberPool>
{

    private const string Path = "Prefabs/DamageNumber";

    private List<DamageNumberItem> damageNumberList;
    private DamageNumberItem damageNumberPrefab;

    public DamageNumberPool()
    {
        damageNumberList = new List<DamageNumberItem>();
        damageNumberPrefab = Resources.Load<DamageNumberItem>(Path);
    }

    public DamageNumberItem GetDamageNumber(Vector3 position)
    {
        foreach (DamageNumberItem damageNumber in damageNumberList)
        {
            if (damageNumber == null)
            {
                continue;
            }

            if (!damageNumber.gameObject.activeSelf)
            {
                damageNumber.transform.position = position;
                damageNumber.transform.rotation = Quaternion.identity;
                damageNumber.gameObject.SetActive(true);
                return damageNumber;
            }
        }

        DamageNumberItem newDamageNumber = Object.Instantiate(damageNumberPrefab, position, Quaternion.identity);

        damageNumberList.Add(newDamageNumber);
        return newDamageNumber;
    }

    public void ReleaseDamageNumber(DamageNumberItem damageNumber)
    {
        if (damageNumber == null)
        {
            return;
        }

        damageNumber.gameObject.SetActive(false);
    }
}