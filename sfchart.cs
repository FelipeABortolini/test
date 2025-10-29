using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using T.Library;
using T.Wpf.Base;
using T.Wpf.RunControls;
using T.Wpf.RunServices;
using Humanizer;
using System.Linq.Expressions;
using System.Data;
using T.Kernel.Core;
using T.TSystem;

namespace T.Wpf.Components.TrendChart
{
    #region YAxis Class

    public class YAxisInternal : Canvas
    {
        public YAxisInternal()
        {
        }
    }

    #endregion YAxis Class

    #region XAxis Class

    public class XAxisInternal : Canvas
    {
        public XAxisInternal()
        {
        }
    }

    #endregion XAxis Class

    public class TNavigationControl : StackPanel
    {
        public TNavigationControl()
        {

        }
    }

    public class TLegendWindow : Canvas
    {
        public TLegendWindow()
        {
        }
    }

    public class TLegendWindowBorder : Border
    {
        public TLegendWindowBorder()
        {
        }
    }

    public class TAnnotationsMenu : Canvas
    {
        public TAnnotationsMenu()
        {
        }
    }

    #region Auxiliary Classes

    internal class TAnnotations
    {
        private double pointX;

        public double PointX
        {
            get { return pointX; }
            set { pointX = value; }
        }

        private double pointY;

        public double PointY
        {
            get { return pointY; }
            set { pointY = value; }
        }

        private object[] objValues;

        public object[] ObjValues
        {
            get { return objValues; }
            set { objValues = value; }
        }

        private string[] colors;

        public string[] Colors
        {
            get { return colors; }
            set { colors = value; }
        }

        private bool isCursorValueOutput = true;

        public bool IsCursorValueOutput
        {
            get { return isCursorValueOutput; }
            set { isCursorValueOutput = value; }
        }

        private bool isPrimaryCursor;

        public bool IsPrimaryCursor
        {
            get { return isPrimaryCursor; }
            set { isPrimaryCursor = value; }
        }

        private bool isCloseButtonVisible = true;

        public bool IsCloseButtonVisible
        {
            get { return isCloseButtonVisible; }
            set { isCloseButtonVisible = value; }
        }

        private string closeButtonId;

        public string CloseButtonId
        {
            get { return closeButtonId; }
            private set { closeButtonId = value; }
        }

        private string elementId;

        public string ElementId
        {
            get { return elementId; }
            private set { elementId = value; }
        }

        private bool isOrientationHorizontal = true;

        private string content = "";
        private bool closeButtonAdded = false;

        public bool CursorWasClosed { get; set; }

        private double LineSize = 0.0;
        private string LineColor = "";
        private string LineThickness = "";
        private int lineThicknessInt = 0;
        private string LineStyle = "";
        private string CursorLine = "";

        private bool ShowCursorValuesEnabled = false;

        private int cursorValueOutputWidth = 5;

        private bool IsLineOnly = false;

        public TAnnotations(Point point, bool isPrimaryCursor, SfChart.OrientationMode orientationMode, double lineSize, string lineColor, string cursorLine)
        {
            this.PointX = point.X;
            this.PointY = point.Y;
            this.IsPrimaryCursor = isPrimaryCursor;
            this.IsCursorValueOutput = true;
            this.ShowCursorValuesEnabled = false;
            this.LineSize = lineSize;
            this.IsLineOnly = true;

            if (string.IsNullOrEmpty(lineColor))
                lineColor = (isPrimaryCursor) ? "#FF2F4F4F" : "#FF006400";
            this.LineColor = this.RemoveAlphaFromColor(lineColor);
            this.CursorLine = cursorLine;

            this.LineThickness = this.GetPropertyFromCursorLine("StrokeThickness");
            if (string.IsNullOrEmpty(this.LineThickness))
                this.LineThickness = "2";
            int.TryParse(this.LineThickness, out this.lineThicknessInt);

            this.LineStyle = "dashed";
            string style = this.GetPropertyFromCursorLine("StrokeDashArray");
            if (string.IsNullOrEmpty(style))
                this.LineStyle = "solid";

            this.isOrientationHorizontal = true;
            if (orientationMode != SfChart.OrientationMode.Horizontal)
                this.isOrientationHorizontal = false;

            if (isPrimaryCursor)
                this.ElementId = "Primary";
            else
                this.ElementId = "Secondary";

            this.ElementId += "CursorLine_" + Guid.NewGuid().ToString();
        }

        public TAnnotations(Point point, object[] objValues, string[] colors, bool isPrimaryCursor, bool isCloseButtonVisible, bool isCursorValue, bool showCursorValuesEnabled, SfChart.OrientationMode orientationMode, double lineSize, string lineColor, string cursorLine)
        {
            this.PointX = point.X;
            this.PointY = point.Y;
            this.ObjValues = objValues;
            this.Colors = colors;
            this.IsPrimaryCursor = isPrimaryCursor;
            this.IsCloseButtonVisible = isCloseButtonVisible;
            this.IsCursorValueOutput = isCursorValue;
            this.ShowCursorValuesEnabled = showCursorValuesEnabled;
            this.LineSize = lineSize;

            if (string.IsNullOrEmpty(lineColor))
                lineColor = (isPrimaryCursor) ? "#FF2F4F4F" : "#FF006400";
            this.LineColor = this.RemoveAlphaFromColor(lineColor);
            this.CursorLine = cursorLine;

            this.LineThickness = this.GetPropertyFromCursorLine("StrokeThickness");
            if (string.IsNullOrEmpty(this.LineThickness))
                this.LineThickness = "2";
            int.TryParse(this.LineThickness, out this.lineThicknessInt);

            this.LineStyle = "dashed";
            string style = this.GetPropertyFromCursorLine("StrokeDashArray");
            if (string.IsNullOrEmpty(style))
                this.LineStyle = "solid";

            this.isOrientationHorizontal = true;
            if (orientationMode != SfChart.OrientationMode.Horizontal)
                this.isOrientationHorizontal = false;

            this.ElementId = "secondaryCursor";
            this.CloseButtonId = "secondaryCloseButton";
            if (this.IsPrimaryCursor)
            {
                this.ElementId = "primaryCursor";
                this.CloseButtonId = "primaryCloseButton";
            }

            this.CloseButtonId += ("_" + Guid.NewGuid().ToString());
        }

        public object CreateJsObject()
        {
            this.content = string.Empty;

            object annotationObj = TInterop.ExecuteJavaScriptBase(@"new Object()");

            TInterop.ExecuteJavaScriptBase("$0.coordinateUnits = 'Pixel'", annotationObj);
            TInterop.ExecuteJavaScriptBase("$0.region = 'Chart'", annotationObj);

            string aux = this.PointX.ToString(TCultureInfo.InvariantCulture);
            TInterop.ExecuteJavaScriptBase("$0.x = Number($1)", annotationObj, aux);

            aux = this.PointY.ToString(TCultureInfo.InvariantCulture);
            TInterop.ExecuteJavaScriptBase("$0.y = Number($1)", annotationObj, aux);

            TInterop.ExecuteJavaScriptBase("$0.verticalAlignment = 'Bottom'", annotationObj, aux);
            if (this.isOrientationHorizontal == false && this.IsCursorValueOutput)
                TInterop.ExecuteJavaScriptBase("$0.horizontalAlignment = 'Far'", annotationObj, aux);

            if (this.IsLineOnly)
            {
                this.CreateCursorLineContent();
                this.content = $"<div id=\"{this.ElementId}\">" + this.content + "</div>";
                TInterop.ExecuteJavaScriptBase("$0.content = $1", annotationObj, this.content);
                return annotationObj;
            }

            if (this.IsCursorValueOutput)
            {
                this.ElementId += "Value_" + Guid.NewGuid().ToString();
            }
            else
            {
                this.ElementId += "Timestamp_" + Guid.NewGuid().ToString();
            }

            for (int i = 0; i < this.ObjValues.Length; i++)
            {
                object objValue = this.ObjValues[i];
                string color = this.Colors[i];
                if (this.IsCursorValueOutput)
                {
                    int marginTop = 1;
                    if (i == 1 && this.isOrientationHorizontal == false)
                        marginTop = 7;

                    string style = $"height:15px; line-height:15px; margin-top:{marginTop}px; pointer-events: none;";

                    this.content += $"<div style=\"{style}\">";

                    if (this.isOrientationHorizontal)
                    {
                        this.AddCursorOutputToContent(objValue, color);
                        if (this.closeButtonAdded == false)
                            this.AddCloseButtonToContent();
                    }
                    else
                    {
                        if (this.closeButtonAdded == false)
                            this.AddCloseButtonToContent();
                        this.AddCursorOutputToContent(objValue, color);
                    }

                    this.content += "</div>";
                }
                else // Is Timestamp Value in the bottom of the page
                {
                    this.content += "<div style=\"height: 18px; width:110%; border:1px; border-style:solid; border-color:#000000; border-radius: 5px; background:rgba(255, 255, 255, 1)\">";
                    this.AddCursorTimestampToContent(objValue);
                    this.content += "</div>";
                    break;
                }
                this.closeButtonAdded = true;
            }
            this.content = $"<div id=\"{this.ElementId}\">" + this.content;
            this.content += $"</div>";

            this.content = this.content.Replace("[PLACEHOLDER_WIDTH]", this.cursorValueOutputWidth + "px");
            this.content = this.content.Replace("[PLACEHOLDER_MARGIN_LEFT]", (this.LineSize - this.cursorValueOutputWidth - 16) + "px");
            //if (this.cursorValueOutputWidth > 15 && this.isOrientationHorizontal)
            //{
            //    aux = (this.PointX - (this.cursorValueOutputWidth - 15)).ToString(TCultureInfo.InvariantCulture);
            //    TInterop.ExecuteJavaScriptBase("$0.x = Number($1)", annotationObj, aux);
            //}

            TInterop.ExecuteJavaScriptBase("$0.content = $1", annotationObj, this.content);
            return annotationObj;
        }

        private void CreateCursorLineContent()
        {
            string display = "";
            string marginLeftLine = "margin-left:0px";
            string lineDim = $"width:{this.LineSize.ToString(TCultureInfo.InvariantCulture)}px; height: {this.lineThicknessInt + 2}px";
            string lineBorder = $"border-top: {this.LineThickness}px {this.LineStyle} {this.LineColor};";

            if (this.isOrientationHorizontal)
            {
                display = "display:inline-block";
                marginLeftLine = "margin-left:0px";
                lineDim = $"height:{this.LineSize.ToString(TCultureInfo.InvariantCulture)}px; width: {this.lineThicknessInt}px";
                lineBorder = $"border-left: {this.lineThicknessInt}px {this.LineStyle} {this.LineColor};";
            }

            string line = $"<div style=\"{lineDim}; {display}; {lineBorder}; {marginLeftLine}; pointer-events: none;\"></div>";
            this.content = line;
        }

        private void AddCursorOutputToContent(object objValue, string color)
        {
            if (objValue == null)
                return;

            string backgroundValue = "#d3d3d3";
            string backgroundColorSeries = color;
            string strValue = TConvert.ToString(objValue);
            if (string.IsNullOrEmpty(strValue))
            {
                backgroundValue = "transparent";
                backgroundColorSeries = "transparent";
            }

            int len = strValue.Replace("-", string.Empty).Replace(".", string.Empty).Replace(",", string.Empty).Length;

            int width = len * 9;
            if (this.cursorValueOutputWidth < width)
                this.cursorValueOutputWidth = width;

            string display = "";
            string marginLeftValueBox = "margin-left:[PLACEHOLDER_MARGIN_LEFT]";
            string marginLeftColorBox = "margin-left:" + (this.LineSize - 16).ToString(TCultureInfo.InvariantCulture) + "px";
            string marginTopColorBox = "margin-top:-15px";

            if (this.isOrientationHorizontal)
            {
                display = "display:inline-block";
                marginLeftValueBox = "margin-left:-2px";
                marginLeftColorBox = "margin-left:1px";
                marginTopColorBox = "margin-top:0px";
            }
            string cursorValue = string.Empty;
            string colorBox = string.Empty;

            string height = "inherit";
            string widthCursorValue = "[PLACEHOLDER_WIDTH]";
            string widthColorBox = "15px";

            if (this.ShowCursorValuesEnabled)
            {
                cursorValue = $"<div style=\"height:{height}; width:{widthCursorValue}; {display}; {marginLeftValueBox}; background:{backgroundValue}; text-align:right;\">{strValue}</div>";
                colorBox = $"<div style=\"height:{height}; width:{widthColorBox}; background:{backgroundColorSeries}; {display}; color:transparent; {marginLeftColorBox}; {marginTopColorBox};\">.</div>";
            }

            this.content += cursorValue;
            this.content += colorBox;
        }

        private void AddCloseButtonToContent()
        {
            string background = "transparent";
            string color = "transparent";
            if (this.IsCloseButtonVisible)
            {
                background = "#cc6464";
                color = "#FFFFFF";
            }

            string commomContentStyle = $"height:inherit; width: 15px; background:{background};color:{color};text-align:center;";

            if (this.isOrientationHorizontal)
                this.content += $"<div id=\"{this.CloseButtonId}\" align=\"center\" style=\"{commomContentStyle}; display:inline-block; margin-left:1px;\">X</div>";
            else
            {
                string marginLeft = (this.LineSize - 15).ToString(TCultureInfo.InvariantCulture) + "px";
                this.content = $"<div id=\"{this.CloseButtonId}\" align=\"center\" style=\"{commomContentStyle}; margin-left:{marginLeft}; margin-bottom:0px; margin-top: -15px;\">X</div>" + content;
            }
        }

        private void AddCursorTimestampToContent(object objValue)
        {
            //#d3d3d3
            this.content += $"<div style=\"text-align: center;vertical-align:middle;\">{objValue}</div>";
        }

        private string RemoveAlphaFromColor(string color)
        {
            if (color.StartsWith("#") == false)
                return color;

            try
            {
                string aux = color.Substring(1);
                string hexR = aux.Substring(2, 2);
                string hexG = aux.Substring(4, 2);
                string hexB = aux.Substring(6, 2);

                int r = Convert.ToInt32(hexR, 16);
                int g = Convert.ToInt32(hexG, 16);
                int b = Convert.ToInt32(hexB, 16);

                return $"rgb({r}, {g}, {b})";
            }
            catch
            {
            }

            return color;
        }

        private string GetPropertyFromCursorLine(string property)
        {
            string[] split = this.CursorLine.Split(';');

            foreach (string cursorProperty in split)
            {
                if (cursorProperty.StartsWith(property, StringComparison.InvariantCultureIgnoreCase))
                    return cursorProperty.Substring(property.Length + 1); // +1 to account for the equal '=' sign
            }

            return null;
        }
    }

    internal class TStripLine
    {
        private double start = double.NaN;

        public double Start
        {
            get { return start; }
            private set { start = value; }
        }

        private double end = double.NaN;

        public double End
        {
            get { return end; }
            private set { end = value; }
        }

        private double xAxisDelta = double.NaN;

        private double delta = double.NaN;

        private string color = "black";

        private string cursorLine = null;

        private string dashArray = null;

        private string thickness = null;

        private const string DEFAULT_THICKNESS = "2";

        private double defaultNumericalThickness = double.NaN;

        public TStripLine()
        {
        }

        public TStripLine(string cursorLine, string color, double start, double end)
        {
            this.UpdateStripLineParameters(start, end, color, cursorLine);
        }

        private string RemoveAlphaFromColor(string color)
        {
            if (color.StartsWith("#") == false)
                return color;

            try
            {
                string aux = color.Substring(1);
                string hexR = aux.Substring(2, 2);
                string hexG = aux.Substring(4, 2);
                string hexB = aux.Substring(6, 2);

                int r = Convert.ToInt32(hexR, 16);
                int g = Convert.ToInt32(hexG, 16);
                int b = Convert.ToInt32(hexB, 16);

                return $"rgb({r}, {g}, {b})";
            }
            catch
            {
            }

            return color;
        }

        private string GetPropertyFromCursorLine(string property)
        {
            string[] split = this.cursorLine.Split(';');

            foreach (string cursorProperty in split)
            {
                if (cursorProperty.StartsWith(property, StringComparison.InvariantCultureIgnoreCase))
                    return cursorProperty.Substring(property.Length + 1); // +1 to account for the equal '=' sign
            }

            return null;
        }

        public void UpdateStripLineParameters(double updatedStart, double updatedEnd, string updatedColor, string updatedCursorLine)
        {
            this.start = updatedStart;
            this.end = updatedEnd;
            this.delta = this.end - this.start;

            this.xAxisDelta = (this.end - this.start) / 0.0025;
            //   double end = start + (0.0025 * xAxisDelta);

            this.color = updatedColor;
            this.cursorLine = updatedCursorLine;
            this.dashArray = this.GetPropertyFromCursorLine("StrokeDashArray");
            this.thickness = this.GetPropertyFromCursorLine("StrokeThickness");

            if (string.IsNullOrEmpty(this.thickness) || this.thickness.Equals(DEFAULT_THICKNESS, StringComparison.InvariantCultureIgnoreCase))
                return;

            if (double.TryParse(this.thickness, out double numericThickness) && numericThickness > 0.0)
            {
                this.end = start + (0.0025 * xAxisDelta) * (numericThickness / 2.0);
            }
        }

        public object CreateJsObject()
        {
            string aux;

            object stripLineObj = TInterop.ExecuteJavaScriptBase(@"new Object()");

            if (double.IsNaN(this.start) || double.IsNaN(this.end))
                return stripLineObj;

            TInterop.ExecuteJavaScriptBase("$0.start = $1", stripLineObj, this.start);
            TInterop.ExecuteJavaScriptBase("$0.end = $1", stripLineObj, this.end);

            TInterop.ExecuteJavaScriptBase("$0.zIndex = 'Over'", stripLineObj);

            aux = this.RemoveAlphaFromColor(this.color);
            TInterop.ExecuteJavaScriptBase("$0.color = $1", stripLineObj, aux);
            TInterop.ExecuteJavaScriptBase("$0.visible = true", stripLineObj);
            TInterop.ExecuteJavaScriptBase("$0.opacity = 1", stripLineObj);

            if (string.IsNullOrEmpty(this.dashArray) == false)
                TInterop.ExecuteJavaScriptBase("$0.dashArray = $1", stripLineObj, this.dashArray);

            return stripLineObj;
        }
    }

    internal class TAxis
    {
        private bool isXAxisNumeric = false;
        public bool IsXAxisNumeric
        {
            get { return isXAxisNumeric; }
            set 
            { 
                isXAxisNumeric = value;                
            }
        }


        private bool isXAxis = false;
        public bool IsXAxis
        {
            get { return isXAxis; }
            set { isXAxis = value; }
        }


        private TimeSpan? defaultInterval = null;

        public TimeSpan? DefaultInterval
        {
            get { return defaultInterval; }
            private set { defaultInterval = value; }
        }

        private double? defaultIntervalNumeric = null;

        public double? DefaultIntervalNumeric
        {
            get { return defaultIntervalNumeric; }
            private set { defaultIntervalNumeric = value; }
        }

        private bool isVisible = true;

        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }

        private string valueType = "Double"; /// Double       -> Renders a Numeric Axis

                                             /// DateTime     -> Renders a DateTime Axis
                                             /// Category     -> Renders a Category Axis
                                             /// Logarithimic -> Renders a Log Axis
        public string ValueType
        {
            get { return valueType; }
            set { valueType = value; }
        } ///

        private bool isInversed = false; /// It specifies whether the axis to be rendered in inversed manner or not.

        public bool IsInversed
        {
            get { return isInversed; }
            set { isInversed = value; }
        }

        private string name = null; /// Unique identifier of an axis. To associate an axis with the series, set this name to the xAxisName/yAxisName properties of the series.

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private double? zoomFactor = null;

        public double? ZoomFactor
        {
            get { return zoomFactor; }
            set { zoomFactor = value; }
        }

        private double? zoomPosition = null;

        public double? ZoomPosition
        {
            get { return zoomPosition; }
            set { zoomPosition = value; }
        }

        private double labelPadding = double.NaN;

        public double LabelPadding
        {
            get { return labelPadding; }
            set { labelPadding = value; }
        }

        public double MinimumNumeric = 0;
        public double MaximumNumeric = 100;

        private DateTime MinimumDateTime = DateTime.MinValue;
        private DateTime MaximumDateTime = DateTime.MinValue;

        private int numberOfLabels = 2;

        public int NumberOfLabels
        {
            get { return numberOfLabels; }
            set { numberOfLabels = value; }
        }


        private double interval = -1; /// Specifies the interval for an axis.

        public double Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        private int rowIndex = int.MinValue;

        public int RowIndex
        {
            get { return rowIndex; }
            set { rowIndex = value; }
        }

        private int columnIndex = int.MinValue;

        public int ColumnIndex
        {
            get { return columnIndex; }
            set { columnIndex = value; }
        }

        private string intervalType = "Auto";/// Auto: Defines the interval of the axis based on data.
                                             /// Years: Defines the interval of the axis in years.
                                             /// Months: Defines the interval of the axis in months.
                                             /// Days: Defines the interval of the axis in days.
                                             /// Hours: Defines the interval of the axis in hours.
                                             /// Minutes: Defines the interval of the axis in minutes.

        public string IntervalType
        {
            get { return intervalType; }
            set { intervalType = value; }
        }

        private string majorGridLinesColor = "#D3D3D3";

        public string MajorGridLinesColor
        {
            get { return majorGridLinesColor; }
            set { majorGridLinesColor = value; }
        }

        private string majorGridLinesDashArray = "";

        public string MajorGridLinesDashArray
        {
            get { return majorGridLinesDashArray; }
            set { majorGridLinesDashArray = value; }
        }

        private int majorGridLinesWidth = 1;

        public int MajorGridLinesWidth
        {
            get { return majorGridLinesWidth; }
            set { majorGridLinesWidth = value; }
        }

        private string lineStyleColor = "#000000";

        public string LineStyleColor
        {
            get { return lineStyleColor; }
            set
            {
                this.LabelStyleColor = value;
                lineStyleColor = value;
            }
        }

        private string labelStyleColor = "#000000";

        public string LabelStyleColor
        {
            get { return labelStyleColor; }
            set { labelStyleColor = value; }
        }

        private string labelFormat = null;

        public string LabelFormat
        {
            get { return labelFormat; }
            set { labelFormat = value; }
        }

        private TStripLine stripLineLeftMouseClick = null;

        public TStripLine StripLineLeftMouseClick
        {
            get { return stripLineLeftMouseClick; }
            set { stripLineLeftMouseClick = value; }
        }

        private TStripLine stripLineRightMouseClick = null;

        public TStripLine StripLineRightMouseClick
        {
            get { return stripLineRightMouseClick; }
            set { stripLineRightMouseClick = value; }
        }

        public void UpdateXMinAndXMax(double xMin, double xMax)
        {
            if (!this.isXAxis)
                return;

            this.MinimumNumeric = xMin;
            this.MaximumNumeric = xMax;
            this.Interval = (this.NumberOfLabels % 2 == 0) ? this.numberOfLabels : this.numberOfLabels - 1;
        }

        public void UpdateXMinAndXMax(DateTime xMin, DateTime xMax)
        {
            if (!this.isXAxis)
                return;

            this.MinimumDateTime = xMin;// xMin.ToLocalTime();
            this.MaximumDateTime = xMax;//xMax.ToLocalTime();

            this.CalculateIntervalLimits(xMin, xMax);
        }

        public void UpdateYMinAndYMax(double yMin, double yMax)
        {
            if (this.isXAxis)
                return;

            this.MinimumNumeric = yMin;
            this.MaximumNumeric = yMax;
            this.Interval = (this.NumberOfLabels % 2 == 0) ? this.numberOfLabels : this.numberOfLabels - 1;
        }

        public void CalculateIntervalLimits(DateTime xMin, DateTime xMax)
        {
            if (xMin == null)
                xMin = this.MinimumDateTime;
            if (xMax == null)
                xMax = this.MaximumDateTime;

            TimeSpan duration = xMax - xMin;

            if (duration.TotalDays >= 1)
            {
                if (duration.TotalDays >= 30)
                    this.IntervalType = "Months";
                else if (duration.TotalDays >= 365)
                    this.IntervalType = "Years";
                else
                    this.IntervalType = "Days";

                this.Interval = Math.Floor(duration.TotalDays);
            }
            else if (duration.TotalHours >= 1)
            {
                this.Interval = Math.Floor(duration.TotalHours);
                this.IntervalType = "Hours";
            }
            else if (duration.TotalMinutes >= 1)
            {
                this.IntervalType = "Minutes";
                this.Interval = Math.Floor(duration.TotalMinutes);
            }
            else
            {
                this.IntervalType = "Auto";
            }
        }

        public ulong DateTimeToJavaTimeStamp(DateTime utcTime)
        {
            double val = utcTime.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalMilliseconds;
            if (val < 0)
                return 0;
            return TConvert.To<ulong>(val);
        }

        public DateTime JavaTimeStampToDateTime(double axisMin, double javaTimeStamp)
        {
            double diff = javaTimeStamp - axisMin;
            return this.MinimumDateTime.AddMilliseconds(diff);

            //return new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddMilliseconds(javaTimeStamp);
        }

        public object CreateJsObject(int idx = -1)
        {
            string aux;

            object axisObj = TInterop.ExecuteJavaScriptBase(@"new Object()");
            //TInterop.ExecuteJavaScriptBase("$0.span = 2", axisObj);
            if (idx >= 0)
                TInterop.ExecuteJavaScriptBase("$0.rowIndex = $1", axisObj, idx);

            // Aplicar columnIndex se foi configurado
            if (this.ColumnIndex != int.MinValue)
            {
                string colIndexStr = this.ColumnIndex.ToString(TCultureInfo.InvariantCulture);
                TInterop.ExecuteJavaScriptBase("$0.columnIndex = Number($1)", axisObj, colIndexStr);
            }

            TInterop.ExecuteJavaScriptBase("$0.visible = false", axisObj);
            if (this.IsVisible)
                TInterop.ExecuteJavaScriptBase("$0.visible = true", axisObj);

            if (!string.IsNullOrEmpty(this.Name))
                TInterop.ExecuteJavaScriptBase("$0.name = $1", axisObj, this.Name);
            TInterop.ExecuteJavaScriptBase("$0.opposedPosition = false", axisObj);

            TInterop.ExecuteJavaScriptBase("$0.isInversed = false", axisObj);
            if (this.IsInversed)
                TInterop.ExecuteJavaScriptBase("$0.isInversed = true", axisObj);

            if (this.LabelFormat != null)
                TInterop.ExecuteJavaScriptBase("$0.labelFormat = $1", axisObj, this.LabelFormat);

            if (double.IsNaN(this.LabelPadding) == false)
                TInterop.ExecuteJavaScriptBase("$0.labelPadding = $1", axisObj, this.LabelPadding);

            // Zoom
            if (this.ZoomFactor != null)
                TInterop.ExecuteJavaScriptBase("$0.zoomFactor = $1", axisObj, this.ZoomFactor);
            if (this.ZoomPosition != null)
                TInterop.ExecuteJavaScriptBase("$0.zoomPosition = $1", axisObj, this.ZoomPosition);

            // Min - Max and Interval
            if (this.isXAxis)
            {
                if(this.isXAxisNumeric)
                {
                    this.ValueType = "Double";
                    this.LabelFormat = "N0";

                    aux = this.MinimumNumeric.ToString(TCultureInfo.InvariantCulture);
                    TInterop.ExecuteJavaScriptBase("$0.minimum = Number($1)", axisObj, aux);
                    aux = this.MaximumNumeric.ToString(TCultureInfo.InvariantCulture);
                    TInterop.ExecuteJavaScriptBase("$0.maximum = Number($1)", axisObj, aux);
                    aux = this.Interval.ToString(TCultureInfo.InvariantCulture);
                    TInterop.ExecuteJavaScriptBase("$0.interval = Number($1)", axisObj, aux);
                    TInterop.ExecuteJavaScriptBase("$0.intervalType = ''", axisObj);
                }
                else
                {
                    this.ValueType = "DateTime";
                    this.LabelFormat = "MM/dd/yyyy hh:mm:ss";

                    string jsTimeMin = this.DateTimeToJavaTimeStamp(this.MinimumDateTime).ToString(TCultureInfo.InvariantCulture);
                    object objDateMin = TInterop.ExecuteJavaScriptBase(@"new Date(Number($0))", jsTimeMin);
                    TInterop.ExecuteJavaScriptBase("$0.minimum = $1", axisObj, objDateMin);

                    string jsTimeMax = this.DateTimeToJavaTimeStamp(this.MaximumDateTime).ToString(TCultureInfo.InvariantCulture);
                    object objDateMax = TInterop.ExecuteJavaScriptBase(@"new Date(Number($0))", jsTimeMax);
                    TInterop.ExecuteJavaScriptBase("$0.maximum = $1", axisObj, objDateMax);
               
                    aux = this.Interval.ToString(TCultureInfo.InvariantCulture);
                    TInterop.ExecuteJavaScriptBase("$0.interval = Number($1)", axisObj, aux);               
                    TInterop.ExecuteJavaScriptBase("$0.intervalType = $1", axisObj, this.IntervalType);

                }
                TInterop.ExecuteJavaScriptBase("$0.valueType = $1", axisObj, this.ValueType);
                TInterop.ExecuteJavaScriptBase("$0.labelFormat = $1", axisObj, this.LabelFormat);
            }
            else
            {
                aux = 0.ToString(); /// The SfChart will always be in the [0, 100] range, the labels will assume the correct values for the tags, but will be scaled
                TInterop.ExecuteJavaScriptBase("$0.minimum = Number($1)", axisObj, aux);
                aux = 100.ToString(); /// The SfChart will always be in the [0, 100] range, the labels will assume the correct values for the tags, but will be scaled
                TInterop.ExecuteJavaScriptBase("$0.maximum = Number($1)", axisObj, aux);
                aux = this.Interval.ToString(TCultureInfo.InvariantCulture);
                TInterop.ExecuteJavaScriptBase("$0.interval = Number($1)", axisObj, aux);
            }
            // Grid Lines
            object gridLinesObj = TInterop.ExecuteJavaScriptBase(@"new Object()");
            TInterop.ExecuteJavaScriptBase("$0.color = $1", gridLinesObj, this.MajorGridLinesColor);
            TInterop.ExecuteJavaScriptBase("$0.dashArray = $1", gridLinesObj, this.MajorGridLinesDashArray);
            aux = this.MajorGridLinesWidth.ToString(TCultureInfo.InvariantCulture);
            TInterop.ExecuteJavaScriptBase("$0.width = Number($1)", gridLinesObj, aux);
            TInterop.ExecuteJavaScriptBase("$0.majorGridLines = $1", axisObj, gridLinesObj);

            // Line Style
            object lineStyleObj = TInterop.ExecuteJavaScriptBase(@"new Object()");
            TInterop.ExecuteJavaScriptBase("$0.color = $1", lineStyleObj, this.LineStyleColor);
            if (this.isXAxis == false)
            {
                TInterop.ExecuteJavaScriptBase("$0.width = 0", lineStyleObj, this.LineStyleColor);
            }
            TInterop.ExecuteJavaScriptBase("$0.lineStyle = $1", axisObj, lineStyleObj);

            // FontSize
            object labelStyleObj = TInterop.ExecuteJavaScriptBase(@"new Object()");
            TInterop.ExecuteJavaScriptBase("$0.size = '0px'", labelStyleObj);
            TInterop.ExecuteJavaScriptBase("$0.color = 'transparent'", labelStyleObj);
            //if (this.isXAxis)
            //{
            //    TInterop.ExecuteJavaScriptBase("$0.size = 11", labelStyleObj);
            //    TInterop.ExecuteJavaScriptBase("$0.color = $1", labelStyleObj, this.LabelStyleColor);
            //}
            TInterop.ExecuteJavaScriptBase("$0.labelStyle = $1", axisObj, labelStyleObj);
            TInterop.ExecuteJavaScriptBase("$0.edgeLabelPlacement = 'Shift'", axisObj);

            /*
            // StripLines
            object stripLineObjs = TInterop.ExecuteJavaScriptBase(@"[]");
            object stripLineObj;
            if (this.StripLineLeftMouseClick != null)
            {
                stripLineObj = this.StripLineLeftMouseClick.CreateJsObject();
                TInterop.ExecuteJavaScriptBase(@"$0.push($1)", stripLineObjs, stripLineObj);
            }
            if (this.StripLineRightMouseClick != null)
            {
                stripLineObj = this.StripLineRightMouseClick.CreateJsObject();
                TInterop.ExecuteJavaScriptBase(@"$0.push($1)", stripLineObjs, stripLineObj);
            }
            TInterop.ExecuteJavaScriptBase(@"$0.stripLines = $1", axisObj, stripLineObjs);
            */

            return axisObj;
        }

        public void SetXAxisDefaultInterval(DateTime xMin, DateTime xMax)
        {
            if (!this.isXAxis)
                return;

            if (this.DefaultInterval != null)
                return;

            this.DefaultInterval = xMax - xMin;
        }

        public void SetXAxisDefaultInterval(double xMin, double xMax)
        {
            if (!this.isXAxis)
                return;

            if (this.DefaultIntervalNumeric != null)
                return;

            this.DefaultIntervalNumeric = xMax - xMin;
        }


        /// <summary>
        /// Constructor for X Axis
        /// </summary>
        /// <param name="xMin"></param>
        /// <param name="xMax"></param>
        public TAxis(DateTime xMin, DateTime xMax)
        {
            this.isXAxis = true;
            this.MinimumDateTime = xMin;
            this.MaximumDateTime = xMax;
            this.MajorGridLinesWidth = 0;

            //this.DesiredInterval = (this.NumberOfLabels % 2 == 0) ? this.numberOfLabels : this.numberOfLabels - 1;
            this.ValueType = "DateTime";
            this.LabelFormat = "MM/dd/yyyy hh:mm:ss";
        }

