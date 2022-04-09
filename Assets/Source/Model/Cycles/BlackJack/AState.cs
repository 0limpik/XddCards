﻿using System;

namespace Xdd.Model.Cycles.BlackJack
{
    public abstract class AState
    {
        public event Action<AState> OnIncorectState;

        public event Action<bool> OnChangeExecute;

        public bool IsExecute
        {
            get => _IsExecute;
            set
            {
                if (value)
                {
                    _IsExecute = value;
                    Enter();
                }

                OnChangeExecute?.Invoke(value);

                if (!value)
                {
                    Exit();
                    _IsExecute = value;
                }
            }
        }
        private bool _IsExecute;

        protected void CheckExecute()
        {
            if (!IsExecute)
            {
                OnIncorectState?.Invoke(this);
                throw new InvalidOperationException(this.GetType().Name);
            }
        }

        protected abstract void Enter();
        protected abstract void Exit();

        public virtual bool CanEnter(out string message)
        {
            message = null;
            return true;
        }

        public virtual bool CanExit(out string message)
        {
            message = null;
            return true;
        }
    }
}