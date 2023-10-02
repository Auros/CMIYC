namespace CMIYC.Items
{
    public interface IWeaponItemPickerUpper
    {
        /// <summary>
        /// Called when an item comes into contact with this GameObject.
        /// </summary>
        /// <param name="pickupEvent">Projectile hit information.</param>
        void OnItemPickup(WeaponItemPickupEvent pickupEvent);
    }
}
