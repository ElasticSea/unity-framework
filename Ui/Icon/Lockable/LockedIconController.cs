using UnityEngine;

namespace ElasticSea.Framework.Ui.Icon.Lockable
{
    public class LockedIconController
    {
        private readonly Icon icon;
        private readonly LockableIcon lockableIcon;
        private readonly Material backplateMaterial;
        private readonly Material[] defaultMaterials;
        private readonly Material[] lockedMaterials;

        public string Id => icon.Id;

        public LockedIconController(Icon icon, LockableIcon lockableIcon, Material backplateMaterial, Material[] defaultMaterials, Material[] lockedMaterials)
        {
            this.icon = icon;
            this.lockableIcon = lockableIcon;
            this.backplateMaterial = backplateMaterial;
            this.defaultMaterials = defaultMaterials;
            this.lockedMaterials = lockedMaterials;
        }

        public void SetUnlockedDelta(float delta)
        {
            var isLocked = lockableIcon.IsLockedProperty;
            var isLockedValue = delta < 0.5;
            if (isLockedValue != isLocked.Value)
            {
                isLocked.Value = isLockedValue;
            }
            icon.FrontplateRenderer.sharedMaterials = lockableIcon.IsLocked ? lockedMaterials : defaultMaterials;
            backplateMaterial.SetFloat("_Disabled", 1 - delta);
            icon.transform.localRotation = Quaternion.Euler(0, delta * 360, 0);
        }
    }
}