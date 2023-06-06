using System;

namespace Checkers
{
    public interface ISerializable
    {
        public event Action<BaseClickComponent> ChipDestroyed;
        
        public event Action ObjectsMoved;
        
        public event Action<ColorType> GameEnded;
        
        public event Action StepOver; 
    }
}