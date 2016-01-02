using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SQLite;
using MaterialSkin;
using MaterialSkin.Controls;

namespace Reminders
{
    public partial class Form1 : MaterialForm
    {
        public MaterialSkinManager materialSkinManager;
        public SQLiteConnection conn;

        public Form1()
        {
            InitializeComponent();

            materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!File.Exists("db.sqlite"))
                SQLiteConnection.CreateFile("db.sqlite");


            conn = new SQLiteConnection("Data Source=db.sqlite;Version=3;");
            conn.Open();

            LoadData();
        }

        private void button1_Click(object sender, EventArgs e)
        {            
            add a = new add(this);
            a.ShowDialog();          
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Select a task");
                return;
            }
            
            (new modify(this, ids[listBox1.SelectedIndex])).ShowDialog();           
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                //   notifyIcon1.Visible = true;
                this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                // notifyIcon1.Visible = false;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        List<int> ids = new List<int>();
        List<Reminder> reminders = new List<Reminder>();

        public void LoadData()
        {
            listBox1.Items.Clear();
            reminders.Clear();
            ids.Clear();
            string sql = "select * from reminders order by id";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = int.Parse(reader["id"].ToString());
                string title = reader["title"].ToString();
                DateTime datetime = DateTime.ParseExact(reader["date"] + " " + reader["time"], "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                string task = reader["task"].ToString();            
                bool shown = Convert.ToBoolean(Convert.ToInt16(reader["shown"].ToString()));
                int repeatevery = int.Parse(reader["repeatevery"].ToString());
                reminders.Add(new Reminder(id, title, datetime, task, shown, repeatevery));
                ids.Add(id);
                listBox1.Items.Add(title + " - " + ((shown) ? "Shown" : datetime.ToString()));
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("Select a task");
                return;
            }
            int id = ids[listBox1.SelectedIndex];
            SQLiteCommand command = new SQLiteCommand("delete from reminders where id = '" + id + "'", conn);
            command.ExecuteNonQuery();
            LoadData();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            bool n = false;
            foreach (Reminder r in reminders)
            {
                if (DateTime.Now.Year == r.datetime.Year && DateTime.Now.Month == r.datetime.Month && DateTime.Now.Day == r.datetime.Day && DateTime.Now.Hour == r.datetime.Hour && DateTime.Now.Minute == r.datetime.Minute && !r.shown)
                {
                    notifyIcon1.BalloonTipTitle = r.title;
                    notifyIcon1.BalloonTipText = r.task;
                    notifyIcon1.ShowBalloonTip(5000);                  
                    n = true;
                    SQLiteCommand q;
                    if (r.repeatevery != 0)
                    {
                        q = new SQLiteCommand("update reminders set date = '" + r.datetime.AddDays(r.repeatevery).ToShortDateString() + "' where id = '" + r.id + "'", conn);
                        q.ExecuteNonQuery();                      
                    }
                    else
                    {
                        r.shown = true;
                        q = new SQLiteCommand("update reminders set shown = '1' where id = '" + r.id + "'", conn);
                        q.ExecuteNonQuery();
                    }
                    break;
                }
            }

            if (n)
                LoadData();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            notifyIcon1.Visible = false;
        }
    }

    class Reminder
    {
        public bool shown = false;
        public int id;
        public string title;
        public DateTime datetime;
        public string task;
        public int repeatevery;
        public Reminder(int id, string title, DateTime datetime, string task, bool shown, int repeatevery)
        {
            this.id = id;
            this.title = title;
            this.datetime = datetime;
            this.task = task;       
            this.shown = shown;
            this.repeatevery = repeatevery;
        }
    }
}