        /// <summary>
        /// Constructor for Y Axis
        /// </summary>
        /// <param name="xMin"></param>
        /// <param name="xMax"></param>
        public TAxis(double xMin, double xMax, bool isXAxis)
        {
            this.isXAxis = isXAxis;
            this.isXAxisNumeric = isXAxis;
            this.MinimumNumeric = xMin;
            this.MaximumNumeric = xMax;
            this.labelFormat = "N0";
            this.Interval = (this.NumberOfLabels % 2 == 0) ? this.numberOfLabels : this.numberOfLabels - 1;
        }
    }

    internal class Marker
    {
        private string imageUrl = null;

        public string ImageUrl
        {
            get { return imageUrl; }
            set { imageUrl = value; }
        }

        private string dataLabelFormat = null;

        public string DataLabelFormat
        {
            get { return dataLabelFormat; }
            set { dataLabelFormat = value; }
        }

        private int width = 10;

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        private int height = 10;

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        private bool visible;

        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        private int shapeInt = 0;

        public int ShapeInt
        {
            get { return shapeInt; }
            set
            {
                shapeInt = value;
                this.TranslateCodeToShape(value);
            }
        }

        private string fillColor;

        public string FillColor
        {
            get { return fillColor; }
            set
            {
                fillColor = value;
            }
        }

        private string shape;

        public string Shape
        {
            get { return shape; }
            set { shape = value; }
        }

        private void TranslateCodeToShape(int code)
        {
            string shape = null;
            switch (code)
            {
                case 1:
                    shape = "Circle";
                    break;

                case 6:
                    shape = "Circle";
                    fillColor = "transparent";
                    break;

                case 2:
                    shape = "Rectangle";
                    break;

                case 3:
                    shape = "Triangle";
                    break;

                case 4:
                    shape = "InvertedTriangle";
                    break;

                case 0:
                default:
                    this.Visible = false;
                    break;
            }

            this.shape = shape;
        }

        public Marker(bool visible, int shapeCode)
        {
            this.Visible = visible;
            this.ShapeInt = shapeCode;
        }
    }

    internal class TSeries
    {
        private object dataSource = null;

        public object DataSource
        {
            get { return dataSource; }
            set { dataSource = value; }
        }

        private string yAxisName;

        public string YAxisName
        {
            get { return yAxisName; }
            set { yAxisName = value; }
        }

        private int rowIndex = int.MinValue;

        public int RowIndex
        {
            get { return rowIndex; }
            set { rowIndex = value; }
        }

        private string pointColorMapping = null;

        public string PointColorMapping
        {
            get { return pointColorMapping; }
            set { pointColorMapping = value; }
        }

        private string type = "Line";

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        private string dashArray = null;

        public string DashArray
        {
            get { return dashArray; }
            set { dashArray = value; }
        }

        private int width = 1;

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        private double opacity = 1;

        public double Opacity
        {
            get { return opacity; }
            set { opacity = value; }
        }

        private bool visible = true;

        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        private bool fillArea = false;

        public bool FillArea
        {
            get { return fillArea; }
            set { fillArea = value; }
        }

        private string fillColor = "transparent";

        public string FillColor
        {
            get { return fillColor; }
            set { fillColor = value; }
        }

        public string PenLabel
        {
            get { return penLabel; }
            set { penLabel = value; }
        }

        public string XName
        {
            get { return xName; }
            set { xName = value; }
        }

        public string YName
        {
            get { return yName; }
            set { yName = value; }
        }

        private Marker marker;

        public Marker Marker
        {
            get { return marker; }
            set { marker = value; }
        }

        private object penObject = null;
        private bool isTrendPen = false;
        private bool useSquare = false;
        private string penLabel;
        private string xName = "x";
        private string yName = "y";

        public object GetJsSeriesObject()
        {
            object objSeries = TInterop.ExecuteJavaScriptBase(@"new Object()");
            TInterop.ExecuteJavaScriptBase("$0.dataSource = $1", objSeries, this.DataSource);
            TInterop.ExecuteJavaScriptBase("$0.xName = $1", objSeries, this.xName);
            TInterop.ExecuteJavaScriptBase("$0.yName = $1", objSeries, this.yName);
            TInterop.ExecuteJavaScriptBase("$0.yAxisName = $1", objSeries, this.YAxisName);

            if (string.IsNullOrEmpty(this.PenLabel) == false)
                TInterop.ExecuteJavaScriptBase("$0.name = $1", objSeries, this.PenLabel);

            TInterop.ExecuteJavaScriptBase("$0.type = $1", objSeries, this.Type);

            if (string.IsNullOrEmpty(this.PointColorMapping) == false)
                TInterop.ExecuteJavaScriptBase("$0.pointColorMapping = true", this.PointColorMapping);

            // Pen Color
            string aux = this.Opacity.ToString(TCultureInfo.InvariantCulture);
            TInterop.ExecuteJavaScriptBase("$0.opacity = Number($1)", objSeries, aux);

            aux = this.Width.ToString(TCultureInfo.InvariantCulture);
            TInterop.ExecuteJavaScriptBase("$0.width = Number($1)", objSeries, aux);

            TInterop.ExecuteJavaScriptBase("$0.fill = 'transparent'", objSeries);
            if (string.IsNullOrEmpty(this.FillColor) == false)
                TInterop.ExecuteJavaScriptBase("$0.fill = $1", objSeries, this.FillColor);
            if (string.IsNullOrEmpty(this.DashArray) == false)
                TInterop.ExecuteJavaScriptBase("$0.dashArray = $1", objSeries, this.DashArray);

            TInterop.ExecuteJavaScriptBase("$0.visible = true", objSeries);
            if (this.Visible == false)
            {
                //TInterop.ExecuteJavaScriptBase("$0.visible = true", objSeries);
                TInterop.ExecuteJavaScriptBase("$0.fill = 'transparent'", objSeries);
                if (this.Marker != null) { this.Marker.Visible = false; }
            }

            // Marker
            if (this.Marker != null && this.Marker.Visible)
            {
                object objMarker = TInterop.ExecuteJavaScriptBase(@"new Object()");

                TInterop.ExecuteJavaScriptBase("$0.visible = true", objMarker);

                if (string.IsNullOrEmpty(this.marker.Shape) == false)
                    TInterop.ExecuteJavaScriptBase("$0.shape = $1", objMarker, this.marker.Shape);
                if (string.IsNullOrEmpty(this.marker.FillColor) == false)
                    TInterop.ExecuteJavaScriptBase("$0.fill = $1", objMarker, this.marker.FillColor);
                if (string.IsNullOrEmpty(this.marker.ImageUrl) == false)
                    TInterop.ExecuteJavaScriptBase("$0.imageUrl = $1", objMarker, this.marker.ImageUrl);
                if (string.IsNullOrEmpty(this.marker.DataLabelFormat) == false)
                {
                    object objDataLabel = TInterop.ExecuteJavaScriptBase(@"new Object()");
                    TInterop.ExecuteJavaScriptBase("$0.visible = false", objDataLabel);
                    TInterop.ExecuteJavaScriptBase("$0.template = $1", objDataLabel, this.marker.DataLabelFormat);
                    TInterop.ExecuteJavaScriptBase("$0.dataLabel = $1", objMarker, objDataLabel);
                }

                TInterop.ExecuteJavaScriptBase("$0.width = $1", objMarker, this.marker.Width);
                TInterop.ExecuteJavaScriptBase("$0.height = $1", objMarker, this.marker.Height);

                TInterop.ExecuteJavaScriptBase("$0.marker = $1", objSeries, objMarker);
            }
            // Animation
            object animationSeriesObj = TInterop.ExecuteJavaScriptBase(@"new Object()");
            TInterop.ExecuteJavaScriptBase("$0.enable = false", animationSeriesObj);
            TInterop.ExecuteJavaScriptBase("$0.animation = $1", objSeries, animationSeriesObj);

            return objSeries;
        }

        public TSeries(object dataSource, string type, string xName, string yName, string penLabel, Marker marker)
        {
            DataSource = dataSource;
            Type = type;
            XName = xName;
            YName = yName;
            PenLabel = penLabel;
            Marker = marker;
        }

        public TSeries(object dataSource, string type, object penObj, bool isTrendPen, bool useSquare, Marker marker, string yAxisName)
        {
            this.DataSource = dataSource;
            this.Type = type;
            this.Marker = marker;
            this.YAxisName = yAxisName;
            this.penObject = penObj;
            this.useSquare = useSquare;
            this.isTrendPen = isTrendPen;

            this.ParsePenObj(penObj, isTrendPen);
        }

        private void ParsePenObj(object penObj, bool isTrendPen)
        {
            if (isTrendPen)
            {
                this.ParseTrendPenObj(penObj);
                return;
            }

            this.ParseDrillingPenObj(penObj);
        }

        private void ParseTrendPenObj(object trendPenObj)
        {
            TrendPen trendPen = trendPenObj as TrendPen;
            this.penLabel = trendPen.PenLabelOutput;
            this.Visible = trendPen.Visible;

            string penSettings = trendPen.PenSettings;

            string[] settings = penSettings.Split(';');

            if (settings != null && settings.Length > 0)
                this.ParsePenSettings(settings);
        }

        private void ParseDrillingPenObj(object trendPenObj)
        {
            TrendPen trendPen = trendPenObj as TrendPen;
            this.penLabel = trendPen.PenLabelOutput;
            this.Visible = trendPen.Visible;

            string penSettings = trendPen.PenSettings;
            string[] settings = penSettings.Split(';');

            if (settings != null && settings.Length > 0)
                this.ParsePenSettings(settings);
        }

        private void ParsePenSettings(string[] settings)
        {
            foreach (string prop in settings)
            {
                if (prop.StartsWith("Stroke=", StringComparison.InvariantCultureIgnoreCase))
                {
                    string temp = prop.Substring("Stroke=".Length);
                    string colorAsText = prop.Substring("Stroke=".Length);

                    if (colorAsText.StartsWith("#"))
                    {
                        string tempColor = colorAsText.Substring(3);
                        tempColor = "#" + tempColor;
                        colorAsText = tempColor;
                        this.FillColor = colorAsText;
                        this.Marker.FillColor = colorAsText;
                    }
                }
                if (prop.StartsWith("Fill=", StringComparison.InvariantCultureIgnoreCase))
                {
                    this.Opacity = 0.5;
                    this.FillArea = true;
                    this.Type = "Area";
                    if (this.useSquare)
                        this.Type = "StepArea";
                }
                if (prop.StartsWith("Marker=", StringComparison.InvariantCultureIgnoreCase))
                {
                    string temp = prop.Substring("Marker=".Length);
                    int nTemp = -1;
                    try
                    {
                        nTemp = int.Parse(temp);
                    }
                    catch
                    {
                        nTemp = -1;
                    }

                    this.marker.Visible = true;
                    this.marker.ShapeInt = nTemp;
                }

                if (prop.StartsWith("StrokeDashArray=", StringComparison.InvariantCultureIgnoreCase))
                {
                    string temp = prop.Substring("StrokeDashArray=".Length);
                    this.DashArray = temp;
                }
                if (prop.StartsWith("StrokeThickness=", StringComparison.InvariantCultureIgnoreCase))
                {
                    string temp = prop.Substring("StrokeThickness=".Length);
                    int nTemp = 0;
                    try
                    {
                        nTemp = int.Parse(temp);
                    }
                    catch
                    {
                        nTemp = 1;
                    }

                    this.Width = nTemp;
                }
            }
        }
    }

    #endregion Auxiliary Classes

    public class SfChart : Canvas, IDisposable
    {
        #region Properties Copied From ChartPlotter

        private static int TICK_SIZE = 5;

        private TextBlock[] xLabels = null;
        private Line[] xTicks = null;

        private TextBlock[] yLabels = null;
        private Line[] yTicks = null;

        private Line[] xGridLines = null;

        private Line[] yGridLines = null;

        private TextBlock[] legendLabels = null;
        private Rectangle[] legendLines = null;

        public Func<int> GetPenCount = null;
        public Func<int, object> GetPenCol = null;
        public Func<object, IEnumerable<TrendPointInfo>> GetPoints = null;
        public Func<object, IEnumerable<TrendPointInfo>> GetPoints2 = null;
        public Func<bool, bool, List<object[]>> GetPensYScale = null;

        private List<LineGraph> pens = new List<LineGraph>();

        private List<YLabelGroup> yLabelsGroup = new List<YLabelGroup>();

        private List<object[]> lastPenYScales = new List<object[]>();

        private Dictionary<YLabelGroup, List<LineGraph>> dicLabelGroupToPen = new Dictionary<YLabelGroup, List<LineGraph>>();

        private Rectangle zoomRectangle = null;

        private long hasRecalcLayoutPendent = 0;

        internal object miGetMarkerSize = null;

        internal object miGetMarkerVisible = null;

        internal object miGetMarkerFill = null;

        internal object miGetMarkerTooltip = null;

        private TAnnotationsMenu annotationsMenu;

        public TAnnotationsMenu AnnotationsMenu
        {
            get { return annotationsMenu; }
            set { annotationsMenu = value; }
        }

        private TLegendWindow legendWindow;

        public TLegendWindow LegendWindow
        {
            get { return legendWindow; }
            set { legendWindow = value; }
        }

        private TLegendWindowBorder legendWindowBorder;

        public TLegendWindowBorder LegendWindowBorder
        {
            get { return legendWindowBorder; }
            set { legendWindowBorder = value; }
        }

        private YAxisInternal yAxis;

        public YAxisInternal YAxis
        {
            get { return yAxis; }
            set { yAxis = value; }
        }

        private TextBox yLabelEditable;

        public TextBox YLabelEditable
        {
            get { return yLabelEditable; }
            set { yLabelEditable = value; }
        }

        private TextBox xLabelEditable;

        public TextBox XLabelEditable
        {
            get { return xLabelEditable; }
            set { xLabelEditable = value; }
        }

        private XAxisInternal xAxis;

        public XAxisInternal XAxis
        {
            get { return xAxis; }
            set { xAxis = value; }
        }

        private TNavigationControl navigationControl;

        public TNavigationControl NavigationControl
        {
            get { return navigationControl; }
            set { navigationControl = value; }
        }

        public List<LineGraph> Pens
        {
            get { return this.pens; }
        }

        private Brush labelsBrush = Brushes.Black;

        public Brush LabelsBrush
        {
            get { return this.labelsBrush; }
            set
            {
                this.labelsBrush = value;

                string colorAsText = value.ToString();
                if (colorAsText.StartsWith("#"))
                {
                    string temp = colorAsText.Substring(3);
                    temp = "#" + temp;
                    colorAsText = temp;
                }

                this.primaryXAxis.LineStyleColor = colorAsText;
                this.primaryYAxis.LineStyleColor = colorAsText;

                //
                if (this.xLabels != null)
                {
                    foreach (TextBlock element in this.xLabels)
                        element.Foreground = value;
                }
                if (this.yLabels != null)
                {
                    foreach (TextBlock element in this.yLabels)
                        element.Foreground = value;
                }
                if (this.xTicks != null)
                {
                    foreach (Line element in this.xTicks)
                        element.Stroke = value;
                }
                if (this.yTicks != null)
                {
                    foreach (Line element in this.yTicks)
                        element.Stroke = value;
                }
                //

                this.RecalcLayout();
            }
        }

        private Brush gridLinesBrush = Brushes.Black;

        public Brush GridLinesBrush
        {
            get { return this.gridLinesBrush; }
            set
            {
                this.gridLinesBrush = value;

                string colorAsText = value.ToString();

                if (colorAsText.StartsWith("#"))
                {
                    string temp = colorAsText.Substring(3);
                    temp = "#" + temp;
                    colorAsText = temp;
                }

                this.primaryXAxis.MajorGridLinesColor = colorAsText;
                this.primaryYAxis.MajorGridLinesColor = colorAsText;

                //
                if (this.xGridLines != null)
                {
                    foreach (Line element in this.xGridLines)
                        element.Stroke = value;
                }
                if (this.yGridLines != null)
                {
                    foreach (Line element in this.yGridLines)
                        element.Stroke = value;
                }
                //

                this.RecalcLayout();
            }
        }

        private string gridLinesStroke = "";

        public string GridLinesStroke
        {
            get { return this.gridLinesStroke; }
            set
            {
                this.gridLinesStroke = (value == null ? "" : value).Trim();

                string[] split = value.Split(';');

                if (split.Length < 1)
                    return;

                foreach (string prop in split)
                {
                    if (prop.StartsWith("StrokeDashArray=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        string temp = prop.Substring("StrokeDashArray=".Length);
                        this.primaryXAxis.MajorGridLinesDashArray = temp;
                        this.primaryYAxis.MajorGridLinesDashArray = temp;
                    }
                    if (prop.StartsWith("StrokeThickness=", StringComparison.InvariantCultureIgnoreCase))
                    {
                        string temp = prop.Substring("StrokeThickness=".Length);
                        int nTemp = 0;
                        try
                        {
                            nTemp = int.Parse(temp);
                        }
                        catch
                        {
                            nTemp = 1;
                        }

                        this.primaryXAxis.MajorGridLinesWidth = 0;
                        this.primaryYAxis.MajorGridLinesWidth = nTemp;
                    }
                }

                //
                if (this.xGridLines != null)
                {
                    foreach (Line element in this.xGridLines)
                    {
                        this.SetLineStroke(element, this.gridLinesStroke);
                    }
                }
                if (this.yGridLines != null)
                {
                    foreach (Line element in this.yGridLines)
                    {
                        this.SetLineStroke(element, this.gridLinesStroke);
                    }
                }
                //

                this.RecalcLayout();
            }
        }

        private Base.eLegendPlacement legendPlacement = Base.eLegendPlacement.None;

        public Base.eLegendPlacement LegendPlacement
        {
            get { return this.legendPlacement; }
            set
            {
                this.legendPlacement = value;
                if (this.typeChart == TypeChart.DrillingChart)
                    this.legendPlacement = Base.eLegendPlacement.None;

                this.RecalcLayout();
            }
        }

        private double renderedHeight;

        public double RenderedHeight
        {
            get { return renderedHeight; }
            set { renderedHeight = value; this.Height = value; }
        }

        private double renderedWidth;

        public double RenderedWidth
        {
            get { return renderedWidth; }
            set { renderedWidth = value; this.Width = value; }
        }

        private bool enableMouseActions = false;

        public bool EnableMouseActions
        {
            get { return this.enableMouseActions; }
            set
            {
                if (this.enableMouseActions == value)
                    return;
                this.enableMouseActions = value;
            }
        }

        private bool enableMouseCursorConnection = false;

        public bool EnableMouseCursorConnection
        {
            get { return this.enableMouseCursorConnection; }
            set
            {
                if (this.enableMouseCursorConnection == value)
                    return;
                this.enableMouseCursorConnection = value;
            }
        }

        private bool mergeSameYScales = false;

        public bool MergeSameYScales
        {
            get { return this.mergeSameYScales; }
            set
            {
                if (this.mergeSameYScales == value)
                    return;
                this.mergeSameYScales = value;
                this.RecalcLayout();
            }
        }

        private bool onlyZoomXAxis = false;

        public bool OnlyZoomXAxis
        {
            get { return this.onlyZoomXAxis; }
            set { this.onlyZoomXAxis = value; }
        }

        private bool canNextZoom = false;

        public bool CanNextZoom
        {
            get { return this.canNextZoom; }
            set { this.canNextZoom = value; }
        }

        private bool canBackZoom = false;

        public bool CanBackZoom
        {
            get { return this.canBackZoom; }
            set { this.canBackZoom = value; }
        }

        private bool showRefreshButton = true;

        public bool ShowRefreshButton
        {
            get { return this.showRefreshButton; }
            set { this.showRefreshButton = value; }
        }

        private bool needsRecreateChartForStackPens = false;

        public bool NeedsRecreateChartForStackPens
        {
            get { return this.needsRecreateChartForStackPens; }
            set { this.needsRecreateChartForStackPens = value; }
        }

        private int expectedPensCount = 0;

        public int ExpectedPensCount
        {
            get { return this.expectedPensCount; }
            set { this.expectedPensCount = value; }
        }

        private bool yLabelsEditable = true;

        public bool YLabelsEditable
        {
            get { return this.yLabelsEditable; }
            set
            {
                if (this.yLabelsEditable == value)
                    return;

                this.yLabelsEditable = value;
            }
        }

        private bool xLabelsEditable = true;

        public bool XLabelsEditable
        {
            get { return this.xLabelsEditable; }
            set
            {
                if (this.xLabelsEditable == value)
                    return;

                this.xLabelsEditable = value;
            }
        }

        private bool isEnabledYScaleForEachPen = false;

        public bool IsEnabledYScaleForEachPen
        {
            get { return this.isEnabledYScaleForEachPen; }
            set
            {
                if (this.isEnabledYScaleForEachPen == value)
                    return;
                this.isEnabledYScaleForEachPen = value;

                if (this.yLabels != null)
                {
                    foreach (FrameworkElement el in this.yLabels)
                        this.YAxis.Children.Remove(el);
                    this.yLabels = null;
                }

                this.RecalcLayout();
            }
        }

        private int showDuration = 0;

        public int ShowDuration
        {
            get { return this.showDuration; }
            set
            {
                if (this.showDuration == value)
                    return;
                this.showDuration = value;
                this.RecalcLayout();
            }
        }

        private int orientation = 0;
        /// <summary>
        /// 0 -> Horizontal
        /// 1 -> Vertical Top to Bottom
        /// 2 -> Vertical Bottom to Top
        /// </summary>

        public int Orientation
        {
            get { return this.orientation; }
            set
            {
                if (this.orientation == value)
                    return;

                switch (value)
                {
                    case 1:
                        this.orientationMode = OrientationMode.VerticalTopToBottom;
                        this.isTransposed = true;
                        this.primaryXAxis.IsInversed = true;
                        this.primaryYAxis.IsInversed = false;
                        if (this.NavigationControl != null)
                            this.NavigationControl.Orientation = System.Windows.Controls.Orientation.Vertical;
                        break;

                    case 2:
                        this.orientationMode = OrientationMode.VerticalBottomToTop;
                        this.isTransposed = true;
                        this.primaryXAxis.IsInversed = false;
                        this.primaryYAxis.IsInversed = false;
                        if (this.NavigationControl != null)
                            this.NavigationControl.Orientation = System.Windows.Controls.Orientation.Vertical;
                        break;

                    case 0:
                    default:
                        this.orientationMode = OrientationMode.Horizontal;
                        this.isTransposed = false;
                        this.primaryXAxis.IsInversed = false;
                        this.primaryYAxis.IsInversed = false;
                        if (this.NavigationControl != null)
                            this.NavigationControl.Orientation = System.Windows.Controls.Orientation.Horizontal;
                        break;
                }

                this.orientation = value;
                this.RecalcLayout();
            }
        }

        private int chartType = 0;

        public int ChartType
        {
            get { return this.chartType; }
            set
            {
                if (this.chartType == value)
                    return;
                this.chartType = value;
                this.RecalcLayout();
            }
        }

        private Brush cursorColor = Brushes.Black;

        public Brush CursorColor
        {
            get { return this.cursorColor; }
            set
            {
                this.cursorColor = value;
            }
        }

        private string cursorLine = "";

        public string CursorLine
        {
            get
            {
                return this.cursorLine;
                //string str = "";
                //try
                //{
                //    string val = "";

                //    val = TConvert.To<String>(this.LineCursor.StrokeDashArray);
                //    if (!String.IsNullOrEmpty(val))
                //        str += (str.Length > 0 ? ";" : "") + "StrokeDashArray=" + val;

                //    //val = TConvert.To<String>(this.LineCursor.StrokeDashCap);
                //    //if (!String.IsNullOrEmpty(val))
                //    //    str += (str.Length > 0 ? ";" : "") + "StrokeDashCap=" + val;

                //    val = TConvert.To<String>(this.LineCursor.StrokeDashOffset);
                //    if (!String.IsNullOrEmpty(val))
                //        str += (str.Length > 0 ? ";" : "") + "StrokeDashOffset=" + val;

                //    val = TConvert.To<String>(this.LineCursor.StrokeEndLineCap);
                //    if (!String.IsNullOrEmpty(val))
                //        str += (str.Length > 0 ? ";" : "") + "StrokeEndLineCap=" + val;

                //    val = TConvert.To<String>(this.LineCursor.StrokeLineJoin);
                //    if (!String.IsNullOrEmpty(val))
                //        str += (str.Length > 0 ? ";" : "") + "StrokeLineJoin=" + val;

                //    val = TConvert.To<String>(this.LineCursor.StrokeMiterLimit);
                //    if (!String.IsNullOrEmpty(val))
                //        str += (str.Length > 0 ? ";" : "") + "StrokeMiterLimit=" + val;

                //    val = TConvert.To<String>(this.LineCursor.StrokeThickness);
                //    if (!String.IsNullOrEmpty(val))
                //        str += (str.Length > 0 ? ";" : "") + "StrokeThickness=" + val;

                //    return str;
                //}
                //catch
                //{
                //}
                //return str;
            }
            set
            {
                this.cursorLine = value;
                //if (string.IsNullOrEmpty(value))
                //    this.LineCursor.StrokeThickness = 2;

                //string val = "";
                //val = TString.GetAttribute("StrokeDashArray", value);
                //if (!String.IsNullOrEmpty(val))
                //    this.LineCursor.StrokeDashArray = TConvert.To<DoubleCollection>(val);
                ////val = TString.GetAttribute("StrokeDashCap", value);
                ////if (!String.IsNullOrEmpty(val))
                ////    this.LineCursor.StrokeDashCap = TConvert.To<PenLineCap>(val);
                //val = TString.GetAttribute("StrokeDashOffset", value);
                //if (!String.IsNullOrEmpty(val))
                //    this.LineCursor.StrokeDashOffset = TConvert.To<Double>(val);
                //val = TString.GetAttribute("StrokeEndLineCap", value);
                //if (!String.IsNullOrEmpty(val))
                //    this.LineCursor.StrokeEndLineCap = TConvert.To<PenLineCap>(val);
                //val = TString.GetAttribute("StrokeLineJoin", value);
                //if (!String.IsNullOrEmpty(val))
                //    this.LineCursor.StrokeLineJoin = TConvert.To<PenLineJoin>(val);
                //val = TString.GetAttribute("StrokeMiterLimit", value);
                //if (!String.IsNullOrEmpty(val))
                //    this.LineCursor.StrokeMiterLimit = TConvert.To<Double>(val);
                //val = TString.GetAttribute("StrokeStartLineCap", value);
                //if (!String.IsNullOrEmpty(val))
                //    this.LineCursor.StrokeStartLineCap = TConvert.To<PenLineCap>(val);
                //val = TString.GetAttribute("StrokeThickness", value);
                //if (!String.IsNullOrEmpty(val))
                //    this.LineCursor.StrokeThickness = TConvert.To<Double>(val);
                //else
                //    this.LineCursor.StrokeThickness = 2;
            }
        }

        private Brush secondaryCursorColor = Brushes.Black;

        public Brush SecondaryCursorColor
        {
            get { return this.secondaryCursorColor; }
            set
            {
                this.secondaryCursorColor = value;
            }
        }

        private string secondaryCursorLine = "";

        public string SecondaryCursorLine
        {
            get
            {
                return this.secondaryCursorLine;
            }
            set
            {
                this.secondaryCursorLine = value;
            }
        }

        private int numOfLabelsX = 0;

        public int NumOfLabelsX
        {
            get { return this.numOfLabelsX; }
            set
            {
                if (value < 0 || this.numOfLabelsX == value)
                    return;
                this.numOfLabelsX = value;
                this.RecalcLayout();
            }
        }

        private int numOfLabelsY = 0;

        public int NumOfLabelsY
        {
            get { return this.numOfLabelsY; }
            set
            {
                if (value < 0 || this.numOfLabelsY == value)
                    return;
                this.numOfLabelsY = value;
                this.RecalcLayout();
            }
        }

        private bool disableOutputLabels = false;

        public bool DisableOutputLabels
        {
            get { return this.disableOutputLabels; }
            set { this.disableOutputLabels = value; }
        }

        private bool disableOutputSubYDivisions = false;

        public bool DisableOutputSubYDivisions
        {
            get { return this.disableOutputSubYDivisions; }
            set { this.disableOutputSubYDivisions = value; }
        }

        private bool disableOutputSubXDivisions = false;

        public bool DisableOutputSubXDivisions
        {
            get { return this.disableOutputSubXDivisions; }
        }

        private int subXDivisions = 0;

        public int SubXDivisions
        {
            get { return this.subXDivisions; }
            set
            {
                if (value < 0 || this.subXDivisions == value)
                    return;
                this.subXDivisions = value;

                this.RecalcLayout();
            }
        }

        private int subYDivisions = 0;

        public int SubYDivisions
        {
            get { return this.subYDivisions; }
            set
            {
                if (value < 0 || this.subYDivisions == value)
                    return;
                this.subYDivisions = value;

                this.RecalcLayout();
            }
        }

        private string labelFormatX = "*";

        public string LabelFormatX
        {
            get
            {
                return this.labelFormatX;
            }
            set
            {
                string _value = value;

                bool oldDisableOutputSubXDivisions = this.disableOutputSubXDivisions;

                this.disableOutputSubXDivisions = false;
                try
                {
                    if (!string.IsNullOrEmpty(_value) && (_value.EndsWith(" x", StringComparison.OrdinalIgnoreCase) || _value.EndsWith(" _", StringComparison.OrdinalIgnoreCase)))
                    {
                        this.disableOutputSubXDivisions = true;
                        _value = _value.Remove(_value.Length - 2).Trim();
                    }
                }
                catch (Exception ex)
                {
                    this.disableOutputSubXDivisions = false;
                    TException.Log(ex);
                }

                if (_value == this.LabelFormatX && oldDisableOutputSubXDivisions == this.disableOutputSubXDivisions)
                    return;

                this.labelFormatX = _value;

                this.RecalcLayout();
            }
        }

        private string labelFormatY = "N02";

        public string LabelFormatY
        {
            get { return this.labelFormatY; }
            set
            {
                bool oldDisableOutputSubYDivisions = this.disableOutputSubYDivisions;

                this.disableOutputSubYDivisions = false;

                string _value = value;
                try
                {
                    if (!string.IsNullOrEmpty(_value) && (_value.EndsWith(" x", StringComparison.OrdinalIgnoreCase) || _value.EndsWith("_", StringComparison.OrdinalIgnoreCase)))
                    {
                        this.disableOutputSubYDivisions = true;
                        _value = _value.Remove(_value.Length - 2).Trim();
                    }
                }
                catch (Exception ex)
                {
                    this.disableOutputSubYDivisions = false;
                    TException.Log(ex);
                }

                if (_value == this.LabelFormatY && this.disableOutputSubYDivisions == oldDisableOutputSubYDivisions)
                    return;

                this.labelFormatY = _value;

                this.RecalcLayout();
            }
        }

        private double yPadding = double.NaN;

        public double YPadding
        {
            get { return this.yPadding; }
            set
            {
                if (value == yPadding)
                    return;
                this.yPadding = value;
                this.primaryYAxis.LabelPadding = value;

                this.RecalcLayout();
            }
        }

        private double xPadding = double.NaN;

        public double XPadding
        {
            get { return this.xPadding; }
            set
            {
                if (value == xPadding)
                    return;
                this.xPadding = value;
                this.primaryXAxis.LabelPadding = XPadding;

                this.RecalcLayout();
            }
        }

        private double yMin = 0;

        public double YMin
        {
            get { return this.yMin; }
            set
            {
                if (value == yMin)
                    return;
                this.yMin = value;

                this.RecalcLayout();
            }
        }

        private double yMax = 100;

        public double YMax
        {
            get { return this.yMax; }
            set
            {
                if (value == yMax)
                    return;
                this.yMax = value;

                this.RecalcLayout();
            }
        }

        private double xMinValue = 0;

        public double XMinValue
        {
            get { return this.xMinValue; }
            set
            {
                if (value == xMinValue)
                    return;
                this.xMinValue = value;

                this.RecalcLayout();
            }
        }

        private double xMaxValue = 100;

        public double XMaxValue
        {
            get { return this.xMaxValue; }
            set
            {
                if (value == xMaxValue)
                    return;
                this.xMaxValue = value;

                this.RecalcLayout();
            }
        }

        private DateTime xMin;

        public DateTime XMin
        {
            get { return this.xMin; }
            set
            {
                if (value == xMin)
                    return;
                this.xMin = value;

                this.RecalcLayout();
            }
        }

        private DateTime xMax;

        public DateTime XMax
        {
            get { return this.xMax; }
            set
            {
                if (value == xMax)
                    return;
                this.xMax = value;

                this.RecalcLayout();
            }
        }

        private Point cursorPosition = new Point(0, 0);

        public Point CursorPosition
        {
            get
            {
                double pointX = this.GetEscalonatedCursorPositionPoint(this.cursorPositionX, true);
                double pointY = this.GetEscalonatedCursorPositionPoint(this.cursorPositionY, false);
                return new Point(pointX, pointY);
            }
            set
            {
                if (this.cursorPosition.X == value.X && this.cursorPosition.Y == value.Y)
                    return;

                double escalonatedX = Math.Min(100, this.GetEscalonatedCursorPositionPoint(value.X, true));
                this.cursorPosition = new Point(escalonatedX, 0);

                this.UpdatePrimaryCursor();
                if (this.PositionChanged != null)
                    this.PositionChanged(this, new GenericObjectEventArgs(escalonatedX, null, null));
            }
        }

        private double cursorPositionX = 100;

        public double CursorPositionX
        {
            get { return this.cursorPositionX; }
            set
            {
                double pos = value;

                Point point = new Point(pos, 0);
                if (this.cursorPositionX == pos)
                    return;

                point.X = pos;
                this.cursorPositionX = value;
                this.CursorPosition = point;
            }
        }

        private double cursorPositionY = 0;

        public double CursorPositionY
        {
            get
            { return this.cursorPositionY; }
            set
            {
                double pos = value;

                Point point = new Point(this.cursorPositionX, pos);
                if (this.cursorPositionY == pos)
                    return;

                point.Y = pos;
                this.cursorPositionY = pos;
            }
        }

        private Point secondaryCursorPosition = new Point(0, 0);

        public Point SecondaryCursorPosition
        {
            get
            {
                double pointX = this.GetEscalonatedCursorPositionPoint(this.secondaryCursorPositionX, true);
                double pointY = this.GetEscalonatedCursorPositionPoint(this.secondaryCursorPositionY, false);
                return new Point(pointX, pointY);
            }
            set
            {
                if (this.secondaryCursorPosition.X == value.X && this.secondaryCursorPosition.Y == value.Y)
                    return;

                double escalonatedX = this.GetEscalonatedCursorPositionPoint(value.X, true);
                this.secondaryCursorPosition = new Point(escalonatedX, 0);

                this.UpdateSecondaryCursor();
                if (this.PositionChanged2 != null)
                    this.PositionChanged2(this, new GenericObjectEventArgs(escalonatedX, null, null));
            }
        }

        private double secondaryCursorPositionX = 0;

        public double SecondaryCursorPositionX
        {
            get { return this.secondaryCursorPositionX; }
            set
            {
                double pos = value;

                Point point = new Point(pos, 0);
                if (this.secondaryCursorPositionX == pos)
                    return;

                point.X = pos;
                this.secondaryCursorPositionX = pos;
                this.SecondaryCursorPosition = point;
            }
        }

        private double secondaryCursorPositionY = 0;

        public double SecondaryCursorPositionY
        {
            get { return this.secondaryCursorPositionY; }
            set
            {
                double pos = value;

                Point point = new Point(this.secondaryCursorPositionX, pos);
                if (this.secondaryCursorPositionY == pos)
                    return;

                point.Y = pos;
                this.secondaryCursorPositionY = pos;
            }
        }

        private TimeSpan intervalBetweenCursors = new TimeSpan(0, 0, 1);

        public TimeSpan IntervalBetweenCursors
        {
            get { return this.intervalBetweenCursors; }
            set
            {
                this.intervalBetweenCursors = value;
            }
        }

        private bool enabledCursor = true;

        public bool EnabledCursor
        {
            get
            {
                return this.enabledCursor;
            }
            set
            {
                if (this.enabledCursor == value)
                    return;
                this.enabledCursor = value;
            }
        }

        private bool enabledSecondaryCursor = true;

        public bool EnabledSecondaryCursor
        {
            get
            {
                return this.enabledSecondaryCursor;
            }
            set
            {
                if (this.enabledSecondaryCursor == value)
                    return;
                this.enabledSecondaryCursor = value;
            }
        }

        private bool enableNavigationControl = false;

        public bool EnableNavigationControl
        {
            get { return this.enableNavigationControl; }
            set
            {
                this.enableNavigationControl = value;

                if (this.NavigationControl == null)
                    return;

                this.NavigationControl.Visibility = this.enableNavigationControl ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private bool isExpanded = false;

        public bool IsExpanded
        {
            get { return this.isExpanded; }
            set
            {
                if (this.isExpanded == value)
                    return;

                this.isExpanded = value;
                this.ExpandChanged(null, null);
            }
        }

        private Rect chartArea;

        public Rect ChartArea
        {
            get { return this.chartArea; }
            set
            {
                //if (this.chartArea == value)
                //    return;

                this.chartArea = value;
                if (this.ViewportPropertyChanged != null)
                    this.ViewportPropertyChanged(null, null);
            }
        }

        private Rect ChartBorderArea;

        private bool cursorOutputTooltip = false;

        public bool CursorOutputTooltip
        {
            get { return this.cursorOutputTooltip; }
            set
            {
                if (this.cursorOutputTooltip == value)
                    return;

                this.cursorOutputTooltip = value;
                this.RecalcLayout();
            }
        }

        private bool secondaryCursorOutputTooltip = false;

        public bool SecondaryCursorOutputTooltip
        {
            get { return this.secondaryCursorOutputTooltip; }
            set
            {
                if (this.secondaryCursorOutputTooltip == value)
                    return;

                this.secondaryCursorOutputTooltip = value;
                this.RecalcLayout();
            }
        }

        private bool showPrimaryCursorValues = false;

        public bool ShowPrimaryCursorValues
        {
            get { return this.showPrimaryCursorValues; }
            set
            {
                if (this.showPrimaryCursorValues == value)
                    return;

                this.showPrimaryCursorValues = value;
                this.RecalcLayout();
            }
        }

        private bool showSecondaryCursorValues = false;

        public bool ShowSecondaryCursorValues
        {
            get { return this.showSecondaryCursorValues; }
            set
            {
                if (this.showSecondaryCursorValues == value)
                    return;

                this.showSecondaryCursorValues = value;
                this.RecalcLayout();
            }
        }

        private bool showPrimaryCloseButton = false;

        public bool ShowPrimaryCloseButton
        {
            get { return this.showPrimaryCursorValues; }
            set
            {
                if (this.showPrimaryCloseButton == value)
                    return;

                this.showPrimaryCloseButton = value;
                this.RecalcLayout();
            }
        }

        private bool showSecondaryCloseButton = false;

        public bool ShowSecondaryCloseButton
        {
            get { return this.showSecondaryCloseButton; }
            set
            {
                if (this.showSecondaryCloseButton == value)
                    return;

                this.showSecondaryCloseButton = value;
                this.RecalcLayout();
            }
        }

        private bool enableMarkerTooltip = false;

        public bool EnableMarkerTooltip
        {
            get { return this.enableMarkerTooltip; }
            set
            {
                this.enableMarkerTooltip = value;
                this.RecalcLayout();
            }
        }

        private CoordinateTransform transform = null;

        public CoordinateTransform Transform
        {
            get { return this.transform; }
        }

        private Rect output;

        public Rect Output
        {
            get { return output; }
            set { output = value; }
        }

        private object instanceOfContentsJS;

        public object InstanceOfContentsJS
        {
            get { return instanceOfContentsJS; }
            set { instanceOfContentsJS = value; }
        }

        private object instanceOfContents;

        public object InstanceOfContents
        {
            get { return instanceOfContents; }
            set { instanceOfContents = value; }
        }

        private bool isXYChart = false;
        internal bool IsXYChart
        {
            get { return isXYChart; }
            set
            {
                isXYChart = value;
                if (this.primaryXAxis == null)
                    return;
                this.primaryXAxis.IsXAxis = true;
                if(isXYChart)
                {
                    this.primaryXAxis.IsXAxisNumeric = true;
                    this.primaryXAxis.UpdateXMinAndXMax(this.XMinValue, this.XMaxValue);
                }
            }
        }

        private bool mousePressed = false;
        private Point startPosition = new Point();
        private bool scrollX = false;

        private Point zoomStartPosition = new Point(-1, -1);

        public delegate void PositionChangedEventHandler(object sender, GenericObjectEventArgs e);

        public event PositionChangedEventHandler PositionChanged = null;

        public event PositionChangedEventHandler PositionChanged2 = null;

        public delegate void CursorValueChangedEventHandler(object sender, GenericObjectEventArgs e);

        public event CursorValueChangedEventHandler CursorValueChanged = null;

        public event CursorValueChangedEventHandler CursorValueChanged2 = null;

        public delegate void CursorOutputChangedEventHandler(object sender, GenericObjectEventArgs e);

        public event CursorOutputChangedEventHandler CursorOutputChanged = null;

        public event CursorOutputChangedEventHandler CursorOutputChanged2 = null;

        public delegate void VisibleRectChangedEventHandler(object sender, VisibleRectEventArgs e);

        public event VisibleRectChangedEventHandler VisibleRectChanged = null;

        public delegate void ExpandedChangedEventHandler(object sender, GenericObjectEventArgs e);

        public event ExpandedChangedEventHandler ExpandChanged = null;

        public delegate void AnnotationsMenuOpeningEventHandler(object sender, GenericObjectEventArgs e);

        public event AnnotationsMenuOpeningEventHandler AnnotationsContextMenuOpening = null;

        public delegate void NavigationScrollLeftEventHandler(object sender, GenericObjectEventArgs e);

        public event NavigationScrollLeftEventHandler NavControlScrollLeft = null;

        public delegate void NavigationScrollRightEventHandler(object sender, GenericObjectEventArgs e);

        public event NavigationScrollRightEventHandler NavControlScrollRight = null;

        public delegate void NavigationZoomOutEventHandler(object sender, GenericObjectEventArgs e);

        public event NavigationZoomOutEventHandler NavControlZoomOut = null;

        public delegate void NavigationZoomInEventHandler(object sender, GenericObjectEventArgs e);

        public event NavigationZoomInEventHandler NavControlZoomIn = null;

        public delegate void ViewportPropertyChangedEventHandler(object sender, VisibleRectEventArgs e);

        public event ViewportPropertyChangedEventHandler ViewportPropertyChanged = null;

        public delegate void RaiseDataChangedEventHandler(object sender, GenericObjectEventArgs e);

        public event RaiseDataChangedEventHandler RaiseDataChanged = null;

        #endregion Properties Copied From ChartPlotter

        #region Dependencies for HTML5

        private static string[] baseDependenciesCSS = new string[]
           {
            "base",
            "buttons",
            "calendars",
            "dropdowns",
            "grids",
            "inputs",
            "lists",
            "navigations",
            "popups",
            "splitbuttons",
           };

        private static string[] baseDependenciesJS = new string[]
        {
            "base",
            "charts",
            "data",
            "pdf-export",
            "file-utils",
            "compression",
            "svg-base",
        };

        private static bool dependenciesLoaded = false;
        private static bool firstTime = true;
        private bool isReady = false;

        public static async Task LoadDependencies()
        {
            if (!dependenciesLoaded)
            {
                try
                {
                    dependenciesLoaded = true;

                    List<string> cssList = new List<string>();
                    foreach (string str in baseDependenciesCSS)
                    {
                        string scriptFile = "Libraries/ej2-" + str + "/styles/material.css";
                        cssList.Add(scriptFile);
                    }
                    await (Application.Current as THTML5Client.App).MainPage.LoadCSSFileNames(cssList.ToArray());

                    List<string> jsList = new List<string>();
                    jsList.Add("Libraries/ej2/dist/ej2.min.js");
                    foreach (string str in baseDependenciesJS)
                    {
                        string scriptFile = "Libraries/ej2-" + str + "/dist/global/ej2-" + str + ".min.js";
                        jsList.Add(scriptFile);
                    }
                    await (Application.Current as THTML5Client.App).MainPage.LoadJSFileNames(jsList.ToArray());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(TConvert.ToString(DateTime.Now) + " :: Error while downloading TrendChart dependencies: " + ex.Message);
                    dependenciesLoaded = false;
                }
            }
        }

        #endregion Dependencies for HTML5

        private TypeChart typeChart = TypeChart.TrendChart;

        private OrientationMode orientationMode = OrientationMode.Horizontal;

        private object chart = null;

        private bool isTransposed = false;

        private TAxis primaryXAxis = null;
        private TAxis primaryYAxis = null;

        private List<TSeries> listOfChartSeries = null;
        private List<TAxis> listOfChartAxes = null;

        private TAnnotations primaryCursorValuesAnnotation = null;
        private TAnnotations primaryCursorLineAnnotation = null;
        private TAnnotations primaryCursorTimestampAnnotation = null;

        private TAnnotations secondaryCursorValuesAnnotation = null;
        private TAnnotations secondaryCursorLineAnnotation = null;
        private TAnnotations secondaryCursorTimestampAnnotation = null;

        private Dictionary<int, LineGraph> mapPenLabelToLineGraph = null;

        private Dictionary<object, LineGraph> mapLegendItemToPen = null;

        private string chartControlId = null;

        private bool primaryCursorClicked = false;
        private bool secondaryCursorClicked = false;

        private bool isChartMousePressed = false;

        //private Point pointChartMouseDown = new Point();
        private string chartZoomRectangleId = "chartZoomRectangle";

        private object chartZoomRectangle = null;

        private bool primaryCursorWasClosed = true;
        private bool secondaryCursorWasClosed = true;
        private bool axisLimitChanged = false;

        private double xAxisMin = 0.0;

        private bool legendSizeCalculated = false;
        private int initializationLoops = 0;
        private double legendWidth = double.NaN;
        private double legendHeight = double.NaN;

        private bool isStackPensMode = false;
        private bool previousIsStackPensMode = false;

        public bool IsStackPensMode
        {
            get { return this.isStackPensMode; }
            set
            {
                this.isStackPensMode = value;
            }
        }

        public enum TypeChart
        {
            TrendChart,
            DrillingChart
        }

        public enum OrientationMode
        {
            Horizontal,
            VerticalTopToBottom,
            VerticalBottomToTop
        }


        public SfChart()
        {
            this.xMax = DateTime.UtcNow;
            this.xMin = this.xMax.Subtract(TimeSpan.FromMinutes(1));

            TAxis _xAxis;
            if (this.IsXYChart)
                _xAxis = new TAxis(this.XMinValue, this.XMaxValue, true);
            else
                _xAxis = new TAxis(this.XMin, this.XMax);
            this.primaryXAxis = _xAxis;
            this.primaryXAxis.IsVisible = false;

            this.primaryYAxis = new TAxis(this.YMin, this.YMax, false);
            this.primaryYAxis.IsVisible = false;


            this.listOfChartSeries = new List<TSeries>();
            this.listOfChartAxes = new List<TAxis>();

            this.mapPenLabelToLineGraph = new Dictionary<int, LineGraph>();
            this.mapLegendItemToPen = new Dictionary<object, LineGraph>();

            this.transform = CoordinateTransform.CreateDefault();

            this.Loaded += SfChartPlotter_Loaded;
            this.Unloaded += SfChartPlotter_Unloaded;
        }

        public void Dispose()
        {
            this.XAxis.MouseLeftButtonDown -= this.XAxis_PointerPressed;
            this.XAxis.MouseLeftButtonUp -= this.Axis_PointerReleased;
            this.XAxis.MouseMove -= this.XAxis_PointerMoved;

            this.YAxis.MouseLeftButtonDown -= this.YAxis_PointerPressed;
            this.YAxis.MouseLeftButtonUp -= this.Axis_PointerReleased;
            this.YAxis.MouseMove -= this.YAxis_PointerMoved;
        }

        private async void SfChartPlotter_Loaded(object sender, RoutedEventArgs e)
        {         
            this.XAxis.MouseLeftButtonDown += this.XAxis_PointerPressed;
            this.XAxis.MouseLeftButtonUp += this.Axis_PointerReleased;
            this.XAxis.MouseMove += this.XAxis_PointerMoved;

            this.YAxis.MouseLeftButtonDown += this.YAxis_PointerPressed;
            this.YAxis.MouseLeftButtonUp += this.Axis_PointerReleased;
            this.YAxis.MouseMove += this.YAxis_PointerMoved;

            if (firstTime)
            {
                firstTime = false;
                Console.WriteLine(TConvert.ToString(DateTime.Now) + " :: TrendChart initialization");
            }

            await LoadDependencies();
            this.DiscoverTypeOfChart();          
        }

        private void SfChartPlotter_Unloaded(object sender, RoutedEventArgs e)
        {
            this.DestroyChart();
        }

        private void DiscoverTypeOfChart()
        {
            bool shouldContinue = true;
            int idx = 0;
            string className;
            object div = OpenSilver.Interop.GetDiv(this);
            object parentNode = TInterop.ExecuteJavaScriptBase("$0.parentNode", div);

            string trendChartClass = "TrendChartPlotter".ToLower();
            string drillingChartClass = "DrillingChartPlotter".ToLower();

            while (shouldContinue)
            {
                className = TConvert.ToString(TInterop.ExecuteJavaScriptReturnNET("$0.className", parentNode)).ToLower();
                if (className.Contains(trendChartClass))
                {
                    shouldContinue = false;
                    this.typeChart = TypeChart.TrendChart;
                    break;
                }
                else if (className.Contains(drillingChartClass))
                {
                    shouldContinue = false;
                    this.typeChart = TypeChart.DrillingChart;
                    break;
                }
                parentNode = TInterop.ExecuteJavaScriptBase("$0.parentNode", parentNode);
                if (idx > 15)
                {
                    parentNode = null;
                    shouldContinue = false;
                }
                idx++;
            }

            return;
        }

         public void CreateChart()
        {
            try
            {
                bool stackPensModeChanged = this.previousIsStackPensMode != this.isStackPensMode;

                int visiblePensCount = 0;
                for (int i = 0; i < this.GetPenCount(); i++)
                {
                    if (this.pens[i].Visibility == Visibility.Visible)
                        visiblePensCount++;
                }

                if (stackPensModeChanged && this.chart != null)
                    this.DestroyChart();

                bool chartDontExist = this.chart == null;
                double opacityToApply = this.initializationLoops < 3 ? 0.0 : 1.0;
                this.Opacity = opacityToApply;
                this.XAxis.Opacity = opacityToApply;
                this.YAxis.Opacity = opacityToApply;
                this.LegendWindowBorder.Opacity = opacityToApply;
                this.LegendWindow.Opacity = opacityToApply;

                #region CREATE DIVS FOR INITIALIZATION

                try
                {
                    if (chartDontExist)
                    {
                        object div = OpenSilver.Interop.GetDiv(this);
                        if(div == null)
                        {
                            return;
                        }
                        this.chartControlId = TConvert.ToString(TInterop.ExecuteJavaScriptBase("$0.id ", div));

                        int pensCountForRows = this.needsRecreateChartForStackPens && this.expectedPensCount > 0
                            ? this.expectedPensCount
                            : visiblePensCount;

                        object rowsArray;
                        object columnsArray;

                        if (this.IsStackPensMode && pensCountForRows > 0)
                        {
                            if (this.orientationMode == OrientationMode.Horizontal)
                            {
                                rowsArray = TInterop.ExecuteJavaScriptBase(@"[]");
                                double heightPerPen = 100.0 / pensCountForRows;
                                string heightString = heightPerPen.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + "%";

                                for (int i = 0; i < pensCountForRows; i++)
                                {
                                    object rowObj = TInterop.ExecuteJavaScriptBase(@"new Object()");
                                    TInterop.ExecuteJavaScriptBase("$0.height = $1", rowObj, heightString);
                                    TInterop.ExecuteJavaScriptBase("$0.push($1)", rowsArray, rowObj);
                                }
                                columnsArray = TInterop.ExecuteJavaScriptBase(@"[{width:'100%'}]");
                            }
                            else
                            {
                                columnsArray = TInterop.ExecuteJavaScriptBase(@"[]");
                                double widthPerPen = 100.0 / pensCountForRows;
                                string widthString = widthPerPen.ToString("F2", System.Globalization.CultureInfo.InvariantCulture) + "%";

                                for (int i = 0; i < pensCountForRows; i++)
                                {
                                    object colObj = TInterop.ExecuteJavaScriptBase(@"new Object()");
                                    TInterop.ExecuteJavaScriptBase("$0.width = $1", colObj, widthString);
                                    TInterop.ExecuteJavaScriptBase("$0.push($1)", columnsArray, colObj);
                                }
                                rowsArray = TInterop.ExecuteJavaScriptBase(@"[{height:'100%'}]");
                            }
                        }
                        else
                        {
                            rowsArray = TInterop.ExecuteJavaScriptBase(@"[{height:'100%'}]");
                            columnsArray = TInterop.ExecuteJavaScriptBase(@"[{width:'100%'}]");
                        }

                        this.chart = TInterop.ExecuteJavaScriptBase(@"new ej.charts.Chart({
                            background : 'rgba(0, 0, 0, 0.0)',
                            enableCanvas: false,
                            width:'100%',
                            height:'100%',
                            rows: $0,
                            columns: $1,
                            legendSettings:{ visible:false },
                        })", rowsArray, columnsArray);

                        if (this.orientationMode != OrientationMode.Horizontal)
                            this.primaryXAxis.LabelFormat = this.primaryXAxis.LabelFormat.Replace(" ", " <br> ");

                        TInterop.ExecuteJavaScriptBase("$0.appendTo($1)", this.chart, "#" + this.chartControlId);

                        // Zoom Rectangle
                        this.chartZoomRectangle = TInterop.ExecuteJavaScriptBase("document.createElement('div')");
                        this.chartZoomRectangleId = TConvert.ToString(TInterop.ExecuteJavaScriptBase("$0.id", this.chartZoomRectangle));

                        TInterop.ExecuteJavaScriptBase("$0.style.zIndex = 9", this.chartZoomRectangle);
                        TInterop.ExecuteJavaScriptBase("$0.style.position = 'absolute'", this.chartZoomRectangle);
                        TInterop.ExecuteJavaScriptBase("$0.style.backgroundColor = 'rgba(86,86,86,0.5)'", this.chartZoomRectangle);
                        TInterop.ExecuteJavaScriptBase("$0.style.border = 'black'", this.chartZoomRectangle);
                        TInterop.ExecuteJavaScriptBase("$0.style.visibility = 'hidden'", this.chartZoomRectangle);
                        TInterop.ExecuteJavaScriptBase("document.getElementById($0).parentNode.appendChild($1)", this.chartControlId, this.chartZoomRectangle);

                        #region CHART EVENTS

                        try
                        {
                            TInterop.ExecuteJavaScriptBase("$0.selectionComplete = $1", this.chart, TInterop.CreateJavascriptCallback((Action<object>)this.OnSelectionComplete));

                            TInterop.ExecuteJavaScriptBase("$0.zoomComplete = $1", this.chart, TInterop.CreateJavascriptCallback((Action<object>)this.OnZoomCompleted));
                            TInterop.ExecuteJavaScriptBase("$0.onZooming = $1", this.chart, TInterop.CreateJavascriptCallback((Action<object>)this.OnZoomCompleted));
                            TInterop.ExecuteJavaScriptBase("document.getElementById($0).onmousedown = $1", this.chartControlId, TInterop.CreateJavascriptCallback((Action<object>)this.OnChartMouseDown));
                            TInterop.ExecuteJavaScriptBase("document.getElementById($0).onmouseup = $1", this.chartControlId, TInterop.CreateJavascriptCallback((Action<object>)this.OnChartMouseUp));
                            TInterop.ExecuteJavaScriptBase("document.getElementById($0).onmousemove = $1", this.chartControlId, TInterop.CreateJavascriptCallback((Action<object>)this.OnChartMouseMove));

                            TInterop.ExecuteJavaScriptBase("$0.resized = $1", this.chart, TInterop.CreateJavascriptCallback((Action<object>)this.OnChartResize));
                            TInterop.ExecuteJavaScriptBase("$0.sharedTooltipRender = $1", this.chart, TInterop.CreateJavascriptCallback((Action<object>)this.OnTooltipRendered));
                            TInterop.ExecuteJavaScriptBase("$0.tooltipRender = $1", this.chart, TInterop.CreateJavascriptCallback((Action<object>)this.OnTooltipRendered));
                            TInterop.ExecuteJavaScriptBase("$0.axisLabelRender = $1", this.chart, TInterop.CreateJavascriptCallback((Action<object>)this.OnLabelRendered));
                            TInterop.ExecuteJavaScriptBase("$0.pointRender = $1", this.chart, TInterop.CreateJavascriptCallback((Action<object>)this.OnPointRendered));
                            TInterop.ExecuteJavaScriptBase("document.getElementById($0).oncontextmenu = function (args) {args.preventDefault(); args.stopPropagation(); return false;}", this.chartControlId);
                            TInterop.ExecuteJavaScriptBase("window.oncontextmenu = function (args) {args.preventDefault(); args.stopPropagation(); return false;}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to apply chart events: {ex.Message}");
                        }

                        #endregion

                        #region CHART ZOOM

                        try
                        {
                            if (this.EnableNavigationControl || this.EnableMouseActions)
                            {
                                object zoomSettings = TInterop.ExecuteJavaScriptBase(@"new Object()");

                                if (this.OnlyZoomXAxis)
                                    TInterop.ExecuteJavaScriptBase("$0.mode = 'X'", zoomSettings);
                                TInterop.ExecuteJavaScriptBase("$0.enableSelectionZooming = false", zoomSettings);
                                TInterop.ExecuteJavaScriptBase("$0.enableScrollbar = false", zoomSettings);
                                TInterop.ExecuteJavaScriptBase("$0.enableMouseWheelZooming = false", zoomSettings);
                                TInterop.ExecuteJavaScriptBase("$0.toolbarItems = [ 'Zoom', 'ZoomIn', 'ZoomOut', 'Pan', 'Reset']", zoomSettings);
                                TInterop.ExecuteJavaScriptBase("$0.zoomSettings = $1", this.chart, zoomSettings);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to apply chart zoom config: {ex.Message}");
                        }

                        #endregion

                        #region CHART TOOLTIP

                        try
                        {
                            object tooltipSettings = TInterop.ExecuteJavaScriptBase(@"new Object()");
                            TInterop.ExecuteJavaScriptBase("$0.enableAnimation = false", tooltipSettings);
                            TInterop.ExecuteJavaScriptBase("$0.enable = true", tooltipSettings);
                            TInterop.ExecuteJavaScriptBase("$0.tooltip = $1", this.chart, tooltipSettings);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to apply chart tooltip config: {ex.Message}");
                        }

                        #endregion

#if DEBUG
                        /// TEMP -> Variable for easy debug in Browser console
                        OpenSilver.Interop.ExecuteJavaScript("window[$0] = $1", "sfChart" + this.chartControlId, this.chart);
#endif
                    }
                }
                catch
                {
                    this.chart = null;
                    return;
                }

                #endregion

                #region CHART SERIES (DATA)

                try
                {
                    TInterop.ExecuteJavaScriptBase("$0.isTransposed = false", this.chart);
                    if (this.isTransposed)
                        TInterop.ExecuteJavaScriptBase("$0.isTransposed = true", this.chart);

                    object chartSeries = TInterop.ExecuteJavaScriptBase(@"[]");
                    object objSeries;
                    if (this.listOfChartSeries != null && this.listOfChartSeries.Count > 0)
                    {
                        foreach (TSeries tSerie in this.listOfChartSeries)
                        {
                            objSeries = tSerie.GetJsSeriesObject();
                            TInterop.ExecuteJavaScriptBase("$0.push($1)", chartSeries, objSeries);
                        }
                    }
                    TInterop.ExecuteJavaScriptBase("$0.series = $1", this.chart, chartSeries);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to apply chart series config: {ex.Message}");
                }

                #endregion CHART SERIES (DATA)

                #region CHART ADDITIONAL AXES

                try
                {
                    object chartAxes = TInterop.ExecuteJavaScriptBase(@"[]");
                    object objAxes;
                    int idx = this.isStackPensMode ? 0 : 1;
                    if (this.listOfChartAxes != null && this.listOfChartAxes.Count > 0)
                    {
                        foreach (TAxis axes in this.listOfChartAxes)
                        {
                            int axisRowIndex = this.IsStackPensMode && axes.RowIndex != int.MinValue ? axes.RowIndex : idx;

                            string aux = axisRowIndex.ToString(TCultureInfo.InvariantCulture);
                            object serie = TInterop.ExecuteJavaScriptBase("$0.series[Number($1)]", this.chart, aux);
                            object seriesFillColor = null;
                            if (serie != null)
                                seriesFillColor = TInterop.ExecuteJavaScriptBase("$0.fill", serie);
                            if (seriesFillColor != null)
                                axes.LabelStyleColor = seriesFillColor.ToString();

                            objAxes = this.IsStackPensMode ? axes.CreateJsObject(axisRowIndex) : axes.CreateJsObject();
                            TInterop.ExecuteJavaScriptBase("$0.push($1)", chartAxes, objAxes);
                            idx++;
                        }
                    }
                    TInterop.ExecuteJavaScriptBase("$0.axes = $1", this.chart, chartAxes);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to apply chart additional axes config: {ex.Message}");
                }

                #endregion CHART ADDITIONAL AXES

                #region CHART ANNOTATIONS -> Show Values and Tooltip for selected point

                try
                {
                    object annObjects = TInterop.ExecuteJavaScriptBase(@"[]");

                    if (this.primaryCursorLineAnnotation != null && this.primaryCursorClicked)
                    {
                        object annObj = this.primaryCursorLineAnnotation.CreateJsObject();
                        TInterop.ExecuteJavaScriptBase("$0.push($1)", annObjects, annObj);
                    }
                    if (this.primaryCursorValuesAnnotation != null && this.primaryCursorClicked)
                    {
                        object annObj = this.primaryCursorValuesAnnotation.CreateJsObject();
                        TInterop.ExecuteJavaScriptBase("$0.push($1)", annObjects, annObj);
                    }
                    if (this.primaryCursorTimestampAnnotation != null && this.primaryCursorClicked)
                    {
                        object annObj = this.primaryCursorTimestampAnnotation.CreateJsObject();
                        TInterop.ExecuteJavaScriptBase("$0.push($1)", annObjects, annObj);
                    }

                    if (this.secondaryCursorLineAnnotation != null && this.secondaryCursorClicked)
                    {
                        object annObj = this.secondaryCursorLineAnnotation.CreateJsObject();
                        TInterop.ExecuteJavaScriptBase("$0.push($1)", annObjects, annObj);
                    }
                    if (this.secondaryCursorValuesAnnotation != null && this.secondaryCursorClicked)
                    {
                        object annObj = this.secondaryCursorValuesAnnotation.CreateJsObject();
                        TInterop.ExecuteJavaScriptBase("$0.push($1)", annObjects, annObj);
                    }
                    if (this.secondaryCursorTimestampAnnotation != null && this.secondaryCursorClicked)
                    {
                        object annObj = this.secondaryCursorTimestampAnnotation.CreateJsObject();
                        TInterop.ExecuteJavaScriptBase("$0.push($1)", annObjects, annObj);
                    }

                    TInterop.ExecuteJavaScriptBase("$0.annotations = $1", this.chart, annObjects);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to apply chart annotations config: {ex.Message}");
                }

                #endregion CHART ANNOTATIONS -> Show Values and Tooltip for selected point

                #region X AND Y AXIS

                try
                {
                    // Primary X Axis
                    object xAxis = this.primaryXAxis.CreateJsObject();
                    TInterop.ExecuteJavaScriptBase("$0.primaryXAxis = $1", this.chart, xAxis);

                    // Primary Y Axis
                    object yAxis;
                    if (this.isStackPensMode)
                    {
                        yAxis = this.primaryYAxis.CreateJsObject();
                        TInterop.ExecuteJavaScriptBase("$0.visible = false", yAxis);
                    }
                    else
                    {
                        yAxis = this.primaryYAxis.CreateJsObject();
                    }
                    TInterop.ExecuteJavaScriptBase("$0.primaryYAxis = $1", this.chart, yAxis);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to apply chart primary X and Y Axis config: {ex.Message}");
                }

                #endregion X AND Y AXIS

                #region CALCULATE INTERVAL BETWEEN CURSORS

                try
                {
                    bool stripLinesExists = this.primaryXAxis.StripLineLeftMouseClick != null && this.primaryXAxis.StripLineRightMouseClick != null;
                    TimeSpan intervalCursors;
                    if (stripLinesExists)
                    {
                        DateTime primarytDt = this.primaryXAxis.JavaTimeStampToDateTime(this.xAxisMin, this.primaryXAxis.StripLineLeftMouseClick.Start);
                        DateTime secondaryDt = this.primaryXAxis.JavaTimeStampToDateTime(this.xAxisMin, this.primaryXAxis.StripLineRightMouseClick.Start);

                        if (primarytDt > secondaryDt)
                            intervalCursors = primarytDt - secondaryDt;
                        else
                            intervalCursors = secondaryDt - primarytDt;

                        this.IntervalBetweenCursors = intervalCursors;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to calculate interval between cursors: {ex.Message}");
                }

                #endregion CALCULATE INTERVAL BETWEEN CURSORS

                #region CHART REFRESH

                try
                {
                    if(!this.IsXYChart)
                        this.RefreshChart();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to refresh chart: {ex.Message}");
                }

                #endregion CHART REFRESH

                #region CALCULATE CHART AREAS
                this.CalculateHTMLChartAreas();
                #endregion CALCULATE CHART AREAS               

                this.isReady = this.initializationLoops >= 3 || this.IsXYChart;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Initialize Exception: {ex.Message}");
            }

            this.axisLimitChanged = false;
            this.listOfChartAxes.Clear();
            this.listOfChartSeries.Clear();

            this.previousIsStackPensMode = this.isStackPensMode;

            this.initializationLoops += 1;
          
            if (this.initializationLoops <= 3)
            {
                this.RecalcLayout();
            }
        }

        private void CalculateHTMLChartAreas()
        {
            if (this.chart == null)
                return;

            try
            {
                this.xAxisMin = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.primaryXAxis.actualRange.min", this.chart));

                string svgComp = this.chartControlId + "_svg";
                object chartSvgArea = TInterop.ExecuteJavaScriptBase("document.getElementById($0)", svgComp);
                if (chartSvgArea != null)
                {
                    double areaX = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.x.baseVal.value", chartSvgArea));
                    double areaY = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.y.baseVal.value", chartSvgArea));
                    double areaHeight = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.height.baseVal.value", chartSvgArea));
                    double areaWidth = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.width.baseVal.value", chartSvgArea));

                    if (areaHeight < 0)
                        areaHeight = 0;
                    if (areaWidth < 0)
                        areaWidth = 0;

                    this.ChartBorderArea = new Rect(areaX, areaY, areaWidth, areaHeight);
                }

                string chartAreaBorderId = this.chartControlId + "_ChartAreaBorder";
                object chartPlottableArea = TInterop.ExecuteJavaScriptBase("document.getElementById($0)", chartAreaBorderId);

                if (chartPlottableArea != null)
                {
                    double areaX = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.x.baseVal.value", chartPlottableArea));
                    double areaY = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.y.baseVal.value", chartPlottableArea));
                    double areaHeight = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.height.baseVal.value", chartPlottableArea));
                    double areaWidth = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.width.baseVal.value", chartPlottableArea));

                    if (areaHeight < 0)
                        areaHeight = 0;
                    if (areaWidth < 0)
                        areaWidth = 0;

                    this.ChartArea = new Rect(areaX, areaY, areaWidth, areaHeight);
                    this.Output = this.ChartArea;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to calculate chart areas: {ex.Message}");
            }
        }

        public void DestroyChart()
        {
            try
            {
                if (this.chart != null)
                {
                    object div = OpenSilver.Interop.GetDiv(this);
                    object myNode = TInterop.ExecuteJavaScriptBase("$0.lastElementChild", div);
                    while (myNode != null)
                    {
                        TInterop.ExecuteJavaScriptBase("$0.removeChild($1)", div, myNode);
                        myNode = TInterop.ExecuteJavaScriptBase("$0.lastElementChild", div);
                    }

                    this.listOfChartSeries.Clear();
                }
            }
            catch (Exception ex)
            {
                TException.Log(ex);
            }

            this.chart = null;
            this.initializationLoops = 0;
        }

        private Point TransformPoint(Point point)
        {
            point = ConfigHelper.TransformPoint(point);

            if (this.Parent is Grid && ((this.Parent as Grid).Parent is FrameworkElement) && ((this.Parent as Grid).Parent as FrameworkElement).RenderTransform is RotateTransform)
                point = new Point(point.Y, point.X);

            return point;
        }

        private Point GetLegendWindowReferencePoint(Base.eLegendPlacement legendPlacement)
        {
            double _top = 0;
            double _left = 0;

            switch (legendPlacement)
            {
                default:
                case Base.eLegendPlacement.None:
                    return new Point(0, 0);

                case Base.eLegendPlacement.TopLeft:
                    _left = this.ChartArea.Left;

                    if (this.orientationMode == OrientationMode.Horizontal)
                        _left += this.YAxis.Width;
                    else /*if (this.orientationMode == OrientationMode.VerticalTopToBottom || this.orientationMode == OrientationMode.VerticalBottomToTop)*/
                        _left += this.XAxis.Width;

                    _top = this.ChartArea.Y + 10;
                    if (this.orientationMode == OrientationMode.VerticalTopToBottom)
                        _top += this.YAxis.Height;
                    break;

                case Base.eLegendPlacement.TopRight:
                    _left = this.ChartArea.Right - this.LegendWindow.Width;
                    if (this.orientationMode != OrientationMode.Horizontal)
                        _left = this.RenderedWidth - this.LegendWindow.Width - 10;

                    _top = this.ChartArea.Y + 10;
                    if (this.orientationMode == OrientationMode.VerticalTopToBottom)
                        _top += this.YAxis.Height;
                    else if (this.orientationMode == OrientationMode.VerticalBottomToTop)
                        _top += 10;
                    break;

                case Base.eLegendPlacement.BottomLeft:
                    _left = this.ChartArea.Left;
                    if (this.orientationMode == OrientationMode.Horizontal)
                        _left += this.YAxis.Width;
                    else /*if (this.orientationMode == OrientationMode.VerticalTopToBottom || this.orientationMode == OrientationMode.VerticalBottomToTop)*/
                        _left += this.XAxis.Width;

                    _top = this.ChartArea.Bottom - this.LegendWindow.Height - 15;
                    if (this.orientationMode == OrientationMode.VerticalTopToBottom)
                        _top += this.YAxis.Height - 10;
                    break;

                case Base.eLegendPlacement.BottomRight:
                    _left = this.ChartArea.Right - this.LegendWindow.Width;
                    if (this.orientationMode != OrientationMode.Horizontal)
                        _left = this.RenderedWidth - this.LegendWindow.Width - 10; //_left += this.ChartArea.Left - 15;

                    _top = this.ChartArea.Bottom - this.LegendWindow.Height - 15;
                    if (this.orientationMode == OrientationMode.VerticalTopToBottom)
                        _top += this.YAxis.Height;
                    else if (this.orientationMode == OrientationMode.VerticalBottomToTop)
                        _top += 0;
                    break;

                case Base.eLegendPlacement.RightPanel:
                case Base.eLegendPlacement.BottomPanel:
                    return new Point(0, 0);
            }

            return new Point(_left, _top);
        }


        private void OnPointRendered(object args)
        {
            try
            {
                string markerImageUrl = TConvert.ToString(TInterop.ExecuteJavaScriptBase("$0.series.marker.imageUrl", args));
                markerImageUrl = markerImageUrl ?? "";
                if (markerImageUrl.Contains("Alarm"))
                    return;

                int pointIndex = TConvert.To<int>(TInterop.ExecuteJavaScriptBase("$0.point.index", args));
                object dataSourceRow = TInterop.ExecuteJavaScriptBase("$0.series.dataSource[$1]", args, pointIndex);

                bool isVisible = TConvert.To<bool>(TInterop.ExecuteJavaScriptBase("$0['visibilityCallback']", dataSourceRow));
                double size = TConvert.ToDouble(TInterop.ExecuteJavaScriptBase("$0['sizeCallback']", dataSourceRow));
                string fillColor = TConvert.ToString(TInterop.ExecuteJavaScriptBase("$0['fillCallback']", dataSourceRow));

                TInterop.ExecuteJavaScriptBase("$0.point.marker.visible = $1", args, IsVisible);
                TInterop.ExecuteJavaScriptBase("$0.height = $1", args, size);
                TInterop.ExecuteJavaScriptBase("$0.width = $1", args, size);
                TInterop.ExecuteJavaScriptBase("$0.fill = $1", args, fillColor);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Exception || OnPointRendered || Error: {ex.Message}");
            }
        }

        private void OnLabelRendered(object args)
        {
            try
            {
                string axisLabelName = "label_" + TConvert.ToString(TInterop.ExecuteJavaScriptBase("$0.axis.name", args));
                TInterop.ExecuteJavaScriptBase("$0.text = $1", args, axisLabelName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception || OnLabelRendered || Error: {ex.Message}");
            }
        }

        private void OnTooltipRendered(object args)
        {
            bool isMarkerVisible = false;
            bool isAlarmSerie = false;
            string labelTemplate = "";
            object marker = TInterop.ExecuteJavaScriptBase("$0.series.marker", args);
            object properties = marker == null ? null : TInterop.ExecuteJavaScriptBase("$0.properties", marker);
            if(properties != null)
            {
                isMarkerVisible = TConvert.To<bool>(TInterop.ExecuteJavaScriptBase("$0.visible", properties));
                labelTemplate = TConvert.ToString(TInterop.ExecuteJavaScriptBase("$0.dataLabel.template", properties));
                isAlarmSerie = isMarkerVisible && string.IsNullOrEmpty(labelTemplate) == false;
            }

            TInterop.ExecuteJavaScriptBase("$0.headerText = ''", args);
            TInterop.ExecuteJavaScriptBase("$0.text = ''", args);

            int pointIndex = 0;
            if (isAlarmSerie)
            {
                double valueY = TConvert.ToDouble(TInterop.ExecuteJavaScriptBase("$0.data.pointY", args));

                pointIndex = TConvert.To<int>(TInterop.ExecuteJavaScriptBase("$0.pointIndex", args));
                string alarmState = TConvert.ToString(TInterop.ExecuteJavaScriptBase("$0.series.dataSource[$1].alarmState", args, pointIndex));
                string alarmTimestamp = TConvert.ToString(TInterop.ExecuteJavaScriptBase("$0.series.dataSource[$1].alarmTimestamp", args, pointIndex));

                labelTemplate = labelTemplate.Replace("${alarmState}", alarmState);
                labelTemplate = labelTemplate.Replace("${alarmTimestamp}", alarmTimestamp);
                labelTemplate = labelTemplate.Replace("${point.y}", valueY.ToString(TCultureInfo.InvariantCulture));

                TInterop.ExecuteJavaScriptBase("$0.text = $1", args, labelTemplate);
                return;
            }

            if (this.EnableMarkerTooltip == false || isMarkerVisible == false)
            {
                TInterop.ExecuteJavaScriptBase("$0.cancel = true", args);
                TInterop.ExecuteJavaScriptBase("$0.text = ''", args);
                return;
            }

            pointIndex = TConvert.To<int>(TInterop.ExecuteJavaScriptBase("$0.point.index", args));
            object dataSourceRow = TInterop.ExecuteJavaScriptBase("$0.series.dataSource[$1]", args, pointIndex);
            string tooltip = TConvert.ToString(TInterop.ExecuteJavaScriptBase("$0['tooltipCallback']", dataSourceRow));
            TInterop.ExecuteJavaScriptBase("$0.text = $1", args, tooltip);
        }

        /// <summary>
        /// Close Button pressed for Primary Cursor line
        /// </summary>
        /// <param name="args"></param>
        private void OnPrimaryCursorCloseButonPressed(object args)
        {
            try
            {
                this.ResetLabelsEditable();

                if (this.primaryCursorValuesAnnotation != null)
                {
                    object primaryCursorNode = TInterop.ExecuteJavaScriptBase("document.getElementById($0)", this.primaryCursorValuesAnnotation.ElementId);

                    if (primaryCursorNode != null)
                        TInterop.ExecuteJavaScriptBase("$0.parentNode.removeChild($1)", primaryCursorNode, primaryCursorNode);

                    this.primaryCursorWasClosed = true;

                    this.RecalcLayout();
                }
            }
            catch (Exception ex)
            {
                TException.Log(ex);
            }
            //this.primaryCursorValuesAnnotation = null;
            //this.primaryCursorTimestampAnnotation = null;
        }

        /// <summary>
        /// Close Button pressed for Secondary Cursor line
        /// </summary>
        /// <param name="args"></param>
        private void OnSecondaryCursorCloseButonPressed(object args)
        {
            try
            {
                this.ResetLabelsEditable();

                if (this.secondaryCursorValuesAnnotation != null)
                {
                    object secondaryCursorNode = TInterop.ExecuteJavaScriptBase("document.getElementById($0)", this.secondaryCursorValuesAnnotation.ElementId);

                    if (secondaryCursorNode != null)
                        TInterop.ExecuteJavaScriptBase("$0.parentNode.removeChild($1)", secondaryCursorNode, secondaryCursorNode);

                    this.secondaryCursorWasClosed = true;

                    this.RecalcLayout();
                }
            }
            catch (Exception ex)
            {
                TException.Log(ex);
            }
        }

        private bool CheckClickedPointBelongsToRect(Rect rect, Point point)
        {
            return rect.Contains(point);
        }

        /// <summary>
        /// Method to handle SecondaryCursor Event (mouse right click)
        /// </summary>
        /// <param name="args"> args.which: 1 -> LeftButton | 2 -> MiddleButton | 3 -> RightButton </param>
        private void OnChartMouseDown(object args)
        {
            try
            {
                // Reset Label Edit values
                this.ResetLabelsEditable();

                string targetId = TConvert.ToString(TInterop.ExecuteJavaScriptBase("$0.target.id", args));

                bool chartAreaBorderClicked = targetId.StartsWith(this.chartControlId + "_ChartAreaBorder", StringComparison.InvariantCultureIgnoreCase);
                bool seriesClicked = targetId.StartsWith(this.chartControlId + "_Series", StringComparison.InvariantCultureIgnoreCase);

                bool primaryCursorCloseButtonClicked = targetId.StartsWith("primaryCursorValue_", StringComparison.InvariantCultureIgnoreCase);
                bool secondaryCursorCloseButtonClicked = targetId.StartsWith("secondaryCursorValue_", StringComparison.InvariantCultureIgnoreCase);
                if (primaryCursorCloseButtonClicked || secondaryCursorCloseButtonClicked)
                {
                    double pX = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.clientX", args));
                    double pY = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.clientY", args));
                    Point point = new Point(pX, pY);

                    object rectObj = TInterop.ExecuteJavaScriptBase("document.getElementById($0).getBoundingClientRect()", targetId);
                    double x = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.x", rectObj));
                    double y = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.y", rectObj));
                    double height = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.height", rectObj));
                    double width = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.width", rectObj));
                    Rect rect = new Rect(x, y, width, height);

                    if (CheckClickedPointBelongsToRect(rect, point))
                    {
                        if (primaryCursorCloseButtonClicked)
                            this.OnPrimaryCursorCloseButonPressed(args);
                        if (secondaryCursorCloseButtonClicked)
                            this.OnSecondaryCursorCloseButonPressed(args);

                        this.annotationsMenu.Visibility = Visibility.Collapsed;
                        return;
                    }
                }

                bool annotationsClicked = string.IsNullOrEmpty(targetId);

                if (chartAreaBorderClicked == false && seriesClicked == false && annotationsClicked == false)
                    return;

                int mouseButtonClickedCode = TConvert.To<int>(TInterop.ExecuteJavaScriptBase("$0.which", args));

                double pointX = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.offsetX", args));
                double pointY = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.offsetY", args));

                if (annotationsClicked)
                {
                    string chartId = this.chartControlId + "_ChartAreaBorder";
                    object rect = TInterop.ExecuteJavaScriptBase("document.getElementById($0).getBoundingClientRect()", chartId);
                    pointX = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.clientX", args)) - TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.left", rect));
                    pointY = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.clientY", args)) - TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.top", rect));
                }

                if (pointX < this.ChartArea.X)
                    pointX = this.ChartArea.X + 2;

                if (pointX > this.ChartArea.Width)
                    pointX = this.ChartArea.Width - 2;

                if (pointY < this.ChartArea.Y)
                    pointY = this.ChartArea.Y + 2;

                if (pointY > this.ChartArea.Height)
                    pointY = this.ChartArea.Height - 2;

                if (mouseButtonClickedCode == 1)
                {
                    this.isChartMousePressed = true;
                    this.primaryCursorWasClosed = false;
                    this.primaryCursorClicked = true;

                    this.CursorPositionX = pointX;
                    this.CursorPositionY = pointY;

                    if (this.EnableMouseActions && this.chartZoomRectangle != null)
                    {
                        // Must Add the Axis dimensions to the ZoomRectangle
                        double rectX = pointX;
                        double rectY = pointY;
                        if (this.orientationMode == OrientationMode.Horizontal)
                        {
                            rectX = pointX + this.YAxis.Width;
                            //rectY = pointY - this.XAxis.Height;
                        }
                        else if (this.orientationMode == OrientationMode.VerticalTopToBottom)
                        {
                            rectX = pointX + this.XAxis.Width;
                            rectY = pointY + this.YAxis.Height;
                        }
                        else if (this.orientationMode == OrientationMode.VerticalBottomToTop)
                        {
                            rectX = pointX + this.XAxis.Width;
                            //rectY = pointY - this.YAxis.Height;
                        }

                        string aux = rectX.ToString(TCultureInfo.InvariantCulture) + "px";
                        TInterop.ExecuteJavaScriptBase("$0.style.left = $1", this.chartZoomRectangle, aux);

                        aux = rectY.ToString(TCultureInfo.InvariantCulture) + "px";
                        TInterop.ExecuteJavaScriptBase("$0.style.top = $1", this.chartZoomRectangle, aux);
                        TInterop.ExecuteJavaScriptBase("$0.style.visibility = 'visible'", this.chartZoomRectangle);
                    }

                    this.annotationsMenu.Visibility = Visibility.Collapsed;
                    this.RecalcLayout();
                    return;
                }

                if (mouseButtonClickedCode == 3)
                {
                    //TInterop.ExecuteJavaScriptBase("$0.preventDefault()", args);
                    if (this.EnabledSecondaryCursor == false)
                        return;

                    this.secondaryCursorWasClosed = false;
                    this.secondaryCursorClicked = true;

                    this.SecondaryCursorPositionX = pointX;
                    this.SecondaryCursorPositionY = pointY;

                    if (this.annotationsMenu.Children.Count > 0)
                    {
                        GenericObjectEventArgs e = new GenericObjectEventArgs(pointX, pointY, null);
                        this.AnnotationsContextMenuOpening(null, e);
                    }

                    this.RecalcLayout();
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception >> OnMouseDown :: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void OnChartMouseMove(object args)
        {
            try
            {             
                string targetId = TConvert.ToString(TInterop.ExecuteJavaScriptBase("$0.target.id", args));

                bool chartAreaBorderClicked = targetId.StartsWith(this.chartControlId + "_ChartAreaBorder", StringComparison.InvariantCultureIgnoreCase);
                bool seriesClicked = targetId.StartsWith(this.chartControlId + "_Series", StringComparison.InvariantCultureIgnoreCase);

                if (chartAreaBorderClicked == false && seriesClicked == false)
                    return;

                double pointX = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.offsetX", args));
                double pointY = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.offsetY", args));

                if (pointX < this.ChartArea.X)
                    pointX = this.ChartArea.X + 2;

                if (pointX > this.ChartArea.Width)
                    pointX = this.ChartArea.Width - 2;

                if (pointY < this.ChartArea.Y)
                    pointY = this.ChartArea.Y + 2;

                if (pointY > this.ChartArea.Height)
                    pointY = this.ChartArea.Height - 2;

                int mouseButtonClickedCode = TConvert.To<int>(TInterop.ExecuteJavaScriptBase("$0.which", args));

                if (this.EnableMouseCursorConnection && mouseButtonClickedCode == 0)
                {
                    this.isChartMousePressed = true;
                    this.primaryCursorWasClosed = false;
                    this.primaryCursorClicked = true;

                    this.CursorPositionX = pointX;
                    this.CursorPositionY = pointY;

                    //this.RecalculateStripLine(pointX, pointY, this.EnabledCursor, true);

                    this.RecalcLayout();

                    return;
                }

                if (mouseButtonClickedCode != 1)
                    return;

                if (this.isChartMousePressed == false)
                    return;

                if (this.EnableMouseActions && this.chartZoomRectangle != null)
                {
                    string xAuxWidth;
                    string xAuxLeft;
                    string yAuxHeight;
                    string yAuxTop;

                    double diffX = pointX - this.CursorPositionX;
                    bool isRightToLeft = diffX < 0;
                    diffX = Math.Abs(diffX);

                    double diffY = pointY - this.CursorPositionY;
                    bool isDownToUp = diffY < 0;
                    diffY = Math.Abs(diffY);

                    double dxAuxWidth = diffX;
                    double dxAuxLeft = pointX;
                    double dyAuxHeight = diffY;
                    double dyAuxTop = pointY;

                    xAuxWidth = dxAuxWidth + "px";
                    TInterop.ExecuteJavaScriptBase("$0.style.width = $1", this.chartZoomRectangle, xAuxWidth);
                    yAuxHeight = dyAuxHeight + "px";
                    TInterop.ExecuteJavaScriptBase("$0.style.height = $1", this.chartZoomRectangle, yAuxHeight);

                    if (isRightToLeft)
                    {
                        xAuxLeft = pointX.ToString(TCultureInfo.InvariantCulture) + "px";
                        TInterop.ExecuteJavaScriptBase("$0.style.left = $1", this.chartZoomRectangle, xAuxLeft);
                    }

                    if (isDownToUp)
                    {
                        yAuxTop = pointY.ToString(TCultureInfo.InvariantCulture) + "px";
                        TInterop.ExecuteJavaScriptBase("$0.style.top = $1", this.chartZoomRectangle, yAuxTop);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception >> OnMouseMove :: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void OnChartMouseUp(object args)
        {
            try
            {
                if (this.EnableMouseActions == false)
                    return;

                if (!this.isChartMousePressed)
                    return;
                
                string targetId = TConvert.ToString(TInterop.ExecuteJavaScriptBase("$0.target.id", args));

                bool chartAreaBorderClicked = targetId.StartsWith(this.chartControlId + "_ChartAreaBorder", StringComparison.InvariantCultureIgnoreCase);
                bool seriesClicked = targetId.StartsWith(this.chartControlId + "_Series", StringComparison.InvariantCultureIgnoreCase);

                if (chartAreaBorderClicked == false && seriesClicked == false)
                    return;

                int mouseButtonClickedCode = TConvert.To<int>(TInterop.ExecuteJavaScriptBase("$0.which", args));

                if (mouseButtonClickedCode != 1)
                    return;

                this.isChartMousePressed = false;

                double pointX = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.offsetX", args));
                double pointY = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.offsetY", args));

                if (this.chartZoomRectangle != null)
                {
                    TInterop.ExecuteJavaScriptBase("$0.style.left = '0px'", this.chartZoomRectangle);
                    TInterop.ExecuteJavaScriptBase("$0.style.top = '0px'", this.chartZoomRectangle);
                    TInterop.ExecuteJavaScriptBase("$0.style.width = '0px'", this.chartZoomRectangle);
                    TInterop.ExecuteJavaScriptBase("$0.style.height = '0px'", this.chartZoomRectangle);
                    TInterop.ExecuteJavaScriptBase("$0.style.visibility = 'hidden'", this.chartZoomRectangle);
                }

                double moveSum = Math.Abs(this.CursorPositionX - pointX);
                double threshold = 0.1 * this.ChartArea.Width;
                if (moveSum < threshold)
                    return;

                Point pBefore = new Point(this.CursorPositionX, this.CursorPositionY);
                Point pNow = new Point(pointX, pointY);

                double leftX = pBefore.X;
                double rightX = pNow.X;
                double width = rightX - leftX;
                if (leftX > rightX)
                {
                    double _ph = leftX;
                    leftX = rightX;
                    rightX = _ph;
                    width = rightX - leftX;
                }

                double topY = pBefore.Y;
                double bottomY = pNow.Y;
                double height = bottomY - topY;
                if (topY > bottomY)
                {
                    double _ph = topY;
                    topY = bottomY;
                    bottomY = _ph;
                    height = bottomY - topY;
                }

                Rect zoomRect = new Rect(leftX, topY, width, height);

                Point p1 = zoomRect.TopLeft.ScreenToViewport(this.Transform);
                Point p2 = zoomRect.BottomRight.ScreenToViewport(this.Transform);

                DataRect newVisible = new DataRect();
                try
                {
                    newVisible = new DataRect(p1, p2);
                }
                catch
                {
                    return;
                }

                long x1 = (long)(newVisible.XMin * 10000000000.0);
                long x2 = (long)(newVisible.XMax * 10000000000.0);
                DateTime newXMin = new DateTime(x1 >= DateTime.MinValue.Ticks && x1 <= DateTime.MaxValue.Ticks ? x1 : DateTime.MinValue.Ticks, DateTimeKind.Utc);
                DateTime newXMax = new DateTime(x2 >= DateTime.MinValue.Ticks && x2 <= DateTime.MaxValue.Ticks ? x2 : DateTime.MinValue.Ticks, DateTimeKind.Utc);

                Dictionary<object, Point> dicMinMax = null;

                if (this.IsEnabledYScaleForEachPen)
                {
                    dicMinMax = new Dictionary<object, Point>();
                    for (int i = 0; i < this.GetPenCount(); i++)
                    {
                        object column = this.GetPenCol(i);
                        dicMinMax.Add(column, new Point(newVisible.YMin, newVisible.YMax));
                    }
                }

                if (this.VisibleRectChanged != null)
                    this.VisibleRectChanged(this, new VisibleRectEventArgs(newXMin, newXMax, newVisible.YMin, newVisible.YMax, dicMinMax, false, true));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception >> OnMouseUp :: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public void SetAnnotationContextMenu(ContextMenu contextMenu, bool shouldMakeVisible)
        {
            this.annotationsMenu.Children.Clear();
            this.annotationsMenu.Children.Add(contextMenu);

            double width = contextMenu.ActualWidth;
            this.annotationsMenu.Width = width;

            double height = contextMenu.ActualHeight;
            this.annotationsMenu.Height = height;

            Visibility visibility = Visibility.Collapsed;
            if (shouldMakeVisible)
                visibility = Visibility.Visible;
            this.annotationsMenu.Visibility = visibility;

            double positionX = this.secondaryCursorPositionX + this.YAxis.Width - TICK_SIZE;
            double positionY = this.secondaryCursorPositionY;
            if (this.orientationMode == OrientationMode.VerticalTopToBottom)
            {
                positionX = this.secondaryCursorPositionX + this.XAxis.Width - TICK_SIZE;
                positionY = this.secondaryCursorPositionY + this.YAxis.Height;
            }
            if (this.orientationMode == OrientationMode.VerticalBottomToTop)
            {
                positionX = this.secondaryCursorPositionX + this.XAxis.Width - TICK_SIZE;
            }

            this.annotationsMenu.Margin = new Thickness(positionX, positionY, 0, 0);
        }

        private void RecalculatePrimaryCursorTooltip(bool cursorEnabled, bool cursorShowValueTooltipEnable, bool cursorShowTimestampTooltipEnable)
        {
            this.RecalculateCursorValueTooltip(cursorEnabled, cursorShowValueTooltipEnable, true);
            this.RecalculateCursorTimestampTooltip(cursorEnabled, cursorShowTimestampTooltipEnable, true);

            if (this.primaryCursorClicked)
                this.RecalculateStripLine(this.CursorPositionX, this.CursorPositionY, this.EnabledCursor, true);
        }

        private void RecalculateSecondaryCursorTooltip(bool cursorEnabled, bool cursorShowValueTooltipEnable, bool cursorShowTimestampTooltipEnable)
        {
            this.RecalculateCursorValueTooltip(cursorEnabled, cursorShowValueTooltipEnable, false);
            this.RecalculateCursorTimestampTooltip(cursorEnabled, cursorShowTimestampTooltipEnable, false);

            if (this.secondaryCursorClicked)
                this.RecalculateStripLine(this.SecondaryCursorPositionX, this.SecondaryCursorPositionY, this.EnabledSecondaryCursor, false);
        }

        private void RecalculateCursorValueTooltip(bool cursorEnabled, bool cursorShowValueTooltipEnable, bool isPrimaryCursor)
        {
            try
            {
                if (cursorEnabled == false)
                {
                    if (isPrimaryCursor)
                    {
                        this.primaryCursorValuesAnnotation = null;
                        this.primaryCursorLineAnnotation = null;
                    }
                    else
                    {
                        this.secondaryCursorValuesAnnotation = null;
                        this.secondaryCursorLineAnnotation = null;
                    }

                    return;
                }

                if (isPrimaryCursor && this.primaryCursorWasClosed)
                {
                    this.primaryCursorValuesAnnotation = null;
                    this.primaryCursorLineAnnotation = null;
                    return;
                }

                if (isPrimaryCursor == false && this.secondaryCursorWasClosed)
                {
                    this.secondaryCursorValuesAnnotation = null;
                    this.secondaryCursorLineAnnotation = null;
                    return;
                }

                double lineX = this.SecondaryCursorPositionX;
                double lineY = (this.ChartArea.Top - this.ChartBorderArea.Top) + 5;  //(this.ChartArea.Bottom - this.ChartArea.Top) * 0.01;
                double px1 = this.SecondaryCursorPositionX;
                double py1 = this.secondaryCursorPositionY;
                if (isPrimaryCursor)
                {
                    px1 = this.CursorPositionX;
                    py1 = this.CursorPositionY;
                    lineX = this.CursorPositionX;
                }

                /*
                annotationPlacementX = annotationPlacementX - 5;
                if (isPrimaryCursor == false)
                    annotationPlacementX = annotationPlacementX - 12;
                */

                if (this.orientationMode != OrientationMode.Horizontal)
                {
                    lineX = this.ChartArea.Left;
                    lineY = isPrimaryCursor ? this.CursorPositionY : this.SecondaryCursorPositionY;
                }

                Point cursorLinePoint = new Point(lineX, lineY);

                List<object> objValues = new List<object>();
                List<string> colors = new List<string>();

                int i = 0;
                foreach (TSeries series in this.listOfChartSeries)
                {
                    Point valuePoint = this.CalculateActualValuePointFromClickedCoodinates(px1, py1, i);
                    DateTime dt = this.primaryXAxis.JavaTimeStampToDateTime(this.xAxisMin, valuePoint.X);

                    double dataPointX = dt.Ticks;
                    dataPointX = dataPointX / 10000000000.0;

                    double y = Double.NaN;
                    double y2 = Double.NaN;
                    if (this.pens != null && this.pens[i] != null)
                    {
                        List<Point> visiblePoints = this.pens[i].VisiblePoints;
                        int searchIndex = this.SearchXBetween(dataPointX, visiblePoints);

                        if (searchIndex >= 0 && searchIndex < this.pens[i].VisiblePoints.Count)
                        {
                            Point ptBefore = visiblePoints[searchIndex];
                            if (searchIndex + 1 < visiblePoints.Count)
                            {
                                Point ptAfter = visiblePoints[searchIndex + 1];
                                double ratio = (dataPointX - ptBefore.X) / (ptAfter.X - ptBefore.X);
                                y = ptBefore.Y + (ptAfter.Y - ptBefore.Y) * ratio;
                                if (y != ptAfter.Y)
                                    y2 = ptBefore.Y;
                            }
                        }
                    }

                    double objValue = double.IsNaN(y) ? double.MinValue : y;
                    bool isVisible = false;
                    bool isDouble = true;
                    if (objValue != double.MinValue)
                    {
                        string strType = "System.Double";
                        if (this.typeChart == TypeChart.TrendChart)
                        {
                            TrendPen tPen = this.pens[i].TrendPen as TrendPen;

                            if(this.IsEnabledYScaleForEachPen)
                                objValue = SfChart.ConvertTrendPenToDisplayValue(tPen, TConvert.ToDouble(objValue));
                            objValue = this.ConvertYBack(tPen, objValue);
                            strType = tPen.TagRef.TypeString;
                            isVisible = tPen.Visible;
                        }
                        else
                        {
                            Base.DrillingPen tPen = this.pens[i].TrendPen as Base.DrillingPen;
                            if (this.IsEnabledYScaleForEachPen)
                                objValue = SfChart.ConvertDrillingPenToDisplayValue(tPen, TConvert.ToDouble(objValue));
                            objValue = this.ConvertYBack(tPen, objValue);
                            strType = tPen.TagRef[0].TypeString;
                            isVisible = tPen.Visible;
                        }

                        if (strType.Equals("System.Int32", StringComparison.InvariantCulture))
                            isDouble = false;
                    }

                    if (isVisible)
                    {
                        string valueToAdd = TConvert.To<int>(objValue).ToString(TCultureInfo.InvariantCulture);
                        if (isDouble)
                            valueToAdd = objValue.ToString("N02");

                        objValues.Add(valueToAdd);
                        colors.Add(series.FillColor);
                    }

                    i++;
                }

                double lineSize = this.ChartArea.Width - this.ChartArea.Left;
                if (this.orientationMode == OrientationMode.Horizontal)
                    lineSize = this.ChartArea.Height - this.ChartArea.Top;

                if (isPrimaryCursor)
                {
                    this.primaryCursorValuesAnnotation = new TAnnotations(cursorLinePoint, objValues.ToArray(), colors.ToArray(), true, this.ShowPrimaryCloseButton, true, cursorShowValueTooltipEnable, this.orientationMode, lineSize, this.CursorColor.ToString(), this.CursorLine);
                    this.primaryCursorLineAnnotation = new TAnnotations(cursorLinePoint, true, this.orientationMode, lineSize, this.CursorColor.ToString(), this.CursorLine);
                }
                else
                {
                    this.secondaryCursorValuesAnnotation = new TAnnotations(cursorLinePoint, objValues.ToArray(), colors.ToArray(), false, this.ShowSecondaryCloseButton, true, cursorShowValueTooltipEnable, this.orientationMode, lineSize, this.SecondaryCursorColor.ToString(), this.SecondaryCursorLine);
                    this.secondaryCursorLineAnnotation = new TAnnotations(cursorLinePoint, false, this.orientationMode, lineSize, this.SecondaryCursorColor.ToString(), this.SecondaryCursorLine);
                }

                return;
            }
            catch
            {
            }
            if (isPrimaryCursor)
            {
                this.primaryCursorValuesAnnotation = null;
                this.primaryCursorLineAnnotation = null;
            }
            else
            {
                this.secondaryCursorValuesAnnotation = null;
                this.secondaryCursorLineAnnotation = null;
            }
        }

        private double ConvertYBack(object pen, double originalValue)
        {
            if (pen == null || this.chart == null)
                return originalValue;

            int yMin = 0; /// Always in the [0, 100] range
            int yMax = 100;

            // determine the source [min,max] range
            double srcMin = this.YMin;
            double srcMax = this.YMax;
            if (this.IsEnabledYScaleForEachPen)
            {
                if (this.typeChart == TypeChart.TrendChart)
                {
                    srcMin = (pen as TrendPen).MinValue;
                    srcMax = (pen as TrendPen).MaxValue;
                }
                else
                {
                    srcMin = (pen as DrillingPen).MinValue;
                    srcMax = (pen as DrillingPen).MaxValue;
                }
            }

            if (srcMin == srcMax || yMin == yMax)
                return originalValue;

            // inverse linear map: [YMinValue,YMaxValue] → [srcMin,srcMax]
            return ((originalValue - yMin) * (srcMax - srcMin) / (yMax - yMin)) + srcMin;
        }

        private void RecalculateCursorTimestampTooltip(bool cursorEnabled, bool cursorShowTimestampTooltipEnable, bool isPrimaryCursor)
        {
            try
            {
                if (cursorEnabled == false || cursorShowTimestampTooltipEnable == false)
                {
                    if (isPrimaryCursor)
                        this.primaryCursorTimestampAnnotation = null;
                    else
                        this.secondaryCursorTimestampAnnotation = null;

                    return;
                }

                if (isPrimaryCursor && this.primaryCursorWasClosed)
                {
                    this.primaryCursorTimestampAnnotation = null;
                    return;
                }

                if (isPrimaryCursor == false && this.secondaryCursorWasClosed)
                {
                    this.secondaryCursorTimestampAnnotation = null;
                    return;
                }

                double lineX = isPrimaryCursor ? this.CursorPositionX : this.SecondaryCursorPositionX;
                lineX -= 7;
                double lineY = this.ChartArea.Bottom * 0.955;

                if (this.orientationMode != OrientationMode.Horizontal)
                {
                    lineX = this.ChartArea.Left + 15;
                    lineY = isPrimaryCursor ? this.CursorPositionY : this.SecondaryCursorPositionY;
                    lineY -= 7;
                }
                Point cursorLinePoint = new Point(lineX, lineY);

                double px1 = isPrimaryCursor ? this.CursorPositionX : this.SecondaryCursorPositionX;
                double py1 = isPrimaryCursor ? this.CursorPositionY : this.SecondaryCursorPositionY;

                List<object> objValues = new List<object>();
                List<string> colors = new List<string>();

                Point valuePoint = this.CalculateActualValuePointFromClickedCoodinates(px1, py1, 0);
                colors.Add("");

                DateTime dt = this.primaryXAxis.JavaTimeStampToDateTime(this.xAxisMin, valuePoint.X);

                int idx = this.xLabels.Length - 1;
                if (idx < 0)
                    idx = 0;
                objValues.Add(this.GetStringXAxis(idx, dt));

                double lineSize = this.ChartArea.Width;
                if (this.orientationMode == OrientationMode.Horizontal)
                    lineSize = this.ChartArea.Height;
                if (isPrimaryCursor)
                {
                    this.primaryCursorTimestampAnnotation = new TAnnotations(cursorLinePoint, objValues.ToArray(), colors.ToArray(), true, false, false, false, this.orientationMode, lineSize, this.CursorColor.ToString(), this.CursorLine);
                }
                else
                {
                    this.secondaryCursorTimestampAnnotation = new TAnnotations(cursorLinePoint, objValues.ToArray(), colors.ToArray(), false, false, false, false, this.orientationMode, lineSize, this.SecondaryCursorColor.ToString(), this.SecondaryCursorLine);
                }

                return;
            }
            catch
            {
            }

            if (isPrimaryCursor)
                this.primaryCursorTimestampAnnotation = null;
            else
                this.secondaryCursorTimestampAnnotation = null;
        }

        private void RecalculateStripLine(double pointX, double pointY, bool cursorEnabled, bool isPrimaryCursor)
        {
            try
            {
                if (cursorEnabled == false)
                    return;

                double xAxisDelta = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.primaryXAxis.actualRange.delta", this.chart));

                Point actualValues = this.CalculateActualValuePointFromClickedCoodinates(pointX, pointY, 0);
                double start = actualValues.X;
                double end = start + (0.0025 * xAxisDelta);

                TStripLine stripLine;

                if (isPrimaryCursor)
                {
                    if (this.primaryCursorWasClosed)
                    {
                        this.primaryXAxis.StripLineLeftMouseClick = null;
                        return;
                    }
                    stripLine = this.primaryXAxis.StripLineLeftMouseClick;
                    if (stripLine == null)
                        stripLine = new TStripLine(this.CursorLine, this.CursorColor.ToString(), start, end);
                    else
                        stripLine.UpdateStripLineParameters(start, end, this.cursorColor.ToString(), this.CursorLine);

                    this.primaryXAxis.StripLineLeftMouseClick = stripLine;
                }
                else
                {
                    if (this.secondaryCursorWasClosed)
                    {
                        this.primaryXAxis.StripLineRightMouseClick = null;
                        return;
                    }
                    stripLine = this.primaryXAxis.StripLineRightMouseClick;
                    if (stripLine == null)
                        stripLine = new TStripLine(this.SecondaryCursorLine, this.secondaryCursorColor.ToString(), start, end);
                    else
                        stripLine.UpdateStripLineParameters(start, end, this.secondaryCursorColor.ToString(), this.SecondaryCursorLine);
                    this.primaryXAxis.StripLineRightMouseClick = stripLine;
                }

                return;
            }
            catch
            {
            }
            if (isPrimaryCursor)
                this.primaryXAxis.StripLineLeftMouseClick = null;
            else
                this.primaryXAxis.StripLineRightMouseClick = null;
        }

        private Point CalculateActualValuePointFromClickedCoodinates(double mouseX, double mouseY, int seriesIdx)
        {
            try
            {
                object clipRect = TInterop.ExecuteJavaScriptBase("$0.series[$1].clipRect", this.chart, seriesIdx);
                if (clipRect == null)
                    return new Point(mouseX, mouseY);

                double clipRectX = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.series[$1].clipRect.x", this.chart, seriesIdx));
                double clipRectY = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.series[$1].clipRect.y", this.chart, seriesIdx));
                double clipRectWidth = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.series[$1].clipRect.width", this.chart, seriesIdx));
                double clipRectHeight = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.series[$1].clipRect.height", this.chart, seriesIdx));

                double xAxisMin = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.series[$1].xAxis.visibleRange.min", this.chart, seriesIdx));

                double xAxisDelta = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.series[$1].xAxis.visibleRange.delta", this.chart, seriesIdx));

                bool xAxisIsInversed = TConvert.To<bool>(TInterop.ExecuteJavaScriptBase("$0.series[$1].xAxis.isInversed", this.chart, seriesIdx));

                double xValHorizontal = mouseX - clipRectX;
                double xValVertical = mouseY - clipRectY;

                double xSizeHorizontal = clipRectWidth;
                double xSizeVertical = clipRectHeight;

                //double actualXValue = (xAxisIsInversed == false) ? xValHorizontal / xSizeHorizontal : (1 - (xValHorizontal / xSizeHorizontal));
                double actualXValue = xValHorizontal / xSizeHorizontal;
                if (this.orientationMode == OrientationMode.VerticalBottomToTop)
                    actualXValue = (1 - (xValVertical / xSizeVertical));
                else if (this.orientationMode == OrientationMode.VerticalTopToBottom)
                    actualXValue = xValVertical / xSizeVertical;

                actualXValue = actualXValue * (xAxisDelta) + xAxisMin;

                // The same logic used by WPF TrendChart
                double actualYValue = 0;
                if (this.pens != null && seriesIdx < this.pens.Count)
                {
                    DateTime dt = this.primaryXAxis.JavaTimeStampToDateTime(xAxisMin, actualXValue);
                    double dataPointX = dt.Ticks / 10000000000.0;
                    
                    List<Point> visiblePoints = this.pens[seriesIdx].VisiblePoints;
                    int searchIndex = this.SearchXBetween(dataPointX, visiblePoints);
                    
                    if (searchIndex >= 0 && searchIndex < visiblePoints.Count)
                    {
                        Point ptBefore = visiblePoints[searchIndex];
                        if (searchIndex + 1 < visiblePoints.Count)
                        {
                            Point ptAfter = visiblePoints[searchIndex + 1];
                            double ratio = (dataPointX - ptBefore.X) / (ptAfter.X - ptBefore.X);
                            actualYValue = ptBefore.Y + (ptAfter.Y - ptBefore.Y) * ratio;
                        }
                        else
                        {
                            actualYValue = ptBefore.Y;
                        }
                    }
                }

                return new Point(actualXValue, actualYValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception || CalculateActualValuePointFromClickedCoodinates || Error: {ex.Message}");
            }

            return new Point(0, 0);
        }

        private void OnChartResize(object args)
        {
            string newHeightStr = TConvert.ToString(TInterop.ExecuteJavaScriptBase("$0.currentSize.height", args));
            string newWidthStr = TConvert.ToString(TInterop.ExecuteJavaScriptBase("$0.currentSize.width", args));

            this.RecalcLayout();
        }

        private void OnSelectionComplete(object args)
        {
            int seriesIndex = TConvert.To<int>(TInterop.ExecuteJavaScriptBase("$0.selectedDataValues[0].seriesIndex", args));

            if (this.mapPenLabelToLineGraph.TryGetValue(seriesIndex, out LineGraph pen))
            {
                if (this.typeChart == TypeChart.TrendChart)
                {
                    TrendPen trendPen = pen.TrendPen as TrendPen;
                    trendPen.ShowHighlighted = trendPen.ShowHighlighted ? false : true;
                    pen.TrendPen = trendPen;
                }
                else
                {
                    TrendPen trendPen = pen.TrendPen as TrendPen;
                    trendPen.ShowHighlighted = trendPen.ShowHighlighted ? false : true;
                    pen.TrendPen = trendPen;
                }
                this.mapPenLabelToLineGraph[seriesIndex] = pen;
            }
        }

        private void OnZoomCompleted(object args)
        {
            try
            {
                string axisType = TConvert.To<string>(TInterop.ExecuteJavaScriptBase("$0.axis.properties.valueType", args));
                double zoomFactor = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.currentZoomFactor", args));
                double zoomPosition = TConvert.To<double>(TInterop.ExecuteJavaScriptBase("$0.currentZoomPosition", args));

                // isXAxis
                if (axisType.Equals("DateTime", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (zoomFactor == 1)
                    {
                        this.primaryXAxis.ZoomFactor = null;
                        this.primaryXAxis.ZoomPosition = null;

                        return;
                    }

                    this.primaryXAxis.Interval = this.NumOfLabelsX % 2 == 0 ? this.NumOfLabelsX : this.NumOfLabelsX - 1;
                    this.primaryXAxis.IntervalType = "Auto";
                    this.primaryXAxis.ZoomFactor = zoomFactor;
                    this.primaryXAxis.ZoomPosition = zoomPosition;
                    return;
                }

                this.primaryYAxis.ZoomFactor = zoomFactor;
                this.primaryYAxis.ZoomPosition = zoomPosition;
            }
            catch { }
        }

        private void BtnZoomInClicked(object args)
        {
            this.ResetLabelsEditable();

            if (this.NavControlZoomIn != null)
                this.NavControlZoomIn(null, null);
        }

        private void BtnZoomOutClicked(object args)
        {
            this.ResetLabelsEditable();

            if (this.NavControlZoomOut != null)
                this.NavControlZoomOut(null, null);
        }

        private void BtnExpandClicked(object args)
        {
            this.ResetLabelsEditable();

            this.IsExpanded = !this.IsExpanded;
        }

        private void BtnGoBackInTimeClicked(object args)
        {
            this.ResetLabelsEditable();

            if (this.NavControlScrollLeft != null)
                this.NavControlScrollLeft(null, null);
        }

        private void BtnBackToDefaultSettings(object args)
        {
            this.ResfreshRequested();
        }

        internal void ResfreshRequested()
        {
            this.ResetLabelsEditable();

            if(this.VisibleRectChanged != null)
            {
                this.VisibleRectChanged(this, null);
                this.RecalcLayout();
            }
        }


        private void BtnGoForwardInTimeClicked(object args)
        {
            this.ResetLabelsEditable();

            if (this.NavControlScrollRight != null)
                this.NavControlScrollRight(null, null);
        }

        private void LegendElement_PointerPressed(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            LineGraph pen = null;
            if (sender is TextBlock)
                pen = (sender as TextBlock).Tag as LineGraph;
            else if (sender is Line)
                pen = (sender as Line).Tag as LineGraph;

            this.ResetLabelsEditable();

            if (pen == null)
                return;

            bool isVisible = false;
            if (this.typeChart == TypeChart.TrendChart)
            {
                TrendPen tPen = pen.TrendPen as TrendPen;
                isVisible = tPen.Visible;
                tPen.Visible = !isVisible;
            }
            else /*(this.typeChart == TypeChart.DrillingChart)*/
            {
                TrendPen tPen = pen.TrendPen as TrendPen;
                isVisible = tPen.Visible;
                tPen.Visible = !isVisible;
            }

            pen.Visibility = isVisible ? Visibility.Collapsed : Visibility.Visible;

            if (sender is TextBlock)
            {
                (sender as TextBlock).Foreground = isVisible ? Brushes.Black : Brushes.Gray;
                (sender as TextBlock).Opacity = isVisible ? 1.0 : 0.6;
            }

            return;
        }

        private void LegendItem_PointerMoved(object sender, MouseEventArgs e)
        {
            e.Handled = true;

            if (sender is TextBlock)
            {
                TextBlock tBlock = sender as TextBlock;
                tBlock.Cursor = Cursors.Hand;
            }
            else if (sender is Line)
            {
                Line line = sender as Line;
                line.Cursor = Cursors.Hand;
            }
        }

        private void XAxisLabelEditable_PointerPressed(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (this.XLabelsEditable == false)
                return;

            TextBlock tBlock = sender as TextBlock;

            this.XLabelEditable.Visibility = Visibility.Visible;
            this.XLabelEditable.Text = tBlock.Text;
            this.XLabelEditable.BorderBrush = Brushes.Black;
            this.XLabelEditable.Background = Brushes.White;
            this.XLabelEditable.Foreground = Brushes.Black;
            this.xLabelEditable.BorderThickness = new Thickness(0.5);
            this.XLabelEditable.FontSize = this.yLabels[0].FontSize - 1;
            this.XLabelEditable.VerticalContentAlignment = VerticalAlignment.Center;
            this.XLabelEditable.FontFamily = this.yLabels[0].FontFamily;
            this.XLabelEditable.FontWeight = this.yLabels[0].FontWeight;
            this.xLabelEditable.Tag = tBlock.Tag;

            this.XLabelEditable.Width = this.GetTextBlockActualWidth(tBlock) + TICK_SIZE;
            this.XLabelEditable.Height = this.GetTextBlockActualHeight(tBlock) + TICK_SIZE;

            double getTop = tBlock.GetTop();
            double top = getTop;
            if (this.orientationMode == OrientationMode.Horizontal)
                top = getTop + this.ChartArea.Bottom;
            else if (this.orientationMode == OrientationMode.VerticalTopToBottom)
                top = getTop + this.YAxis.Height;
            else if (this.orientationMode == OrientationMode.VerticalBottomToTop)
                top = getTop;

            double getLeft = tBlock.GetLeft();
            double left = getLeft + this.XAxis.Margin.Left;
            if (this.orientationMode != OrientationMode.Horizontal)
                left = getLeft;

            this.XLabelEditable.Margin = new Thickness(left, top, 0, 0);  //SetLocation(left, top);
            this.XLabelEditable.Focus();
            this.XLabelEditable.SelectAll();

            this.XLabelEditable.KeyDown += XLabelEditable_KeyDown;
        }

        private void XAxisLabelEditable_PointerMoved(object sender, MouseEventArgs e)
        {
            e.Handled = true;

            if (this.XLabelsEditable == false)
                return;

            TextBlock tBlock = sender as TextBlock;
            tBlock.Cursor = Cursors.Hand;
        }

        private void YAxisLabelEditable_PointerPressed(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (this.YLabelsEditable == false)
                return;

            TextBlock tBlock = sender as TextBlock;

            this.YLabelEditable.Visibility = Visibility.Visible;
            this.YLabelEditable.Text = tBlock.Text;
            this.YLabelEditable.BorderBrush = Brushes.Black;
            this.YLabelEditable.Background = Brushes.White;
            this.YLabelEditable.Foreground = Brushes.Black;
            this.YLabelEditable.BorderThickness = new Thickness(0.5);
            this.YLabelEditable.FontSize = this.yLabels[0].FontSize - 1;
            this.YLabelEditable.VerticalContentAlignment = VerticalAlignment.Center;
            this.YLabelEditable.FontFamily = this.yLabels[0].FontFamily;
            this.YLabelEditable.FontWeight = this.yLabels[0].FontWeight;
            this.YLabelEditable.Tag = tBlock.Tag;

            this.YLabelEditable.Width = this.GetTextBlockActualWidth(tBlock) + TICK_SIZE;
            this.YLabelEditable.Height = this.GetTextBlockActualHeight(tBlock) + TICK_SIZE;

            double getTop = tBlock.GetTop();
            double top = getTop;
            if (this.orientationMode == OrientationMode.Horizontal)
                top = getTop + this.ChartArea.Top;
            else if (this.orientationMode == OrientationMode.VerticalTopToBottom)
                top = getTop;
            else if (this.orientationMode == OrientationMode.VerticalBottomToTop)
                top = getTop + this.ChartArea.Height;

            double getLeft = tBlock.GetLeft();
            double left = getLeft;
            if (this.orientationMode == OrientationMode.Horizontal)
                left = getLeft + this.YAxis.Margin.Left;
            else /*if (this.orientationMode != OrientationMode.Horizontal)*/
            {
                if (getLeft > this.XAxis.Width)
                    left = getLeft + this.XAxis.Width - this.ChartArea.Left - TICK_SIZE;
                else // is yMin
                    left = this.XAxis.Width - this.ChartArea.Left - getLeft + TICK_SIZE;
            }

            this.YLabelEditable.Margin = new Thickness(left, top, 0, 0);  //SetLocation(left, top);
            this.YLabelEditable.Focus();
            this.YLabelEditable.SelectAll();

            this.YLabelEditable.KeyDown -= YLabelEditable_KeyDown;
            this.YLabelEditable.KeyDown += YLabelEditable_KeyDown;
        }

        private void YAxisLabelEditable_PointerMoved(object sender, MouseEventArgs e)
        {
            e.Handled = true;

            if (this.XLabelsEditable == false)
                return;

            TextBlock tBlock = sender as TextBlock;
            tBlock.Cursor = Cursors.Hand;
        }

        private void XAxis_PointerPressed(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (!this.enableMouseActions || this.transform == null)
                return;

            this.YLabelEditable.Visibility = Visibility.Collapsed;
            Point clickedPoint = e.GetPosition(this);

            if (this.orientationMode != OrientationMode.Horizontal)
                clickedPoint = e.GetPosition(this.XAxis);

            this.ResetLabelsEditable();

            this.mousePressed = true;
            this.startPosition = this.transform.ScreenToViewport(this.TransformPoint(clickedPoint));

            this.XAxis.CaptureMouse();
            this.scrollX = true;
        }

        private void YAxis_PointerPressed(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (!this.enableMouseActions || this.transform == null)
                return;

            this.YLabelEditable.Visibility = Visibility.Collapsed;
            Point clickedPoint = e.GetPosition(this);

            if (this.orientationMode != OrientationMode.Horizontal)
                clickedPoint = e.GetPosition(this.YAxis);

            this.ResetLabelsEditable();

            this.mousePressed = true;
            this.startPosition = this.transform.ScreenToViewport(this.TransformPoint(clickedPoint));

            this.YAxis.CaptureMouse();
            this.scrollX = false;
        }

        private void YLabelEditable_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            this.ResetLabelsEditable();

            if (string.IsNullOrEmpty(this.YLabelEditable.Text))
                return;

            double editVal = TConvert.To<double>(this.YLabelEditable.Text);

            object tag = this.YLabelEditable.Tag;

            if (tag is Array || tag is object[])
            {
                object[] obj = tag as object[];
                bool isMin = TConvert.To<bool>(obj[0]);
                LineGraph pen = obj[1] as LineGraph;
                double min = pen.MinValue;
                double max = pen.MaxValue;

                if (isMin)
                {
                    min = editVal;
                    if (min >= max)
                        return;
                }
                else
                {
                    max = editVal;
                    if (max <= min)
                        return;
                }

                if (this.typeChart == TypeChart.TrendChart)
                {
                    TrendPen trendPen = pen.TrendPen as TrendPen;
                    trendPen.MinValue = min;
                    trendPen.MaxValue = max;
                    this.RaiseDataChanged(null, new GenericObjectEventArgs(trendPen, null, null));
                }
                else
                {
                    TrendPen trendPen = pen.TrendPen as TrendPen;
                    trendPen.MinValue = min;
                    trendPen.MaxValue = max;
                    this.RaiseDataChanged(null, new GenericObjectEventArgs(trendPen, null, null));
                }
            }
            else
            {
                int labelIndex = TConvert.To<int>(tag);
                double min = this.yMin;
                double max = this.yMax;

                if (labelIndex == 0)
                {
                    min = editVal;
                    if (min >= max)
                        return;

                    this.yMin = min;
                }
                else
                {
                    max = editVal;
                    if (max <= min)
                        return;

                    this.yMax = max;
                }

                if (this.ChartArea != null && this.ChartArea.Width > 0 && this.ChartArea.Height > 0)
                    this.transform = CoordinateTransform.FromRects(
                        new Rect(this.xMin.Ticks / 10000000000.0, this.yMin, Math.Abs((this.xMax.Ticks / 10000000000.0) - (this.xMin.Ticks / 10000000000.0)), Math.Abs(this.yMax - this.yMin)),
                        new Rect(0, 0, this.ChartArea.Width, this.ChartArea.Height));

                foreach (LineGraph pen in this.pens)
                {
                    if (this.typeChart == TypeChart.TrendChart)
                    {
                        TrendPen trendPen = pen.TrendPen as TrendPen;
                        trendPen.MinValue = min;
                        trendPen.MaxValue = max;
                        this.RaiseDataChanged(null, new GenericObjectEventArgs(trendPen, null, null));
                    }
                    else
                    {
                        TrendPen trendPen = pen.TrendPen as TrendPen;
                        trendPen.MinValue = min;
                        trendPen.MaxValue = max;
                        this.RaiseDataChanged(null, new GenericObjectEventArgs(trendPen, null, null));
                    }
                }

                if (this.VisibleRectChanged != null)
                    this.VisibleRectChanged(this, new VisibleRectEventArgs(this.xMin, this.xMax, min, max, null, true));
            }
        }

        private void XLabelEditable_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            this.ResetLabelsEditable();

            if (string.IsNullOrEmpty(this.XLabelEditable.Text))
                return;

            if (DateTime.TryParse(this.XLabelEditable.Text, out DateTime newDtVal))
            {
                int labelIndex = TConvert.To<int>((sender as TextBox).Tag);
                bool isMin = labelIndex == 0;
                // Min
                if (isMin)
                {
                    if (newDtVal >= this.xMax)
                        return;

                    this.VisibleRectChanged(this, new VisibleRectEventArgs(newDtVal, this.xMax, this.YMin, this.YMax, null, false));
                }
                else
                {
                    if (newDtVal <= this.xMin)
                        return;

                    this.VisibleRectChanged(this, new VisibleRectEventArgs(this.xMin, newDtVal, this.YMin, this.YMax, null, false));
                }
            }
        }

        private void Axis_PointerReleased(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            this.YAxis.Cursor = Cursors.Arrow;

            if (this.mousePressed)
            {
                this.mousePressed = false;
                this.startPosition = new Point(0, 0);
                if (this.scrollX)
                    this.XAxis.ReleaseMouseCapture();
                else
                    this.YAxis.ReleaseMouseCapture();
            }
        }

        private void XAxis_PointerMoved(object sender, MouseEventArgs e)
        {
            e.Handled = true;

            if (!this.EnableMouseActions)
            {
                this.XAxis.Cursor = Cursors.Arrow;
                return;
            }

            this.XAxis.Cursor = this.Orientation == 0 ? Cursors.SizeWE : Cursors.SizeNS;

            this.Axis_PointerMoved(sender, e);
        }

        private void YAxis_PointerMoved(object sender, MouseEventArgs e)
        {
            e.Handled = true;

            if (!this.EnableMouseActions)
            {
                this.XAxis.Cursor = System.Windows.Input.Cursors.Arrow;
                return;
            }

            this.YAxis.Cursor = this.Orientation == 0 ? Cursors.SizeNS : Cursors.SizeWE;

            this.Axis_PointerMoved(sender, e);
        }

        private void Axis_PointerMoved(object sender, MouseEventArgs e)
        {
            e.Handled = true;

            if (!this.enableMouseActions)
                return;

            bool onlyChangedY = false;

            if (!this.mousePressed)
                return;

            Point mousePos = this.TransformPoint(e.GetPosition(this));

            Point dataMousePos = this.transform.ScreenToViewport(mousePos);
            DataRect visible = this.Transform.ViewportRect;
            double delta;

            Dictionary<object, Point> dicMinMax = null;

            if (this.scrollX)
            {
                delta = dataMousePos.X - this.startPosition.X;
                visible.XMin -= delta;
            }
            else
            {
                onlyChangedY = true;
                delta = dataMousePos.Y - this.startPosition.Y;
                visible.YMin -= delta;
                if (this.IsEnabledYScaleForEachPen)
                {
                    dicMinMax = new Dictionary<object, Point>();
                    for (int i = 0; i < this.GetPenCount(); i++)
                    {
                        object column = this.GetPenCol(i);
                        dicMinMax.Add(column, new Point(visible.YMin, visible.YMax));
                    }
                }
            }

            if (this.VisibleRectChanged != null)
            {
                long x1 = (long)(visible.XMin * 10000000000.0);
                long x2 = (long)(visible.XMax * 10000000000.0);
                DateTime xMin = new DateTime(x1 >= DateTime.MinValue.Ticks && x1 <= DateTime.MaxValue.Ticks ? x1 : DateTime.MinValue.Ticks, DateTimeKind.Utc);
                DateTime xMax = new DateTime(x2 >= DateTime.MinValue.Ticks && x2 <= DateTime.MaxValue.Ticks ? x2 : DateTime.MinValue.Ticks, DateTimeKind.Utc);
                this.VisibleRectChanged(this, new VisibleRectEventArgs(xMin, xMax, visible.YMin, visible.YMax, dicMinMax, onlyChangedY));
            }
        }

        private void ResetLabelsEditable()
        {
            this.XLabelEditable.Visibility = Visibility.Collapsed;
            this.YLabelEditable.Visibility = Visibility.Collapsed;
        }

        public void SetXMinMax(DateTime _xMin, DateTime _xMax)
        {
            this.xMin = _xMin;
            this.xMax = _xMax;

            this.primaryXAxis.SetXAxisDefaultInterval(_xMin, _xMax);

            this.RecalcLayout();
        }

        public void SetYMinMax(double _yMin, double _yMax)
        {
            this.yMin = _yMin;
            this.yMax = _yMax;

            this.axisLimitChanged = true;

            this.RecalcLayout();
        }

        public void SetXYMinMax(DateTime _xMin, DateTime _xMax, double _yMin, double _yMax)
        {
            this.xMin = _xMin;
            this.xMax = _xMax;
            this.yMin = _yMin;
            this.yMax = _yMax;

            this.primaryXAxis.SetXAxisDefaultInterval(_xMin, _xMax);

            this.RecalcLayout();
        }

        public void SetXYMinMax(double _xMin, double _xMax, double _yMin, double _yMax)
        {
            this.xMinValue = _xMin;
            this.xMaxValue = _xMax;
            this.yMin = _yMin;
            this.yMax = _yMax;

            this.primaryXAxis.SetXAxisDefaultInterval(_xMin, _xMax);

            this.RecalcLayout();
        }

        private void Plotter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.RecalcLayout();
        }

        public void RecalcLayout()
        {
            if (this.hasRecalcLayoutPendent != 0)
            {
                int waitTime = this.IsXYChart ? 1 : 5;

                long utcNowTicks = DateTime.UtcNow.Ticks;
                if (utcNowTicks < this.hasRecalcLayoutPendent)
                    this.hasRecalcLayoutPendent = utcNowTicks;
                if (utcNowTicks - this.hasRecalcLayoutPendent < TimeSpan.FromSeconds(5).Ticks)
                    return;
            }
            this.hasRecalcLayoutPendent = DateTime.UtcNow.Ticks;

            this.Dispatcher.BeginInvoke(this._RecalcLayout);
        }

        private void _RecalcLayout()
        {
            //DateTime now1 = DateTime.Now;
            try
            {

                //TDiagnostics.ConsoleWrite($"SfChart :: _RecalcLayout :: Start");


                this.hasRecalcLayoutPendent = 0;

                #region Methods To Update SfChart

                this.UpdatePrimaryAxis();
                bool shouldReturnAllScales = true;
                bool shouldApplyUnitsConversion = false;
                List<object[]> penScales = this.GetPensYScale(shouldReturnAllScales, shouldApplyUnitsConversion);

                // Count total visible pens once
                int totalVisiblePens = 0;
                foreach (LineGraph p in this.pens)
                {
                    if (p.Visibility == Visibility.Visible)
                        totalVisiblePens++;
                }

                int idx = 0;
                int visiblePenIdx = 0; // Track visible pens for RowIndex calculation
                foreach (LineGraph pen in this.pens)
                {
                    double yAxisMin = pen.MinValue;
                    double yAxisMax = pen.MaxValue;
                    Brush stroke = null;
                    bool isAutoEnabled = false;
                    if (this.typeChart == TypeChart.TrendChart)
                        isAutoEnabled = (pen.TrendPen as TrendPen).Auto;
                    else
                        isAutoEnabled = (pen.TrendPen as TrendPen).Auto;

                    if (this.IsEnabledYScaleForEachPen == false)
                    {
                        yAxisMin = this.yMin;
                        yAxisMax = this.YMax;
                    }
                    else if (penScales != null && penScales.Count > 0 && penScales.Count >= idx && isAutoEnabled == false)
                    {
                        yAxisMin = TConvert.ToDouble(penScales[idx][1]);
                        yAxisMax = TConvert.ToDouble(penScales[idx][2]);
                    }

                    if (penScales != null && penScales.Count > 0 && penScales.Count >= idx)
                        stroke = penScales[idx][0] as Brush;

                    if(this.IsXYChart)
                    {
                        /// When chart is XY we assume the Yaxis Limits are already set.
                        yAxisMin = 0;  /// The SfChart will always be in the [0, 100] range, the labels will assume the correct values for the tags, but will be scaled
                        yAxisMax = 100; /// The SfChart will always be in the [0, 100] range, the labels will assume the correct values for the tags, but will be scaled
                    }

                    int currentVisibleIdx = pen.Visibility == Visibility.Visible ? visiblePenIdx : -1;
                    this.UpdateSeries(pen, yAxisMin, yAxisMax, idx, currentVisibleIdx, totalVisiblePens, stroke);

                    if (idx > 0 || this.isStackPensMode)
                    {
                        this.UpdateGridAxes(pen, yAxisMin, yAxisMax, idx, currentVisibleIdx, totalVisiblePens);
                    }

                    idx++;

                    if (pen.Visibility == Visibility.Visible)
                        visiblePenIdx++;
                }

                this.UpdateCursors();
                this.CreateChart();
                // This method must be called after create chart to guarante the annotation elements are created.
                this.AddCloseButtonEventToCursorLines();

                #endregion Methods To Update SfChart

                #region Methods to Update Axis Elements

                if (this.chart != null && this.isReady)
                {
                    this.CreateLabels();

                    this.CreateLegendWindowItems();

                    this.UpdateVisuals();
                }

                #endregion Methods to Update Axis Elements

                if(this.IsXYChart)
                {                   
                    if(!this.registeredUpdateEventForXyChart)
                    {
                        this.registeredUpdateEventForXyChart = true;
                        ConfigHelper.RegisterEvent(ConfigHelper.ObjServer.DB.GetObjRef("Client.Second", false), null, null, this, (EventHandler<RuntimeEventArgs>)(async delegate (object sender, RuntimeEventArgs e)
                        {

                            this.isReady = true;

                            this.canRefreshChart = DateTime.Now.Second % 3 == 0;
                            if (this.canRefreshChart)
                                this.RefreshChart();

                            this.CreateLabels();
                            this.CreateLegendWindowItems();
                            this.UpdateVisuals();
                            await Task.CompletedTask;
                        }), false, -1);

                    }
                }


                this.transform = CoordinateTransform.FromRects(new Rect(this.xMin.Ticks / 10000000000.0, this.yMin, Math.Abs((this.xMax.Ticks / 10000000000.0) - (this.xMin.Ticks / 10000000000.0)), Math.Abs(this.yMax - this.yMin)),
              new Rect(0, 0, this.ChartArea.Width, this.ChartArea.Height));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"_RecalcLayout Exception :: {ex.Message} || {ex.StackTrace}");
            }

            //TDiagnostics.ConsoleWrite($"SfChart :: _RecalcLayout :: Finish :: {DateTime.Now - now1}");
        }

        private bool canRefreshChart = false;
        private bool registeredUpdateEventForXyChart = false;

        public void UpdateVisuals()
        {
            if (!this.IsXYChart && this.initializationLoops < 3)
                return;
           
            this.UpdatePanels();

            this.UpdateLegendItems();

            this.UpdateTicks();

            this.UpdateGridLines();
            
            this.UpdateLabels();

        }

        private void AddCloseButtonEventToCursorLines()
        {
            try
            {
                if (this.primaryCursorValuesAnnotation != null && this.primaryCursorClicked)
                {
                    if (this.primaryCursorValuesAnnotation.IsCloseButtonVisible)
                        TInterop.ExecuteJavaScriptBase("document.getElementById($0).onmousedown = $1", this.primaryCursorValuesAnnotation.CloseButtonId, TInterop.CreateJavascriptCallback((Action<object>)this.OnPrimaryCursorCloseButonPressed));
                }

                if (this.secondaryCursorValuesAnnotation != null && this.secondaryCursorClicked)
                {
                    if (this.secondaryCursorValuesAnnotation.IsCloseButtonVisible)
                        TInterop.ExecuteJavaScriptBase("document.getElementById($0).onmousedown = $1", this.secondaryCursorValuesAnnotation.CloseButtonId, TInterop.CreateJavascriptCallback((Action<object>)this.OnSecondaryCursorCloseButonPressed));
                }
            }
            catch { }
        }

        public bool RefreshChart()
        {
            try
            {
                if (this.chart == null)
                    return false;

                TInterop.ExecuteJavaScriptBase(@"$0.refresh()", this.chart);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RefreshChart Exception :: {ex.Message} || {ex.StackTrace}");
            }
            return false;
        }

        private void UpdateCursors()
        {
            this.RecalculatePrimaryCursorTooltip(this.EnabledCursor, this.ShowPrimaryCursorValues, this.CursorOutputTooltip);
            this.UpdatePrimaryCursor();

            this.RecalculateSecondaryCursorTooltip(this.EnabledSecondaryCursor, this.ShowSecondaryCursorValues, this.SecondaryCursorOutputTooltip);
            this.UpdateSecondaryCursor();
        }

        private void UpdateGridLines()
        {
            try
            {
                if (this.ChartArea.Height < 1 || this.ChartArea.Width < 1)
                    return;

                double offsetWidth = this.ChartArea.X;
                double offsetHeight = this.ChartArea.Y;

                double height = this.ChartArea.Height;
                double width = this.ChartArea.Width;

                double x1 = this.ChartArea.Left;
                double x2 = this.ChartArea.Right;

                double y1;
                double y2;

                double piece = this.numOfLabelsY == 1 ? 0.0 : this.YAxis.Height / (this.numOfLabelsY - 1);
                if (this.orientationMode != OrientationMode.Horizontal)
                    piece = this.numOfLabelsY == 1 ? 0.0 : this.YAxis.Width / (this.numOfLabelsY - 1);

                for (int i = 0; i < this.numOfLabelsY; i++)
                {
                    double value = i * piece + offsetHeight;
                    y1 = value;
                    y2 = value;
                    if (this.orientationMode != OrientationMode.Horizontal)
                    {
                        x1 = value;
                        x2 = value;

                        y1 = this.ChartArea.Top;
                        y2 = this.ChartArea.Bottom + offsetHeight;
                    }
                    this.yGridLines[i].Width = width;
                    this.yGridLines[i].Height = height;
                    this.yGridLines[i].X1 = x1;
                    this.yGridLines[i].Y1 = y1;
                    this.yGridLines[i].X2 = x2;
                    this.yGridLines[i].Y2 = y2;
                }

                piece = this.numOfLabelsX == 1 ? 0.0 : this.XAxis.Width / (this.numOfLabelsX - 1);
                if (this.orientationMode != OrientationMode.Horizontal)
                    piece = this.numOfLabelsX == 1 ? 0.0 : this.XAxis.Height / (this.numOfLabelsX - 1);

                y1 = offsetHeight;
                y2 = this.ChartArea.Bottom + (2 * offsetHeight);

                for (int i = 0; i < this.numOfLabelsX; i++)
                {
                    double value = i * piece + offsetWidth;

                    if (i > 0 && this.orientationMode != OrientationMode.VerticalTopToBottom)
                    {
                        value -= TICK_SIZE;
                    }

                    x1 = value;
                    x2 = value;

                    if (this.orientationMode != OrientationMode.Horizontal)
                    {
                        //value += TICK_SIZE;
                        x1 = this.ChartArea.Left;
                        x2 = this.ChartArea.Right + offsetWidth;

                        if (this.orientationMode == OrientationMode.VerticalBottomToTop)
                            value = this.ChartArea.Bottom - (i * piece);

                        y1 = value;
                        y2 = value;
                    }

                    this.xGridLines[i].Width = width;
                    this.xGridLines[i].Height = height;
                    this.xGridLines[i].X1 = x1;
                    this.xGridLines[i].Y1 = y1;
                    this.xGridLines[i].X2 = x2;
                    this.xGridLines[i].Y2 = y2;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in UpdateGridLines : {ex.Message}");
            }
        }

        private double GetEscalonatedCursorPositionPoint(double originaValue, bool isXPoint)
        {
            try
            {
                Rect rect = this.ChartArea;

                if (isXPoint)
                {
                    double escalonatedX = (originaValue - rect.X) * 100 / (rect.Width - rect.X);

                    if (double.IsNaN(escalonatedX) || double.IsInfinity(escalonatedX))
                        escalonatedX = 0;

                    return escalonatedX;
                }
                double escalonatedY = (originaValue - rect.Y) * 100 / (rect.Height - rect.Y);
                if (double.IsNaN(escalonatedY) || double.IsInfinity(escalonatedY))
                    escalonatedY = 0;

                return escalonatedY;
            }
            catch { }

            return originaValue;
        }

        private Point GetCursorPositionPixelValue(Point originalPoint, bool isXPoint)
        {
            Point escalonatedPoint = new Point(100, 0);

            try
            {
                Rect rect = this.ChartArea;
                double escalonatedY = originalPoint.Y * (rect.Height - rect.Y) / 100 + rect.Y;
                double escalonatedX = originalPoint.X * (rect.Width - rect.X) / 100 + rect.X;

                escalonatedPoint = new Point(escalonatedX, escalonatedY);

                if (isXPoint)
                    return new Point(escalonatedX, originalPoint.Y);

                return new Point(originalPoint.X, escalonatedY);
            }
            catch { }

            return escalonatedPoint;
        }

        private void UpdateAlarmItems(LineGraph pen, double yAxisMin, double yAxisMax, int idx)
        {
            string colorAlarm = null;

            if (pen.AlarmItems == null || pen.AlarmItems.Count < 1)
                return;

            List<Point> points = new List<Point>(pen.AlarmItems.Count);
            List<object[]> alarmInfo = new List<object[]>(pen.AlarmItems.Count);

            int counterActive = 0;
            int counterNorm = 0;
            int counterAck = 0;

            foreach (TSystem.AlarmEventInfoBase alarm in pen.AlarmItems)
            {
                DateTime dtCreated = alarm.DateCreated.UtcDateTime;
                double ts = 0;

                double value = TConvert.To<double>(alarm.Value);

                EnumerableDataSource<TrendPointInfo> data = pen.DataSource as EnumerableDataSource<TrendPointInfo>;
                if (data != null)
                    ts = data.XMapping(new TrendPointInfo(dtCreated, value, 192, TSystem.eTrendLogType.OnExecuting, true));

                points.Add(new Point(ts, value));

                string state = "";
                string dtAlarm = this.GetStringXAxis(0, dtCreated.ToLocalTime());

                state = TConvert.ToString(TConvert.To<TSystem.eAlarmState>(alarm.State));
                if (alarm.State == (int)TSystem.eAlarmState.Active)
                {
                    dtAlarm = this.GetStringXAxis(0, alarm.ActiveTime.ToLocalTime().DateTime);
                    state = "Active";
                    counterActive++;
                }
                else if (alarm.State == (int)TSystem.eAlarmState.Normalized)
                {
                    dtAlarm = this.GetStringXAxis(0, alarm.NormTime.ToLocalTime().DateTime);
                    state = "Normalized";
                    counterNorm++;
                }
                else if (alarm.State == (int)TSystem.eAlarmState.Acknowledge)
                {
                    dtAlarm = this.GetStringXAxis(0, alarm.AckTime.ToLocalTime().DateTime);
                    state = "Acknowledge";
                    counterAck++;
                }

                alarmInfo.Add(new object[] { state, dtAlarm, alarm.State });

                colorAlarm = alarm.ColorFG;
            }

            object chartDataActive = TInterop.ExecuteJavaScriptBase(@"[]");
            object chartDataNorm = TInterop.ExecuteJavaScriptBase(@"[]");
            object chartDataAck = TInterop.ExecuteJavaScriptBase(@"[]");
            object objData;
            double dataValue;
            DateTime timestamp;
            for (int i = 0; i < points.Count; i++)
            {
                Point point = points[i];
                object[] info = alarmInfo[i];

                objData = TInterop.ExecuteJavaScriptBase(@"new Object()");

                dataValue = point.Y;
                TInterop.ExecuteJavaScriptBase("$0.y = Number($1)", objData, dataValue);
                long l = TConvert.To<long>(point.X * 10000000000.0);
                timestamp = new DateTime(l);

                string jsTime = this.primaryXAxis.DateTimeToJavaTimeStamp(timestamp).ToString(TCultureInfo.InvariantCulture);

                object objDate = TInterop.ExecuteJavaScriptBase(@"new Date(Number($0))", jsTime);
                TInterop.ExecuteJavaScriptBase("$0.x = $1", objData, objDate);

                // Add auxiliary info for Alarms
                TInterop.ExecuteJavaScriptBase("$0.alarmState = $1", objData, info[0]);
                TInterop.ExecuteJavaScriptBase("$0.alarmTimestamp = $1", objData, info[1]);
                int alarmState = TConvert.To<int>(info[2]);
                if (alarmState == (int)TSystem.eAlarmState.Active)
                {
                    TInterop.ExecuteJavaScriptBase("$0.push($1)", chartDataActive, objData);
                }
                else if (alarmState == (int)TSystem.eAlarmState.Normalized)
                {
                    TInterop.ExecuteJavaScriptBase("$0.push($1)", chartDataNorm, objData);
                }
                else if (alarmState == (int)TSystem.eAlarmState.Acknowledge)
                {
                    TInterop.ExecuteJavaScriptBase("$0.push($1)", chartDataAck, objData);
                }
            }

            string penLabel = this.typeChart == TypeChart.TrendChart ? (pen.TrendPen as TrendPen).PenLabelOutput : (pen.TrendPen as TrendPen).PenLabelOutput;

            string toolTipText = $"{penLabel}<br>" +
                                 "Value: ${point.y}<br>" +
                                 "Timestamp: ${alarmTimestamp}<br>" +
                                 "State: ${alarmState}";

            if (counterActive > 0)
            {
                Marker marker = new Marker(true, 3);
                marker.FillColor = null;
                marker.Shape = "Image";
                marker.Height = 15;
                marker.Width = 15;
                marker.DataLabelFormat = toolTipText;
                marker.ImageUrl = "Resources/TTrendChart/Icons/AlarmActive.png";

                TSeries series = new TSeries(chartDataActive, "Line", "x", "y", null, marker);
                series.FillColor = null;//fillColorActive.Image;
                if (idx > 0)
                    series.YAxisName = $"yAxis{idx}";
                series.Visible = this.typeChart == TypeChart.TrendChart ? (pen.TrendPen as TrendPen).Visible : (pen.TrendPen as TrendPen).Visible;
                series.Width = 1;

                this.listOfChartSeries.Add(series);
            }

            if (counterNorm > 0)
            {
                Marker marker = new Marker(true, 3);
                marker.FillColor = null;
                marker.Shape = "Image";
                marker.Height = 15;
                marker.Width = 15;
                marker.DataLabelFormat = toolTipText;
                marker.ImageUrl = "Resources/TTrendChart/Icons/AlarmNormalized.png";

                TSeries series = new TSeries(chartDataActive, "Line", "x", "y", null, marker);
                series.FillColor = null;//fillColorActive.Image;
                if (idx > 0)
                    series.YAxisName = $"yAxis{idx}";
                series.Visible = this.typeChart == TypeChart.TrendChart ? (pen.TrendPen as TrendPen).Visible : (pen.TrendPen as TrendPen).Visible;
                series.Width = 1;

                this.listOfChartSeries.Add(series);
            }

            if (counterAck > 0)
            {
                Marker marker = new Marker(true, 3);
                marker.FillColor = null;
                marker.Shape = "Image";
                marker.Height = 15;
                marker.Width = 15;
                marker.DataLabelFormat = toolTipText;
                marker.ImageUrl = "Resources/TTrendChart/Icons/AlarmActiveAcked.png";

                TSeries series = new TSeries(chartDataActive, "Line", "x", "y", null, marker);
                series.FillColor = null;//fillColorActive.Image;
                if (idx > 0)
                    series.YAxisName = $"yAxis{idx}";
                series.Visible = this.typeChart == TypeChart.TrendChart ? (pen.TrendPen as TrendPen).Visible : (pen.TrendPen as TrendPen).Visible;
                series.Width = 1;

                this.listOfChartSeries.Add(series);
            }
        }

        private void UpdateSeries(LineGraph pen, double yAxisMin, double yAxisMax, int idx, int visiblePenIdx, int totalVisiblePens, Brush brush = null)
        {
            try
            {
                Marker marker = new Marker(false, -1);
                string penSettings;
                string penLabelOutput;
                double penMin;
                double penMax;
                bool penVisible;
                string spcDescription;
                bool isSPC = false;
                if (this.typeChart == TypeChart.TrendChart)
                {
                    TrendPen trendPenObj = pen.TrendPen as TrendPen;
                    penSettings = trendPenObj.PenSettings;
                    if (string.IsNullOrWhiteSpace(penSettings))
                        penSettings = $"Stroke={trendPenObj.PenColor}";
                    penLabelOutput = trendPenObj.PenLabelOutput;
                    penMin = trendPenObj.MinValue;
                    penMax = trendPenObj.MaxValue;
                    penVisible = trendPenObj.Visible;
                }
                else if (this.typeChart == TypeChart.DrillingChart)
                {
                    TrendPen drillingPenObj = pen.TrendPen as TrendPen;
                    penSettings = drillingPenObj.PenSettings;
                    if (string.IsNullOrWhiteSpace(penSettings))
                        penSettings = $"Stroke={drillingPenObj.PenColor}";
                    penLabelOutput = drillingPenObj.PenLabelOutput;
                    penMin = drillingPenObj.MinValue;
                    penMax = drillingPenObj.MaxValue;
                    penVisible = drillingPenObj.Visible;
                }
                else               
                    return;

                spcDescription = pen.SPCDescription;
                isSPC = !string.IsNullOrEmpty(spcDescription);
                if(isSPC && brush != null)
                {
                    penLabelOutput += $"({spcDescription})";

                    string color = ((SolidColorBrush)brush).Color.ToString();
                    penSettings = $"Stroke={color}";
                }
                this.mapPenLabelToLineGraph[idx] = pen;
                List<Point> points = GetDataPoints(pen.DataSource).ToList();
                object _chartData = TInterop.ExecuteJavaScriptBase(@"[]");

                if (this.IsXYChart)
                {
                    penSettings = $"Stroke=#FF008000";
                    
                    bool isUsingPointXYInfo = pen.DataSource is EnumerableDataSource<PointXYInfo>;
                    EnumerableDataSource<PointXYInfo> pointsXYInfoData = null;
                    TLinkedList<PointXYInfo> pointsXYInfo = null;
                    EnumerableDataSource<TrendPointInfo> pointsTrendPointInfoData = null;
                    TLinkedList<TrendPointInfo> pointsTrendPointInfo = null;

                    if (isUsingPointXYInfo)
                        pointsXYInfoData = pen.DataSource as EnumerableDataSource<PointXYInfo>;
                    else
                        pointsTrendPointInfoData = pen.DataSource as EnumerableDataSource<TrendPointInfo>;

                    if(isUsingPointXYInfo)
                        pointsXYInfo = pointsXYInfoData.Data as TLinkedList<PointXYInfo>;
                    else
                        pointsTrendPointInfo = pointsTrendPointInfoData.Data as TLinkedList<TrendPointInfo>;
                    
                    object pi;

                    int pIdx = 0;


                    double dataValue;
                    double dataX;
                    object objData;
                    foreach (Point point in points)
                    {
                        objData = TInterop.ExecuteJavaScriptBase(@"new Object()");
                        
                        if(isUsingPointXYInfo)
                            pi = pointsXYInfo.ElementAt(pIdx);
                        else
                            pi = pointsTrendPointInfo.ElementAt(pIdx);
                        
                        dataValue = point.Y;
                        if (isUsingPointXYInfo)
                            dataX = point.X;
                        else
                            dataX = (pi as TrendPointInfo).XValue;


                        TInterop.ExecuteJavaScriptBase("$0.y = Number($1)", objData, dataValue);
                        TInterop.ExecuteJavaScriptBase("$0.x = Number($1)", objData, dataX);
                        pIdx += 1;
                        TInterop.ExecuteJavaScriptBase("$0.push($1)", _chartData, objData);
                    }
                }
                else
                {
                    var pointsInfoData = pen.DataSource as EnumerableDataSource<TrendPointInfo>;
                    var pointsInfo = pointsInfoData.Data as TLinkedList<TrendPointInfo>;
                    using (var infoEnum = pointsInfo.GetEnumerator())
                    {
                        int n = points.Count;
                        var xs = new string[n];  // timestamps (string invariável)
                        var ys = new double[n];
                        var tips = new string[n];
                        var vis = new bool[n];
                        var size = new double[n];
                        var fill = new string[n];

                        TimeSpan offset = TimeSpan.Zero;
                        if (this.typeChart == TypeChart.TrendChart)
                            offset = (pen.TrendPen as TrendPen).HorizontalOffset;
                        else
                            offset = (pen.TrendPen as TrendPen).HorizontalOffset;

                        int i = 0;
                        foreach (var point in points)
                        {
                            if (!infoEnum.MoveNext()) 
                                break;
                            var pi = infoEnum.Current;

                            ys[i] = point.Y;

                            long l = TConvert.To<long>(point.X * 10000000000.0);
                            var timestamp = new DateTime(l);
                            xs[i] = this.primaryXAxis
                                       .DateTimeToJavaTimeStamp(timestamp)
                                       .ToString(TCultureInfo.InvariantCulture);

                            var dt = (offset != TimeSpan.Zero)
                                        ? (pi.dt - this.GetHorizontalOffset(pen.TrendPen))
                                        : pi.dt;

                            tips[i] = this.FormatTooltip2(pen.TrendPen, ys[i], dt, pi.quality, pi);
                            vis[i] = this.GetMarkerVisible(pi, pen.TrendPen);
                            size[i] = this.GetMarkerSize(pen.TrendPen);

                            string f = this.GetMarkerFill(pi, pen.TrendPen).ToString();
                            if (f.StartsWith("#"))
                            {
                                f = f.Substring(1);
                                bool hasAlpha = f.Length > 6;
                                int colorIdx = hasAlpha ? 2 : 0;
                                f = "#" + f.Substring(colorIdx);
                            }
                            fill[i] = f;

                            i++;
                        }

                        var payload = new { xs, ys, tips, vis, size, fill };
                        string json = System.Text.Json.JsonSerializer.Serialize(payload);

                        TInterop.ExecuteJavaScriptBase(@"
                            (function (chart, json) {
                              var p = JSON.parse(json);
                              for (var i = 0; i < p.ys.length; i++) {
                                var t = p.xs[i];
                                t = (typeof t === 'number') ? t : Number(t);
                                if (t < 1e12) t *= 1000;               // in seconds
                                else if (t > 1e15)  t = Math.floor(t/1e4) - 62135596800000; // ticks .NET
                                var obj = {};
                                obj.y = Number(p.ys[i]);
                                obj.x = new Date(t);
                                obj.tooltipCallback    = p.tips[i];
                                obj.visibilityCallback = p.vis[i];
                                obj.sizeCallback       = p.size[i];
                                obj.fillCallback       = p.fill[i];
                                chart.push(obj);
}
                            })($0, $1);
                            ", _chartData, json);
                    }
                }

                string chartType = "Line";
                if (pen.UseSquare)
                    chartType = "StepLine";

                TSeries series = new TSeries(_chartData, chartType, "x", "y", penLabelOutput, marker);

                if (idx > 0 || this.isStackPensMode)
                    series.YAxisName = $"yAxis{idx}";
                series.Visible = penVisible;
              
                string[] penSettingsArray = penSettings.Split(';');

                if (penSettingsArray != null && penSettingsArray.Length > 0)
                {
                    foreach (string prop in penSettingsArray)
                    {
                        if (prop.StartsWith("Stroke=", StringComparison.InvariantCultureIgnoreCase))
                        {
                            string temp = prop.Substring("Stroke=".Length);
                            string colorAsText = prop.Substring("Stroke=".Length);

                            if (colorAsText.StartsWith("#"))
                            {
                                string tempColor = colorAsText.Substring(3);
                                tempColor = "#" + tempColor;
                                colorAsText = tempColor;
                                series.FillColor = colorAsText;
                                series.Marker.FillColor = colorAsText;
                            }
                        }
                        if (prop.StartsWith("Fill=", StringComparison.InvariantCultureIgnoreCase))
                        {
                            series.Opacity = 0.5;
                            series.FillArea = true;
                            series.Type = "Area";
                            if (pen.UseSquare)
                                series.Type = "StepArea";
                        }
                        if (prop.StartsWith("Marker=", StringComparison.InvariantCultureIgnoreCase))
                        {
                            string temp = prop.Substring("Marker=".Length);
                            int nTemp = -1;
                            try
                            {
                                nTemp = int.Parse(temp);
                            }
                            catch
                            {
                                nTemp = -1;
                            }

                            marker.Visible = true;
                            marker.ShapeInt = nTemp;
                        }

                        if (prop.StartsWith("StrokeDashArray=", StringComparison.InvariantCultureIgnoreCase))
                        {
                            string temp = prop.Substring("StrokeDashArray=".Length);
                            series.DashArray = temp;
                        }
                        if (prop.StartsWith("StrokeThickness=", StringComparison.InvariantCultureIgnoreCase))
                        {
                            string temp = prop.Substring("StrokeThickness=".Length);
                            int nTemp = 0;
                            try
                            {
                                nTemp = int.Parse(temp);
                            }
                            catch
                            {
                                nTemp = 1;
                            }

                            series.Width = nTemp;
                        }
                    }
                }

                if (idx == 0)
                {
                    this.primaryYAxis.UpdateYMinAndYMax(yAxisMin, yAxisMax);
                    this.primaryYAxis.LabelStyleColor = series.FillColor;
                }

                this.listOfChartSeries.Add(series);
                this.UpdateAlarmItems(pen, yAxisMin, yAxisMax, idx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateSeries Erros :: {ex.Message} || {ex.StackTrace}");
            }
        }

        private void UpdateGridAxes(LineGraph pen, double penScaleMin, double penScaleMax, int idx, int visiblePenIdx, int totalVisiblePens)
        {
            try
            {
                TAxis axis = new TAxis(penScaleMin, penScaleMax, false);
                axis.UpdateYMinAndYMax(penScaleMin, penScaleMax);
                axis.Name = $"yAxis{idx}";
                axis.LabelFormat = this.LabelFormatY;
                axis.LabelPadding = this.primaryYAxis.LabelPadding;
                axis.NumberOfLabels = this.NumOfLabelsY;
                axis.MajorGridLinesWidth = 0;
                axis.MajorGridLinesColor = this.primaryYAxis.MajorGridLinesColor;
                axis.ZoomFactor = this.primaryYAxis.ZoomFactor;
                axis.ZoomPosition = this.primaryYAxis.ZoomPosition;
                axis.IsInversed = this.primaryYAxis.IsInversed;
                axis.IsVisible = false;

                if (this.isStackPensMode)
                {
                    if (visiblePenIdx >= 0)
                    {
                        if (this.orientationMode == OrientationMode.Horizontal)
                        {
                            int invertedIdx = (totalVisiblePens - 1) - visiblePenIdx;
                            axis.RowIndex = invertedIdx;
                        }
                        else
                        {
                            axis.ColumnIndex = visiblePenIdx;
                        }
                    }
                    else
                    {
                        axis.RowIndex = int.MinValue;
                        axis.ColumnIndex = int.MinValue;
                    }
                }

                this.listOfChartAxes.Add(axis);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateGridAxes Error :: {ex.Message} || {ex.StackTrace}");
            }
        }

        private IEnumerable<Point> GetDataPoints(IPointDataSource data)
        {
            using (IPointEnumerator enumerator = data.GetEnumerator(new DependencyObject()))
            {
                Point p = new Point();
                while (enumerator.MoveNext())
                {
                    enumerator.GetCurrent(ref p);
                    yield return p;
                    p = new Point();
                }
            }
        }

        public TimeSpan GetHorizontalOffset(object column)
        {
            if (column == null)
                return TimeSpan.Zero;

            if (this.typeChart == TypeChart.TrendChart)
                return (column as TrendPen).HorizontalOffset;

            return (column as TrendPen).HorizontalOffset;
        }

        private void CreateLabels()
        {
            this.CreateXLabels();
            this.CreateYLabels();
        }

        private void UpdatePanels()
        {
            try
            {
                double legendWidth = 0;
                double legendHeight = 0;
                bool legendIsInsideChartBounds = (this.LegendPlacement != Base.eLegendPlacement.RightPanel && this.LegendPlacement != Base.eLegendPlacement.BottomPanel);
                bool legendIsInBottomPanel = this.LegendPlacement == Base.eLegendPlacement.BottomPanel;
                bool legendIsInRightPanel = this.legendPlacement == Base.eLegendPlacement.RightPanel;

                bool legendExists = this.LegendPlacement != Base.eLegendPlacement.None;
                if (legendExists && this.pens.Count > 0)
                {
                    if (this.legendLabels == null)
                        return;
                    if (this.legendLines == null)
                        return;

                    for (int i = 0; i < this.pens.Count; i++)
                    {
                        if (i >= this.legendLabels.Length || i >= this.legendLines.Length)
                            continue;

                        double height = this.GetTextBlockActualHeight(this.legendLabels[i]);
                        if (legendIsInBottomPanel)
                        {
                            if (legendHeight < height)
                                legendHeight = height;
                        }
                        else
                            legendHeight += (height + 2);

                        double width = this.GetTextBlockActualWidth(this.legendLabels[i]) + 2 + this.legendLines[i].ActualWidth + (2 * TICK_SIZE);
                        if (legendIsInBottomPanel)
                        {
                            width += (2 * TICK_SIZE);
                            legendWidth += (width + 2);
                        }
                        else
                        {
                            if (legendWidth < width)
                                legendWidth = width;
                        }
                    }
                }

                double xAxisHeight = 0;
                double xAxisWidth = 0;
                if (this.numOfLabelsX > 0 && this.disableOutputLabels == false)
                {
                    if (this.xLabels == null)
                        return;

                    for (int i = 0; i < this.numOfLabelsX; i++)
                    {
                        if (i >= this.xLabels.Length)
                            break;

                        double height = this.GetTextBlockActualHeight(this.xLabels[i]);
                        if (xAxisHeight < height)
                            xAxisHeight = height;

                        double width = this.GetTextBlockActualWidth(this.xLabels[i]);
                        if (xAxisWidth < width)
                            xAxisWidth = width;
                    }
                }

                if (this.orientationMode != OrientationMode.Horizontal && this.isStackPensMode && this.isEnabledYScaleForEachPen)
                {
                    if (this.yLabels != null)
                    {
                        const double MIN_X_AXIS_HEIGHT = 20.0;
                        const double MIN_X_AXIS_WIDTH = 20.0;
                        double yLabelsHeight = MIN_X_AXIS_HEIGHT;
                        double yLabelsWidth = MIN_X_AXIS_WIDTH;


                        for (int i = 0; i < this.yLabels.Length; i++)
                        {
                            if (this.yLabels[i] == null)
                                continue;

                            double actualHeight = this.GetTextBlockActualHeight(this.yLabels[i]);
                            if (yLabelsHeight < actualHeight)
                                yLabelsHeight = actualHeight;

                            double actualWidth = this.GetTextBlockActualWidth(this.yLabels[i]);
                            if (yLabelsWidth < actualWidth)
                                yLabelsWidth = actualWidth;
                        }

                        if (xAxisHeight < yLabelsHeight)
                            xAxisHeight = yLabelsHeight;

                        if (xAxisWidth < yLabelsWidth)
                            xAxisWidth = yLabelsWidth;
                    }
                }

                double yAxisHeight = 0;
                double yAxisWidth = 0;
                if (this.numOfLabelsY > 0 && this.disableOutputLabels == false)
                {
                    if (this.isEnabledYScaleForEachPen)
                    {
                        if (this.isStackPensMode)
                        {
                            if (this.yLabels == null)
                                return;

                            const double MIN_Y_AXIS_WIDTH = 20.0;
                            yAxisWidth = MIN_Y_AXIS_WIDTH;

                            for (int i = 0; i < this.yLabels.Length; i++)
                            {
                                if (this.yLabels[i] == null)
                                    continue;

                                double actualWidth = this.GetTextBlockActualWidth(this.yLabels[i]);
                                if (yAxisWidth < actualWidth)
                                    yAxisWidth = actualWidth;

                                double actualHeight = this.yLabels[i].ActualHeight;
                                if (yAxisHeight < actualHeight)
                                    yAxisHeight = actualHeight;
                            }
                        }
                        else
                        {
                            if (this.yLabelsGroup == null)
                                return;

                            for (int i = 0; i < this.numOfLabelsY; i++)
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    double yAxisHeightGroup = 0;
                                    double yAxisWidthGroup = 0;

                                    int groupIndex = 0;
                                    foreach (YLabelGroup group in this.yLabelsGroup)
                                    {
                                        if (group.Pens == null || j >= group.Pens.Length)
                                        {
                                            groupIndex++;
                                            continue;
                                        }

                                        if (group.Pens[j] == null)
                                        {
                                            groupIndex++;
                                            continue;
                                        }

                                        if (i >= group.Pens[j].Length)
                                        {
                                            groupIndex++;
                                            continue;
                                        }

                                        if (group.Pens[j][i] != null)
                                        {
                                            if (group.PensWidth == null || j >= group.PensWidth.Length || i >= group.PensWidth[j].Length)
                                            {
                                                groupIndex++;
                                                continue;
                                            }
                                            if (group.PensHeight == null || j >= group.PensHeight.Length || i >= group.PensHeight[j].Length)
                                            {
                                                groupIndex++;
                                                continue;
                                            }

                                            double actualWidth = group.PensWidth[j][i];
                                            yAxisWidthGroup += actualWidth;
                                            if (group.GroupWidth < actualWidth)
                                                group.GroupWidth = actualWidth;

                                            double actualHeight = group.PensHeight[j][i];
                                            if (yAxisHeightGroup < actualHeight)
                                                yAxisHeightGroup = actualHeight;

                                            if (group.GroupHeight < actualHeight)
                                                group.GroupHeight = actualHeight;
                                        }
                                        groupIndex++;
                                    }
                                    if (yAxisWidth < (yAxisWidthGroup + 4 * (this.yLabelsGroup.Count - 1)))
                                        yAxisWidth = (yAxisWidthGroup + 4 * (this.yLabelsGroup.Count - 1));

                                    if (yAxisHeight < (yAxisHeightGroup + 4 + (this.yLabelsGroup.Count - 1)))
                                        yAxisHeight = (yAxisHeightGroup + 4 + (this.yLabelsGroup.Count - 1));
                                }
                            }
                        }
                    }
                    else
                    {
                        if (this.yLabels == null)
                            return;

                        int labelsToCheck = this.isStackPensMode ? this.yLabels.Length : this.numOfLabelsY;

                        for (int i = 0; i < labelsToCheck; i++)
                        {
                            if (i >= this.yLabels.Length)
                                break;

                            if (this.yLabels[i] == null)
                                continue;

                            double actualWidth = this.GetTextBlockActualWidth(this.yLabels[i]);
                            if (yAxisWidth < actualWidth)
                                yAxisWidth = actualWidth;

                            double actualHeight = this.yLabels[i].ActualHeight;
                            if (yAxisHeight < actualHeight)
                                yAxisHeight = actualHeight;
                        }
                    }
                }

                this.LegendWindow.Margin = new Thickness(0, 0, 0, 0);
                this.LegendWindowBorder.Margin = new Thickness(0, 0, 0, 0);

                if(!legendExists)
                {
                    this.LegendWindowBorder.BorderBrush = Brushes.Transparent;
                    this.legendWindowBorder.BorderThickness = new Thickness(0);
                }

                this.XAxis.Margin = new Thickness(0, 0, 0, 0);
                this.YAxis.Margin = new Thickness(0, 0, 0, 0);
                this.Margin = new Thickness(0, 0, 0, 0);

                double chartBorderOffset = this.ChartArea.Width;
                if (this.ChartBorderArea.Width > this.ChartArea.Width)
                    chartBorderOffset = (this.ChartBorderArea.Width - chartBorderOffset) / 1.25;

                this.LegendWindowBorder.Width = legendWidth + (3 * TICK_SIZE);
                this.LegendWindowBorder.Height = legendHeight;
                this.LegendWindow.Width = this.LegendWindowBorder.Width;
                this.LegendWindow.Height = this.LegendWindowBorder.Height;

                if (this.orientationMode == OrientationMode.Horizontal)
                {
                    int cnt = this.pens.Count - 1;
                    if (cnt == 0)
                        cnt = 1;

                    yAxisHeight = (yAxisHeight + 2) * cnt;

                    this.YAxis.Width = yAxisWidth > 0 ? 2 + yAxisWidth + 2 + TICK_SIZE : TICK_SIZE;

                    this.XAxis.Height = xAxisHeight > 0 ? 2 + TICK_SIZE + xAxisHeight : 0;
                    this.YAxis.Height = Math.Max(0.0, this.ChartArea.Height);
                    this.Height = Math.Max(0.0, this.RenderedHeight - this.XAxis.Height);

                    double widthRemain = Math.Max(0.0, this.RenderedWidth - this.YAxis.Width);
                    if (legendIsInRightPanel)
                    {
                        widthRemain = Math.Max(0.0, widthRemain - this.LegendWindow.Width);
                        if (this.LegendWindow.Height > this.YAxis.Height)
                        {
                            this.LegendWindowBorder.Height = this.YAxis.Height;
                            this.LegendWindow.Height = this.YAxis.Height;
                        }
                    }
                    if (legendIsInBottomPanel)
                    {
                        double legendRows = Math.Ceiling(legendWidth / widthRemain);

                        this.LegendWindowBorder.Height = Math.Max(0.0, (legendHeight + 2 * TICK_SIZE) * legendRows);
                        this.LegendWindow.Height = this.LegendWindowBorder.Height;

                        this.Height = Math.Max(0, this.Height - (this.LegendWindow.Height + TICK_SIZE));
                    }

                    this.Width = widthRemain + 20 - TICK_SIZE;
                    this.YAxis.Margin = new Thickness(0, this.ChartArea.Y, 0, 0);

                    double correctedMargin = this.YAxis.Width - chartBorderOffset;

                    // adjust end element to be aligned with end of chart
                    if (legendIsInRightPanel)
                    {
                        this.LegendWindowBorder.Margin = new Thickness(this.RenderedWidth - this.LegendWindow.Width - TICK_SIZE, this.ChartArea.Y, 0, 0);
                    }
                    else if (legendIsInBottomPanel)
                    {
                        this.LegendWindowBorder.Width = widthRemain;
                        this.LegendWindow.Width = widthRemain;
                        this.LegendWindowBorder.Margin = new Thickness(correctedMargin + 10, this.YAxis.Height + this.XAxis.Height + 7, 0, 0); ;
                    }

                    this.XAxis.Width = widthRemain;
                    this.XAxis.Margin = new Thickness(correctedMargin + 10, this.YAxis.Height + 7, 0, 0);
                    this.Margin = new Thickness(correctedMargin + TICK_SIZE, 0, 0, 0);
                }
                else
                {
                    yAxisHeight = 0;
                    // When is vertical mode, all non null pens from group will be lined up vertically.
                    if (this.IsEnabledYScaleForEachPen)
                    {
                        if (this.yLabelsGroup == null)
                            return;

                        for (int i = 0; i < this.numOfLabelsY; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                int groupIndex = 0;
                                foreach (YLabelGroup group in this.yLabelsGroup)
                                {
                                    if (group.Pens == null || j >= group.Pens.Length)
                                    {
                                        groupIndex++;
                                        continue;
                                    }

                                    if (group.Pens[j] == null)
                                    {
                                        groupIndex++;
                                        continue;
                                    }

                                    if (i >= group.Pens[j].Length)
                                    {
                                        groupIndex++;
                                        continue;
                                    }

                                    if (group.Pens[j][i] == null)
                                    {
                                        groupIndex++;
                                        continue;
                                    }

                                    if (group.PensHeight == null || j >= group.PensHeight.Length || i >= group.PensHeight[j].Length)
                                    {
                                        groupIndex++;
                                        continue;
                                    }

                                    double actualHeight = group.PensHeight[j][i];
                                    yAxisHeight += actualHeight;
                                    groupIndex++;
                                }
                            }
                        }
                        yAxisHeight = yAxisHeight / 3;
                    }
                    else
                    {
                        if (this.yLabels == null)
                            return;

                        for (int i = 0; i < this.numOfLabelsY; i++)
                        {
                            if (i >= this.yLabels.Length)
                                break;

                            double actualHeight = this.GetTextBlockActualHeight(this.yLabels[i]);
                            yAxisHeight += actualHeight;
                        }
                    }

                    this.XAxis.Width = xAxisWidth > 0 ? 2 + xAxisWidth /*+ 2*/ + TICK_SIZE : 0;
                    this.YAxis.Height = yAxisHeight > 0 ? TICK_SIZE + yAxisHeight : 0;

                    if (this.orientationMode != OrientationMode.Horizontal && this.isStackPensMode && this.isEnabledYScaleForEachPen)
                    {
                        const double MIN_Y_AXIS_HEIGHT = 20.0;
                        if (this.YAxis.Height < MIN_Y_AXIS_HEIGHT)
                            this.YAxis.Height = MIN_Y_AXIS_HEIGHT;
                    }

                    this.XAxis.Height = Math.Max(0.0, this.ChartArea.Height);
                    this.Height = this.RenderedHeight - this.YAxis.Height;

                    double widthRemain = Math.Max(0.0, this.RenderedWidth - this.XAxis.Width);
                    if (legendIsInRightPanel)
                    {
                        widthRemain = Math.Max(0.0, widthRemain - this.LegendWindow.Width);
                        if (this.LegendWindow.Height > this.XAxis.Height)
                        {
                            this.LegendWindowBorder.Height = this.XAxis.Height;
                            this.LegendWindow.Height = this.XAxis.Height;
                        }
                    }
                    if (legendIsInBottomPanel)
                    {
                        double legendRows = Math.Ceiling(legendWidth / widthRemain);

                        this.LegendWindowBorder.Height = (legendHeight + 2 * TICK_SIZE) * legendRows;
                        this.LegendWindow.Height = this.LegendWindowBorder.Height;

                        this.Height -= (this.LegendWindow.Height + TICK_SIZE);
                    }

                    this.Width = widthRemain + 20;

                    //this.XAxis.Margin = new Thickness(0, this.ChartArea.Y, 0, 0);
                    double correctedMargin = this.XAxis.Width - chartBorderOffset;

                    this.YAxis.Width = widthRemain;
                    if (this.orientationMode == OrientationMode.VerticalBottomToTop)
                    {
                        this.XAxis.Margin = new Thickness(0, this.ChartArea.Y, 0, 0);
                        this.YAxis.Margin = new Thickness(correctedMargin, this.XAxis.Height + 5, 0, 0);

                        if (legendIsInRightPanel)
                        {
                            this.LegendWindowBorder.Margin = new Thickness(this.RenderedWidth - this.LegendWindow.Width - TICK_SIZE, TICK_SIZE - 2, 0, 0);
                        }
                        else if (legendIsInBottomPanel)
                        {
                            this.LegendWindowBorder.Width = widthRemain;
                            this.LegendWindow.Width = widthRemain;
                            this.LegendWindowBorder.Margin = new Thickness(correctedMargin + 10, this.YAxis.Height + this.XAxis.Height + 7, 0, 0); ;
                        }

                        this.Margin = new Thickness(correctedMargin, -7, 0, 0);
                    }
                    if (this.orientationMode == OrientationMode.VerticalTopToBottom)
                    {
                        this.XAxis.Margin = new Thickness(0, this.YAxis.Height, 0, 0);
                        this.YAxis.Margin = new Thickness(correctedMargin, 0, 0, 0);

                        if (legendIsInRightPanel)
                        {
                            this.LegendWindowBorder.Margin = new Thickness(this.RenderedWidth - this.LegendWindow.Width - TICK_SIZE, this.YAxis.Height + TICK_SIZE - 2, 0, 0);
                        }
                        else if (legendIsInBottomPanel)
                        {
                            this.LegendWindowBorder.Width = widthRemain;
                            this.LegendWindow.Width = widthRemain;
                            this.LegendWindowBorder.Margin = new Thickness(correctedMargin + 10, this.YAxis.Height + this.XAxis.Height + TICK_SIZE, 0, 0); ;
                        }

                        this.Margin = new Thickness(correctedMargin, this.YAxis.Height - 7, 0, 0);
                    }
                    // adjust end element to be aligned with end of chart
                }

                if(this.chart != null)
                {
                    string _width = this.Width.ToString(TCultureInfo.InvariantCulture);
                    TInterop.ExecuteJavaScriptBase("$0.width = $1", this.chart, _width);

                    string _height = this.Height.ToString(TCultureInfo.InvariantCulture);
                    TInterop.ExecuteJavaScriptBase("$0.height = $1", this.chart, _height);
                }

                //this.RefreshChart();
                this.CalculateHTMLChartAreas();
                Point refPointLegend = GetLegendWindowReferencePoint(this.LegendPlacement);
                if (legendExists)
                {
                    this.LegendWindow.Background = Brushes.White;
                    this.LegendWindowBorder.BorderBrush = Brushes.Black;
                    this.legendWindowBorder.BorderThickness = new Thickness(1);

                    if(this.LegendWindow.Height > 2)
                        this.LegendWindow.Height -= 2;
                    if(this.LegendWindow.Width > 2)
                        this.LegendWindow.Width -= 2;
                    if (legendIsInsideChartBounds)
                    {
                        this.LegendWindowBorder.Margin = new Thickness(refPointLegend.X, refPointLegend.Y, 0, 0);
                        this.LegendWindow.Margin = new Thickness(0, 0, 0, 0);
                    }
                }

                if (this.EnableNavigationControl)
                {
                    if (this.orientationMode == OrientationMode.Horizontal)
                    {
                        this.NavigationControl.Height = this.XAxis.Height;
                        int left = TConvert.ToInt(this.XAxis.Margin.Left + (this.XAxis.Width - this.XAxis.Margin.Left) / 2 - this.NavigationControl.ActualWidth / 2);
                        this.NavigationControl.Margin = new Thickness(left, this.XAxis.Margin.Top, 0, 0);

                    }
                    else
                    {
                        this.NavigationControl.Width = this.XAxis.Width;
                        int top = TConvert.ToInt(this.XAxis.Margin.Top + (this.XAxis.Height - this.XAxis.Margin.Top) / 2 - this.NavigationControl.ActualHeight / 2);
                        this.NavigationControl.Margin = new Thickness(this.XAxis.Width / 4, top, 0, 0);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdatePanels Exception :: {ex.Message} || {ex.StackTrace}");
            }
        }

        private double GetTextBlockActualHeight(TextBlock textBlock)
        {
            try
            {
                if (textBlock == null)
                    return 0;

                if (textBlock.Text.Contains(Environment.NewLine))
                {
                    TextBlock _tmp = new TextBlock() { Text = textBlock.Text, FontFamily = textBlock.FontFamily, FontSize = textBlock.FontSize, FontWeight = textBlock.FontWeight };
                    _tmp.Text = _tmp.Text.Substring(0, _tmp.Text.IndexOf(Environment.NewLine));
                    return _tmp.ActualHeight;
                }
              
                return textBlock.ActualHeight;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetTextBlockActualHeight Exception for text: {textBlock.Text} :: {ex.Message}");               
            }
            return 0;
        }

        private double GetTextBlockActualWidth(TextBlock textBlock)
        {
            try
            {
                if (textBlock == null)
                    return 0;

                if (textBlock.Text.Contains(Environment.NewLine))
                {
                    TextBlock _tmp = new TextBlock() { Text = textBlock.Text, FontFamily = textBlock.FontFamily, FontSize = textBlock.FontSize, FontWeight = textBlock.FontWeight };
                    _tmp.Text = _tmp.Text.Substring(0, _tmp.Text.IndexOf(Environment.NewLine));
                    return _tmp.ActualWidth;
                }
            
                return textBlock.ActualWidth;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetTextBlockActualWidth Exception for text: {textBlock.Text} :: {ex.Message}");              
            }
            return 0;
        }


        private void CreateLegendWindowItems()
        {
            try
            {
                if (this.pens.Count < 0)
                    return;

                if (this.legendPlacement == Base.eLegendPlacement.None)
                    return;

                if (this.legendLines != null && this.legendLines.Length != this.pens.Count)
                {
                    foreach (FrameworkElement el in this.legendLines)
                        this.LegendWindow.Children.Remove(el);
                    this.legendLines = null;
                }

                if (this.legendLines == null)
                {
                    this.legendLines = new Rectangle
                        [this.pens.Count];
                    for (int i = 0; i < this.pens.Count; i++)
                        this.legendLines[i] = null;
                }

                if (this.legendLabels != null && this.legendLabels.Length != this.pens.Count)
                {
                    foreach (FrameworkElement el in this.legendLabels)
                        this.LegendWindow.Children.Remove(el);
                    this.legendLabels = null;
                }

                if (this.legendLabels == null)
                {
                    this.legendLabels = new TextBlock[this.pens.Count];
                    for (int i = 0; i < this.pens.Count; i++)
                        this.legendLabels[i] = null;
                }

                for (int i = 0; i < this.pens.Count; i++)
                {
                    string penLabel = "";
                    bool isVisible = true;
                    TrendPen pen = null;
                    if (this.typeChart == TypeChart.TrendChart)
                    {
                        pen = this.pens[i].TrendPen as TrendPen;
                        penLabel = pen.PenLabelOutput;
                        isVisible = pen.Visible;                        
                    }
                    else
                    {
                        pen = this.pens[i].TrendPen as TrendPen;
                        penLabel = pen.PenLabelOutput;
                        isVisible = pen.Visible;
                    }
                
                    if (this.legendLabels[i] == null)
                    {
                        this.legendLabels[i] = new TextBlock()
                        {
                            //Background = Brushes.Transparent,
                            Foreground = Brushes.Black,
                            Opacity = 1.0,
                            //BorderBrush = Brushes.Transparent,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top,
                            FontFamily = new FontFamily("Arial"),
                            FontSize = 10,
                            IsHitTestVisible = true,
                            Tag = this.pens[i],
                            TextWrapping = TextWrapping.NoWrap,
                            TextTrimming = TextTrimming.CharacterEllipsis,
                        };
                        
                        this.legendLabels[i].MouseLeftButtonDown += this.LegendElement_PointerPressed;
                        this.legendLabels[i].MouseMove += this.LegendItem_PointerMoved;
                    }

                     
                    int leftMargin = 0;
                    if (i > 0 && this.LegendPlacement == eLegendPlacement.BottomPanel)
                        leftMargin = 10;
                    this.legendLabels[i].Margin = new Thickness(leftMargin, 0, 0, 0);

                    this.legendLabels[i].Foreground = Brushes.Gray;
                    this.legendLabels[i].Opacity = 0.6;
                    if (isVisible)
                    {
                        this.legendLabels[i].Foreground = Brushes.Black;
                        this.legendLabels[i].Opacity = 1.0;
                    }

                    if (this.LegendWindow != null && this.LegendWindow.Children.Contains(this.legendLabels[i]) == false)
                        this.LegendWindow.Children.Add(this.legendLabels[i]);

                    this.legendLabels[i].SetLocation(0, 0);

                    bool shouldReturnAllScales = true;
                    bool shouldApplyUnitsConversion = false;
                    List<object[]> penYScales = this.GetPensYScale(shouldReturnAllScales, shouldApplyUnitsConversion);
                    Brush nowStroke = penYScales[i][0] as Brush;

                    if(this.isXYChart)
                    {
                        nowStroke = Brushes.Green;
                    }

                    if (this.legendLines[i] == null)
                    {
                        this.legendLines[i] = new Rectangle()
                        {
                            IsHitTestVisible = true,
                            Width = TICK_SIZE * 2.5,
                            Height = this.GetTextBlockActualHeight(this.legendLabels[i]) > 0 ? this.GetTextBlockActualHeight(this.legendLabels[i]) / 3 : 2,
                            VerticalAlignment = VerticalAlignment.Center,
                            Fill = nowStroke,
                            Tag = this.pens[i]
                        };

                        RotateTransform transform = new RotateTransform() { Angle = -30 };
                        this.legendLines[i].RenderTransform = transform;
                        this.legendLines[i].RenderTransformOrigin = new Point(0.5, 0.5);    

                        this.legendLines[i].MouseLeftButtonDown += this.LegendElement_PointerPressed;
                        this.legendLines[i].MouseMove += this.LegendItem_PointerMoved;
                    }

                    leftMargin = 0;
                    if (i > 0 && this.LegendPlacement == eLegendPlacement.BottomPanel)
                        leftMargin = 5;
                    this.legendLines[i].Margin = new Thickness(leftMargin, this.GetTextBlockActualHeight(this.legendLabels[i]) / 2, 0, 0);

                    if (this.LegendWindow != null && this.LegendWindow.Children.Contains(this.legendLines[i]) == false)
                        this.LegendWindow.Children.Add(this.legendLines[i]);

                    this.legendLines[i].SetLocation(0, 0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CreateLegendItems Exception :: {ex.Message} || {ex.StackTrace}");
            }
        }

        private Size CalculateStringSizeInPixels(string content)
        {
            content = content ?? "";
            TextBlock tmp = new TextBlock()
            {
                //Background = Brushes.Transparent,
                Foreground = Brushes.Transparent,
                Opacity = 0.0,
                //BorderBrush = Brushes.Transparent,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                FontFamily = new FontFamily("Arial"),
                FontSize = 10,
                IsHitTestVisible = true,
                Text = content
            };

            this.Children.Add(tmp);
            double contentWidth = tmp.ActualWidth;
            double contentHeight = this.GetTextBlockActualHeight(tmp);
            this.Children.Remove(tmp);
            return new Size(contentWidth, contentHeight);
        }


        private void UpdateTicks()
        {
            try
            {
                if (this.disableOutputLabels)
                    return;

                double offsetWidth = this.ChartArea.X;
                double offsetHeight = this.ChartArea.Y;

                double height = this.YAxis.Height;
                double width = this.YAxis.Width;

                double x1 = this.YAxis.Width - TICK_SIZE;
                double x2 = this.YAxis.Width;

                double y1;
                double y2;

                double piece = this.numOfLabelsY == 1 ? 0.0 : this.YAxis.Height / (this.numOfLabelsY - 1);
                if (this.orientationMode != OrientationMode.Horizontal)
                    piece = this.numOfLabelsY == 1 ? 0.0 : this.YAxis.Width / (this.numOfLabelsY - 1);

                for (int i = 0; i < this.numOfLabelsY; i++)
                {
                    double value = i * piece;
                    y1 = value;
                    y2 = value;
                    if (this.orientationMode != OrientationMode.Horizontal)
                    {
                        x1 = value + offsetWidth;
                        x2 = value + offsetWidth;

                        if (this.orientationMode == OrientationMode.VerticalTopToBottom)
                        {
                            y1 = this.YAxis.Height - TICK_SIZE;
                            y2 = this.YAxis.Height;
                        }
                        else
                        {
                            y2 = TICK_SIZE;
                            y1 = 0;
                        }
                    }

                    this.yTicks[i].Width = width;
                    this.yTicks[i].Height = height;
                    this.yTicks[i].X1 = x1;
                    this.yTicks[i].Y1 = y1;
                    this.yTicks[i].X2 = x2;
                    this.yTicks[i].Y2 = y2;
                }

                piece = this.numOfLabelsX == 1 ? 0.0 : this.XAxis.Width / (this.numOfLabelsX - 1);
                if (this.orientationMode != OrientationMode.Horizontal)
                    piece = this.numOfLabelsX == 1 ? 0.0 : this.XAxis.Height / (this.numOfLabelsX - 1);

                height = this.XAxis.Height;
                width = this.XAxis.Width;

                y1 = 0;
                y2 = TICK_SIZE;

                for (int i = 0; i < this.numOfLabelsX; i++)
                {
                    double value = i * piece;

                    if (i == 0)
                    {
                        x1 = value + TICK_SIZE + 2;
                        x2 = value + TICK_SIZE + 2;
                    }

                    x1 = value;
                    x2 = value;

                    if (this.orientationMode != OrientationMode.Horizontal)
                    {
                        x1 = width - TICK_SIZE;
                        x2 = width;

                        if (this.orientationMode == OrientationMode.VerticalTopToBottom)
                        {
                            y1 = value + TICK_SIZE - 2 /*offsetHeight*/;
                            y2 = value + TICK_SIZE - 2 /*offsetHeight*/;
                        }
                        else
                        {
                            y1 = value - (offsetHeight - TICK_SIZE + 2);
                            y2 = value - (offsetHeight - TICK_SIZE + 2);
                        }
                    }

                    this.xTicks[i].Width = width;
                    this.xTicks[i].Height = height;
                    this.xTicks[i].X1 = x1;
                    this.xTicks[i].Y1 = y1;
                    this.xTicks[i].X2 = x2;
                    this.xTicks[i].Y2 = y2;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateTicks Exception :: {ex.Message} || {ex.StackTrace}");
            }
        }

        private void UpdatePrimaryAxis()
        {
            if(this.IsXYChart)
            {
                this.primaryXAxis.UpdateXMinAndXMax(this.XMinValue, this.XMaxValue);
            }
            else
            {
                this.primaryXAxis.UpdateXMinAndXMax(this.XMin, this.XMax);
            }
            if (!string.IsNullOrEmpty(this.labelFormatX) && !this.labelFormatX.Equals("*"))
                this.primaryXAxis.LabelFormat = this.labelFormatX;

            this.primaryYAxis.UpdateYMinAndYMax(this.YMin, this.YMax);
            if (!string.IsNullOrEmpty(this.labelFormatY) && !this.labelFormatY.Equals("*"))
                this.primaryYAxis.LabelFormat = this.labelFormatY;
        }

        private void UpdateLabels()
        {
            this.UpdateXLabels();
            this.UpdateYLabels();
        }

        private void UpdateYLabels()
        {
            try
            {
                if (this.disableOutputLabels)
                    return;

                if (this.isStackPensMode && this.pens.Count > 0)
                {
                    this.UpdateYLabelsStackPensMode();
                    return;
                }

                double piece = this.numOfLabelsY == 1 ? 0.0 : this.YAxis.Height / (this.numOfLabelsY - 1);
                //if (this.orientationMode != OrientationMode.Horizontal)
                //    piece = this.numOfLabelsY == 1 ? 0.0 : this.YAxis.Width / (this.numOfLabelsY - 1);

                if (this.isEnabledYScaleForEachPen)
                {
                    for (int i = this.numOfLabelsY - 1; i >= 0; i--)
                    {
                        double value = i * piece;

                        double rowBreakOffset = 0;
                        for (int j = 0; j < 3; j++)
                        {
                            double offset = 0;
                            for (int g = 0; g < this.yLabelsGroup.Count; g++)
                            {
                                YLabelGroup group = this.yLabelsGroup[g];

                                if (group.Pens[j][i] == null)
                                    continue;

                                int k = (this.numOfLabelsY - 1) - i;

                                Point margin;
                                if (i == this.numOfLabelsY - 1) // yStartValue
                                {
                                    double y = this.YAxis.Height - group.PensHeight[j][k];
                                    if (this.orientationMode != OrientationMode.Horizontal)
                                        y = 2;
                                    int _switch = group.GroupSize;
                                    if (g > 0)
                                        _switch = 3;
                                    switch (_switch)
                                    {
                                        case 2:
                                            if (j == 1)
                                            {
                                                if (this.orientationMode == OrientationMode.Horizontal)
                                                    y = y - group.PensHeight[j][k] + 2;
                                                else
                                                    y = y + (group.PensHeight[j][k] * rowBreakOffset) - 2;
                                            }
                                            break;

                                        case 3:
                                            if (j == 0)
                                            {
                                                if (this.orientationMode == OrientationMode.Horizontal)
                                                    y = y - group.PensHeight[j][k] + 2;
                                                else
                                                    y = y + (group.PensHeight[j][k] * rowBreakOffset) - 2;
                                            }
                                            if (j == 1)
                                            {
                                                if (this.orientationMode == OrientationMode.Horizontal)
                                                    y = y - (2 * group.PensHeight[j][k]) + 2 * 2;
                                                else
                                                    y = y + (group.PensHeight[j][k] * rowBreakOffset) - 2;
                                            }

                                            if (j == 2 && this.orientationMode != OrientationMode.Horizontal)
                                            {
                                                y = y + (group.PensHeight[j][k] * rowBreakOffset) - 2;
                                            }

                                            break;
                                    }

                                    margin = new Point(this.YAxis.Width - (TICK_SIZE + 2) - group.PensWidth[j][k] - offset, y);

                                    if (this.orientationMode != OrientationMode.Horizontal)
                                        margin = new Point(group.PensWidth[j][k], y + TICK_SIZE - 2);
                                }
                                else if (i == 0) // yEndValue
                                {
                                    double y = 0;
                                    if (this.orientationMode != OrientationMode.Horizontal)
                                        y = 2;
                                    int _switch = group.GroupSize;
                                    if (g > 0)
                                        _switch = 3;
                                    switch (_switch)
                                    {
                                        case 2:
                                            if (j == 0)
                                                if (this.orientationMode == OrientationMode.Horizontal)
                                                    y = y + group.PensHeight[j][k] - 2;
                                                else
                                                    y = y + (group.PensHeight[j][k] * rowBreakOffset) - 2;
                                            break;

                                        case 3:
                                            if (j == 0)
                                            {
                                                if (this.orientationMode == OrientationMode.Horizontal)
                                                    y = y + group.PensHeight[j][k] - 2;
                                                else
                                                    y = y + (group.PensHeight[j][k] * rowBreakOffset) - 2;
                                            }
                                            if (j == 1 && this.orientationMode != OrientationMode.Horizontal)
                                            {
                                                y = y + (group.PensHeight[j][k] * rowBreakOffset) - 2;
                                            }
                                            if (j == 2)
                                            {
                                                if (this.orientationMode == OrientationMode.Horizontal)
                                                    y = y + (2 * group.PensHeight[j][k]) - 2 * 2;
                                                else
                                                    y = y + (group.PensHeight[j][k] * rowBreakOffset) - 2;
                                            }
                                            break;
                                    }
                                    margin = new Point(this.YAxis.Width - (TICK_SIZE + 2) - group.PensWidth[j][k] - offset, y);

                                    if (this.orientationMode != OrientationMode.Horizontal)
                                        margin = new Point(this.YAxis.Width - group.PensWidth[j][k], y + TICK_SIZE - 2);
                                }
                                else
                                {
                                    double y = value - group.PensHeight[j][k] / 2;
                                    if (this.orientationMode != OrientationMode.Horizontal)
                                        y = 2;
                                    double pieceX = this.numOfLabelsY == 1 ? 0.0 : this.YAxis.Width / (this.numOfLabelsY - 1);
                                    pieceX = pieceX * (this.NumOfLabelsY - 1 - i) + (this.ChartArea.X / 2);
                                    if (j == 0 && this.orientationMode != OrientationMode.Horizontal)
                                    {
                                        y = y + (group.PensHeight[j][k] * rowBreakOffset) - 2;
                                    }
                                    if (j == 1)
                                    {
                                        if (this.orientationMode == OrientationMode.Horizontal)
                                            y = y - group.PensHeight[j][k] + 2;
                                        else
                                            y = y + (group.PensHeight[j][k] * rowBreakOffset) - 2;
                                    }
                                    if (j == 2)
                                    {
                                        if (this.orientationMode == OrientationMode.Horizontal)
                                            y = y + group.PensHeight[j][k] - 2;
                                        else
                                            y = y + (group.PensHeight[j][k] * rowBreakOffset) - 2;
                                    }

                                    margin = new Point(this.YAxis.Width - (TICK_SIZE + 2) - group.PensWidth[j][k] - offset, y);

                                    if (this.orientationMode != OrientationMode.Horizontal)
                                        margin = new Point(pieceX, y + TICK_SIZE);
                                }

                                group.Pens[j][k].SetLocation(margin.X, margin.Y);

                                offset += group.GroupWidth + 4;

                                rowBreakOffset++;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = this.numOfLabelsY - 1; i >= 0; i--)
                    {
                        double value = i * piece;
                        Point margin;
                        int j = (this.numOfLabelsY - 1) - i;
                        if (i == this.numOfLabelsY - 1)
                        {
                            margin = new Point(this.YAxis.Width - (TICK_SIZE + 2) - this.GetTextBlockActualWidth(this.yLabels[j]), this.YAxis.Height - this.GetTextBlockActualHeight(this.yLabels[j]));
                        }
                        else if (i == 0)
                        {
                            margin = new Point(this.YAxis.Width - (TICK_SIZE + 2) - this.GetTextBlockActualWidth(this.yLabels[j]), 0);
                        }
                        else
                        {
                            margin = new Point(this.YAxis.Width - (TICK_SIZE + 2) - this.GetTextBlockActualWidth(this.yLabels[j]), value - this.GetTextBlockActualHeight(this.yLabels[j]) / 2);
                        }
                        this.yLabels[j].SetLocation(margin.X, margin.Y);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateYLabels Exception :: {ex.Message} || {ex.StackTrace}");
            }
        }

        private void UpdateYLabelsStackPensMode()
        {
            try
            {
                // Get visible pens count
                int visiblePensCount = 0;
                foreach (var pen in this.pens)
                {
                    if (pen.Visibility == Visibility.Visible)
                        visiblePensCount++;
                }

                if (visiblePensCount == 0)
                    return;

                double rowHeight = this.orientationMode == OrientationMode.Horizontal
                    ? this.YAxis.Height / visiblePensCount
                    : this.YAxis.Width / visiblePensCount;

                if (this.isEnabledYScaleForEachPen)
                {
                    bool shouldReturnAllScales = false;
                    bool shouldApplyUnitsConversion = true;
                    List<object[]> penYScales = this.GetPensYScale(shouldReturnAllScales, shouldApplyUnitsConversion);

                    int visiblePenIndex = 0;
                    int penScaleIndex = 0;

                    // Iterate through all pens to find visible ones
                    for (int p = 0; p < this.pens.Count; p++)
                    {
                        if (this.pens[p].Visibility != Visibility.Visible)
                            continue;

                        if (penScaleIndex >= penYScales.Count)
                            break;

                        int offset = p * this.numOfLabelsY;

                        double rowBaseY = visiblePenIndex * rowHeight;

                        double piece = this.numOfLabelsY == 1 ? 0.0 : rowHeight / (this.numOfLabelsY - 1);

                        int nullCount = 0;
                        int processedCount = 0;

                        for (int i = this.numOfLabelsY - 1; i >= 0; i--)
                        {
                            if (this.yLabels[offset + i] == null)
                            {
                                nullCount++;
                                continue;
                            }

                            processedCount++;

                            int k = (this.numOfLabelsY - 1) - i;
                            double labelValue = i * piece;

                            double x, y;
                            int labelIndex;

                            if (this.orientationMode == OrientationMode.Horizontal)
                            {
                                labelIndex = k;

                                double textHeight = 0;
                                if (i == this.numOfLabelsY - 1) // Top label
                                {
                                    textHeight = this.GetTextBlockActualHeight(this.yLabels[offset + labelIndex]);
                                    y = rowBaseY + rowHeight - textHeight;
                                }
                                else if (i == 0) // Bottom label
                                    y = rowBaseY;
                                else // Middle labels
                                {
                                    this.yLabels[offset + labelIndex].Text = "";
                                    textHeight = this.GetTextBlockActualHeight(this.yLabels[offset + labelIndex]) / 2;
                                    y = rowBaseY + rowHeight - labelValue - textHeight;
                                }

                                double textWidth = this.GetTextBlockActualWidth(this.yLabels[offset + labelIndex]);
                                x = this.YAxis.Width - (TICK_SIZE + 2) - textWidth;
                            }
                            else
                            {

                                labelIndex = i;

                                double textWidth = Math.Max(0, this.GetTextBlockActualWidth(this.yLabels[offset + labelIndex]));
                                double textHeight = Math.Max(0, this.GetTextBlockActualHeight(this.yLabels[offset + labelIndex]));

                                if (i == this.numOfLabelsY - 1) // Right label (max)
                                {
                                    x = rowBaseY + rowHeight - textWidth + (p < this.pens.Count - 1 ? 4 : 0);
                                }
                                else if (i == 0) // Left label (min)
                                {
                                    x = rowBaseY + 8;
                                }
                                else // Middle labels
                                {
                                    this.yLabels[offset + labelIndex].Text = "";
                                    double proportionalValue = i * piece;
                                    x = rowBaseY + proportionalValue - textWidth / 2;
                                }

                                if (this.orientationMode == OrientationMode.VerticalTopToBottom)
                                    y = 2;
                                else
                                    y = this.YAxis.Height - textHeight - 2;
                            }

                            this.yLabels[offset + labelIndex].SetLocation(x, y);
                        }

                        visiblePenIndex++;
                        penScaleIndex++;
                    }
                }
                else
                {
                    int visiblePenIndex = 0;

                    for (int p = 0; p < this.pens.Count; p++)
                    {
                        if (this.pens[p].Visibility != Visibility.Visible)
                            continue;

                        double rowBaseY = visiblePenIndex * rowHeight;

                        double piece = this.numOfLabelsY == 1 ? 0.0 : rowHeight / (this.numOfLabelsY - 1);

                        int offset = p * this.numOfLabelsY;

                        for (int i = this.numOfLabelsY - 1; i >= 0; i--)
                        {

                            if (this.yLabels[offset + i] == null)
                                continue;

                            int k = (this.numOfLabelsY - 1) - i;
                            double labelValue = i * piece;

                            double x, y;
                            int labelIndex;

                            if (this.orientationMode == OrientationMode.Horizontal)
                            {
                                labelIndex = k;

                                if (i == this.numOfLabelsY - 1) // Top label
                                {
                                    double textHeight = this.GetTextBlockActualHeight(this.yLabels[offset + labelIndex]);
                                    y = rowBaseY + rowHeight - textHeight;
                                }
                                else if (i == 0) // Bottom label
                                {
                                    y = rowBaseY;
                                }
                                else // Middle labels
                                {
                                    this.yLabels[offset + labelIndex].Text = "";
                                    double textHeight = this.GetTextBlockActualHeight(this.yLabels[offset + labelIndex]) / 2;
                                    y = rowBaseY + rowHeight - labelValue - textHeight;
                                }

                                x = this.YAxis.Width - (TICK_SIZE + 2) - this.GetTextBlockActualWidth(this.yLabels[offset + labelIndex]);
                            }
                            else
                            {
                                labelIndex = i;

                                double textWidth = Math.Max(0, this.GetTextBlockActualWidth(this.yLabels[offset + labelIndex]));
                                double textHeight = Math.Max(0, this.GetTextBlockActualHeight(this.yLabels[offset + labelIndex]));

                                if (i == this.numOfLabelsY - 1) // Right label (max)
                                {
                                    x = rowBaseY + rowHeight - textWidth + (p < this.pens.Count - 1 ? 4 : 0);
                                }
                                else if (i == 0) // Left label (min)
                                {
                                    x = rowBaseY + 8;
                                }
                                else // Middle labels
                                {
                                    this.yLabels[offset + labelIndex].Text = "";
                                    double proportionalValue = i * piece;
                                    x = rowBaseY + proportionalValue - textWidth / 2;
                                }

                                if (this.orientationMode == OrientationMode.VerticalTopToBottom)
                                    y = 2;
                                else
                                    y = this.YAxis.Height - textHeight - 2;
                            }

                            this.yLabels[offset + labelIndex].SetLocation(x, y);
                        }

                        visiblePenIndex++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateYLabelsStackPensMode Exception :: {ex.Message} || {ex.StackTrace}");
            }
        }

        private void UpdateXLabels()
        {
            try
            {
                if (this.disableOutputLabels)
                    return;

                double piece = this.numOfLabelsX == 1 ? 0.0 : this.XAxis.ActualWidth / (this.numOfLabelsX - 1);
                if (this.orientationMode != OrientationMode.Horizontal)
                {
                    piece = this.numOfLabelsX == 1 ? 0.0 : this.XAxis.ActualHeight / (this.numOfLabelsX - 1);
                }

                for (int i = 0; i < this.numOfLabelsX; i++)
                {
                    double value = i * piece;
                    Point margin;
                    if (i == 0) // xStartValue
                    {
                        if (this.orientationMode == OrientationMode.Horizontal)
                            margin = new Point(0, TICK_SIZE);
                        else if (this.orientationMode == OrientationMode.VerticalTopToBottom)
                        {
                            margin = new Point(0, TICK_SIZE);
                        }
                        else // (this.orientationMode == OrientationMode.VerticalBottomToTop)
                        {
                            // Label must be at the bottom of the chart
                            margin = new Point(0, this.XAxis.ActualHeight - this.GetTextBlockActualHeight(this.xLabels[i]) - TICK_SIZE);
                        }
                    }
                    else if (i == this.numOfLabelsX - 1) // xEndValue
                    {
                        if (this.orientationMode == OrientationMode.Horizontal)
                            margin = new Point(this.XAxis.Width - this.GetTextBlockActualWidth(this.xLabels[i]) - 2, TICK_SIZE);
                        else if (this.orientationMode == OrientationMode.VerticalTopToBottom)
                        {
                            // Label must be at the bottom of the chart
                            margin = new Point(0, this.XAxis.ActualHeight - this.GetTextBlockActualHeight(this.xLabels[i]));
                        }
                        else // (this.orientationMode == OrientationMode.VerticalBottomToTop)
                        {
                            margin = new Point(0, TICK_SIZE);
                        }
                    }
                    else
                    {
                        if (this.orientationMode == OrientationMode.Horizontal)
                            margin = new Point(value - this.GetTextBlockActualWidth(this.xLabels[i]) / 2, TICK_SIZE);
                        else if (this.orientationMode == OrientationMode.VerticalTopToBottom)
                            margin = new Point(0, value - this.GetTextBlockActualHeight(this.xLabels[i]) / 2 + TICK_SIZE);
                        else // (this.orientationMode == OrientationMode.VerticalBottomToTop){
                            margin = new Point(0, value - this.GetTextBlockActualHeight(this.xLabels[i]) / 2 + TICK_SIZE);
                    }
                    this.xLabels[i].SetLocation(margin.X, margin.Y);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateXLabels Exception :: {ex.Message} || {ex.StackTrace}");
            }
        }

        private void UpdateLegendItems()
        {
            try
            {
                if (this.pens.Count < 0)
                    return;

                if (this.legendPlacement == Base.eLegendPlacement.None)
                    return;

                int LINE_LEFT_MARGIN = 4;
                int HORIZONTAL_MARGIN = (int)(TICK_SIZE * 4);
                double piece = this.pens.Count == 1 ? 0.0 : this.LegendWindow.ActualHeight / (this.pens.Count);

                bool isLegendInBottomPanel = this.legendPlacement == eLegendPlacement.BottomPanel;
                if (isLegendInBottomPanel)
                    piece = this.pens.Count == 1 ? 0.0 : this.LegendWindow.ActualWidth / (this.pens.Count);

                double offsetLine = LINE_LEFT_MARGIN;
                double offsetLabel = HORIZONTAL_MARGIN;
                double rowBreakOffset = 0;
                bool isSPC = false;
                string spcDescription;
                for (int i = 0; i < this.pens.Count; i++)
                {
                    TrendPen pen = null;
                    string penLabel = "";
                    if (this.typeChart == TypeChart.TrendChart)
                    {
                        pen = this.pens[i].TrendPen as TrendPen;
                        penLabel = pen.PenLabelOutput;
                    }
                    else
                    {
                        pen = this.pens[i].TrendPen as TrendPen;
                        penLabel = pen.PenLabelOutput;
                    }

                    Point marginLabel;
                    Point marginLine;
                    double value = i * piece;

                    spcDescription = this.pens[i].SPCDescription;
                    isSPC = !string.IsNullOrEmpty(spcDescription);

                    marginLine = new Point(LINE_LEFT_MARGIN, value);
                    marginLabel = new Point(HORIZONTAL_MARGIN, value);
                    string label = penLabel;
                    if (isSPC)
                        label += $"({spcDescription})";
                    double contentWidth = this.legendLines[i].ActualWidth + TICK_SIZE+ this.CalculateStringSizeInPixels(label).Width;

                    if (isLegendInBottomPanel)
                    {
                        while (contentWidth + TICK_SIZE > this.LegendWindow.ActualWidth)
                        {
                            if (!label.StartsWith("..."))
                                label = "..." + label;

                            if (label.Length < 25)
                                break;

                            label = label.Remove(3, 1);

                            contentWidth = this.legendLines[i].ActualWidth + TICK_SIZE + this.CalculateStringSizeInPixels(label).Width;
                        }

                        marginLine = new Point(offsetLine, TICK_SIZE + rowBreakOffset);
                        marginLabel = new Point(offsetLabel, TICK_SIZE + rowBreakOffset);

                        if (offsetLabel + contentWidth >= this.LegendWindow.ActualWidth)
                        {
                            rowBreakOffset += (this.GetTextBlockActualHeight(this.legendLabels[i]) + TICK_SIZE);
                            offsetLine = LINE_LEFT_MARGIN;
                            offsetLabel = HORIZONTAL_MARGIN;

                            marginLine.X = offsetLine;
                            marginLabel.X = offsetLabel;                            
                        }
                        else
                        {
                            offsetLine += (this.legendLines[i].ActualWidth + TICK_SIZE + this.legendLabels[i].ActualWidth);
                            offsetLabel = (offsetLine + this.legendLines[i].ActualWidth);
                        }
                    }
                    else
                    {
                        while (contentWidth + TICK_SIZE > this.Width / 3)
                        {
                           if (!label.StartsWith("..."))
                                label = "..." + label;

                            if (label.Length < 25)
                                break;

                            label = label.Remove(3, 1);

                            contentWidth = this.legendLines[i].ActualWidth + TICK_SIZE + this.CalculateStringSizeInPixels(label).Width;
                        }
                    }

                    this.legendLabels[i].Text = label;
                    this.legendLines[i].SetLocation(marginLine.X + TICK_SIZE, marginLine.Y);
                    this.legendLabels[i].SetLocation(marginLabel.X + TICK_SIZE, marginLabel.Y);                       
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateLegendItems Exception :: {ex.Message} || {ex.StackTrace}");
            }
        }

        private void CreateXLabels()
        {
            try
            {
                if (this.numOfLabelsX <= 0)
                    return;

                List<UIElement> elements = new List<UIElement>(this.Children);
                foreach (UIElement element in elements)
                {
                    if (!(element is Line))
                        continue;

                    Line line = element as Line;
                    if (string.Equals(TConvert.ToString(line.Tag), "XGridLines", StringComparison.OrdinalIgnoreCase))
                    {
                        object xgridLineDiv = OpenSilver.Interop.GetDiv(line);
                        if(xgridLineDiv == null)
                        {
                            this.Children.Remove(line);
                            continue;
                        }

                        string id = TConvert.ToString(TInterop.ExecuteJavaScriptBase("$0.id ", xgridLineDiv));
                        if (string.IsNullOrEmpty(id))
                        {
                            this.Children.Remove(line);
                            continue;
                        }
                    }
                }

                if (this.xLabels != null && this.xLabels.Length != this.numOfLabelsX && !this.disableOutputLabels)
                {
                    foreach (FrameworkElement el in this.xLabels)
                        this.XAxis.Children.Remove(el);
                    this.xLabels = null;
                }

                if (this.xLabels == null && !this.disableOutputLabels)
                {
                    this.xLabels = new TextBlock[this.numOfLabelsX];
                    for (int i = 0; i < this.numOfLabelsX; i++)
                        this.xLabels[i] = null;
                }

                if (this.xTicks != null && this.xTicks.Length != this.numOfLabelsX && !this.disableOutputLabels)
                {
                    foreach (FrameworkElement el in this.xTicks)
                        this.XAxis.Children.Remove(el);
                    this.xTicks = null;
                }

                if (this.xTicks == null && this.disableOutputLabels == false)
                {
                    this.xTicks = new Line[this.numOfLabelsX];
                    for (int i = 0; i < this.numOfLabelsX; i++)
                        this.xTicks[i] = null;
                }

                if (this.xGridLines != null && this.xGridLines.Length != this.numOfLabelsX)
                {
                    foreach (FrameworkElement el in this.xGridLines)
                        this.Children.Remove(el);
                    this.xGridLines = null;
                }

                if (this.xGridLines == null)
                {
                    this.xGridLines = new Line[this.numOfLabelsX];
                    for (int i = 0; i < this.numOfLabelsX; i++)
                        this.xGridLines[i] = null;
                }

                object piece = this.numOfLabelsX == 1 ? (long)0 : TConvert.To<long>(((this.xMax - this.xMin).Ticks / (this.numOfLabelsX - 1)));
                if(this.IsXYChart)
                    piece =  this.numOfLabelsY == 1 ? 0.0 : (this.XMaxValue - this.XMinValue) / (this.numOfLabelsX - 1);

                for (int i = 0; i < this.numOfLabelsX; i++)
                {
                    DateTime dt = DateTime.Now;
                    double val = 0;

                    if(this.IsXYChart)
                    {
                        if (i > 0 && i == this.numOfLabelsX - 1)
                            val = this.xMaxValue;
                        else
                            val = this.XMinValue + (i * TConvert.ToDouble(piece));
                    }
                    else
                    {
                        if (i > 0 && i == this.numOfLabelsX - 1)
                            dt = this.xMax;
                        else
                            dt = this.xMin.Add(TimeSpan.FromTicks(i * TConvert.To<long>(piece)));
                    }

                    if (this.disableOutputLabels == false)
                    {
                        if (this.xLabels[i] == null)
                        {
                            this.xLabels[i] = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, FontFamily = new FontFamily("Arial"), FontSize = 12, IsHitTestVisible = false };
                            if (this.Orientation != 0)
                                this.xLabels[i].TextAlignment = TextAlignment.Center;
                        }

                        if (this.XLabelsEditable && i == 0 || i == this.numOfLabelsX - 1)
                        {
                            this.xLabels[i].IsHitTestVisible = true;
                            this.xLabels[i].MouseLeftButtonDown += this.XAxisLabelEditable_PointerPressed;
                            this.xLabels[i].MouseMove += this.XAxisLabelEditable_PointerMoved;
                            this.xLabels[i].Tag = i;
                        }

                        this.xLabels[i].Margin = new Thickness(0);
                        this.xLabels[i].Foreground = this.LabelsBrush;

                        string text;
                        if (this.IsXYChart)
                            text = this.GetStringXAxis(i, val);
                        else
                            text = this.GetStringXAxis(i, dt);
                        
                        this.xLabels[i].Text = text;

                        if (!this.XAxis.Children.Contains(this.xLabels[i]))
                            this.XAxis.Children.Add(this.xLabels[i]);

                        this.xLabels[i].SetLocation(0, 0);
                    }

                    if (this.xGridLines[i] == null)
                        this.xGridLines[i] = new Line() { IsHitTestVisible = false, Width = this.ChartArea.Width, Height = this.ChartArea.Height, Tag = "XGridLines" };
                    this.xGridLines[i].Stroke = this.GridLinesBrush;
                    this.SetLineStroke(this.xGridLines[i], this.gridLinesStroke);
                    if (!this.Children.Contains(this.xGridLines[i]))
                        this.Children.Add(this.xGridLines[i]);

                    if (!this.disableOutputLabels)
                    {
                        if (this.xTicks[i] == null)
                            this.xTicks[i] = new Line() { IsHitTestVisible = false, Width = this.XAxis.ActualWidth, Height = this.XAxis.ActualHeight };
                        this.xTicks[i].Stroke = this.LabelsBrush;
                        if (!this.XAxis.Children.Contains(this.xTicks[i]))
                            this.XAxis.Children.Add(this.xTicks[i]);
                    }
                }

                if (!this.disableOutputLabels)
                {
                    for (int i = 0; i < this.numOfLabelsX; i++)
                    {
                        //if (this.labelFormatX != null && this.labelFormatX.Trim() == "*")
                        {
                            //this.xLabels[i].Background = this.Background;
                            if (i == 0 && this.Orientation == 0)
                                this.xLabels[i].Padding = new Thickness(0, 0, TICK_SIZE, 0);
                        }
                    }

                    for (int i = 0; i < this.numOfLabelsX; i++)
                    {
                        this.xLabels[i].Opacity = 1.0;
                        for (int j = 0; j < this.SubXDivisions; j++)
                        {
                            i++;
                            if (i >= this.numOfLabelsX)
                                break;
                            this.xLabels[i].Opacity = this.DisableOutputSubXDivisions ? 0.0 : 0.3;
                        }
                    }

                    if (/*this.labelFormatX != null && this.labelFormatX.Trim() == "*" && */this.numOfLabelsX > 2 && this.Orientation == 0)
                    {
                        for (int i = 3; i <= this.numOfLabelsX - 3; i += 2)
                        {
                            this.xLabels[i].Visibility = Visibility.Collapsed;
                        }
                        this.xLabels[1].Visibility = Visibility.Collapsed;
                        this.xLabels[this.numOfLabelsX - 2].Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CreateXLabels Exception :: {ex.Message} || {ex.StackTrace}");
            }
        }

        private void CreateYLabels()
        {
            try
            {
                if (this.numOfLabelsY <= 0)
                    return;

                List<UIElement> elements = new List<UIElement>(this.Children);
                foreach (UIElement element in elements)
                {
                    if (!(element is Line))
                        continue;

                    Line line = element as Line;
                    if(string.Equals(TConvert.ToString(line.Tag), "YGridLines", StringComparison.OrdinalIgnoreCase))
                    { 
                        object ygridLineDiv = OpenSilver.Interop.GetDiv(line);
                        if (ygridLineDiv == null)
                        {
                            this.Children.Remove(line);
                            continue;
                        }

                        string id = TConvert.ToString(TInterop.ExecuteJavaScriptBase("$0.id ", ygridLineDiv));
                        if (string.IsNullOrEmpty(id))
                        {
                            this.Children.Remove(line);
                            continue;
                        }
                    }                    
                }

                this.CreateYLabels_Ticks_GridLines();

                if (this.disableOutputLabels)
                    return;

                if (this.isStackPensMode && this.pens.Count > 0)
                {
                    this.CreateYLabels_StackPensMode();
                    return;
                }

                if (this.IsEnabledYScaleForEachPen)
                {
                    this.CreateYLabels_EnabledYScaleForEachPen();
                    return;
                }

                this.CreateYLabels_Not_EnabledYScaleForEachPen();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CreateYLabels Exception :: {ex.Message} || {ex.StackTrace}");
            }
        }

        private void CreateYLabels_Ticks_GridLines()
        {
            if (this.yTicks != null && this.yTicks.Length != this.numOfLabelsY)
            {
                foreach (FrameworkElement el in this.yTicks)
                    this.YAxis.Children.Remove(el);
                this.yTicks = null;
            }

            if (this.yTicks == null)
            {
                this.yTicks = new Line[this.numOfLabelsY];
                for (int i = 0; i < this.numOfLabelsY; i++)
                    this.yTicks[i] = null;
            }

            if (this.yGridLines != null && this.yGridLines.Length != this.numOfLabelsY)
            {
                foreach (FrameworkElement el in this.yGridLines)
                    this.Children.Remove(el);
                this.yGridLines = null;
            }

            if (this.yGridLines == null)
            {
                this.yGridLines = new Line[this.numOfLabelsY];
                for (int i = 0; i < this.numOfLabelsY; i++)
                    this.yGridLines[i] = null;
            }

            for (int i = this.numOfLabelsY - 1; i >= 0; i--)
            {
                if (this.yGridLines[i] == null)
                    this.yGridLines[i] = new Line() { IsHitTestVisible = false, Width = this.ChartArea.Width, Height = this.ChartArea.Height, Tag = "YGridLines" };
                this.yGridLines[i].Stroke = this.GridLinesBrush;
                this.SetLineStroke(this.yGridLines[i], this.gridLinesStroke);

                if (this.Children.Contains(this.yGridLines[i]) == false)
                    this.Children.Add(this.yGridLines[i]);

                if (this.yTicks[i] == null)
                    this.yTicks[i] = new Line() { IsHitTestVisible = false, Width = this.YAxis.ActualWidth, Height = this.YAxis.ActualHeight };
                this.yTicks[i].Stroke = this.LabelsBrush;
                if (this.YAxis.Children.Contains(this.yTicks[i]) == false)
                    this.YAxis.Children.Add(this.yTicks[i]);
            }
        }

        private void CreateYLabels_Not_EnabledYScaleForEachPen()
        {
            if (this.yLabels != null && this.yLabels.Length != this.numOfLabelsY)
            {
                foreach (FrameworkElement el in this.yLabels)
                    this.YAxis.Children.Remove(el);
                this.yLabels = null;
            }

            if (this.yLabels == null)
            {
                this.yLabels = new TextBlock[this.numOfLabelsY];
                for (int i = 0; i < this.numOfLabelsY; i++)
                    this.yLabels[i] = null;
            }

            double piece = this.numOfLabelsY == 1 ? 0.0 : (this.yMax - this.yMin) / (this.numOfLabelsY - 1);

            for (int i = this.numOfLabelsY - 1; i >= 0; i--)
            {
                double value;
                if (i > 0 && i == this.numOfLabelsY - 1)
                    value = this.yMax;
                else
                    value = this.yMin + (i * piece);

                if (this.yLabels[i] == null)
                    this.yLabels[i] = new TextBlock() { /*Background = Brushes.Transparent, BorderBrush = Brushes.Transparent,*/ HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, FontFamily = new FontFamily("Arial"), FontSize = 10, IsHitTestVisible = false };

                if (this.YLabelsEditable && (i == 0 || i == this.numOfLabelsY - 1))
                {
                    this.yLabels[i].IsHitTestVisible = true;
                    this.yLabels[i].MouseLeftButtonDown += this.YAxisLabelEditable_PointerPressed;
                    this.yLabels[i].MouseMove += this.YAxisLabelEditable_PointerMoved;
                    this.yLabels[i].Tag = i;
                }

                this.yLabels[i].Margin = new Thickness(0);
                this.yLabels[i].Text = this.GetStringYAxis(i, value);
                this.yLabels[i].Foreground = this.LabelsBrush;

                if (!this.YAxis.Children.Contains(this.yLabels[i]))
                    this.YAxis.Children.Add(this.yLabels[i]);

                this.yLabels[i].SetLocation(0, 0);
            }

            for (int i = 0; i < this.numOfLabelsY; i++)
            {
                this.yLabels[i].Opacity = 1.0;
                for (int j = 0; j < this.SubYDivisions; j++)
                {
                    i++;
                    if (i >= this.numOfLabelsY)
                        break;
                    this.yLabels[i].Opacity = this.DisableOutputSubYDivisions ? 0.0 : 0.3;
                }
            }
        }

        private void CreateYLabels_EnabledYScaleForEachPen()
        {
            if (this.yLabels != null && this.yLabels.Length != (this.numOfLabelsY * this.pens.Count))
            {
                foreach (FrameworkElement el in this.yLabels)
                    this.YAxis.Children.Remove(el);
                this.yLabels = null;
            }

            if (this.pens.Count == 0)
                return;

            if (this.yLabels == null)
            {
                this.yLabels = new TextBlock[this.numOfLabelsY * this.pens.Count];
                for (int i = 0; i < (this.numOfLabelsY * this.pens.Count); i++)
                    this.yLabels[i] = null;
            }

            bool shouldReturnAllScales = false;
            bool shouldApplyUnitsConversion = true;
            List<object[]> penYScales = this.GetPensYScale(shouldReturnAllScales, shouldApplyUnitsConversion);

            for (int i = 0; i < this.yLabels.Length; i++)
            {
                if (this.yLabels[i] != null)
                    this.yLabels[i].Visibility = i < (this.numOfLabelsY * penYScales.Count) ? Visibility.Visible : Visibility.Collapsed;
            }

            bool reset = this.lastPenYScales.Count != penYScales.Count;

            if (!reset)
            {
                for (int i = 0; i < penYScales.Count; i++)
                {
                    Brush nowStroke = penYScales[i][0] as Brush;
                    double nowYMin = TConvert.ToDouble(penYScales[i][1]);
                    double nowYMax = TConvert.ToDouble(penYScales[i][2]);

                    Brush prevStroke = this.lastPenYScales[i][0] as Brush;
                    double prevYMin = TConvert.ToDouble(this.lastPenYScales[i][1]);
                    double prevYMax = TConvert.ToDouble(this.lastPenYScales[i][2]);

                    if (nowYMin != prevYMin || nowYMax != prevYMax || !(nowStroke is SolidColorBrush) || !(prevStroke is SolidColorBrush) || (nowStroke as SolidColorBrush).Color.ToString(TCultureInfo.InvariantCulture) != (prevStroke as SolidColorBrush).Color.ToString(TCultureInfo.InvariantCulture))
                    {
                        reset = true;
                        break;
                    }
                }
            }

            if (reset)
            {
                foreach (YLabelGroup oldGroup in this.yLabelsGroup)
                {
                    if (oldGroup.Pens != null)
                    {
                        for (int j = 0; j < oldGroup.Pens.Length; j++)
                        {
                            if (oldGroup.Pens[j] != null)
                            {
                                for (int k = 0; k < oldGroup.Pens[j].Length; k++)
                                {
                                    if (oldGroup.Pens[j][k] != null && this.YAxis.Children.Contains(oldGroup.Pens[j][k]))
                                    {
                                        this.YAxis.Children.Remove(oldGroup.Pens[j][k]);
                                    }
                                }
                            }
                        }
                    }
                }

                this.lastPenYScales = penYScales;

                this.yLabelsGroup.Clear();

                for (int i = 0; i < (penYScales.Count + 3) / 3; i++)
                {
                    YLabelGroup _group = new YLabelGroup();
                    _group.Pens = new TextBlock[3][];
                    _group.PensWidth = new double[3][];
                    _group.PensHeight = new double[3][];
                    for (int j = 0; j < 3; j++)
                    {
                        _group.Pens[j] = new TextBlock[this.numOfLabelsY];
                        _group.PensWidth[j] = new double[this.numOfLabelsY];
                        _group.PensHeight[j] = new double[this.numOfLabelsY];
                        for (int k = 0; k < this.numOfLabelsY; k++)
                        {
                            _group.Pens[j][k] = null;
                            _group.PensWidth[j][k] = double.NaN;
                            _group.PensHeight[j][k] = double.NaN;
                        }
                    }
                    this.yLabelsGroup.Add(_group);
                }
            }

            foreach (YLabelGroup groupRef in this.yLabelsGroup)
                groupRef.GroupSize = 0;

            int group = 0;
            int groupIndex = 0;
            int offset = 0;
            for (int p = 0; p < penYScales.Count; p++, offset += this.numOfLabelsY)
            {
                YLabelGroup groupRef = this.yLabelsGroup[group];

                List<LineGraph> list;
                if (!this.dicLabelGroupToPen.TryGetValue(groupRef, out list))
                    list = new List<LineGraph>();

                list.Add(this.pens[p]);

                this.dicLabelGroupToPen[groupRef] = list;

                Brush stroke = penYScales[p][0] as Brush;
                double _yMin = TConvert.ToDouble(penYScales[p][1]);
                double _yMax = TConvert.ToDouble(penYScales[p][2]);


                bool isAutoEnabled = false;
                if (this.typeChart == TypeChart.TrendChart)
                    isAutoEnabled = (this.pens[p].TrendPen as TrendPen).Auto;
                else
                    isAutoEnabled = (this.pens[p].TrendPen as TrendPen).Auto;

                if (isAutoEnabled)
                {
                    _yMin = this.pens[p].MinValue;
                    _yMax = this.pens[p].MaxValue;
                }

                if(this.IsXYChart)
                {
                    _yMin = this.YMin;
                    _yMax = this.YMax;
                }

                double piece = this.numOfLabelsY == 1 ? 0.0 : (_yMax - _yMin) / (this.numOfLabelsY - 1);
                for (int i = this.numOfLabelsY - 1; i >= 0; i--)
                {
                    double value;
                    if (i > 0 && i == this.numOfLabelsY - 1)
                        value = _yMax;
                    else
                        value = _yMin + (i * piece);

                    if (this.yLabels[offset + i] == null)
                        this.yLabels[offset + i] = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, FontFamily = new FontFamily("Arial"), FontSize = 10, IsHitTestVisible = false };

                    this.yLabels[offset + i].Margin = new Thickness(0);

                    if (this.YLabelsEditable && (value == _yMax || value == _yMin))
                    {
                        bool isMin = value == _yMin;

                        this.yLabels[offset + i].IsHitTestVisible = true;
                        this.yLabels[offset + i].MouseLeftButtonDown += this.YAxisLabelEditable_PointerPressed;
                        this.yLabels[offset + i].MouseMove += this.YAxisLabelEditable_PointerMoved;
                        this.yLabels[offset + i].Tag = new object[] { isMin, this.pens[p] };
                    }

                    string lastValue = this.yLabels[offset + i].Text;
                    string nextValue = this.GetStringYAxis(i, value);

                    bool isEqual = lastValue != null && string.Compare(lastValue, nextValue, true) == 0;
                    if (isEqual == false)
                        this.yLabels[offset + i].Text = nextValue;

                    this.yLabels[offset + i].Foreground = stroke;

                    if (!this.YAxis.Children.Contains(this.yLabels[offset + i]))
                        this.YAxis.Children.Add(this.yLabels[offset + i]);

                    this.yLabels[offset + i].SetLocation(0, 0);

                    groupRef.Pens[groupIndex][i] = this.yLabels[offset + i];

                    groupRef.PensWidth[groupIndex][i] = this.GetTextBlockActualWidth(this.yLabels[offset + i]);
                    groupRef.PensHeight[groupIndex][i] = this.GetTextBlockActualHeight(this.yLabels[offset + i]);
                }
                for (int i = 0; i < this.numOfLabelsY; i++)
                {
                    this.yLabels[offset + i].Opacity = 1.0;
                    for (int j = 0; j < this.SubYDivisions; j++)
                    {
                        i++;
                        if (i >= this.numOfLabelsY)
                            break;
                        this.yLabels[offset + i].Opacity = this.DisableOutputSubYDivisions ? 0.0 : 0.3;
                    }
                }
                groupRef.GroupSize++;
                groupIndex++;
                if (groupIndex >= 3)
                {
                    groupIndex = 0;
                    group++;
                }
            }
        }

        private void CreateYLabels_StackPensMode()
        {
            if (this.yLabels != null && this.yLabels.Length != (this.numOfLabelsY * this.pens.Count))
            {
                foreach (FrameworkElement el in this.yLabels)
                    this.YAxis.Children.Remove(el);
                this.yLabels = null;
            }

            if (this.pens.Count == 0)
                return;

            if (this.yLabels == null)
            {
                this.yLabels = new TextBlock[this.numOfLabelsY * this.pens.Count];
                for (int i = 0; i < (this.numOfLabelsY * this.pens.Count); i++)
                    this.yLabels[i] = null;
            }
            bool shouldReturnAllScales = false;
            bool shouldApplyUnitsConversion = true;
            List<object[]> penYScales = this.GetPensYScale(shouldReturnAllScales, shouldApplyUnitsConversion);

            if (this.isEnabledYScaleForEachPen)
            {
                for (int i = 0; i < this.yLabels.Length; i++)
                {
                    if (this.yLabels[i] != null)
                        this.yLabels[i].Visibility = Visibility.Collapsed;
                }

                int penScaleIndex = 0;

                for (int p = 0; p < this.pens.Count; p++)
                {
                    if (this.pens[p].Visibility != Visibility.Visible)
                        continue;

                    if (penScaleIndex >= penYScales.Count)
                        break;

                    int offset = p * this.numOfLabelsY;

                    Brush stroke = penYScales[penScaleIndex][0] as Brush;
                    double _yMin = TConvert.ToDouble(penYScales[penScaleIndex][1]);
                    double _yMax = TConvert.ToDouble(penYScales[penScaleIndex][2]);

                    bool isAutoEnabled = false;
                    if (this.typeChart == TypeChart.TrendChart)
                        isAutoEnabled = (this.pens[p].TrendPen as TrendPen).Auto;
                    else
                        isAutoEnabled = (this.pens[p].TrendPen as TrendPen).Auto;

                    if (isAutoEnabled)
                    {
                        _yMin = this.pens[p].MinValue;
                        _yMax = this.pens[p].MaxValue;
                    }

                    if (this.IsXYChart)
                    {
                        _yMin = this.YMin;
                        _yMax = this.YMax;
                    }

                    double piece = this.numOfLabelsY == 1 ? 0.0 : (_yMax - _yMin) / (this.numOfLabelsY - 1);

                    int createdCount = 0;

                    for (int i = this.numOfLabelsY - 1; i >= 0; i--)
                    {
                        double value;
                        if (i > 0 && i == this.numOfLabelsY - 1)
                            value = _yMax;
                        else
                            value = _yMin + (i * piece);

                        if (this.yLabels[offset + i] == null)
                        {
                            this.yLabels[offset + i] = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, FontFamily = new FontFamily("Arial"), FontSize = 10, IsHitTestVisible = false };
                            createdCount++;
                        }

                        this.yLabels[offset + i].Margin = new Thickness(0);

                        if (this.YLabelsEditable && (value == _yMax || value == _yMin))
                        {
                            bool isMin = value == _yMin;

                            this.yLabels[offset + i].IsHitTestVisible = true;
                            this.yLabels[offset + i].MouseLeftButtonDown += this.YAxisLabelEditable_PointerPressed;
                            this.yLabels[offset + i].MouseMove += this.YAxisLabelEditable_PointerMoved;
                            this.yLabels[offset + i].Tag = new object[] { isMin, this.pens[p] };
                        }

                        string lastValue = this.yLabels[offset + i].Text;
                        string nextValue = this.GetStringYAxis(i, value);

                        bool isEqual = lastValue != null && string.Compare(lastValue, nextValue, true) == 0;
                        if (isEqual == false)
                            this.yLabels[offset + i].Text = nextValue;

                        this.yLabels[offset + i].Foreground = stroke;

                        if (!this.YAxis.Children.Contains(this.yLabels[offset + i]))
                            this.YAxis.Children.Add(this.yLabels[offset + i]);

                        this.yLabels[offset + i].SetLocation(0, 0);
                    }

                    for (int i = 0; i < this.numOfLabelsY; i++)
                    {
                        if (this.yLabels[offset + i] != null)
                        {
                            this.yLabels[offset + i].Visibility = Visibility.Visible;
                            this.yLabels[offset + i].Opacity = 1.0;
                        }

                        for (int j = 0; j < this.SubYDivisions; j++)
                        {
                            i++;
                            if (i >= this.numOfLabelsY)
                                break;
                            if (this.yLabels[offset + i] != null)
                            {
                                this.yLabels[offset + i].Visibility = Visibility.Visible;
                                this.yLabels[offset + i].Opacity = this.DisableOutputSubYDivisions ? 0.0 : 0.3;
                            }
                        }
                    }

                    penScaleIndex++;
                }
            }
            else
            {
                double piece = this.numOfLabelsY == 1 ? 0.0 : (this.yMax - this.yMin) / (this.numOfLabelsY - 1);

                for (int p = 0; p < this.pens.Count; p++)
                {
                    if (this.pens[p].Visibility != Visibility.Visible)
                        continue;

                    int offset = p * this.numOfLabelsY;

                    int penScaleIndex = 0;
                    for (int j = 0; j < p; j++)
                    {
                        if (this.pens[j].Visibility == Visibility.Visible)
                            penScaleIndex++;
                    }

                    if (penScaleIndex >= penYScales.Count)
                        continue;

                    Brush stroke = penYScales[penScaleIndex][0] as Brush;

                    for (int i = this.numOfLabelsY - 1; i >= 0; i--)
                    {
                        double value;
                        if (i > 0 && i == this.numOfLabelsY - 1)
                            value = this.yMax;
                        else
                            value = this.yMin + (i * piece);

                        if (this.yLabels[offset + i] == null)
                            this.yLabels[offset + i] = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top, FontFamily = new FontFamily("Arial"), FontSize = 10, IsHitTestVisible = false };

                        this.yLabels[offset + i].Margin = new Thickness(0);

                        if (this.YLabelsEditable && (i == 0 || i == this.numOfLabelsY - 1))
                        {
                            this.yLabels[offset + i].IsHitTestVisible = true;
                            this.yLabels[offset + i].MouseLeftButtonDown += this.YAxisLabelEditable_PointerPressed;
                            this.yLabels[offset + i].MouseMove += this.YAxisLabelEditable_PointerMoved;
                            this.yLabels[offset + i].Tag = new object[] { i == 0, this.pens[p] };
                        }

                        this.yLabels[offset + i].Text = this.GetStringYAxis(i, value);
                        this.yLabels[offset + i].Foreground = stroke;

                        if (!this.YAxis.Children.Contains(this.yLabels[offset + i]))
                            this.YAxis.Children.Add(this.yLabels[offset + i]);

                        this.yLabels[offset + i].SetLocation(0, 0);
                    }

                    for (int i = 0; i < this.numOfLabelsY; i++)
                    {
                        this.yLabels[offset + i].Opacity = 1.0;
                        for (int j = 0; j < this.SubYDivisions; j++)
                        {
                            i++;
                            if (i >= this.numOfLabelsY)
                                break;
                            this.yLabels[offset + i].Opacity = this.DisableOutputSubYDivisions ? 0.0 : 0.3;
                        }
                    }
                }
            }
        }

        private string GetStringXAxis(int index, DateTime dt)
        {
            try
            {
                if (this.labelFormatX == "-")
                    return "";

                if (this.ShowDuration > 0 && this.ChartType == 0)
                {
                    TimeSpan sp = this.ShowDuration == 1 ? (dt.ToLocalTime() - this.XMin.ToLocalTime()) : (dt.ToLocalTime() - this.XMax.ToLocalTime());
                    TimeSpan sp2 = new TimeSpan(sp.Days, sp.Hours, sp.Minutes, sp.Seconds);
                    if (ShowDuration == 1 && sp.Milliseconds >= 500)
                        sp2 = sp2 + TimeSpan.FromSeconds(1);
                    if (ShowDuration > 1 && sp.Milliseconds <= -500)
                        sp2 = sp2 + TimeSpan.FromSeconds(-1);
                    try
                    {
                        string str;
                        if (string.IsNullOrEmpty(this.labelFormatX))
                            str = TConvert.ToString(sp2);
                        else
                        {
                            switch (this.labelFormatX)
                            {
                                case "HH:MM:SS":
                                case "hh:mm:ss":
                                    str = sp2.ToString(@"hh\:mm\:ss");
                                    break;

                                case "hh:mm":
                                case "HH:MM":
                                    str = sp2.ToString(@"hh\:mm");
                                    break;

                                case "DD":
                                    str = TConvert.To<int>(sp2.TotalDays).ToString(TCultureInfo.InvariantCulture);
                                    break;

                                case "HH":
                                    str = TConvert.To<int>(sp2.TotalHours).ToString(TCultureInfo.InvariantCulture);
                                    break;

                                case "MM":
                                    str = TConvert.To<int>(sp2.TotalMinutes).ToString(TCultureInfo.InvariantCulture);
                                    break;

                                case "SS":
                                    str = TConvert.To<int>(sp2.TotalSeconds).ToString(TCultureInfo.InvariantCulture);
                                    break;

                                default:
                                    str = sp2.ToString(this.labelFormatX);
                                    break;
                            }
                        }
                        int comma = str.IndexOf(',');
                        if (comma > 0)
                        {
                            string strComma = str.Substring(comma).TrimEnd('0');
                            if (strComma.Length == 1)
                                strComma = "";
                            str = str.Remove(comma) + strComma;
                        }
                        return str;
                    }
                    catch
                    {
                    }
                }

                if (this.labelFormatX == null || this.labelFormatX.Length == 0)
                {
                    if (ChartType == 1)
                        return ((dt.Ticks - (100000000 * 10000000000L)) / 10000000000.0).ToString("N02");

                    return dt.ToLocalTime().ToLongTimeString() + Environment.NewLine + dt.ToLocalTime().ToString(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
                }

                if (ChartType == 1)
                    return ((dt.Ticks - (100000000 * 10000000000L)) / 10000000000.0).ToString(this.labelFormatX.Trim() == "*" ? "N02" : this.labelFormatX);

                if (this.labelFormatX.Trim().ToUpper() == "HH:MM")
                    return dt.ToLocalTime().ToShortTimeString();
                if (this.labelFormatX.Trim().ToUpper() == "HH:MM:SS")
                    return dt.ToLocalTime().ToLongTimeString();

                if (this.labelFormatX.Trim() == "*")
                {
                    try
                    {
                        if (index == 0)
                            return dt.ToLocalTime().ToString(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern) + (this.Orientation == 0 ? " " : Environment.NewLine) + dt.ToLocalTime().ToLongTimeString();
                        TimeSpan duration = this.XMax - this.XMin;
                        //if (duration.TotalHours < 1)
                        //    return dt.ToLocalTime().ToString("mm:ss");
                        if (duration.TotalDays < 1)
                            return dt.ToLocalTime().ToLongTimeString();
                        return dt.ToLocalTime().ToString(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern) + (this.Orientation == 0 ? " " : Environment.NewLine) + dt.ToLocalTime().ToShortTimeString();
                    }
                    catch
                    {
                    }
                }

                string[] fmts = this.labelFormatX.Split(new string[] { "\\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (fmts.Length > 1)
                    return dt.ToLocalTime().ToString(fmts[0]) + Environment.NewLine + dt.ToLocalTime().ToString(fmts[1]);
                return dt.ToLocalTime().ToString(fmts[0]);
            }
            catch
            {
                if (ChartType == 1)
                    return ((dt.Ticks - (100000000 * 10000000000L)) / 10000000000.0).ToString("N02");

                return dt.ToLocalTime().ToLongTimeString() + Environment.NewLine + dt.ToLocalTime().ToShortDateString();
            }
        }

        private string GetStringXAxis(int index, double value)
        {
            string str;
            try
            {
                if (this.labelFormatX == null || this.labelFormatX.Length == 0)
                    str = value.ToString("N02");
                else
                {
                    string format = this.labelFormatX.Trim() == "*" ? "N02" : this.labelFormatX;
                    str = value.ToString(format);
                }
            }
            catch
            {
                str = value.ToString("N02");
            }

            if (str == "-0")
                str = "0";

            return str;
        }

        private string GetStringYAxis(int index, double value)
        {
            string str;
            try
            {
                if (this.labelFormatY == null || this.labelFormatY.Length == 0)
                    str = value.ToString("N02");
                else
                    str = value.ToString(this.labelFormatY);
            }
            catch
            {
                str = value.ToString("N02");
            }

            if (str == "-0")
                str = "0";

            return str;
        }

        private double GetMarkerSize(object column)
        {
            if (!ConfigHelper.IsDesignMode && this.InstanceOfContents != null && this.InstanceOfContents.GetType().GetMethod("GetMarkerSize") != null)
            {
                try
                {
                    double markerSize = 10;
                    object chart = null;
                    ConfigHelper.ObjServer.DB.SetFlagWaitValueFromServer(true);
                    object result = this.InstanceOfContents.GetType().GetMethod("GetMarkerSize").Invoke(this.InstanceOfContents, new object[] { null, chart, column });
                    ConfigHelper.ObjServer.DB.SetFlagWaitValueFromServer(false);

                    if (result != null)
                        markerSize = TConvert.ToDouble(result);
                    return markerSize;
                }
                catch (Exception ex)
                {
                    if (ConfigHelper.ObjServer != null && ConfigHelper.ObjServer.RunException != null)
                        ConfigHelper.ObjServer.RunException.Log(ex);
                }
            }

            if (!ConfigHelper.IsDesignMode && this.miGetMarkerSize != null && this.InstanceOfContentsJS != null)
            {
                try
                {
                    double markerSize = 10;
                    object chart = null;
                    ConfigHelper.ObjServer.DB.SetFlagWaitValueFromServer(true);
                    object result = TInterop.ExecuteJavaScriptBase("$0[$1]($2, $3, $4)", this.InstanceOfContentsJS, "GetMarkerSize", null, chart, column);
                    ConfigHelper.ObjServer.DB.SetFlagWaitValueFromServer(false);
                    if (result != null)
                        markerSize = TConvert.ToDouble(result);
                    return markerSize;
                }
                catch (Exception ex)
                {
                    if (ConfigHelper.ObjServer != null && ConfigHelper.ObjServer.RunException != null)
                        ConfigHelper.ObjServer.RunException.Log(ex);
                }
            }

            return 10;
        }

        private bool GetMarkerVisible(TrendPointInfo pi, object column)
        {
            if (!ConfigHelper.IsDesignMode && this.InstanceOfContents != null && this.InstanceOfContents.GetType().GetMethod("GetMarkerVisible") != null)
            {
                try
                {
                    bool isVisible = true;
                    object chart = null;
                    ConfigHelper.ObjServer.DB.SetFlagWaitValueFromServer(true);
                    object result = this.InstanceOfContents.GetType().GetMethod("GetMarkerVisible").Invoke(this.InstanceOfContents, new object[] { chart, column, pi });
                    ConfigHelper.ObjServer.DB.SetFlagWaitValueFromServer(false);

                    if (result != null)
                        isVisible = TConvert.To<bool>(result);

                    return isVisible;
                }
                catch (Exception ex)
                {
                    if (ConfigHelper.ObjServer != null && ConfigHelper.ObjServer.RunException != null)
                        ConfigHelper.ObjServer.RunException.Log(ex);
                }
            }

            if (!ConfigHelper.IsDesignMode && this.miGetMarkerVisible != null && this.InstanceOfContentsJS != null)
            {
                try
                {
                    bool isVisible = true;
                    object chart = null;
                    ConfigHelper.ObjServer.DB.SetFlagWaitValueFromServer(true);
                    object result = TInterop.ExecuteJavaScriptBase("$0[$1]($2, $3, $4)", this.InstanceOfContentsJS, "GetMarkerVisible", chart, column, pi);
                    ConfigHelper.ObjServer.DB.SetFlagWaitValueFromServer(false);

                    if (result != null)
                        isVisible = TConvert.To<bool>(result);

                    return isVisible;
                }
                catch (Exception)
                {
                }
            }

            if (pi.isVirtual)
                return false;

            if (this.typeChart == TypeChart.TrendChart)
            {
                if (column != null && (column as TrendPen).Marker != (int)TrendChartPlotter.eTrendMarkerType.None && (column as TrendPen).MarkerSettings == 1 && pi.quality == (int)eQuality.Good)
                    return false;

                if (column == null)
                    return false;

                if ((column as TrendPen).Marker == (int)TrendChartPlotter.eTrendMarkerType.None && string.IsNullOrEmpty(pi.annotation))
                    return false;

                return true;
            }

            if (column != null && (column as TrendPen).Marker != (int)TrendChartPlotter.eTrendMarkerType.None && (column as TrendPen).MarkerSettings == 1 && pi.quality == (int)eQuality.Good)
                return false;

            if (column == null)
                return false;

            if ((column as TrendPen).Marker == (int)TrendChartPlotter.eTrendMarkerType.None && string.IsNullOrEmpty(pi.annotation))
                return false;

            return true;
        }

        private Brush GetMarkerFill(TrendPointInfo pi, object column)
        {
            if (!ConfigHelper.IsDesignMode && this.InstanceOfContents != null && this.InstanceOfContents.GetType().GetMethod("GetMarkerFill") != null)
            {
                try
                {
                    Brush fillColor = null;
                    object chart = null;
                    ConfigHelper.ObjServer.DB.SetFlagWaitValueFromServer(true);
                    object result = this.InstanceOfContents.GetType().GetMethod("GetMarkerFill").Invoke(this.InstanceOfContents, new object[] { chart, column, pi });
                    ConfigHelper.ObjServer.DB.SetFlagWaitValueFromServer(false);

                    if (result != null)
                    {
                        fillColor = TConvert.To<Brush>(result);
                        return fillColor;
                    }
                }
                catch (Exception ex)
                {
                    if (ConfigHelper.ObjServer != null && ConfigHelper.ObjServer.RunException != null)
                        ConfigHelper.ObjServer.RunException.Log(ex);
                }
            }

            if (!ConfigHelper.IsDesignMode && this.miGetMarkerFill != null && this.InstanceOfContentsJS != null)
            {
                try
                {
                    Brush fillColor = null;
                    object chart = null;
                    ConfigHelper.ObjServer.DB.SetFlagWaitValueFromServer(true);
                    object result = TInterop.ExecuteJavaScriptBase("$0[$1]($2, $3, $4)", this.InstanceOfContentsJS, "GetMarkerFill", chart, column, pi);
                    ConfigHelper.ObjServer.DB.SetFlagWaitValueFromServer(false);

                    if (result != null)
                    {
                        fillColor = TConvert.To<Brush>(result);
                        return fillColor;
                    }
                }
                catch (Exception)
                {
                }
            }

            if (this.typeChart == TypeChart.TrendChart)
            {
                if (!string.IsNullOrEmpty(pi.annotation))
                {
                    if (column == null || (column as TrendPen).Marker == (int)TrendChartPlotter.eTrendMarkerType.None || pi.quality == (int)eQuality.Good)
                        return (column as TrendPen).PenLine.Stroke;

                    if (column != null && (column as TrendPen).Marker > 0)
                        return (column as TrendPen).PenLine.Fill;
                }

                return pi.quality == (int)eQuality.Good ? (column as TrendPen)._polyLine.Stroke : Brushes.Transparent;
            }

            if (!string.IsNullOrEmpty(pi.annotation))
            {
                if (column == null || (column as TrendPen).Marker == (int)TrendChartPlotter.eTrendMarkerType.None || pi.quality == (int)eQuality.Good)
                    return (column as TrendPen).PenLine.Stroke;

                if (column != null && (column as TrendPen).Marker > 0)
                    return (column as TrendPen).PenLine.Fill;
            }

            return pi.quality == (int)eQuality.Good ? (column as TrendPen)._polyLine.Stroke : Brushes.Transparent;
        }

        private string FormatTooltip2(object column, double value, DateTime dt, int quality, TrendPointInfo pi)
        {
            string tooltip = null;
            if (!ConfigHelper.IsDesignMode && this.InstanceOfContents != null && this.InstanceOfContents.GetType().GetMethod("GetMarkerTooltip") != null)
            {
                try
                {
                    object chart = null;
                    ConfigHelper.ObjServer.DB.SetFlagWaitValueFromServer(true);
                    object result = this.InstanceOfContents.GetType().GetMethod("GetMarkerTooltip").Invoke(this.InstanceOfContents, new object[] { chart, column, pi });
                    ConfigHelper.ObjServer.DB.SetFlagWaitValueFromServer(false);

                    if (result != null)
                        return TConvert.ToString(result);
                }
                catch (Exception ex)
                {
                    if (ConfigHelper.ObjServer != null && ConfigHelper.ObjServer.RunException != null)
                        ConfigHelper.ObjServer.RunException.Log(ex);
                }
            }
            if (!ConfigHelper.IsDesignMode && this.miGetMarkerTooltip != null && this.InstanceOfContentsJS != null)
            {
                try
                {
                    object chart = null;
                    ConfigHelper.ObjServer.DB.SetFlagWaitValueFromServer(true);
                    object result = TInterop.ExecuteJavaScriptBase("$0[$1]($2, $3, $4)", this.instanceOfContentsJS, "GetMarkerTooltip", chart, column, pi);
                    ConfigHelper.ObjServer.DB.SetFlagWaitValueFromServer(false);

                    if (result != null)
                        return TConvert.ToString(result);
                }
                catch (Exception ex)
                {
                    if (ConfigHelper.ObjServer != null && ConfigHelper.ObjServer.RunException != null)
                        ConfigHelper.ObjServer.RunException.Log(ex);
                }
            }


            tooltip = !string.IsNullOrEmpty(pi.annotation) ? pi.annotation : "";
            if (this.EnableMarkerTooltip)
            {
                if (!string.IsNullOrEmpty(tooltip))
                    tooltip += Environment.NewLine;

                tooltip += this.FormatTooltip(column, value, dt, quality);
            }
            return string.IsNullOrEmpty(tooltip) ? String.Empty : tooltip;
        }

        private string FormatTooltip(object column, double value, DateTime dt, int quality)
        {
            if (value == Double.NaN)
                return "";

            value = this.ConvertToDisplayValue(column, value);

            string str = "";

            try
            {
                if (this.LabelFormatY == null || this.LabelFormatY.Length == 0)
                    str = "Value: " + value.ToString("N02");
                else
                    str = "Value: " + value.ToString(this.LabelFormatY);
            }
            catch
            {
                str = "Value: " + value.ToString("N02");
            }

            if (dt.Ticks == 0)
                return str;

            str += (this.ChartType == 0 ? "<br>Timestamp: " : "<br>X: ");

            try
            {
                if (this.LabelFormatX == null || this.LabelFormatX.Length == 0)
                {
                    if (this.ChartType == 1)
                        str += ((dt.Ticks - (100000000 * 10000000000L)) / 10000000000.0).ToString("N02");
                    else
                        str += dt.ToLocalTime().ToLongTimeString() + " " + dt.ToLocalTime().ToShortDateString();
                }
                else
                {
                    if (this.ChartType == 1)
                        str += ((dt.Ticks - (100000000 * 10000000000L)) / 10000000000.0).ToString(this.LabelFormatX.Trim() == "*" ? "N02" : this.LabelFormatX);
                    else
                    {
                        if (this.LabelFormatX.Trim() == "*")
                        {
                            str += dt.ToLocalTime().ToShortDateString() + " " + dt.ToLocalTime().ToString("HH:mm:ss");
                        }
                        else
                        {
                            string[] fmts = this.LabelFormatX.Split(new string[] { "\\n" }, StringSplitOptions.RemoveEmptyEntries);
                            if (fmts.Length > 1)
                                str += dt.ToLocalTime().ToString(fmts[0]) + " " + dt.ToLocalTime().ToString(fmts[1]);
                            else
                                str += dt.ToLocalTime().ToString(fmts[0]);
                        }
                    }
                }
            }
            catch
            {
                if (this.ChartType == 1)
                    str += ((dt.Ticks - (100000000 * 10000000000L)) / 10000000000.0).ToString("N02");
                else
                    str += dt.ToLocalTime().ToLongTimeString() + " " + dt.ToLocalTime().ToShortDateString();
            }

            if (this.ChartType == 0)
            {
                str += "<br>Quality: ";

                switch (quality)
                {
                    case (int)eQuality.Good:
                        str += (ConfigHelper.ObjServer != null && ConfigHelper.ObjServer.DB != null ? ConfigHelper.ObjServer.DB.Locale("Good") : "Good");
                        break;

                    case (int)eQuality.Bad:
                        str += (ConfigHelper.ObjServer != null && ConfigHelper.ObjServer.DB != null ? ConfigHelper.ObjServer.DB.Locale("Bad") : "Bad");
                        break;

                    case (int)eQuality.Undefined:
                        str += (ConfigHelper.ObjServer != null && ConfigHelper.ObjServer.DB != null ? ConfigHelper.ObjServer.DB.Locale("Undefined") : "Undefined");
                        break;

                    default:
                        str += TConvert.ToString(quality);
                        break;
                }
            }

            return str;
        }

        private string FormatTooltip(double yValue, double xValue)
        {
            if (yValue == Double.NaN)
                return "";

            string str = "";

            try
            {
                if (this.LabelFormatX == null || this.LabelFormatX.Length == 0)
                    str = "X: " + xValue.ToString("N02");
                else
                    str = "X: " + xValue.ToString(this.LabelFormatX);
            }
            catch
            {
                str = "X: " + xValue.ToString("N02");
            }

            str += "<br>";

            try
            {
                if (this.LabelFormatY == null || this.LabelFormatY.Length == 0)
                    str += "Y: " + yValue.ToString("N02");
                else
                    str += "Y: " + yValue.ToString(this.LabelFormatY);
            }
            catch
            {
                str += "Y: " + yValue.ToString("N02");
            }

            return str;
        }

        public double ConvertToDisplayValue(object column, double value)
        {
            if (!ConfigHelper.IsDesignMode && !string.IsNullOrEmpty(T.Wpf.RunServices.ConfigHelper.ObjServer.DB.DictionaryUnits) && column != null)
            {
                T.Kernel.Tags.TagObj tagObj = null;

                if (this.typeChart == TypeChart.TrendChart && (column as TrendPen) != null)
                    tagObj = (column as TrendPen).TagRef.MainRunObj as T.Kernel.Tags.TagObj;
                else if ((column as TrendPen) != null)
                    tagObj = (column as Base.DrillingPen).TagRef[0].MainRunObj as T.Kernel.Tags.TagObj;

                if (tagObj != null)
                    value = T.Wpf.RunServices.ConfigHelper.ObjServer.DB.GetDictionaryUnitsValue(tagObj.Units, value);
            }
            return value;
        }

        private void SetLineStroke(Line line, string stroke)
        {
            string val = "";
            val = TString.GetAttribute("StrokeDashArray", this.gridLinesStroke);
            if (!String.IsNullOrEmpty(val))
                line.StrokeDashArray = TConvert.To<DoubleCollection>(val);
            val = TString.GetAttribute("StrokeDashOffset", this.gridLinesStroke);
            if (!String.IsNullOrEmpty(val))
                line.StrokeDashOffset = TConvert.To<Double>(val);
            //val = TString.GetAttribute("StrokeEndLineCap", this.gridLinesStroke);
            //if (!String.IsNullOrEmpty(val))
            //    line.StrokeEndLineCap = TConvert.To<PenLineCap>(val);
            val = TString.GetAttribute("StrokeLineJoin", this.gridLinesStroke);
            if (!String.IsNullOrEmpty(val))
                line.StrokeLineJoin = TConvert.To<PenLineJoin>(val);
            val = TString.GetAttribute("StrokeMiterLimit", this.gridLinesStroke);
            if (!String.IsNullOrEmpty(val))
                line.StrokeMiterLimit = TConvert.To<Double>(val);
            //val = TString.GetAttribute("StrokeStartLineCap", this.gridLinesStroke);
            //if (!String.IsNullOrEmpty(val))
            //    line.StrokeStartLineCap = TConvert.To<PenLineCap>(val);
            val = TString.GetAttribute("StrokeThickness", this.gridLinesStroke);
            if (!String.IsNullOrEmpty(val))
                line.StrokeThickness = TConvert.To<Double>(val);
            else
                line.StrokeThickness = 1;
        }

        internal static TrendPointInfo MinByX(IList<TrendPointInfo> points)
        {
            TrendPointInfo minPoint = points[0];
            foreach (TrendPointInfo p in points)
                if (p.dt < minPoint.dt)
                    minPoint = p;
            return minPoint;
        }

        internal static TrendPointInfo MaxByX(IList<TrendPointInfo> points)
        {
            TrendPointInfo maxPoint = points[0];
            foreach (TrendPointInfo p in points)
                if (p.dt > maxPoint.dt)
                    maxPoint = p;
            return maxPoint;
        }

        internal static TrendPointInfo MinByY(IList<TrendPointInfo> points)
        {
            TrendPointInfo minPoint = points[0];
            foreach (TrendPointInfo p in points)
                if (p.value < minPoint.value)
                    minPoint = p;
            return minPoint;
        }

        internal static TrendPointInfo MaxByY(IList<TrendPointInfo> points)
        {
            TrendPointInfo maxPoint = points[0];
            foreach (TrendPointInfo p in points)
                if (p.value > maxPoint.value)
                    maxPoint = p;
            return maxPoint;
        }

        public IEnumerable<TrendPointInfo> FilterPoints(LineGraph line, IEnumerable<TrendPointInfo> _pointInfos)
        {
            if (_pointInfos == null)
                return _pointInfos;
            return _pointInfos;

            //Console.WriteLine("Points before filtering: " + _pointInfos.Count());

            //List<TrendPointInfo> pointInfos = new List<TrendPointInfo>(_pointInfos);

            //Rect screenRect = new Rect(0, 0, this.RenderedWidth, this.RenderedHeight);

            //var transform = this.Transform;
            ////if (line.DataTransform != null)
            ////    transform = transform.WithDataTransform(line.DataTransform);

            //EnumerableDataSource<TrendPointInfo> data = line.DataSource as EnumerableDataSource<TrendPointInfo>;

            //if (pointInfos.Count >= 2)
            //{
            //    TrendPointInfo pi = pointInfos[0];
            //    Point p = transform.DataToScreen(new Point(data.XMapping(pi), data.YMapping(pi)));
            //    if (p.X < -10000)
            //    {
            //        List<TrendPointInfo> _points = new List<TrendPointInfo>(pointInfos.Count);
            //        bool shouldCheck = true;
            //        foreach (TrendPointInfo _pi in pointInfos)
            //        {
            //            DateTime dt = _pi.dt;
            //            if (shouldCheck)
            //            {
            //                p = transform.DataToScreen(new Point(data.XMapping(_pi), data.YMapping(_pi)));
            //                if (p.X < -10000)
            //                {
            //                    p = transform.ScreenToData(new Point(-10000, p.Y));
            //                    dt = new DateTime((long)(p.X * 10000000000.0), DateTimeKind.Utc);
            //                }
            //                else
            //                    shouldCheck = false;
            //            }
            //            _points.Add(new TrendPointInfo(dt, _pi.value, _pi.quality, _pi.type, _pi.isVirtual, _pi.canSetAnnotation, _pi.annotation, _pi.column));
            //        }
            //        pointInfos = _points;
            //    }
            //    pi = pointInfos[pointInfos.Count - 1];
            //    p = transform.DataToScreen(new Point(data.XMapping(pi), data.YMapping(pi)));
            //    if (p.X > 10000)
            //    {
            //        List<TrendPointInfo> _points = new List<TrendPointInfo>(pointInfos.Count);
            //        foreach (TrendPointInfo _pi in pointInfos)
            //        {
            //            DateTime dt = _pi.dt;
            //            p = transform.DataToScreen(new Point(data.XMapping(_pi), data.YMapping(_pi)));
            //            if (p.X > 10000)
            //            {
            //                p = transform.ScreenToData(new Point(10000, p.Y));
            //                dt = new DateTime((long)(p.X * 10000000000.0), DateTimeKind.Utc);
            //            }
            //            _points.Add(new TrendPointInfo(dt, _pi.value, _pi.quality, _pi.type, _pi.isVirtual, _pi.canSetAnnotation, _pi.annotation, _pi.column));
            //        }
            //        pointInfos = _points;
            //    }
            //}

            //if (screenRect.Width < 10 || pointInfos.Count < screenRect.Width)
            //    return pointInfos;

            //List<Point> points = new List<Point>(pointInfos.Count);
            //foreach (TrendPointInfo pi in pointInfos)
            //    points.Add(new Point(data.XMapping(pi), data.YMapping(pi)));

            //points = transform.DataToScreenAsList(points);

            //TimeSpan duration = this.XMax - this.XMin;

            //if (points.Count == 0 || duration.Ticks == 0)
            //    return pointInfos;

            //List<TrendPointInfo> resultPoints = new List<TrendPointInfo>(pointInfos.Count);
            //List<TrendPointInfo> currentChainPointInfo = new List<TrendPointInfo>(pointInfos.Count);

            //double ticksByPixel = duration.Ticks / screenRect.Width;

            //double currentX = Math.Floor(pointInfos[0].dt.Ticks / ticksByPixel);
            //for (int i = 0; i < points.Count; i++)
            //{
            //    TrendPointInfo pi = pointInfos[i];
            //    if (Math.Floor(pi.dt.Ticks / ticksByPixel) == currentX && (i + 1) < points.Count && string.IsNullOrEmpty(pi.annotation) && !pi.isVirtual)
            //    {
            //        currentChainPointInfo.Add(pi);
            //    }
            //    else
            //    {
            //        // Analyse current chain
            //        if (currentChainPointInfo.Count <= 2)
            //        {
            //            resultPoints.AddRange(currentChainPointInfo);
            //        }
            //        else
            //        {
            //            TrendPointInfo first = MinByX(currentChainPointInfo);
            //            TrendPointInfo last = MaxByX(currentChainPointInfo);
            //            TrendPointInfo min = MinByY(currentChainPointInfo);
            //            TrendPointInfo max = MaxByY(currentChainPointInfo);
            //            resultPoints.Add(first);

            //            TrendPointInfo smaller = min.dt < max.dt ? min : max;
            //            TrendPointInfo greater = min.dt > max.dt ? min : max;
            //            if (smaller != resultPoints[resultPoints.Count - 1])
            //                resultPoints.Add(smaller);
            //            if (greater != resultPoints[resultPoints.Count - 1])
            //                resultPoints.Add(greater);
            //            if (last != resultPoints[resultPoints.Count - 1])
            //                resultPoints.Add(last);
            //        }
            //        currentChainPointInfo.Clear();
            //        if (!string.IsNullOrEmpty(pi.annotation) || pi.isVirtual)
            //        {
            //            resultPoints.Add(pi);
            //        }
            //        else
            //        {
            //            currentChainPointInfo.Add(pi);
            //        }
            //        currentX = Math.Floor(pi.dt.Ticks / ticksByPixel);
            //    }
            //}

            //resultPoints.AddRange(currentChainPointInfo);

            //Console.WriteLine("Points after filtering: " + resultPoints.Count());

            //return resultPoints.ToArray();
        }

        public void UpdatePrimaryCursor()
        {
            this.UpdatePrimaryCursor(null);
        }

        private void UpdatePrimaryCursor(LineGraph pen)
        {
            Rect output = this.ChartArea;
            if (output.Width <= 2 || output.Height <= 2)
                return;

            if (this.chart == null)
                return;

            if (!this.primaryCursorClicked)
                return;

            this.UpdatePrimaryCursor(pen, this.CursorPositionX, this.CursorPositionY);
        }

        private void UpdatePrimaryCursor(LineGraph pen, double pointX, double pointY)
        {
            if (this.pens.Count > 0)
            {
                Point valuePoint = this.CalculateActualValuePointFromClickedCoodinates(pointX, pointY, 0);
                DateTime dt = this.primaryXAxis.JavaTimeStampToDateTime(this.xAxisMin, valuePoint.X);

                if (this.CursorOutputChanged != null)
                    this.CursorOutputChanged(this, new GenericObjectEventArgs(dt, null, null));

                double dataPointX = dt.Ticks;
                dataPointX = dataPointX / 10000000000.0;
                for (int i = 0; i < this.pens.Count; i++)
                {
                    if (pen != null && pen != this.pens[i])
                        continue;

                    List<Point> visiblePoints = this.pens[i].VisiblePoints;
                    int searchIndex = this.SearchXBetween(dataPointX, visiblePoints);
                    double y = Double.NaN;
                    double y2 = Double.NaN;
                    if (searchIndex >= 0 && searchIndex < this.pens[i].VisiblePoints.Count)
                    {
                        Point ptBefore = visiblePoints[searchIndex];
                        if (searchIndex + 1 < visiblePoints.Count)
                        {
                            Point ptAfter = visiblePoints[searchIndex + 1];
                            double ratio = (dataPointX - ptBefore.X) / (ptAfter.X - ptBefore.X);
                            y = ptBefore.Y + (ptAfter.Y - ptBefore.Y) * ratio;
                            if (y != ptAfter.Y)
                                y2 = ptBefore.Y;
                        }
                    }

                    double yMin = Double.NaN;
                    double yMax = Double.NaN;
                    if (this.pens[i].ShowValueArea > 0 || this.pens[i].IsTagRef2Enabled)
                    {
                        double[] yMinMax = this.pens[i].GetYMinValues(this.CursorPosition.X);
                        yMin = yMinMax[0];
                        yMax = yMinMax[1];
                    }

                    if (this.CursorValueChanged != null)
                        this.CursorValueChanged(this, new GenericObjectEventArgs(this.pens[i], y, y2, yMin, yMax));
                }
            }
        }

        public void UpdateSecondaryCursor()
        {
            this.UpdateSecondaryCursor(null);
        }

        private void UpdateSecondaryCursor(LineGraph pen)
        {
            Rect output = this.ChartArea;
            if (output.Width <= 2 || output.Height <= 2)
                return;

            if (this.chart == null)
                return;

            if (!this.secondaryCursorClicked)
                return;

            this.UpdateSecondaryCursor(pen, this.SecondaryCursorPositionX, this.SecondaryCursorPositionY);
        }

        private void UpdateSecondaryCursor(LineGraph pen, double pointX, double pointY)
        {
            if (this.pens.Count > 0)
            {
                Point valuePoint = this.CalculateActualValuePointFromClickedCoodinates(pointX, pointY, 0);
                DateTime dt = this.primaryXAxis.JavaTimeStampToDateTime(this.xAxisMin, valuePoint.X);

                if (this.CursorOutputChanged2 != null)
                    this.CursorOutputChanged2(this, new GenericObjectEventArgs(dt, null, null));

                double dataPointX = dt.Ticks;
                dataPointX = dataPointX / 10000000000.0;
                for (int i = 0; i < this.pens.Count; i++)
                {
                    if (pen != null && pen != this.pens[i])
                        continue;

                    List<Point> visiblePoints = this.pens[i].VisiblePoints;
                    int searchIndex = this.SearchXBetween(dataPointX, visiblePoints);
                    double y = Double.NaN;
                    double y2 = Double.NaN;
                    if (searchIndex >= 0 && searchIndex < this.pens[i].VisiblePoints.Count)
                    {
                        Point ptBefore = visiblePoints[searchIndex];
                        if (searchIndex + 1 < visiblePoints.Count)
                        {
                            Point ptAfter = visiblePoints[searchIndex + 1];
                            double ratio = (dataPointX - ptBefore.X) / (ptAfter.X - ptBefore.X);
                            y = ptBefore.Y + (ptAfter.Y - ptBefore.Y) * ratio;
                            if (y != ptAfter.Y)
                                y2 = ptBefore.Y;
                        }
                    }

                    double yMin = Double.NaN;
                    double yMax = Double.NaN;
                    if (this.pens[i].ShowValueArea > 0 || this.pens[i].IsTagRef2Enabled)
                    {
                        double[] yMinMax = this.pens[i].GetYMinValues(this.SecondaryCursorPosition.X);
                        yMin = yMinMax[0];
                        yMax = yMinMax[1];
                    }
                    if (this.CursorValueChanged2 != null)
                        this.CursorValueChanged2(this, new GenericObjectEventArgs(this.pens[i], y, y2, yMin, yMax));
                }
            }
        }

        private int SearchXBetween(double x, IList<Point> collection)
        {
            if (collection == null || collection.Count == 0)
                return -1;

            int lastIndex = collection.Count - 1;

            if (x < collection[0].X)
                return -1;
            else if (collection[lastIndex].X < x)
                return -1;

            // searching ascending
            if (collection[0].X < x)
            {
                for (int i = 1; i <= lastIndex; i++)
                    if (collection[i].X >= x)
                        return i - 1;
            }
            else // searching descending
            {
                for (int i = -1; i >= 0; i--)
                    if (collection[i].X <= x)
                        return i;
            }

            return -1;
        }

        private string GetPropertyFromString(string property, string source)
        {
            string[] split = source.Split(';');

            foreach (string cursorProperty in split)
            {
                if (cursorProperty.StartsWith(property, StringComparison.InvariantCultureIgnoreCase))
                    return cursorProperty.Substring(property.Length + 1); // +1 to account for the equal '=' sign
            }

            return null;
        }

        public static double ConvertTrendPenToDisplayValue(TrendPen column, double value)
        {
            if (!ConfigHelper.IsDesignMode && !string.IsNullOrEmpty(T.Wpf.RunServices.ConfigHelper.ObjServer.DB.DictionaryUnits) && column != null && column.TagRef != null)
            {
                T.Kernel.Tags.TagObj tagObj = column.TagRef.MainRunObj as T.Kernel.Tags.TagObj;
                if (tagObj != null)
                    value = T.Wpf.RunServices.ConfigHelper.ObjServer.DB.GetDictionaryUnitsValue(tagObj.Units, value);
            }
            return value;
        }

        public double ConvertTrendPenFromDisplayValue(TrendPen column, double value)
        {
            if (!ConfigHelper.IsDesignMode && !string.IsNullOrEmpty(T.Wpf.RunServices.ConfigHelper.ObjServer.DB.DictionaryUnits) && column != null && column.TagRef != null)
            {
                T.Kernel.Tags.TagObj tagObj = column.TagRef.MainRunObj as T.Kernel.Tags.TagObj;
                if (tagObj != null)
                    value = T.Wpf.RunServices.ConfigHelper.ObjServer.DB.SetDictionaryUnitsValue(tagObj.Units, value);
            }
            return value;
        }

        public static double ConvertDrillingPenToDisplayValue(Base.DrillingPen column, double value)
        {
            if (!ConfigHelper.IsDesignMode && !string.IsNullOrEmpty(T.Wpf.RunServices.ConfigHelper.ObjServer.DB.DictionaryUnits) && column != null && column.TagRef != null)
            {
                T.Kernel.Tags.TagObj tagObj = column.TagRef[0].MainRunObj as T.Kernel.Tags.TagObj;
                if (tagObj != null)
                    value = T.Wpf.RunServices.ConfigHelper.ObjServer.DB.GetDictionaryUnitsValue(tagObj.Units, value);
            }
            return value;
        }

        public double ConvertDrillingFromDisplayValue(Base.DrillingPen column, double value)
        {
            if (!ConfigHelper.IsDesignMode && !string.IsNullOrEmpty(T.Wpf.RunServices.ConfigHelper.ObjServer.DB.DictionaryUnits) && column != null && column.TagRef != null)
            {
                T.Kernel.Tags.TagObj tagObj = column.TagRef[0].MainRunObj as T.Kernel.Tags.TagObj;
                if (tagObj != null)
                    value = T.Wpf.RunServices.ConfigHelper.ObjServer.DB.SetDictionaryUnitsValue(tagObj.Units, value);
            }
            return value;
        }

        private void UpdateZoomRect(Point zoomEndPoint)
        {
            if (zoomEndPoint.X < this.zoomStartPosition.X || zoomEndPoint.Y < this.zoomStartPosition.Y)
                return;

            //Rect tmpZoomRect;
            //if (this.Orientation == 0)
            //{
            //    tmpZoomRect = new Rect(this.zoomStartPosition, zoomEndPoint);
            //}
            //else
            //{
            //    Point p1 = this.zoomStartPosition;
            //    Point p2 = zoomEndPoint;
            //    tmpZoomRect = new Rect(new Point(p1.X, this.ChartPanel.Height - p2.Y), new Size(Math.Abs(p2.X - p1.X), Math.Abs(p2.Y - p1.Y)));
            //}

            //if (this.zoomRectangle == null)
            //{
            //    this.zoomRectangle = new Rectangle();
            //    this.zoomRectangle.Fill = Brushes.LightGray;
            //    this.zoomRectangle.Opacity = 0.5;
            //    this.zoomRectangle.Stroke = Brushes.Black;
            //    this.zoomRectangle.HorizontalAlignment = HorizontalAlignment.Left;
            //    this.zoomRectangle.VerticalAlignment = VerticalAlignment.Top;
            //}
            //else
            //{
            //    //this.ChartPanel.Children.Remove(this.zoomRectangle);
            //}

            //this.zoomRectangle.SetLeft(tmpZoomRect.Left);
            //this.zoomRectangle.SetTop(tmpZoomRect.Top);
            //this.zoomRectangle.Width = tmpZoomRect.Width;
            //this.zoomRectangle.Height = tmpZoomRect.Height;

            ////this.ChartPanel.Children.Add(this.zoomRectangle);
        }
    }
}