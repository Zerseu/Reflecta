#region Using

using System;
using System.Windows.Forms;
using Reflecta.Server.UI.Properties;

#endregion

public partial class Form_Main : Form
{
    private Server Agent;
    private bool IsRecording;

    public Form_Main()
    {
        InitializeComponent();
    }

    private void Form_Main_Load(object sender, EventArgs e)
    {
        Agent = new Server();
    }

    private void Form_Main_FormClosing(object sender, FormClosingEventArgs e)
    {
        Agent.Dispose();
    }

    private void Button_Record_Click(object sender, EventArgs e)
    {
        if (!IsRecording)
        {
            IsRecording = true;
            Button_Record.Image = Resources.Stop;
            PictureBox_Working.Image = Resources.Working;
            Agent.StartMoCap();
        }
        else
        {
            Agent.StopMoCap();
            PictureBox_Working.Image = null;
            Button_Record.Image = Resources.Record;
            IsRecording = false;

            if (SaveFileDialog_SaveMoCap.ShowDialog() == DialogResult.OK)
                Agent.SaveMoCap(SaveFileDialog_SaveMoCap.FileName);
        }
    }
}