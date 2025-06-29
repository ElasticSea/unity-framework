using Interactions;
using UnityEngine;
using EventType = Interactions.EventType;

namespace ElasticSea.Framework.Ui.Interactions
{
    public class InteractionEvent
    {
        public object InteractionProvider;
        public Handedness Handedness;
        public Collider Collider;
        public Ray Ray;
        public Vector3 HitNormal;
        public EventType EventType;
        public InteractionType InteractionType;
        public IInteractable RayInteractable;

        public Vector3 InteractionPosition;
        public Vector3 StartInteractionPosition;
        public Vector3 DeltaInteractionPosition;
        public Vector3 NormalizedTimeDeltaInteractionPosition;
        
        public Vector3 DevicePosition;
        public Vector3 StartDevicePosition;
        public Vector3 DeltaDevicePosition;
        public Vector3 NormalizedTimeDeltaDevicePosition;
        public Vector3 DeviceRotation;
    }
}