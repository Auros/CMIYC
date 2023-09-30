using CMIYC.Player;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace CMIYC.UI
{
    public class WeaponAmmoCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text _ammoCounter;
        [SerializeField] private PlayerWeaponManager _weaponManager;

        private int _cachedAmmo;

        // This certainly wouldnt be Auros approved but we're in a game jam, i dont give a fuck
        private void LateUpdate()
        {
            var ammo = _weaponManager.Ammo;

            if (ammo != _cachedAmmo)
            {
                _ammoCounter.text = ammo.ToString();
                _cachedAmmo = ammo;
            }
        }
    }
}
