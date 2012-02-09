using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Resonance
{
    class InputDevices
    {
        private KeyboardState kbd;
        private GamePadState pad1;
        private GamePadState pad2;

        private KeyboardState lastKbd;
        private GamePadState lastPad1;
        private GamePadState lastPad2;

        bool pad1PluggedEver = false;
        bool pad2PluggedEver = false;

        public InputDevices()
        {
        }

        public void Update()
        {
            lastKbd = kbd;
            lastPad1 = pad1;
            lastPad2 = pad2;

            kbd = Keyboard.GetState();
            pad1 = GamePad.GetState(PlayerIndex.One);
            pad2 = GamePad.GetState(PlayerIndex.Two);

            if (pad1.IsConnected) pad1PluggedEver = true;
            if (pad2.IsConnected) pad2PluggedEver = true;
        }

        public KeyboardState Keys
        {
            get { return kbd; }
        }

        public KeyboardState LastKeys
        {
            get { return lastKbd; }
        }

        public GamePadState PlayerOne
        {
            get { return pad1; }
        }

        public GamePadState LastPlayerOne
        {
            get { return lastPad1; }
        }

        public GamePadState PlayerTwo
        {
            get { return pad2; }
        }

        public GamePadState LastPlayerTwo
        {
            get { return lastPad2; }
        }

        public bool PlayerOneHasBeenConnected
        {
            get { return pad1PluggedEver; }
        }

        public bool PlayerTwoHasBeenConnected
        {
            get { return pad2PluggedEver; }
        }

    }
}
