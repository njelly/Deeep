///////////////////////////////////////////////////////////////////////////////////////////////
//
//  PlayerInputManager (c) 2020 Tofunaut
//
//  Created by Nathaniel Ellingson for Deeep on 02/29/2020
//
///////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using Tofunaut.UnityUtils;
using UnityEngine;

namespace Tofunaut.Deeep.Game
{
    public class PlayerInputManager : SingletonBehaviour<PlayerInputManager>
    {
        protected override bool SetDontDestroyOnLoad { get { return true; } }

        private HashSet<ActorInput.IReceiver> _receivers = new HashSet<ActorInput.IReceiver>();
        private ActorInput _input = new ActorInput();

        public static void Add(ActorInput.IReceiver receiver) => _instance?._receivers.Add(receiver);
        public static void Remove(ActorInput.IReceiver receiver) 
        {
            // clear the receiver's current input by sending an empty one
            receiver?.ReceiveInput(new ActorInput());
            _instance?._receivers.Remove(receiver);
        }

        private void Update()
        {
            _input = ActorInput.PollPlayerInput(_input);
            foreach (ActorInput.IReceiver receiver in _receivers)
            {
                receiver.ReceiveInput(_input);
            }
        }
    }
}