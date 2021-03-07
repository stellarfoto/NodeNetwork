﻿namespace Nodexr.Shared.NodeInputs
{
    public class InputCheckbox : NodeInput
    {
        private bool _checked;

        public bool Checked
        {
            get => _checked;
            set
            {
                _checked = value;
                OnValueChanged();
            }
        }

        public override int Height => 19;

        public InputCheckbox(bool isChecked = false)
        {
            this._checked = isChecked;
        }
    }
}
