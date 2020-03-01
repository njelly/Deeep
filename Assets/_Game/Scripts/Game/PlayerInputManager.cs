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
        public interface IReceiver
        {
            void ReceivePlayerInput(ActorInput input);
        }

        protected override bool SetDontDestroyOnLoad { get { return true; } }

        private HashSet<IReceiver> _receivers = new HashSet<IReceiver>();
        private ActorInput _input = new ActorInput();

        public static void Add(IReceiver receiver) => _instance?._receivers.Add(receiver);
        public static void Remove(IReceiver receiver) 
        {
            // clear the receiver's current input by sending an empty one
            receiver?.ReceivePlayerInput(new ActorInput());
            _instance?._receivers.Remove(receiver);
        }

        private void Update()
        {
            _input = ActorInput.PollPlayerInput(_input);
            foreach (IReceiver receiver in _receivers)
            {
                receiver.ReceivePlayerInput(_input);
            }
        }
    }
}