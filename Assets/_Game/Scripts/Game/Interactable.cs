///////////////////////////////////////////////////////////////////////////////////////////////
//
//  Interactable (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 02/28/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.Events;
using System;

namespace Tofunaut.Deeep.Game
{
    // --------------------------------------------------------------------------------------------
    public class Interactable : MonoBehaviour
    {
        [Serializable]
        public struct InteractedEventInfo
        {
            public Actor instigator;
            public Interactable interactedWith;
        }

        [Serializable]
        public class InteractedEvent : UnityEvent<InteractedEventInfo> { }

        [SerializeField] private InteractedEvent _onBeginInteract = new InteractedEvent();
        [SerializeField] private InteractedEvent _onEndInteract = new InteractedEvent();

        public void AddBeginInteractListener(UnityAction<InteractedEventInfo> action) => _onBeginInteract.AddListener(action);
        public void RemoveBeginInteractListener(UnityAction<InteractedEventInfo> action) => _onBeginInteract.RemoveListener(action);

        public void AddEndInteractListener(UnityAction<InteractedEventInfo> action) => _onEndInteract.AddListener(action);
        public void RemoveEndInteractListener(UnityAction<InteractedEventInfo> action) => _onEndInteract.RemoveListener(action);

        public void BeginInteract(Actor instigator)
        {
            _onBeginInteract?.Invoke(new InteractedEventInfo { instigator = instigator, interactedWith = this });
        }

        public void EndInteract(Actor instigator)
        {
            _onEndInteract?.Invoke(new InteractedEventInfo { instigator = instigator, interactedWith = this });
        }
    }
}