using System;
using System.Drawing;
using System.Windows.Forms;
using SerialControl;
using ZedGraph;
using System.Collections;
using System.Collections.Generic;

namespace _3DGimbal
{
    public partial class Main : Form
    {
        #region 全局变量
        /// <summary>
        /// 左侧连接按钮是否被点击
        /// </summary>
        private bool isConnectClick = false;      
        /// <summary>
        /// 左侧波形分析按钮是否被点击
        /// </summary>
        private bool isWaveClick = false;    
        /// <summary>
        /// 左侧波形发生器按钮是否被点击
        /// </summary>
        private bool isCreateWaveClick = false;     
        /// <summary>
        /// 左侧波形回放按钮是否被点击
        /// </summary>
        private bool isPlaybackWaveClick = false;   
        /// <summary>
        /// 左侧技术支持按钮是否被点击
        /// </summary>
        private bool isTechnicalSupportClick = false;       

        /// <summary>
        /// 鼠标移动位置变量
        /// </summary>
        private Point mouseOff;     
        /// <summary>
        /// 标签是否为左键
        /// </summary>
        private bool leftFlag;
                              
        /// <summary>
        /// 左侧梯波按钮是否被点击
        /// </summary>
        private bool isTiboClick = false;  
        /// <summary>
        /// 左侧正弦波按钮是否被点击
        /// </summary>
        private bool isSinWaveClick = false;   
        /// <summary>
        /// 左侧余弦波按钮是否被点击
        /// </summary>
        private bool isCosWaveClick = false;    
        /// <summary>
        /// 左侧线性波按钮是否被点击
        /// </summary>
        private bool isLineWaveClick = false;  
        /// <summary>
        /// 左侧数据库回放按钮是否被点击
        /// </summary>
        private bool isDatabasePlaybackClick = false;
        /// <summary>
        /// 左侧文本按钮是否被点击
        /// </summary>
        private bool isTxtPlaybackClick = false;  
        /// <summary>
        /// 左侧软件支持按钮是否被点击
        /// </summary>
        private bool isSoftwaveSupportClick = false;    
        /// <summary>
        /// 左侧硬件支持按钮是否被点击
        /// </summary>
        private bool isHardwareSupportClick = false;        

        /// <summary>
        /// 绘制曲线的背景面板
        /// </summary>
        private GraphPane myPane = null;  
        /// <summary>
        /// PWM波的曲线
        /// </summary> 
        private LineItem  myPWMCurve = null;            
        /// <summary>
        /// 转速的曲线
        /// </summary>
        private LineItem  myRemolutionCurve = null;   
        /// <summary>
        /// 重量的曲线
        /// </summary>
        private LineItem  myWeightCurve = null;   
        /// <summary>
        /// 升力的曲线
        /// </summary>
        private LineItem  myLiftCurve = null; 
        /// <summary>
        /// 电压的曲线
        /// </summary>
        private LineItem  myVoltageCurve = null;        
        /// <summary>
        /// 绘制PWM波的曲线点集合
        /// </summary>
        private PointPairList PWMCurveList = new PointPairList();        
        /// <summary>
        /// 绘制转速的曲线点集合
        /// </summary>
        private PointPairList RemolutionCurveList = new PointPairList();   
        /// <summary>
        /// 绘制重量的曲线点集合
        /// </summary>
        private PointPairList WeightCurveList = new PointPairList();   
        /// <summary>
        /// 绘制升力的曲线点集合
        /// </summary>
        private PointPairList LiftCurveList = new PointPairList();        
        /// <summary>
        /// 绘制电压的曲线点集合
        /// </summary>
        private PointPairList VoltageCurveList = new PointPairList();   
        /// <summary>
        /// 设置PWM波的占空比
        /// </summary>
        private float PWMSettingValue = 0.0f;                              

        /// <summary>
        /// PWM曲线多选框是否被选中
        /// </summary>
        private bool isCbPWMCurveChecked = false;      
        /// <summary>
        /// PWM曲线多选框是否被选中
        /// </summary>
        private bool isCbRevolutionCurveChecked = false;    
        /// <summary>
        /// PWM曲线多选框是否被选中
        /// </summary>
        private bool isCbWeightCurveChecked = false;
        /// <summary>
        /// PWM曲线多选框是否被选中
        /// </summary>
        private bool isCbLiftCurveChecked = false;  
        /// <summary>
        /// PWM曲线多选框是否被选中
        /// </summary>
        private bool isCbVoltageCurveChecked = false;                       

        /// <summary>
        /// 串口
        /// </summary>
        private SerialCom serialCom;                                        


        #endregion

        public Main()
        {
            InitializeComponent();
            serialCom = new SerialCom();
            myPane = zedGraphControl1.GraphPane;
            serialCom.Visible = false;
            secondCreateWavePanel.Visible = false;
            secondTechSupportPanel.Visible = false;
            secondPlaybackPanel.Visible = false;

            initGraphPane(myPane);            
        }

