#region Usings

using System;
using System.Collections.Generic;
using System.Drawing;

#endregion

namespace Logictracker.Web.Helpers.ColorHelpers
{
    /// <summary>
    /// Helper for auto generating colors.
    /// </summary>
    public class ColorGenerator
    {
        #region Private Properties

        /// <summary>
        /// Current base color index.
        /// </summary>
        private int _colorIndex;

        /// <summary>
        /// Current saturation percentage index.
        /// </summary>
        private int _percentageIndex;

        /// <summary>
        /// Base colors list.
        /// </summary>
        private readonly List<Color> _colors;

        /// <summary>
        /// Saturations percentages list.
        /// </summary>
        private readonly List<int> _percentages = new List<int>(5) { 100, 70, 30, 85, 55 };

        #endregion

        #region Public Properties

        /// <summary>
        /// Determines if the green color should be ignored.
        /// </summary>
        public bool IgnoreGreen
        { 
            set
            {
                if (_colors == null) return;

                if (value) _colors.Remove(Color.Green);
                else if (!_colors.Contains(Color.Green)) _colors.Add(Color.Green);
            }
        }

        /// <summary>
        /// Determines if the red color should be ignored.
        /// </summary>
        public bool IgnoreRed
        {
            set
            {
                if (_colors == null) return;

                if (value) _colors.Remove(Color.Red);
                else if (!_colors.Contains(Color.Red)) _colors.Add(Color.Red);
            }
        }

        /// <summary>
        /// Determines if the yellow color should be ignored.
        /// </summary>
        public bool IgnoreYellow
        {
            set
            {
                if (_colors == null) return;

                if (value) _colors.Remove(Color.Peru);
                else if (!_colors.Contains(Color.Peru)) _colors.Add(Color.Peru);
            }
        }

        /// <summary>
        /// Determines if the blue color should be ignored.
        /// </summary>
        public bool IgnoreBlue
        {
            set
            {
                if (_colors == null) return;

                if (value) _colors.Remove(Color.Blue);
                else if (!_colors.Contains(Color.Blue)) _colors.Add(Color.Blue);
            }
        }

        /// <summary>
        /// Determines if the orange color should be ignored.
        /// </summary>
        public bool IgnoreOrange
        {
            set
            {
                if (_colors == null) return;

                if (value) _colors.Remove(Color.Orange);
                else if (!_colors.Contains(Color.Orange)) _colors.Add(Color.Orange);
            }
        }

        /// <summary>
        /// Determines if the violet color should be ignored.
        /// </summary>
        public bool IgnoreViolet
        {
            set
            {
                if (_colors == null) return;

                if (value) _colors.Remove(Color.Violet);
                else if (!_colors.Contains(Color.Violet)) _colors.Add(Color.Violet);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Insatanciates a new color generator using the default list of base colors.
        /// </summary>
        public ColorGenerator() { _colors = new List<Color>(6) { Color.Green, Color.Red, Color.Blue, Color.Orange, Color.Violet, Color.Peru }; }

        /// <summary>
        /// Instanciates a new color generator using the givenn list of base colors.
        /// </summary>
        /// <param name="colors"></param>
        public ColorGenerator(List<Color> colors) { _colors = colors; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the next step number for generating colors in async postback cases.
        /// </summary>
        /// <returns></returns>
        public int GetNextStep() { return (_percentageIndex * _colors.Count) + _colorIndex; }

        /// <summary>
        /// Gets the next color associated to the givenn initial step.
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public Color GetNextColor(int step)
        {
            _colorIndex = step%_colors.Count;
            //_percentageIndex = (step - _colorIndex) / _colors.Count ;
            _percentageIndex = (step - _colorIndex) % _percentages.Count;
            return GetNextColor();
        }

        /// <summary>
        /// Gets the next color.
        /// </summary>
        /// <returns></returns>
        public Color GetNextColor()
        {
            var originalColor = _colors[_colorIndex];
            var percentage = _percentages[_percentageIndex];

            _colorIndex++;

            if (_colorIndex.Equals(_colors.Count)) _percentageIndex++;

            var red = (originalColor.R * percentage) / 100;
            var green = (originalColor.G * percentage) / 100;
            var blue = (originalColor.B * percentage) / 100;

            if (_colorIndex.Equals(_colors.Count)) _colorIndex = 0;
            if (_percentageIndex.Equals(_percentages.Count)) _percentageIndex = 0;

            return Color.FromArgb(red, green, blue);
        }

        public static List<Color> GetGradientColors(int num)
        {
            const int center = 128;
            const int width = 127;
            const double frequency = 2.4;

            return makeGradientColors(frequency, frequency, frequency, 0, 2, 4, center, width, num);
        }

        private static List<Color> makeGradientColors(double frequency1, double frequency2, double frequency3, int phase1, int phase2, int phase3, int center, int width, int len)
        {
            var colors = new List<Color>();            

            for(var i = 0; i < len; ++i) {
                var red = (int) (Math.Sin(frequency1 * i + phase1) * width + center);
                var grn = (int) (Math.Sin(frequency2 * i + phase2) * width + center);
                var blu = (int) (Math.Sin(frequency3 * i + phase3) * width + center);
                colors.Add(Color.FromArgb(red, grn, blu));
            }

            return colors;
        }

        public static String HexConverter(Color c)
        {
            var rtn = String.Empty;
            try
            {
                rtn = "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
            }
            catch (Exception)
            {
                //doing nothing
            }

            return rtn;
        }

        #endregion
    }

}