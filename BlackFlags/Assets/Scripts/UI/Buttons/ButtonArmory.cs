using UnityEngine;
using GameMechanics.Data;
public class ButtonArmory : MonoBehaviour
{
    [SerializeField] private byte weaponIndex, amount;
    void Start()
    {
        
    }

    public void ToLoad()
    {
        ShipInventory.ToLoad(weaponIndex, amount);
    }

    public void ToEquipment()
    {
        ShipInventory.ToEquipment(weaponIndex, amount);
    }

    private void Refresh()
    {

    }
}
