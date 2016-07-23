using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace _3DGimbal
{
    public partial class XAxis : UserControl
    {
        public XAxis()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 控件的宽度
        /// </summary>
        private int width;
        /// <summary>
        /// 控件的高度
        /// </summary>
        private int height;
        /// <summary>
        /// 缓冲
        /// </summary>
        private Graphics graph;
        /// <summary>
        /// 偏移
        /// </summary>
        private int xOffset = 0;
        /// <summary>
        /// 每次刷新在X轴移动的距离
        /// </summary>
        private int XDistance = 1;
        /// <summary>
        /// 网格的像素长度
        /// </summary>
        private int gridWidth = 10;
        /// <summary>
        /// 保存时间的字符串
        /// 长度为10
        /// </summary>
        private String[] timeStr;
        /// <summary>
        /// 定时器计数，计数50次
        /// </summary>
        private int collectTimeCount = 0;
        /// <summary>
        /// 采样间隔
        /// 定时器执行50次，X坐标轴的字符发生一次变化，就是长标记的位置
        /// </summary>
        private int collectTimeInterval = 50;
        /// <summary>
        /// 绘制X轴坐标的画笔
        /// </summary>
        private Pen xAxisPen = new Pen(Color.Black);
        /// <summary>
        /// 文本显示距离标记左边5个像素
        /// </summary>
        private int textSizeLeft = 5;

        protected override void OnLoad(EventArgs e)
        {
            //base.OnLoad(e);
            //打开双缓冲，防止闪烁
            DoubleBuffered = true;
            height = base.ClientSize.Height - 1;
            width = base.ClientSize.Width - 1;
            timeStr = new string[(int)width / gridWidth / 5]; //每五个网格出现一个字符串
        }

        protected override void OnResize(EventArgs e)
        {
            height = base.ClientSize.Height;
            width = base.ClientSize.Width;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            graph = e.Graphics;
            DrawXAxis(ref graph, xOffset);
        }
        /// <summary>
        /// 绘制X轴坐标
        /// </summary>
        /// <param name="g"></param>
        /// <param name="offset"></param>
        private void DrawXAxis(ref Graphics g, int offset)
        {
            float div;
            float pos = 0;
            int XStrPosition = 0;
            div = this.ClientSize.Width / 10 + 1;  //55
            g.DrawLine(xAxisPen, 0, 5, this.ClientSize.Width, 5);
            for (int i = 0; i < (int)div; i++)
            {
                pos += 10;
                if(i%5!=0)
                {
                    g.DrawLine(xAxisPen, pos - offset, 0, pos - offset, 5);
                }
                else
                {
                    g.DrawLine(new Pen(Color.Black, 2), pos - offset, 0, pos - offset, 5);
                    g.DrawString(timeStr[XStrPosition++], new Font("楷体GB-2312", 10, FontStyle.Regular), new SolidBrush(Color.Black), 10*5 + pos - offset - textSizeLeft, 7);
                    if (XStrPosition >= timeStr.Length)
                    {
                        XStrPosition = 0;
                    }
                }
            }
        }
        /// <summary>
        /// 刷新控件显示
        /// </summary>
        public void XAxisRefreshControl()
        {
            xOffset += XDistance;
            xOffset %= 50;
            int len = (int)width / gridWidth / 5;
            collectTimeCount++;
            if (collectTimeCount == collectTimeInterval)
            {
                collectTimeCount = 0;
                for (int i = 0; i < len; i++)
                {
                    if (i < len - 1)
                    {
                        timeStr[i] = timeStr[i + 1];
                    }
                    else
                    {
                        timeStr[len - 1] = DateTime.Now.Second.ToString();
                    }
                }
            }
            Invalidate();
        }
        


    }
}
