using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using ElasticSea.Framework.Extensions;
using ElasticSea.Framework.Interactions;
using ElasticSea.Framework.Layout;
using ElasticSea.Framework.Ui.Layout.Spatial;
using ElasticSea.Framework.Util;
using ElasticSea.Framework.Util.PropertyDrawers;
using UnityEngine;
using UnityEngine.Audio;
#if !UNITY_VISIONOS
using Gizmoes.Interactables;
#endif

namespace ElasticSea.Framework.Ui.Icon.Lockable
{
    public class LockableIconGrid : MonoBehaviour, ILayoutComponent
    {
        [SerializeField, CustomObjectPicker(typeof(ISpatialLayout))] private Component _spatialLayout;
        public IUniformSpatialLayout SpatialLayout => _spatialLayout as IUniformSpatialLayout;

        [SerializeField] private Mesh backplateMesh;
        [SerializeField] private Transform container;
        [SerializeField] private float pressCircleGrow = 0.02f;
        [SerializeField] private bool globalCache = true;
        [SerializeField] private string globalCachePrefix;
        [SerializeField] private bool onlyScaleDown = false;
        [SerializeField, CustomObjectPicker(typeof(ILockableIconBuildCallback))] private Component[] _callbacks;
        
        [Header("Sfx")]
        [SerializeField] private AudioMixerGroup audioMixerGroup;
        [SerializeField] private AudioClip pressSound;
        [SerializeField] private float pressSoundVolume = 0.2f;
        [SerializeField] private AudioClip unlockSound;
        [SerializeField] private float unlockSoundVolume = 0.2f;
        
        private IIconLockProvider lockProvider;
        private LockedIconController[] controllers;
        private bool[] lockedState;
        private LockableIcon[] lockableIcons;
        public LockableIcon[] Icons => lockableIcons;
        
        public LockableIcon[] Build(LockableIconDataFactory[] lockedIconData)
        {
            Clear();

            var cache = new LockableIconMeshCache(prefix: globalCachePrefix);
            var factory = new IconFactory(backplateMesh);

            if (lockedIconData.Length != SpatialLayout.Count)
            {
                if (SpatialLayout is IUniformGrowSpatialLayout uniformGrowSpatialLayout)
                {
                    uniformGrowSpatialLayout.SetCount(lockedIconData.Length);
                }
            }

            var callbacks = _callbacks.Cast<ILockableIconBuildCallback>().ToArray();

            controllers = new LockedIconController[lockedIconData.Length];
            var lockableIcons = new LockableIcon[lockedIconData.Length];
            for (var i = 0; i < lockedIconData.Length; i++)
            {
                var lockedIconD = lockedIconData[i];
                var id = lockedIconD.Id;
                var locked = cache.GetCachedMeshData(lockedIconD);
                
                var inputData = new IconData()
                {
                    Name = id,
                    Radius = SpatialLayout.Size.FromXY().Min() / 2,
                    Padding = locked.Padding,
                    RawMesh = locked.RawMesh,
                    ProcessedMesh = locked.ProcessedMesh,
                    OnlyScaleDown = onlyScaleDown
                };
                var icon = factory.BuildIcon(inputData);
                locked.ProcessedMesh = icon.Data;
                cache.SetCachedMeshData(id, locked);
                var cell = SpatialLayout.GetCell(i);

                // place in the icon grid
                icon.transform.SetParent(container, false);
                icon.transform.localPosition = cell.cellToLocal.GetPosition() + cell.localBounds.center;
                icon.transform.localRotation = cell.cellToLocal.rotation;

                Color accent = locked.AccentColor;
                Material[] defaultMaterials = locked.ProcessedMesh.Materials;
                Material[] lockedMaterials = locked.LockedMaterials;
                
                var backplateMaterial = new Material(Shader.Find("Shader Graphs/Vision Os Flat Icon"));
                backplateMaterial.SetColor("_Accent", accent);
                backplateMaterial.SetFloat("_MeshOffset", pressCircleGrow);
                var anim = new HideShowAnimation(DOTween.To(t => backplateMaterial.SetFloat("_Pressed", t), 0, 1, 0.15f));
                icon.BackplateRenderer.sharedMaterial = backplateMaterial;

                var lockableIcon = icon.gameObject.AddComponent<LockableIcon>();
                var controller = new LockedIconController(icon, lockableIcon, backplateMaterial, defaultMaterials, lockedMaterials);
                var isLocked = lockProvider?.IsLocked(id) ?? false;
                controller.SetUnlockedDelta(isLocked ? 0 : 1);
                lockableIcon.Id = id;
                lockableIcon.Radius = inputData.Radius;
                lockableIcon.Collider = icon.Collider;
                lockableIcon.Background = icon.BackplateRenderer;
                lockableIcons[i] = lockableIcon;
                
                var interactable = new SimpleInteractable()
                {
                    PressCallback = @event =>
                    {
                        PlaySfx(pressSound, pressSoundVolume);
                        anim.Show();
                    },
                    CancelCallback = @event =>
                    {
                        anim.Hide();
                    },
                    ReleaseCallback = @event =>
                    {
                        anim.Hide();
                        if (lockProvider?.IsLocked(id) != true)
                        {
                            lockableIcon.Trigger();
                        }
                    }
                };

                controllers[i] = controller;
             
#if !UNITY_VISIONOS
                InteractableUtils.SetupInteractable(null, icon.Collider, interactable);
#endif

                var interactableComponent = icon.gameObject.GetOrAddComponent<IteractableComponent>();
                interactableComponent.Interactable = interactable;

                for (var ii = 0; ii < callbacks.Length; ii++)
                {
                    var callback = callbacks[ii];
                    callback.IconBuild(lockableIcon);
                }
            }
            
            OnRectChanged?.Invoke();

            this.lockableIcons = lockableIcons;

            return lockableIcons;
        }

