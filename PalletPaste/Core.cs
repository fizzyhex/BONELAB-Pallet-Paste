using BoneLib.Notifications;
using Il2CppSLZ.Marrow.Warehouse;
using LabFusion.Utilities;
using MelonLoader;
using PalletPaste.SpawnGun;
using UnityEngine;

[assembly: MelonInfo(typeof(PalletPaste.Core), "PalletPaste", "2.0.1", "Fizzyhex", null)]
[assembly: MelonColor(alpha: 255, red: 20, green: 194, blue: 215)]
[assembly: MelonAuthorColor(alpha: 255, red: 108, green: 116, blue: 230)]
[assembly: MelonAdditionalDependencies("LabFusion")]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]

namespace PalletPaste
{
    public class Core : MelonMod
    {
        public override void OnUpdate()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V))
            {
                OnPaste();
            }
        }

        internal static void OnPaste()
        {
            var clipboardLines = GUIUtility.systemCopyBuffer.Split("\n");
            var doNotify = clipboardLines.Length == 1;
            var pastesAccepted = false;
            
            foreach (var line in clipboardLines)
            {
                var safeText = line.Trim();
                
                if (DownloadUtils.IsModPageUrl(safeText))
                {
                    MelonLogger.Msg($"Mod.io link detected: {safeText}");
                    DownloadUtils.DownloadFromModPageUrl(safeText, notify: doNotify);
                    pastesAccepted = true;
                }
                else if (SpawnGunUtils.GetSpawnGun(out var spawnGun))
                {
                    SpawnGunUtils.CopyBarcodeToSpawnGun(spawnGun, new Barcode(safeText), notify: true);
                    break;
                }
            }

            if (pastesAccepted && !doNotify)
            {
                Notifier.Send(new Notification
                {
                    Title = "Pallet Paste",
                    Message = $"Multi-line paste started!",
                    Type = NotificationType.Information,
                });
            }
        }

        public override void OnInitializeMelon()
        {
            Root.Create();
        }
    }
}