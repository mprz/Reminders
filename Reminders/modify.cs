using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace Reminders
{
    public partial class modify : MaterialForm
    {
        int id = -1;
        Form1 form1;
        public modify(Form1 f, int id)
        {
            InitializeComponent();
            this.form1 = f;
            this.id = id;
            f.materialSkinManager.AddFormToManage(this);
        }

        private void modify_Load(object sender, EventArgs e)
        {
            string sql = "select * from reminders where id = '" + id + "'";
            SQLiteCommand command = new SQLiteCommand(sql, form1.conn);
            SQLiteDataReader reader = command.ExecuteReader();
            reader.Read();
            DateTime datetime = DateTime.ParseExact(reader["date"] + " " + reader["time"], "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            textBox1.Text = reader["title"].ToString();
            textBox2.Text = reader["task"].ToString();
            int repeatevery = int.Parse(reader["repeatevery"].ToString());
            if (repeatevery == 7)
                materialCheckBox1.Checked = true;
            else if (repeatevery == 365)
                materialCheckBox2.Checked = true;
            else if (repeatevery == 1)
                materialCheckBox3.Checked = true;
            else if (repeatevery == 30)
                materialCheckBox4.Checked = true;
            else
            {
                if (repeatevery != 0)
                {
                    materialCheckBox5.Checked = true;
                    numericUpDown1.Value = repeatevery;
                }
            }
            dateTimePicker1.Value = datetime;
            dateTimePicker2.Value = datetime;
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

            SQLiteCommand cmd = new SQLiteCommand(form1.conn);
            cmd.Parameters.Add(new SQLiteParameter("@title", textBox1.Text));
            cmd.Parameters.Add(new SQLiteParameter("@task", textBox2.Text));
            cmd.CommandText = "update reminders set title = @title, task = @task, date = '" + dateTimePicker1.Value.ToShortDateString() + "', time = '" + dateTimePicker2.Value.ToShortTimeString() + "', repeatevery = '" + repeatevery + "' where id = '" + id + "'";
            cmd.ExecuteNonQuery();
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