        public void SetLocking(IIconLockProvider lockProvider)
        {
            this.lockProvider = lockProvider;
        }
        
        private void OnEnable()
        {
            var newLockedState = GetLockedState();

            var anim = GetLockedAnim(lockedState, newLockedState);
            
            lockedState = newLockedState;

            DOTween.To(t =>
            {
                for (var i = 0; i < anim.Length; i++)
                {
                    controllers[anim[i]].SetUnlockedDelta(t);
                }
            }, 0, 1, 1).SetEase(Ease.InOutCubic);
            
            PlaySfx(unlockSound, unlockSoundVolume);
        }

        private void PlaySfx(AudioClip clip, float volume)
        {
            if (clip != null && audioMixerGroup != null)
            {
                Utils.PlayClipAtPoint3D(clip, transform.position, volume, audioMixerGroup);
            }
        }

        private int[] GetLockedAnim(bool[] prev, bool[] current)
        {
            if (prev == null)
                return new int[0];
            
            var unlockedIds = new List<int>();
            for (int i = 0; i < prev.Length; i++)
            {
                var prevLocked = prev[i];
                var currentLocked = current[i];
                if (prevLocked && !currentLocked)
                {
                    unlockedIds.Add(i);
                }
            }
            return unlockedIds.ToArray();
        }

        private void OnDisable()
        {
            lockedState = GetLockedState();
        }

        private bool[] GetLockedState()
        {
            if (lockProvider != null)
            {
                var lockedState = new bool[controllers.Length];
                for (int i = 0; i < lockedState.Length; i++)
                {
                    var id = controllers[i].Id;
                    var isLocked = lockProvider.IsLocked(id);
                    lockedState[i] = isLocked;
                }

                return lockedState;
            }

            return null;
        }

        public void Clear()
        {
            container.DestroyChildren();
        }

        public Rect Rect => SpatialLayout.Bounds.FrontSide();
        public event Action OnRectChanged;
    }
}