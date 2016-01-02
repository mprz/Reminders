using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using System.Data.SQLite;

namespace Reminders
{
    public partial class add : MaterialForm
    {
        Form1 form1;
        public add(Form1 f)
        {
            InitializeComponent();
            form1 = f;
            form1.materialSkinManager.AddFormToManage(this);
            dateTimePicker2.Value = DateTime.Now;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int repeatevery = 0;

            if (materialCheckBox1.Checked)
                repeatevery = 7;
            if (materialCheckBox2.Checked)
                repeatevery = 365;
            if (materialCheckBox3.Checked)
                repeatevery = 1;
            if (materialCheckBox4.Checked)
                repeatevery = 30;
            if (materialCheckBox5.Checked)
                repeatevery = (int)numericUpDown1.Value;
            SQLiteCommand q = new SQLiteCommand(form1.conn);
            q.Parameters.Add(new SQLiteParameter("@title", textBox1.Text));
            q.Parameters.Add(new SQLiteParameter("@task", textBox2.Text));
            q.CommandText = "insert into reminders (title, task, date, time, repeatevery) values (@title, @task ,'" + dateTimePicker1.Value.ToShortDateString() + "','" + dateTimePicker2.Value.ToShortTimeString() + "','" + repeatevery + "')";
            q.ExecuteNonQuery();
            form1.LoadData();
            this.Close();
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            checkBox1.CheckedChanged -= checkBox_CheckedChanged;
            materialCheckBox1.CheckedChanged -= checkBox_CheckedChanged;
            materialCheckBox2.CheckedChanged -= checkBox_CheckedChanged;
            materialCheckBox3.CheckedChanged -= checkBox_CheckedChanged;
            materialCheckBox4.CheckedChanged -= checkBox_CheckedChanged;
            materialCheckBox5.CheckedChanged -= checkBox_CheckedChanged;

            foreach (Control ckb in this.Controls)
            {
                if (ckb is MaterialCheckBox)
                {
                    MaterialCheckBox cc = (MaterialCheckBox)ckb;
                    if ((MaterialCheckBox)sender == cc)
                        continue;

                    cc.Checked = false;
                }
            }

            checkBox1.CheckedChanged += checkBox_CheckedChanged;
            materialCheckBox1.CheckedChanged += checkBox_CheckedChanged;
            materialCheckBox2.CheckedChanged += checkBox_CheckedChanged;
            materialCheckBox3.CheckedChanged += checkBox_CheckedChanged;
            materialCheckBox4.CheckedChanged += checkBox_CheckedChanged;
            materialCheckBox5.CheckedChanged += checkBox_CheckedChanged;
        }
    }
}