        #region 顶部鼠标左键拖动程序位置
        /// <summary>
        /// 鼠标在正上方区域按下的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void centerTabPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseOff = new Point(-e.X, -e.Y); //得到变量的值
                leftFlag = true;                  //点击左键按下时标注为true;
            }
        }
        /// <summary>
        /// 鼠标在正上方区域移动的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void centerTabPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                Point mouseSet = Control.MousePosition;
                mouseSet.Offset(mouseOff.X - leftTabPanel.Size.Width, mouseOff.Y);  //设置移动后的位置
                Location = mouseSet;
            }
        }
        /// <summary>
        /// 鼠标在正上方区域取消按下的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void centerTabPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                leftFlag = false;//释放鼠标后标注为false;
            }
        }
        #endregion

        #region 设置鼠标进入item的背景颜色和点击事件
        /// <summary>
        /// 鼠标进入连接无人机控件的时候，设置字体的背景颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectLabelText_MouseEnter(object sender, EventArgs e)
        {
            connectLabelText.ForeColor = setMouseInLabelTextColor();
        }
        /// <summary>
        /// 鼠标离开连接无人机控件的时候，设置字体的背景颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectLabelText_MouseLeave(object sender, EventArgs e)
        {
            connectLabelText.ForeColor = setMouseInLabelTextColor();
        }
        /// <summary>
        /// 鼠标进入连接无人机中右边的图片区域时，设置字体的背景颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectPicBox_MouseEnter(object sender, EventArgs e)
        {
            connectLabelText.ForeColor = setMouseInLabelTextColor();
        }
        /// <summary>
        /// 鼠标离开连接无人机中右边的图片区域时，设置字体的背景颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectPicBox_MouseLeave(object sender, EventArgs e)
        {
            connectLabelText.ForeColor = setMouseInLabelTextColor();
        }
        /// <summary>
        /// 鼠标点击连接无人机右边的图片时，把监听事件传递给其父类面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectPicBox_Click(object sender, EventArgs e)
        {
            connectTabPanel_Click(sender, e);
        }
        /// <summary>
        /// 鼠标点击连接无人机左边的字体时，把监听事件传递给其父类面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectBtnText_Click(object sender, EventArgs e)
        {
            connectTabPanel_Click(sender,e);
        }
        /// <summary>
        /// 鼠标点击连接无人机,对其面板的位置和内容进行定位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectTabPanel_Click(object sender, EventArgs e)
        {
            connectLabelText.ForeColor = setMouseClickLabelTextColor();
            connect_Click_Position();
        }

        private void waveLabelText_Click(object sender, EventArgs e)
        {
            waveTabPanel_Click(sender, e);
        }

        private void wavePicBox_Click(object sender, EventArgs e)
        {
            waveTabPanel_Click(sender, e);
        }

        private void waveTabPanel_Click(object sender, EventArgs e)
        {
            waveLabelText.ForeColor = setMouseClickLabelTextColor();
            wave_Click_Position();
        }

        private void connectTabPanel_MouseEnter(object sender, EventArgs e)
        {
            connectLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void waveTabPanel_MouseEnter(object sender, EventArgs e)
        {
            waveLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void connectTabPanel_MouseLeave(object sender, EventArgs e)
        {
            connectLabelText.ForeColor = setMouseDefaultLabelTextColor();
        }        

        private void waveTabPanel_MouseLeave(object sender, EventArgs e)
        {
            waveLabelText.ForeColor = setMouseDefaultLabelTextColor();
        }

        private void waveLabelText_MouseEnter(object sender, EventArgs e)
        {
            waveLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void waveLabelText_MouseLeave(object sender, EventArgs e)
        {
            waveLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void wavePicBox_MouseEnter(object sender, EventArgs e)
        {
            waveLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void wavePicBox_MouseLeave(object sender, EventArgs e)
        {
            waveLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void exitLabelText_Click(object sender, EventArgs e)
        {
            exitTabPanel_Click(sender, e);
        }

        private void createWaveTabPanel_MouseEnter(object sender, EventArgs e)
        {
            createWaveLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void createWaveTabPanel_MouseLeave(object sender, EventArgs e)
        {
            createWaveLabelText.ForeColor = setMouseDefaultLabelTextColor();
        }

        private void createWaveLabelText_MouseEnter(object sender, EventArgs e)
        {
            createWaveLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void createWaveLabelText_MouseLeave(object sender, EventArgs e)
        {
            createWaveLabelText.ForeColor = setMouseDefaultLabelTextColor();
        }

        private void createWavePicBox_MouseEnter(object sender, EventArgs e)
        {
            createWaveLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void createWavePicBox_MouseLeave(object sender, EventArgs e)
        {
            createWaveLabelText.ForeColor = setMouseDefaultLabelTextColor();
        }

        private void playbackWaveTabPanel_MouseEnter(object sender, EventArgs e)
        {
            playbackWaveLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void playbackWaveTabPanel_MouseLeave(object sender, EventArgs e)
        {
            playbackWaveLabelText.ForeColor = setMouseDefaultLabelTextColor();
        }

        private void playbackWaveLabelText_MouseEnter(object sender, EventArgs e)
        {
            playbackWaveLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void playbackWaveLabelText_MouseLeave(object sender, EventArgs e)
        {
            playbackWaveLabelText.ForeColor = setMouseDefaultLabelTextColor();
        }

        private void playbackWavePicBox_MouseEnter(object sender, EventArgs e)
        {
            playbackWaveLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void playbackWavePicBox_MouseLeave(object sender, EventArgs e)
        {
            playbackWaveLabelText.ForeColor = setMouseDefaultLabelTextColor();
        }

        private void technicalSupportTabPanel_MouseEnter(object sender, EventArgs e)
        {
            technicalSupportLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void technicalSupportTabPanel_MouseLeave(object sender, EventArgs e)
        {
            technicalSupportLabelText.ForeColor = setMouseDefaultLabelTextColor();
        }

        private void technicalSupportLabelText_MouseEnter(object sender, EventArgs e)
        {
            technicalSupportLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void technicalSupportLabelText_MouseLeave(object sender, EventArgs e)
        {
            technicalSupportLabelText.ForeColor = setMouseDefaultLabelTextColor();
        }

        private void technicalSupportPicBox_MouseEnter(object sender, EventArgs e)
        {
            technicalSupportLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void technicalSupportPicBox_MouseLeave(object sender, EventArgs e)
        {
            technicalSupportLabelText.ForeColor = setMouseDefaultLabelTextColor();
        }

        private void exitLabelText_MouseEnter(object sender, EventArgs e)
        {
            exitLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void exitLabelText_MouseLeave(object sender, EventArgs e)
        {
            exitLabelText.ForeColor = setMouseDefaultLabelTextColor();
        }

        private void exitTabPanel_MouseEnter(object sender, EventArgs e)
        {
            exitLabelText.ForeColor = setMouseInLabelTextColor();
        }

        private void exitTabPanel_MouseLeave(object sender, EventArgs e)
        {
            exitLabelText.ForeColor = setMouseDefaultLabelTextColor();
        }

        private void playbackWaveLabelText_Click(object sender, EventArgs e)
        {
            playbackWaveTabPanel_Click(sender, e);
        }

        private void playbackWavePicBox_Click(object sender, EventArgs e)
        {
            playbackWaveTabPanel_Click(sender, e);
        }

        private void playbackWaveTabPanel_Click(object sender, EventArgs e)
        {
            playbackWaveLabelText.ForeColor = setMouseClickLabelTextColor();
            playbackWave_Click_Position();
        }

        private void technicalSupportLabelText_Click(object sender, EventArgs e)
        {
            technicalSupportTabPanel_Click(sender, e);
        }

        private void technicalSupportPicBox_Click(object sender, EventArgs e)
        {
            technicalSupportTabPanel_Click(sender, e);
        }

        private void technicalSupportTabPanel_Click(object sender, EventArgs e)
        {
            technicalSupportLabelText.ForeColor = setMouseClickLabelTextColor();
            technicalSupport_Click_position();
        }

        private void createWaveLabelText_Click(object sender, EventArgs e)
        {
            createWaveTabPanel_Click(sender, e);
        }

        private void createWavePicBox_Click(object sender, EventArgs e)
        {
            createWaveTabPanel_Click(sender, e);
        }

        private void createWaveTabPanel_Click(object sender, EventArgs e)
        {
            createWaveLabelText.ForeColor = setMouseClickLabelTextColor();
            createWave_Click_Position();
        }
        #endregion

        #region 左侧菜单的界面逻辑
        /// <summary>
        /// 点击了连接按钮时，显示连接的控件
        /// </summary>
        private void connect_Click_Position() 
        {
            if(isWaveClick)
            {
                closeWavePanel();
                isWaveClick = !isWaveClick;
            }
            if(isCreateWaveClick)
            {
                closeCreateWavePanel();
                isCreateWaveClick = !isCreateWaveClick;
            }
            if(isPlaybackWaveClick)
            {
                closePlaybackPanel();
                isPlaybackWaveClick = !isPlaybackWaveClick;
            }
            if(isTechnicalSupportClick)
            {
                closeTechnicalSupportPanel();
                isTechnicalSupportClick = !isTechnicalSupportClick;
            }
            isConnectClick = !isConnectClick;
            if (isConnectClick)
            {
                openConnectPanel();
            }
            else 
            {
                closeConnectPanel();
            }
        }
        /// <summary>
        /// 连接面板打开时，对界面的布局
        /// </summary>
        private void openConnectPanel()
        {
            int connectTabPanelSizeX = connectTabPanel.Location.X;
            connectTabPanel.Size = new System.Drawing.Size(connectTabPanelSizeX, serialCom.Size.Height + connectTabPanel.Size.Height);
            serialCom.Parent = connectTabPanel;
            serialCom.Location = new Point(connectTabPanel.Margin.Left, getLeftItemPanelHeight());
            serialCom.Visible = true;
            Image img = connectPicBox.Image;
            img.RotateFlip(RotateFlipType.Rotate90FlipNone);
            connectPicBox.Image = img;
        }
        /// <summary>
        /// 关闭连接面板，界面的布局发生相应的变化
        /// </summary>
        private void closeConnectPanel()
        {
            int connectTabPanelSizeX = connectTabPanel.Location.X;
            connectTabPanel.Size = new System.Drawing.Size(connectTabPanelSizeX, getLeftItemPanelHeight());

            Image img = connectPicBox.Image;
            img.RotateFlip(RotateFlipType.Rotate270FlipNone);
            connectPicBox.Image = img;
        }
        /// <summary>
        /// 点击了波形分析按钮时，显示分析的控件
        /// </summary>
        private void wave_Click_Position()
        {
            if (isConnectClick)
            {
                closeConnectPanel();
                isConnectClick = !isConnectClick;
            }
            if (isCreateWaveClick)
            {
                closeCreateWavePanel();
                isCreateWaveClick = !isCreateWaveClick;
            }
            if (isPlaybackWaveClick)
            {
                closePlaybackPanel();
                isPlaybackWaveClick = !isPlaybackWaveClick;
            }
            if (isTechnicalSupportClick)
            {
                closeTechnicalSupportPanel();
                isTechnicalSupportClick = !isTechnicalSupportClick;
            }
            isWaveClick = !isWaveClick;
            if (isWaveClick)
            {
                openWavePanel();
            }
            else
            {
                closeWavePanel();
            }
        }
        /// <summary>
        /// 打开波形分析面板时的界面布局
        /// </summary>
        private void openWavePanel()
        {
            int waveTabPanelSizeX = waveTabPanel.Location.X;
            waveTabPanel.Size = new System.Drawing.Size(waveTabPanelSizeX, getLeftWavePanelClickedHeight());
            Image img = wavePicBox.Image;
            img.RotateFlip(RotateFlipType.Rotate90FlipNone);
            wavePicBox.Image = img;
        }
        /// <summary>
        /// 关闭波形分析面板时界面布局
        /// </summary>
        private void closeWavePanel()
        {
            int waveTabPanelSizeX = waveTabPanel.Location.X;
            waveTabPanel.Size = new System.Drawing.Size(waveTabPanelSizeX, getLeftItemPanelHeight());
            Image img = wavePicBox.Image;
            img.RotateFlip(RotateFlipType.Rotate270FlipNone);
            wavePicBox.Image = img;
        }
        /// <summary>
        /// 点击了波形产生器按钮时，显示产生的控件
        /// </summary>
        private void createWave_Click_Position()
        {
            if (isConnectClick)
            {
                closeConnectPanel();
                isConnectClick = !isConnectClick;
            }
            if (isWaveClick)
            {
                closeWavePanel();
                isWaveClick = !isWaveClick;
            }
            if (isPlaybackWaveClick)
            {
                closePlaybackPanel();
                isPlaybackWaveClick = !isPlaybackWaveClick;
            }
            if (isTechnicalSupportClick)
            {
                closeTechnicalSupportPanel();
                isTechnicalSupportClick = !isTechnicalSupportClick;
            }
            isCreateWaveClick = !isCreateWaveClick;
            if (isCreateWaveClick)
            {
                openCreateWavePanel();
            }
            else
            {
                closeCreateWavePanel();
            }
        }
        /// <summary>
        /// 波形产生器面板打开时的界面布局
        /// </summary>
        private void openCreateWavePanel()
        {
            int createWaveTabPanelSizeX = createWaveTabPanel.Location.X;
            createWaveTabPanel.Size = new System.Drawing.Size(createWaveTabPanelSizeX, getLeftCreateWavePanelClickedHeight());
            Image img = createWavePicBox.Image;
            img.RotateFlip(RotateFlipType.Rotate90FlipNone);
            createWavePicBox.Image = img;
            secondCreateWavePanel.Parent = createWaveTabPanel;
            secondCreateWavePanel.Location = new System.Drawing.Point(3, getLeftItemPanelHeight());//位置
            secondCreateWavePanel.Visible = true;
        }
        /// <summary>
        /// 波形产生器面板关闭时界面布局
        /// </summary>
        private void closeCreateWavePanel()
        {
            int createWaveTabPanelSizeX = createWaveTabPanel.Location.X;
            createWaveTabPanel.Size = new System.Drawing.Size(createWaveTabPanelSizeX, getLeftItemPanelHeight());

            Image img = createWavePicBox.Image;
            img.RotateFlip(RotateFlipType.Rotate270FlipNone);
            createWavePicBox.Image = img;
        }
        /// <summary>
        /// 点击了波形回放按钮时，显示回放的控件
        /// </summary>
        private void playbackWave_Click_Position()
        {
            if (isConnectClick)
            {
                closeConnectPanel();
                isConnectClick = !isConnectClick;
            }
            if (isCreateWaveClick)
            {
                closeCreateWavePanel();
                isCreateWaveClick = !isCreateWaveClick;
            }
            if (isWaveClick)
            {
                closeWavePanel();
                isWaveClick = !isWaveClick;
            }
            if (isTechnicalSupportClick)
            {
                closeTechnicalSupportPanel();
                isTechnicalSupportClick = !isTechnicalSupportClick;
            }
            isPlaybackWaveClick = !isPlaybackWaveClick;
            if (isPlaybackWaveClick)
            {
                openPlaybackPanel();
            }
            else
            {
                closePlaybackPanel();
            }
        }
        /// <summary>
        /// 打开波形回放面板的界面布局
        /// </summary>
        private void openPlaybackPanel()
        {
            int playbackWaveTabPanelSizeX = playbackWaveTabPanel.Location.X;
            playbackWaveTabPanel.Size = new System.Drawing.Size(playbackWaveTabPanelSizeX, getLeftPlaybackPanelClickedHeight());
            Image img = playbackWavePicBox.Image;
            img.RotateFlip(RotateFlipType.Rotate90FlipNone);
            playbackWavePicBox.Image = img; 

            secondPlaybackPanel.Parent = playbackWaveTabPanel;
            secondPlaybackPanel.Location = new System.Drawing.Point(3, getLeftItemPanelHeight());//位置
            secondPlaybackPanel.Visible = true;

        }
        /// <summary>
        /// 关闭波形回放面板的界面布局
        /// </summary>
        private void closePlaybackPanel()
        {
            int playbackWaveTabPanelSizeX = playbackWaveTabPanel.Location.X;
            playbackWaveTabPanel.Size = new System.Drawing.Size(playbackWaveTabPanelSizeX, getLeftItemPanelHeight());

            Image img = playbackWavePicBox.Image;
            img.RotateFlip(RotateFlipType.Rotate270FlipNone);
            playbackWavePicBox.Image = img;
        }
        /// <summary>
        /// 点击了技术支持按钮时，显示支持的控件
        /// </summary>
        private void technicalSupport_Click_position()
        {
            if (isConnectClick)
            {
                closeConnectPanel();
                isConnectClick = !isConnectClick;
            }
            if (isCreateWaveClick)
            {
                closeCreateWavePanel();
                isCreateWaveClick = !isCreateWaveClick;
            }
            if (isPlaybackWaveClick)
            {
                closePlaybackPanel();
                isPlaybackWaveClick = !isPlaybackWaveClick;
            }
            if (isWaveClick)
            {
                closeWavePanel();
                isWaveClick = !isWaveClick;
            }
            isTechnicalSupportClick = !isTechnicalSupportClick;
            if (isTechnicalSupportClick)
            {
                openTechnicalSupportPanel();
            }
            else
            {
                closeTechnicalSupportPanel();
            }
        }
        /// <summary>
        /// 打开技术支持的面板时的界面布局
        /// </summary>
        private void openTechnicalSupportPanel()
        {
            int technicalSupportTabPanelSizeX = technicalSupportTabPanel.Location.X;
            technicalSupportTabPanel.Size = new System.Drawing.Size(technicalSupportTabPanelSizeX, getLeftTechnicalSupportPanelClickedHeight());
            Image img = technicalSupportPicBox.Image;
            img.RotateFlip(RotateFlipType.Rotate90FlipNone);
            technicalSupportPicBox.Image = img;
            secondTechSupportPanel.Parent = technicalSupportTabPanel;
            secondTechSupportPanel.Location = new System.Drawing.Point(3, getLeftItemPanelHeight());//位置
            secondTechSupportPanel.Visible = true;
        }
        /// <summary>
        /// 关闭技术支持的界面布局
        /// </summary>
        private void closeTechnicalSupportPanel()
        {
            int technicalSupportTabPanelSizeX = technicalSupportTabPanel.Location.X;
            technicalSupportTabPanel.Size = new System.Drawing.Size(technicalSupportTabPanelSizeX, getLeftItemPanelHeight());

            Image img = technicalSupportPicBox.Image;
            img.RotateFlip(RotateFlipType.Rotate270FlipNone);
            technicalSupportPicBox.Image = img;
        }
        #endregion

        #region 左侧子菜单界面颜色逻辑
        /// <summary>
        /// 梯形波菜单界面的颜色逻辑
        /// </summary>
        private void TiBoColorLogic()
        {

            isTiboClick = true;
            if (isSinWaveClick)
            {
                isSinWaveClick = !isSinWaveClick;
                SinWaveLabel.BackColor = Color.FromArgb(255, 51, 63, 75);
            }
            if (isCosWaveClick)
            {
                isCosWaveClick = !isCosWaveClick;
                CosWaveLabel.BackColor = Color.FromArgb(255, 51, 63, 75);
            }
            if (isLineWaveClick)
            {
                isLineWaveClick = !isLineWaveClick;
                LineWaveLabel.BackColor = Color.FromArgb(255, 51, 63, 75);
            }

            TiBoWaveLabel.BackColor = Color.White;
        }
        /// <summary>
        /// 正弦波的颜色逻辑
        /// </summary>
        private void SinWaveColorLogic()
        {
            isSinWaveClick = true;
            if (isTiboClick)
            {
                isTiboClick = !isTiboClick;
                TiBoWaveLabel.BackColor = Color.FromArgb(255, 51, 63, 75);
            }
            if (isCosWaveClick)
            {
                isCosWaveClick = !isCosWaveClick;
                CosWaveLabel.BackColor = Color.FromArgb(255, 51, 63, 75);
            }
            if (isLineWaveClick)
            {
                isLineWaveClick = !isLineWaveClick;
                LineWaveLabel.BackColor = Color.FromArgb(255, 51, 63, 75);
            }

            SinWaveLabel.BackColor = Color.White;
        }
        /// <summary>
        /// 余弦波的颜色逻辑
        /// </summary>
        private void CosWaveColorLogic()
        {
            isCosWaveClick = true;
            if (isTiboClick)
            {
                isTiboClick = !isTiboClick;
                TiBoWaveLabel.BackColor = Color.FromArgb(255, 51, 63, 75);
            }
            if (isSinWaveClick)
            {
                isSinWaveClick = !isSinWaveClick;
                SinWaveLabel.BackColor = Color.FromArgb(255, 51, 63, 75);
            }
            if (isLineWaveClick)
            {
                isLineWaveClick = !isLineWaveClick;
                LineWaveLabel.BackColor = Color.FromArgb(255, 51, 63, 75);
            }

            CosWaveLabel.BackColor = Color.White;
        }
        /// <summary>
        /// 线性波的颜色逻辑
        /// </summary>
        private void LineWaveColorLogic()
        {
            isLineWaveClick = true;
            if (isTiboClick)
            {
                isTiboClick = !isTiboClick;
                TiBoWaveLabel.BackColor = Color.FromArgb(255, 51, 63, 75);
            }
            if (isSinWaveClick)
            {
                isSinWaveClick = !isSinWaveClick;
                SinWaveLabel.BackColor = Color.FromArgb(255, 51, 63, 75);
            }
            if (isCosWaveClick)
            {
                isCosWaveClick = !isCosWaveClick;
                CosWaveLabel.BackColor = Color.FromArgb(255, 51, 63, 75);
            }

            LineWaveLabel.BackColor = Color.White;
        }
        /// <summary>
        /// 数据库回放的颜色逻辑
        /// </summary>
        private void DatabasePlaybackColorLogic()
        {
            isDatabasePlaybackClick = true;
            if (isTxtPlaybackClick)
            {
                isTxtPlaybackClick = !isTxtPlaybackClick;
                TxtPlayback.BackColor = Color.FromArgb(255, 51, 63, 75);
            }

            DatabasePlayback.BackColor = Color.White;
        }
        /// <summary>
        /// 文本回放的颜色逻辑
        /// </summary>
        private void TxtPlaybackColorLogic()
        {
            isTxtPlaybackClick = true;
            if (isDatabasePlaybackClick)
            {
                isDatabasePlaybackClick = !isDatabasePlaybackClick;
                DatabasePlayback.BackColor = Color.FromArgb(255, 51, 63, 75);
            }

            TxtPlayback.BackColor = Color.White;
        }
        /// <summary>
        /// 软件支持的颜色逻辑
        /// </summary>
        private void softwaveSupportColorLogic()
        {
            isSoftwaveSupportClick = true;
            if (isHardwareSupportClick)
            {
                isHardwareSupportClick = !isHardwareSupportClick;
                hardwareSupport.BackColor = Color.FromArgb(255, 51, 63, 75);
            }

            softwaveSupport.BackColor = Color.White;
        }
        /// <summary>
        /// 硬件支持的颜色逻辑
        /// </summary>
        private void hardwareSupportColorLogic()
        {
            isHardwareSupportClick = true;
            if (isSoftwaveSupportClick)
            {
                isSoftwaveSupportClick = !isSoftwaveSupportClick;
                softwaveSupport.BackColor = Color.FromArgb(255, 51, 63, 75);
            }

            hardwareSupport.BackColor = Color.White;
        }
        #endregion

        #region 左侧菜单属性值设置
        /// <summary>
        /// 退出面板被点击时的相应函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitTabPanel_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }
        /// <summary>
        /// 设置鼠标进入面板时字体的颜色
        /// </summary>
        /// <returns></returns>
        private Color setMouseInLabelTextColor()
        {
            return Color.Yellow;
        }
        /// <summary>
        /// 设置左侧菜单字体面板默认的字体颜色
        /// </summary>
        /// <returns></returns>
        private Color setMouseDefaultLabelTextColor()
        {
            return SystemColors.ActiveCaption;
        }
        /// <summary>
        /// 设置鼠标点击面板时字体的颜色
        /// </summary>
        /// <returns></returns>
        private Color setMouseClickLabelTextColor()
        {
            return Color.Pink;
        }
        /// <summary>
        /// 设置左边菜单每一行面板的高度
        /// </summary>
        /// <returns></returns>
        private int getLeftItemPanelHeight()
        {
            return 41;
        }
        /// <summary>
        /// 设置左边波形分析被打开时面板的高度
        /// </summary>
        /// <returns></returns>
        private int getLeftWavePanelClickedHeight()
        {
            return 80;
        }
        /// <summary>
        /// 设置左边波形产生器被打开时面板的高度
        /// </summary>
        /// <returns></returns>
        private int getLeftCreateWavePanelClickedHeight()
        {
            return 125;
        }
        /// <summary>
        /// 设置左边回放面板被打开的高度
        /// </summary>
        /// <returns></returns>
        private int getLeftPlaybackPanelClickedHeight()
        {
            return 80;
        }
        /// <summary>
        /// 设置左边技术支持被打开时面板的高度
        /// </summary>
        /// <returns></returns>
        private int getLeftTechnicalSupportPanelClickedHeight()
        {
            return 80;
        }
        #endregion

        #region 左侧菜单点击事件
        /// <summary>
        /// 梯形波被点击时的相应函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TiBoWaveLabel_Click(object sender, EventArgs e)
        {
            TiBoColorLogic();

        }
        /// <summary>
        /// 正弦波被点击时的相应函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SinWaveLabel_Click(object sender, EventArgs e)
        {
            SinWaveColorLogic();

        }
        /// <summary>
        /// 余弦波被点击时的相应函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CosWaveLabel_Click(object sender, EventArgs e)
        {
            CosWaveColorLogic();

        }
        /// <summary>
        /// 线性波被点击时的响应函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LineWaveLabel_Click(object sender, EventArgs e)
        {
            LineWaveColorLogic();

        }
        /// <summary>
        /// 数据库回放点击时的响应函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DatabasePlayback_Click(object sender, EventArgs e)
        {
            DatabasePlaybackColorLogic();

        }
        /// <summary>
        /// 文本回放点击时的响应函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxtPlayback_Click(object sender, EventArgs e)
        {
            TxtPlaybackColorLogic();

        }
        /// <summary>
        /// 软件支持被点击时的响应函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void softwaveSupport_Click(object sender, EventArgs e)
        {
            softwaveSupportColorLogic();

        }
        /// <summary>
        /// 硬件支持被点击时的响应函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hardwareSupport_Click(object sender, EventArgs e)
        {
            hardwareSupportColorLogic();

        }
        #endregion

        
        private float[] portData = new float[100];  //从串口中读取100条数据的存储队列，用于存储重量
        private int countAve = 0;
        private bool isCalibration = true;  //是否进行标定，每次开机启动标定一次
        private float calibrationValue = 0.0f;  //标定的值，用于标定重物的重量

        private float xAxis = 0.0f;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (serialCom.CheckFrame())
            {
                xAxis += 1.0f;   //换成时间的变化
                switch (serialCom.rxFrame.fnCode)
                {
                    case FunctionCode.FC_GET_WEIGHT:
                        if (isCalibration)
                        {
                            labelCalibrationStatus.Text = "正在标定";
                            CalibrationWeight();
                        }
                        else
                        {
                            labelCalibrationStatus.Text = "标定成功";
                            if (isCbWeightCurveChecked)
                            {
                                WeightCurveList.Add(xAxis, ((calibrationValue - BitConverter.ToSingle(serialCom.rxFrame.data, 0)) / 213.0605137));
                            }
                        }
                        break;
                    case FunctionCode.FC_MOTOR_CUR_VEL:
                        float speed = BitConverter.ToSingle(serialCom.rxFrame.data, 0);
                        if(isCbRevolutionCurveChecked)
                        {
                            RemolutionCurveList.Add(xAxis, speed);
                        }
                        break;
                    case FunctionCode.FC_GET_VOLTAGE:
                        float voltage = BitConverter.ToSingle(serialCom.rxFrame.data, 0);
                        if(isCbVoltageCurveChecked)
                        {
                            VoltageCurveList.Add(xAxis, voltage);
                        }
                        break;
                }
                zedGraphControl1ChangeAxis();
            }
        }

        private void zedGraphControl1ChangeAxis()
        {
            myPane.XAxis.Scale.Min = xAxis - 50;
            myPane.XAxis.Scale.Max = xAxis;
            myPane.XAxis.Scale.MinorStep = 1;//X轴小步长1,也就是小间隔
            myPane.XAxis.Scale.MajorStep = 5;//X轴大步长为5，也就是显示文字的大间隔
            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Refresh();  //更新界面
        }

        /// <summary>
        /// 标定重量，每次运行程序都会用100组数据标定重量
        /// </summary>
        private void CalibrationWeight()
        {
            if (countAve < 100)
            {
                portData[countAve] = BitConverter.ToSingle(serialCom.rxFrame.data, 0);
                countAve++;
            }
            else
            {
                countAve = 0;
                isCalibration = false; //此处只进来一次，用于标定秤
                float a = 0;
                for (int j = 0; j < 100; j++)
                {
                    a += portData[j];
                }
                calibrationValue = a / 100f;
            }
        }



        private void initGraphPane(GraphPane myPane)
        {
            Chart myChart = myPane.Chart;
            myChart.Fill = new Fill(Color.DarkGray);
            myPane.Fill = new Fill(Color.DarkGray);
            //this.zedGraphControl1.GraphPane.Chart.Fill = new Fill(Color.Transparent, Color.Transparent, 45.0f);
            //this.zedGraphControl1.MasterPane.Fill = new Fill(Color.Transparent, Color.Transparent, 45.0f);
            //this.zedGraphControl1.GraphPane.Fill.Color = Color.Transparent;  
            myPane.Title.Text = "波形分析";
            myPane.XAxis.Title.Text = "时间";
            myPane.YAxis.Title.Text = "数值";
            myPWMCurve = myPane.AddCurve("PWM", PWMCurveList, setPWMCurveColor(), SymbolType.None);
            myRemolutionCurve = myPane.AddCurve("转速", RemolutionCurveList, setRemolutionCurveColor(), SymbolType.None);
            myWeightCurve = myPane.AddCurve("重量", WeightCurveList, setWeightCurveColor(), SymbolType.None);
            myLiftCurve = myPane.AddCurve("升力", LiftCurveList, setLiftCurveColor(), SymbolType.None);
            myVoltageCurve = myPane.AddCurve("电压", VoltageCurveList, setVoltageCurveColor(), SymbolType.None);
        }

        private Color setPWMCurveColor()
        {
            return Color.Red;
        }
        private Color setRemolutionCurveColor()
        {
            return Color.Lime;
        }
        private Color setWeightCurveColor()
        {
            return Color.Black;
        }
        private Color setLiftCurveColor()
        {
            return Color.Orange;
        }
        private Color setVoltageCurveColor()
        {
            return Color.DarkOrchid;
        }

        private void cbPWMCurve_CheckedChanged(object sender, EventArgs e)
        {
            if (cbPWMCurve.CheckState == CheckState.Checked) //选中状态
            {
                isCbPWMCurveChecked = true;
            }
            else  //未选中状态
            {
                isCbPWMCurveChecked = false;
                PWMCurveList.Clear();
            }
        }

        private void cbRevolutionCurve_CheckedChanged(object sender, EventArgs e)
        {
            if (cbRevolutionCurve.CheckState == CheckState.Checked) //选中状态
            {
                isCbRevolutionCurveChecked = true;
            }
            else  //未选中状态
            {
                isCbRevolutionCurveChecked = false;
                RemolutionCurveList.Clear();
            }
        }

        private void cbWeightCurve_CheckedChanged(object sender, EventArgs e)
        {
            if (cbWeightCurve.CheckState == CheckState.Checked) //选中状态
            {
                isCbWeightCurveChecked = true;
            }
            else  //未选中状态
            {
                isCbWeightCurveChecked = false;
                WeightCurveList.Clear();
            }
        }

        private void cbLiftCurve_CheckedChanged(object sender, EventArgs e)
        {
            if (cbLiftCurve.CheckState == CheckState.Checked) //选中状态
            {
                isCbLiftCurveChecked = true;
            }
            else  //未选中状态
            {
                isCbLiftCurveChecked = false;
                LiftCurveList.Clear();
            }
        }

        private void cbVoltageCurve_CheckedChanged(object sender, EventArgs e)
        {
            if (cbVoltageCurve.CheckState == CheckState.Checked) //选中状态
            {
                isCbVoltageCurveChecked = true;
            }
            else  //未选中状态
            {
                isCbVoltageCurveChecked = false;
                VoltageCurveList.Clear();
            }
        }

        private void trackBarPWMSetting_ValueChanged(object sender, EventArgs e)
        {
            PWMSettingValue = (float)trackBarPWMSetting.Value/100.0f;
            textBoxPWMSetting.Text = PWMSettingValue.ToString();
        }

        private void btnSettingPWM_Click(object sender, EventArgs e)
        {
            serialCom.txFrame.fnCode = FunctionCode.FC_MOTOR_SET_VEL;
            BitConverter.GetBytes((float)trackBarPWMSetting.Value / 100.0f).CopyTo(serialCom.txFrame.data, 0);
            serialCom.txFrame.isUpdated = true;
            serialCom.SendFrame();
        }

    }
}
