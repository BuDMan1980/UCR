using System;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;
using HidWizards.UCR.Core.Utilities.AxisHelpers;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("Axes to Axes")]
    [PluginInput(DeviceBindingCategory.Range, "X Axis")]
    [PluginInput(DeviceBindingCategory.Range, "Y Axis")]
    [PluginOutput(DeviceBindingCategory.Range, "X Axis")]
    [PluginOutput(DeviceBindingCategory.Range, "Y Axis")]
    public class AxesToAxes : Plugin
    {
        private readonly CircularDeadZoneHelper _circularDeadZoneHelper = new CircularDeadZoneHelper();
        private readonly DeadZoneHelper _deadZoneHelper = new DeadZoneHelper();
        private readonly SensitivityHelper _sensitivityHelper = new SensitivityHelper();
        private double _linearSenstitivityScaleFactor;

        [PluginGui("Invert X", ColumnOrder = 0)]
        public bool InvertX { get; set; }

        [PluginGui("Invert Y", ColumnOrder = 1)]
        public bool InvertY { get; set; }

        [PluginGui("Sensitivity", ColumnOrder = 2)]
        public int Sensitivity { get; set; }

        [PluginGui("Linear", RowOrder = 0, ColumnOrder = 2)]
        public bool Linear { get; set; }

        [PluginGui("Dead zone", RowOrder = 1, ColumnOrder = 0)]
        public int DeadZone { get; set; }

        [PluginGui("Circular", RowOrder = 1, ColumnOrder = 2)]
        public bool CircularDz { get; set; }


        public AxesToAxes()
        {
            DeadZone = 0;
            Sensitivity = 100;
        }

        private void Initialize()
        {
            _deadZoneHelper.Percentage = DeadZone;
            _circularDeadZoneHelper.Percentage = DeadZone;
            _sensitivityHelper.Percentage = Sensitivity;
            _linearSenstitivityScaleFactor = ((double)Sensitivity / 100);
        }

        public override void Update(params long[] values)
        {
            var outputValues = new short[] {(short) values[0], (short) values[1]};
            if (DeadZone != 0)
            {
                if (CircularDz)
                {
                    outputValues = _circularDeadZoneHelper.ApplyRangeDeadZone(outputValues);
                }
                else
                {
                    outputValues[0] = _deadZoneHelper.ApplyRangeDeadZone(outputValues[0]);
                    outputValues[1] = _deadZoneHelper.ApplyRangeDeadZone(outputValues[1]);
                }
                
            }
            if (Sensitivity != 100)
            {
                if (Linear)
                {
                    var tmpValues = new int[2];
                    tmpValues[0] = (int)(outputValues[0] * _linearSenstitivityScaleFactor);
                    tmpValues[1] = (int)(outputValues[1] * _linearSenstitivityScaleFactor);
                    outputValues[0] = Functions.ClampAxisRange(tmpValues[0]);
                    outputValues[1] = Functions.ClampAxisRange(tmpValues[1]);
                }
                else
                {
                    outputValues[0] = _sensitivityHelper.ApplyRangeSensitivity(outputValues[0]);
                    outputValues[1] = _sensitivityHelper.ApplyRangeSensitivity(outputValues[1]);
                }
            }

            //var outputValues = new short[2];
            outputValues[0] = Functions.ClampAxisRange((int) outputValues[0]);
            outputValues[1] = Functions.ClampAxisRange((int) outputValues[1]);

            if (InvertX) outputValues[0] = Functions.Invert(outputValues[0]);
            if (InvertY) outputValues[1] = Functions.Invert(outputValues[1]);



            WriteOutput(0, outputValues[0]);
            WriteOutput(1, outputValues[1]);
        }

        #region Event Handling
        public override void OnActivate()
        {
            base.OnActivate();
            Initialize();
        }

        public override void OnPropertyChanged()
        {
            base.OnPropertyChanged();
            Initialize();
        }
        #endregion
    }
}
