using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EposCmd.Net;
using System.IO;
using EposCmd.Net.DeviceCmdSet.Operation;
using System.Runtime.InteropServices;
using System.Windows.Input;


namespace APDSoftCont
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        private static extern int GetAsynKeyState(Keys vKey);

        DeviceManager connector;
        Device epos;
        StateMachine sm;

        public Form1()
        {
            InitializeComponent();

           
            // maximize screen
            this.ControlBox = false;
        //    this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            this.WindowState = FormWindowState.Maximized;

            //read image
           // Bitmap bmp = new Bitmap("D:\\KCI Project\\APDSoftCont_Final\\APDSoftCont\\APDSoftCont\\ICCC Logo mini.bmp");
            //Load image
           // ICCCLogo.Image = bmp;

          //  Bitmap bmp1 = new Bitmap("D:\\KCI Project\\APDSoftCont_Final\\APDSoftCont\\APDSoftCont\\PAEC monogram mini.bmp");
            //Load image
          //  PAECLogo.Image = bmp1;

            graphicsobj = this.CreateGraphics();

            mypen = new Pen(System.Drawing.Color.Red, 0);
            mypen1 = new Pen(System.Drawing.Color.Red, 0);
            mypen2 = new Pen(System.Drawing.Color.Red, 0);
            mypen3 = new Pen(System.Drawing.Color.Red, 0);
            mypen4 = new Pen(System.Drawing.Color.Red, 0);
            myrect = new Rectangle(1400,125, 20, 20);
            myrect1 = new Rectangle(1400, 215, 20, 20);
            myrect2 = new Rectangle(1400, 295, 20, 20);
            myrect3 = new Rectangle(1400, 395, 20, 20);
            myrect4 = new Rectangle(1400, 480, 20, 20);
            iconcolor = Color.Red;
            iconbrush = new SolidBrush(iconcolor);
            iconcolor1 = Color.Red;
            iconbrush1 = new SolidBrush(iconcolor1);
            iconcolor2 = Color.Red;
            iconbrush2 = new SolidBrush(iconcolor2);
            iconcolor3 = Color.Red;
            iconbrush3 = new SolidBrush(iconcolor3);
            iconcolor4 = Color.Red;
            iconbrush4 = new SolidBrush(iconcolor4);
            
            
        }

        private void button1_Click(object sender, EventArgs ea)
        {
           try
                {
                    
                   connector = new DeviceManager("EPOS2", "MAXON SERIAL V2", "USB", "USB0");

                  //  connector = new DeviceManager();

                    // Get baudrate info
                    uint b = connector.Baudrate;

                    // Set connection properties
                    connector.Baudrate = 1000000;
                    connector.Timeout = 500;

                    ushort nodeid = 1;


                    epos = connector.CreateDevice(nodeid);

                    StateMachine sm = epos.Operation.StateMachine;

                    //  sm.ResetDevice();

                    // epos.Operation.Io.SetAllDigitalOutputs(1);

                    if (sm.GetFaultState())
                    {
                        sm.ClearFault();
                    }

                    //    epos.Configuration.Advanced.InputOutput.DigitalOutputConfiguration(3, EDigitalOutputConfiguration.DocGeneralPurposeB, 1, 1, 1);

                    epos.Configuration.Advanced.InputOutput.DigitalInputConfiguration(5, EDigitalInputConfiguration.DicPositiveLimitSwitch, 1, 1, 1);
                    sm.SetEnableState();
                    donotstop = 0;
                    iconcolor4 = Color.Green;
                    iconbrush4 = new SolidBrush(iconcolor4);
                    graphicsobj.FillEllipse(iconbrush4, myrect4);
                    TextBox.AppendText("INFO: System is successfully connected"); TextBox.AppendText(Environment.NewLine);
            

                }
                catch (DeviceException e)
                {

                    ShowMessageBox(e.ErrorMessage, e.ErrorCode);
                }

                catch (Exception e)
                {
                    //TextBox.Show(e.Message); 
                    TextBox.Paste(e.Message);
                }

                //  button1_Click(sender, ea);

                Start_Communication.Text = "DISCONNECT";
                Start_Communication.BackColor = Color.Red;
            
        }

        private void ShowMessageBox(string text, uint errorCode)
        {
        //    var msg = string.Format("{0}\nErrorCode: {1:X8}", text, errorCode,"/n");
            var msg = string.Format("{0}\nErrorCode: {1:X8}\n", text, errorCode);
            TextBox.Paste(msg);
            TextBox.AppendText(Environment.NewLine);
            
        }

        private void button2_Click(object sender, EventArgs ec)
        {
             Close();
              
           
        }

        private void button1_Click_1(object sender, EventArgs eb)
        {


            // positive limit switch configure

            try
            {

                //epos.Configuration.Advanced.InputOutput.DigitalInputConfiguration(5, EDigitalInputConfiguration.DicPositiveLimitSwitch, 1, 1, 0);
                //  sm.SetEnableState();
                donotstop = 0;
                capopeningflag = false;

                // delatch solenoid

                  button4_Click(sender, eb);

                // rotating nut hold cap

                CurrentMode cm = epos.Operation.CurrentMode;
                cm.ActivateCurrentMode();
                cm.SetCurrentMust(600);
                thismode = "Current Mode";
                updateposition(thismode);
                currentposition.Text = string.Format("{0}", current_position);
                gripping_position = (Math.Abs(current_position)); // positive extreme position


                System.Threading.Thread.Sleep(2000);
                // Clear Fault

                   button5_Click(sender, eb);

                // latch solenoid

                   button3_Click(sender, eb);

                // Clear Fault

                   button5_Click(sender, eb);

                   TextBox.AppendText("INFO: Step I is successfully completed"); TextBox.AppendText(Environment.NewLine);

            }
            catch (DeviceException e)
            {

                ShowMessageBox(e.ErrorMessage, e.ErrorCode);
            }

            catch (Exception e)
            {
                TextBox.Paste(e.Message);
            }


           
           
        }

        private void Form1_Paint(object sender, PaintEventArgs ed)
        {
            graphicsobj.DrawEllipse(mypen, myrect);
            graphicsobj.FillEllipse(iconbrush, myrect);
           

            graphicsobj.DrawEllipse(mypen1, myrect1);
            graphicsobj.FillEllipse(iconbrush1, myrect1);

            graphicsobj.DrawEllipse(mypen2, myrect2);
            graphicsobj.FillEllipse(iconbrush2, myrect2);


            graphicsobj.DrawEllipse(mypen3, myrect3);
            graphicsobj.FillEllipse(iconbrush3, myrect3);

            graphicsobj.DrawEllipse(mypen4, myrect4);
            graphicsobj.FillEllipse(iconbrush4, myrect4);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs ee)
        {
            try
            {
                iconcolor = Color.Green;
                iconbrush = new SolidBrush(iconcolor);
                graphicsobj.FillEllipse(iconbrush, myrect);

                iconcolor1 = Color.Red;
                iconbrush1 = new SolidBrush(iconcolor1);
                graphicsobj.FillEllipse(iconbrush1, myrect1);

                epos.Configuration.Advanced.InputOutput.DigitalOutputConfiguration(4, EDigitalOutputConfiguration.DocGeneralPurposeD, 1, 1, 0);
                // epos.Operation.Io.SetAllDigitalOutputs(1);


                System.Threading.Thread.Sleep(500);

                epos.Configuration.Advanced.InputOutput.DigitalOutputConfiguration(4, EDigitalOutputConfiguration.DocGeneralPurposeD, 1, 1, 1);

                TextBox.AppendText("INFO: Bottle cap is successfully gripped"); TextBox.AppendText(Environment.NewLine);
            }

            catch (DeviceException e)
            {
                Console.WriteLine(e.Message + e.ErrorCode);

            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        private void button4_Click(object sender, EventArgs ef)
        {
           
            try
            {
                iconcolor1 = Color.Green;
                iconbrush1 = new SolidBrush(iconcolor1);
                graphicsobj.FillEllipse(iconbrush1, myrect1);

                iconcolor = Color.Red;
                iconbrush = new SolidBrush(iconcolor);
                graphicsobj.FillEllipse(iconbrush, myrect);

                epos.Configuration.Advanced.InputOutput.DigitalOutputConfiguration(3, EDigitalOutputConfiguration.DocGeneralPurposeB, 1, 1, 0);
                // epos.Operation.Io.SetAllDigitalOutputs(1);

                System.Threading.Thread.Sleep(500);

                epos.Configuration.Advanced.InputOutput.DigitalOutputConfiguration(3, EDigitalOutputConfiguration.DocGeneralPurposeB, 1, 1, 1);

                TextBox.AppendText("INFO: Bottle cap is successfully released"); TextBox.AppendText(Environment.NewLine);

            }
            catch (DeviceException e)
            {

                ShowMessageBox(e.ErrorMessage, e.ErrorCode);
            }

            catch (Exception e)
            {
                TextBox.Paste(e.Message);
            }

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs eg)
        {
            try
            {
                StateMachine sm = epos.Operation.StateMachine;
                // sm.ResetDevice();
                sm.ClearFault();
                TextBox.AppendText("INFO: Fault is successfully cleared"); TextBox.AppendText(Environment.NewLine);
            }

            catch (DeviceException e)
            {

                ShowMessageBox(e.ErrorMessage, e.ErrorCode);
            }

            catch (Exception e)
            {
                TextBox.Paste(e.Message);
            }
        }

        private void button6_Click(object sender, EventArgs eh)
        {
            try
            {
                StateMachine sm = epos.Operation.StateMachine;
                sm.ResetDevice();
                TextBox.AppendText("INFO: Device is successfully restarted"); TextBox.AppendText(Environment.NewLine);
            }

            catch (DeviceException e)
            {

                ShowMessageBox(e.ErrorMessage, e.ErrorCode);
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void Step_II_Click(object sender, EventArgs ei)
        {
            try
            {
                int lag = 0;

                System.Threading.Thread.Sleep(2000);
                epos.Operation.OperationMode.SetOperationMode(EOperationMode.OmdProfilePositionMode);
                StateMachine sm = epos.Operation.StateMachine;
                sm.ClearFault();
                sm.SetEnableState();
                ProfilePositionMode ppm = epos.Operation.ProfilePositionMode;
                ppm.ActivateProfilePositionMode();
                button5_Click(sender, ei);
                current_position = Math.Abs(epos.Operation.MotionInfo.GetPositionIs());
                ppm.SetPositionProfile(3500, 3000, 3000);
                currentposition.Text = string.Format("{0}", current_position);
                lag = Math.Abs(current_position);
                lag = 3000000 - lag;
                //  negetive_extreme_positive = 0;
                if (lag < 0)
                {
                  //  lag = 100000;
                    lag = 1000;
                }
                ppm.MoveToPosition(-lag, true, true);
                donotstop = 1;
                capopeningflag = true;
                thismode = "Profile Position Mode";
                System.Threading.Thread.Sleep(2000);
                updateposition(thismode);

                /* StateMachine sm = epos.Operation.StateMachine;
                 sm.SetEnableState();
                 //cm.ActivateCurrentMode();
                 cm.SetCurrentMust(-300);
                 thismode = "Current Mode";
                 capopeningflag = true;
                 updateposition(thismode);*/

                // delatch solenoid

                button4_Click(sender, ei);

                TextBox.AppendText("INFO: Step II is successfully completed"); TextBox.AppendText(Environment.NewLine);
            }
            catch (DeviceException e)
            {

                ShowMessageBox(e.ErrorMessage, e.ErrorCode);
            }

            catch (Exception e)
            {
                TextBox.Paste(e.Message);
            }
        }

        private void updateposition(string whichmode)    
        {
            try
            {
                StateMachine sm = epos.Operation.StateMachine;
                ProfilePositionMode ppm = epos.Operation.ProfilePositionMode;
                ProfileVelocityMode pvm = epos.Operation.ProfileVelocityMode;
                CurrentMode cm = epos.Operation.CurrentMode;
                HomingMode hm = epos.Operation.HomingMode;

                current_position = Math.Abs(epos.Operation.MotionInfo.GetPositionIs());
                currentposition.Text = string.Format("{0}", current_position);

                current_velocity = Math.Abs(epos.Operation.MotionInfo.GetVelocityIs());
                currentvelocity.Text = string.Format("{0}", current_velocity);

                current_current = Math.Abs(epos.Operation.MotionInfo.GetCurrentIs());

                epos.Operation.MotionInfo.GetMovementState(ref targetReached);

                while (targetReached == false || (current_velocity) >= 100)
                {
                    faultstate = sm.GetFaultState();
                    if (faultstate == true)
                    {
                        sm.ClearFault();
                    }

                    iconcolor2 = Color.Green;
                    iconbrush2 = new SolidBrush(iconcolor2);
                    graphicsobj.FillEllipse(iconbrush2, myrect2);

                    current_position = Math.Abs(epos.Operation.MotionInfo.GetPositionIs());
                    currentposition.Text = string.Format("{0}", epos.Operation.MotionInfo.GetPositionIs());
                    //   Refresh();

                    current_velocity = Math.Abs(epos.Operation.MotionInfo.GetVelocityIs());
                    currentvelocity.Text = string.Format("{0}", epos.Operation.MotionInfo.GetVelocityIs());
                    //   Refresh();

                    //   current_current = Math.Abs(epos.Operation.MotionInfo.GetCurrentIs());

                    epos.Operation.MotionInfo.GetMovementState(ref targetReached);
                    ushort input = 0;

                    bool fault = sm.GetFaultState();
                    epos.Operation.Io.GetAllDigitalInputs(ref input);

                    /*   if(capopeningflag == true)
                       {
                               if (current_position < gripping_position)
                               {
                                   sm.SetQuickStopState();
                                   sm.SetEnableState();
                                   break;
                               }
                       }*/

                    /*  if(current_position >= gripping_position || current_position >= negetive_extreme_positive)
                      {
                              sm.SetQuickStopState();
                              sm.SetEnableState();
                              break;
                      }*/

                    if (input == 6)
                    {
                        if (donotstop == 0)
                        {
                            pvm.HaltVelocityMovement();
                            sm.SetQuickStopState();
                        }

                        iconcolor3 = Color.Green;
                        iconbrush3 = new SolidBrush(iconcolor3);
                        graphicsobj.FillEllipse(iconbrush3, myrect3);
                        break;
                    }

                    if (input == 4)
                    {
                        iconcolor3 = Color.Red;
                        iconbrush3 = new SolidBrush(iconcolor3);
                        graphicsobj.FillEllipse(iconbrush3, myrect3);
                    }

                    Application.DoEvents();

                    if (Control.ModifierKeys == Keys.Alt)
                    {
                        //  epos.Operation.StateMachine.SetQuickStopState();
                        if (whichmode == "Profile Position Mode")
                        {
                            ppm.HaltPositionMovement();
                            sm.SetEnableState();
                            break;
                        }

                        if (whichmode == "Profile Velocity Mode")
                        {
                            pvm.HaltVelocityMovement();
                            
                            sm.SetEnableState();
                            break;
                        }

                        if (whichmode == "Current Mode")
                        {
                            sm.SetQuickStopState();
                            
                            sm.SetEnableState();
                            break;
                        }

                        if (whichmode == "Homing Mode")
                        {
                            hm.StopHoming();
                        }

                        break;

                    }

                    epos.Operation.MotionInfo.GetMovementState(ref targetReached);
                    //epos.Operation.Io.GetAllDigitalInputs(ref input);

                    /*    if (targetReached == true)
                        {
                            iconcolor2 = Color.Red;
                            iconbrush2 = new SolidBrush(iconcolor2);
                            graphicsobj.FillEllipse(iconbrush2, myrect2);
                        }
                        */

                   

                    iconcolor2 = Color.White;
                    iconbrush2 = new SolidBrush(iconcolor2);
                    graphicsobj.FillEllipse(iconbrush2, myrect2);

                }

                iconcolor2 = Color.Red;
                iconbrush2 = new SolidBrush(iconcolor2);
                graphicsobj.FillEllipse(iconbrush2, myrect2);
                
            }

            catch (DeviceException e)
            {

                ShowMessageBox(e.ErrorMessage, e.ErrorCode);
            }

            catch (Exception e)
            {
                TextBox.Paste(e.Message);
            }

            
        }

        private void button1_Click_2(object sender, EventArgs ej)
        {

            try
                {
                    float abstarget_value = 0;
                    target_value = Convert.ToInt32(Enter_Value.Text);
                    abstarget_value = Math.Abs(target_value);
                    StateMachine sm = epos.Operation.StateMachine;
                    faultstate = sm.GetFaultState();
                    if (faultstate == true)
                    {
                        sm.ClearFault();
                    }

                    if (target_value == 0)
                    {
                        TextBox.AppendText("INFO: Please enter the target value"); TextBox.AppendText(Environment.NewLine);
                    }

                /*    if (Select_RN_Mode.Text == "Profile Position Mode")
                    {
                        ProfilePositionMode ppm = epos.Operation.ProfilePositionMode;
                        sm.SetEnableState();
                        ppm.ActivateProfilePositionMode();
                        /* if (Select.Text == "With Solenoid")
                         {
                             // epos.Operation.Io.SetAllDigitalOutputs(outputs);
                             // System.Threading.Thread.Sleep(1000);
                             button1_Click(sender, eb);
                         }*/

                /*        ppm.MoveToPosition(target_value, true, true);
                        thismode = "Profile Position Mode";
                        TextBox.AppendText("INFO: Moving in profile position mode"); TextBox.AppendText(Environment.NewLine);
                        updateposition(thismode);
                    }*/

                    if (Select_RN_Mode.Text == "Profile Velocity Mode")
                    {
                        ProfileVelocityMode pvm = epos.Operation.ProfileVelocityMode;
                        sm.SetEnableState();
                        pvm.ActivateProfileVelocityMode();
                        /*  if (Select.Text == "With Solenoid")
                          {
                              //  epos.Operation.Io.SetAllDigitalOutputs(outputs);
                              //  System.Threading.Thread.Sleep(1000);
                              button1_Click(sender, eb);
                          }*/

                        if (abstarget_value > 2000)
                        {
                            TextBox.AppendText("INFO: The range of velocity is [-2000 - 2000] rpm"); TextBox.AppendText(Environment.NewLine);
                        }
                        else
                        {
                            pvm.MoveWithVelocity(target_value);
                            thismode = "Profile Velocity Mode";
                            TextBox.AppendText("INFO: Moving in profile velocity mode"); TextBox.AppendText(Environment.NewLine);
                            updateposition(thismode);
                        }
                    }

                    if (Select_RN_Mode.Text == "Current Mode")
                    {
                        CurrentMode cm = epos.Operation.CurrentMode;
                        sm.SetEnableState();
                        cm.ActivateCurrentMode();
                        /* if (Select.Text == "With Solenoid")
                         {
                             //  epos.Operation.Io.SetAllDigitalOutputs(outputs);
                             //  System.Threading.Thread.Sleep(1000); 
                             button1_Click(sender, eb);
                         }*/

                        if (abstarget_value > 300)
                        {
                            TextBox.AppendText("INFO: The range of current is [-300 - 300] mA"); TextBox.AppendText(Environment.NewLine);
                        }
                        else
                        {
                            cm.SetCurrentMust(Convert.ToInt16(target_value));
                            thismode = "Current Mode";
                            TextBox.AppendText("INFO: Moving in current mode"); TextBox.AppendText(Environment.NewLine);
                            updateposition(thismode);
                        }
                    }
                    
                    
                }
                catch (DeviceException e)
                {

                    ShowMessageBox(e.ErrorMessage, e.ErrorCode);
                }

                catch (Exception e)
                {
                    TextBox.Paste(e.Message);
                }

            
        }

        private void Select_RN_Mode_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void currentposition_TextChanged(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            string filename = "D:\\APDSoftCont\\APDSoftCont\\bin\\Debug\\APD Operator Manual.pdf";
            System.Diagnostics.Process.Start(filename);
        }

     

    }
   
}
