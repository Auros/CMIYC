using System.Threading;
using AuraTween;
using CMIYC.Player;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace CMIYC.UI
{
#nullable enable
    public class WeaponAmmoCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text _ammoCounter;
        [SerializeField] private PlayerWeaponManager _weaponManager;
        [SerializeField] private TweenManager _tweenManager;

        private CancellationTokenSource? _cts;
        private int _cachedAmmo;

        private void Start()
        {
            if (_weaponManager == null)
            {
                Debug.LogWarning($"lmao someone forgot to assign {nameof(_weaponManager)}, disabling {nameof(WeaponAmmoCounter)}...");
                enabled = false;
                return;
            }

            _cachedAmmo = _weaponManager.CurrentWeaponInstance.Ammo;
        }

        // This certainly wouldnt be Auros approved but we're in a game jam, i dont give a fuck
        private void LateUpdate()
        {
            var ammo = _weaponManager.CurrentWeaponInstance.Ammo;

            if (ammo != _cachedAmmo)
            {
                _ammoCounter.text = ammo.ToString();
                _cachedAmmo = ammo;

                _cts?.Cancel();
                _cts = new();

                AnimateAmmoAsync(_cts.Token).Forget();
            }
        }

        // auratween doesnt have CancellationToken overloads? : (
        // this is so not fortnite #1 victory royale
        // REVIEW: Perhaps abstract this to a utility method if we need to re-use the cancellationtoken code
        private async UniTask AnimateAmmoAsync(CancellationToken token = default)
        {
            var tween = _tweenManager.Run(1.2f, 1f, 1f, (t) => _ammoCounter.transform.localScale = t * Vector3.one, Easer.OutCubic);

            while (tween.IsAlive)
            {
                if (token.IsCancellationRequested)
                {
                    tween.Cancel();
                    return;
                }

                await UniTask.Yield();
            }
        }
    }
#nullable restore
}
