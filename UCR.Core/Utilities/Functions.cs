using System;
using System.Diagnostics;

namespace HidWizards.UCR.Core.Utilities
{
    public static class Functions
    {
        /// <summary>
        /// Inverts an axis.
        /// Given Max or Min values, will return the opposite extreme.
        /// Else returns value * -1
        /// </summary>
        /// <param name="value">The raw value of the axis</param>
        /// <returns>The inverted value of the axis</returns>
        public static short Invert(short value)
        {
            if (value == 0) return 0;
            if (value == short.MaxValue)
            {
                return short.MinValue;
            }

            if (value <= short.MinValue)
            {
                return short.MaxValue;
            }

            return (short) (value * -1);
        }

        /// <summary>
        /// Ensures that an axis value is within permitted range
        /// </summary>
        /// <param name="value">The raw axis value</param>
        /// <returns>The clamped axis value</returns>
        public static short ClampAxisRange(int value)
        {
            if (value == 0) return 0;
            if (value <= Constants.AxisMinValue) return Constants.AxisMinValue;
            return value >= Constants.AxisMaxValue ? short.MaxValue : (short)value;
        }

        /// <summary>
        /// Returns either the low or high half of the axis.
        /// Stretches the half axis returned to fill the full scale
        /// </summary>
        /// <param name="axis">The value of the axis</param>
        /// <param name="positiveRange">Set to true for the high half, else the low half</param>
        /// <returns>The new value for the split axis. If axis is negative and high is specified, returns 0. If axis is positive and low is specified, returns 0</returns>
        public static short SplitAxis(short axis, bool positiveRange)
        {
            int value;
            if (axis == 0) return short.MinValue;
            if (positiveRange)
            {
                if (axis < 0) return short.MinValue;
                value = axis;
                if (value == short.MaxValue) value++;
            }
            else
            {
                if (axis > 0) return short.MinValue;
                value = axis * -1;
            }

            value *= 2;
            value += short.MinValue;

            if (value == 32768) value = 32767;

            return (short) value;
        }

    }
}
