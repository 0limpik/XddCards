using System;

namespace Assets.Source.Model.Cycles.BlackJack
{
    public abstract class State
    {
        public event Action<State> OnIncorectState;

        public event Action<bool> OnChangeExecute;

        public bool IsExecute
        {
            get => _IsExecute;
            set
            {
                if (value)
                {
                    _IsExecute = value;
                    try
                    {
                        Enter();
                    }
                    catch (Exception ex)
                    {
                        throw new BJCycleException(ex);
                    }
                }

                OnChangeExecute?.Invoke(value);

                if (!value)
                {
                    try
                    {
                        Exit();
                    }
                    catch (Exception ex)
                    {
                        throw new BJCycleException(ex);
                    }
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

        public abstract bool CanEnter(out string message);
        public abstract bool CanExit(out string message);
    }
}
