using ElasticSea.Framework.Extensions;
using UnityEngine;

namespace ElasticSea.Framework.Ui.Icon.Lockable
{
    public class LockedIconController
    {
        private readonly Icon icon;
        private readonly Material backplateMaterial;
        private readonly Material[] defaultMaterials;
        private readonly Material[] lockedMaterials;

        public string Id => icon.Id;

        public LockedIconController(Icon icon, Material backplateMaterial, Material[] defaultMaterials, Material[] lockedMaterials)
        {
            this.icon = icon;
            this.backplateMaterial = backplateMaterial;
            this.defaultMaterials = defaultMaterials;
            this.lockedMaterials = lockedMaterials;
        }

        public void SetUnlockedDelta(float delta)
        {
            var isLocked = delta < 0.5;
            icon.FrontplateRenderer.sharedMaterials = isLocked ? lockedMaterials : defaultMaterials;
            backplateMaterial.SetFloat("_Disabled", 1 - delta);
            icon.transform.localRotation = Quaternion.Euler(0, delta * 360, 0);
        }
    }
}