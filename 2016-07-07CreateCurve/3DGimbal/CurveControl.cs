using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace _3DGimbal
{
    public partial class CurveControl : UserControl
    {
        /// <summary>
        /// 波形的最大值
        /// </summary>
        private int range = 100;
        /// <summary>
        /// 控件能容纳的数组
        /// </summary>
        private int[] dataArray;
        /// <summary>
        /// 当前值
        /// </summary>
        private int currentValue = 0;
        /// <summary>
        /// 控件的宽度
        /// </summary>
        private int width;
        /// <summary>
        /// 控件的高度
        /// </summary>
        private int height;
        /// <summary>
        /// 每更新一次X轴的变化量
        /// </summary>
        private int XOffset = 0;
        /// <summary>
        /// 网格依次平移的间距
        /// </summary>
        private int gridShifttingIncrement = 1;
        /// <summary>
        /// 网格宽度
        /// </summary>
        private int gridWidth = 10;
        /// <summary>
        /// 网格高度
        /// </summary>
        private int gridHeight = 10;
        /// <summary>
        /// 绘制曲线的画笔
        /// </summary>
        private Pen penChart = new Pen(Color.Lime);
        /// <summary>
        /// 绘制网格的画笔
        /// </summary>
        private Pen penGrid = new Pen(Color.Green);
        /// <summary>
        /// 缓冲
        /// </summary>
        private Graphics graph;

        private void DrawGrids(ref Graphics g, int offset)
        {
            //网格数（不计边缘）
            float div;
            float pos = 0F;
            //先画垂直方向
            //可以少画一根线
            div = (float)width / (float)gridWidth + 1;
            for (int i = 0; i < (int)div; i++)
            {
                pos += gridWidth;
                g.DrawLine(penGrid, pos - offset, 0, pos - offset, height);
            }
            //画水平方向
            div = (float)height / (float)gridHeight;
            pos = 0F;
            for (int i = 0; i < (int)div; i++)
            {
                pos += gridHeight;
                g.DrawLine(penGrid, 0, pos, width, pos);
            }
        }

        private void DrawChart(ref Graphics g, Pen p, ref int[] val)
        {
            //从 0 到 width 绘制
            int len = width;
            len--;
            for (int i = 0; i < len; i++)
            {
                g.DrawLine(p, i, height - val[i], i + 1, height - val[i + 1]);
            }
            len++;
            g.DrawLine(p, len - 1, height - val[len - 2], len, height - val[len - 1]);
        }

        protected override void OnLoad(EventArgs e)
        {
            //base.OnLoad(e);
            //打开双缓冲，防止闪烁
            DoubleBuffered = true;
            height = base.ClientSize.Height;
            width = base.ClientSize.Width;
            dataArray = new int[width];
            //此处用定时器刷新
        }

        protected override void OnResize(EventArgs e)
        {
            height = base.ClientSize.Height;
            width = base.ClientSize.Width;
            Array.Resize(ref dataArray, width);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            graph = e.Graphics;
            DrawGrids(ref graph, XOffset);
            DrawChart(ref graph, penChart, ref dataArray);
        }

        public void CurveRefreshControl()
        {
            XOffset += gridShifttingIncrement;
            XOffset %= gridWidth;

            int len = width;
            for (int i = 0; i < len; i++)
            {
                //判断数组越界
                if (i < len - 1)
                {
                    dataArray[i] = dataArray[i + 1];
                }
                else
                {
                    dataArray[len - 1] = currentValue;
                }
            }
            //val[len] = currentValue;
            Invalidate();
        }

       
        [Category("内容"), Description("当前值。"), DefaultValue(0)]
        public int Value
        {
            get
            {
                return currentValue;
            }
            set
            {
                //约束 value
                if (value > range)
                {
                    value = range;
                }
                if (value < 0)
                {
                    value = 0;
                }
                //根据 Range 属性修正 value
                //尽量减小误差
                value = (int)((float)value / (float)range * (float)height);
                currentValue = value;
            }
        }
        [Browsable(true)]
        [Category("内容"), Description("数据值范围。绘图时将根据此值缩放 Value 值。"), DefaultValue(100)]
        public int Range
        {
            get
            {
                return range;
            }
            set
            {
                range = value;
            }
        }

        public CurveControl()
        {
            InitializeComponent();
        }


    }
}
