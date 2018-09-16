using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Button to Axis")]
    [PluginInput(DeviceBindingCategory.Momentary, "Button")]
    [PluginOutput(DeviceBindingCategory.Range, "Axis")]
    public class ButtonToAxis : Plugin
    {
        [PluginGui("Invert Input", ColumnOrder = 0)]
        public bool InvertInput { get; set; }

        [PluginGui("Invert Output", ColumnOrder = 1)]
        public bool Invert { get; set; }

        [PluginGui("Absolute", ColumnOrder = 2)]
        public bool Absolute { get; set; }

        [PluginGui("Range target", ColumnOrder = 3)]
        public int Range { get; set; }


        public ButtonToAxis()
        {
            Range = 100;
        }

        public override void Update(params long[] values)
        {
            var value = (short)values[0];

            if (InvertInput) value = (short) (1 - value);

            // ToDo: Review logic, move off into Utilities and unit test
            if (Absolute)
            {
                value = Functions.ClampAxisRange((int) (Constants.AxisMinValue + value * Constants.AxisMaxValue * 2 * (Range / 100.0)));
                if (Invert) value = Functions.Invert(value);
                WriteOutput(0, value);
            }
            else
            {
                var inverse = value == 0 ^ Invert;
                value = Functions.ClampAxisRange((int) ((inverse ? Constants.AxisMinValue : Constants.AxisMaxValue) * (Range / 100.0)));
                WriteOutput(0, value);
            }
        }

    }
}
